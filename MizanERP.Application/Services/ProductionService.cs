using System;
using System.Linq;
using System.Threading.Tasks;
using MizanERP.Application.DTOs;
using MizanERP.Domain.Entities;
using MizanERP.Domain.Enums;
using MizanERP.Domain.ValueObjects;

namespace MizanERP.Application.Services
{
    public class ProductionService : IProductionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Guid> CreateProductionOrderAsync(CreateProductionOrderDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Lines.Count == 0) throw new ArgumentException("Production order must have at least one raw material");

            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null) throw new InvalidOperationException($"Product {dto.ProductId} not found");
            if (product.Type != ProductType.FinishedGood)
                throw new InvalidOperationException($"Product {dto.ProductId} is not a finished good");

            var productionOrder = new ProductionOrder(
                Guid.NewGuid(),
                dto.ProductId,
                new Quantity(dto.Quantity, product.Unit),
                dto.OrderDate
            );

            foreach (var lineDto in dto.Lines)
            {
                var rawMaterial = await _unitOfWork.Products.GetByIdAsync(lineDto.RawMaterialProductId);
                if (rawMaterial == null) throw new InvalidOperationException($"Raw material {lineDto.RawMaterialProductId} not found");
                if (rawMaterial.Type != ProductType.RawMaterial)
                    throw new InvalidOperationException($"Product {lineDto.RawMaterialProductId} is not a raw material");

                if (rawMaterial.InventoryQuantity < lineDto.Quantity)
                    throw new InvalidOperationException($"Insufficient {rawMaterial.Name}. Available: {rawMaterial.InventoryQuantity}, Required: {lineDto.Quantity}");

                var line = new ProductionOrderLine(
                    Guid.NewGuid(),
                    productionOrder.Id,
                    lineDto.RawMaterialProductId,
                    new Quantity(lineDto.Quantity, rawMaterial.Unit)
                );

                productionOrder.AddLine(line);
            }

            await _unitOfWork.ProductionOrders.AddAsync(productionOrder);
            await _unitOfWork.SaveChangesAsync();

            return productionOrder.Id;
        }

        public async Task ConsumeRawMaterialsAsync(Guid productionOrderId)
        {
            if (productionOrderId == Guid.Empty) throw new ArgumentException("Invalid production order ID");

            var productionOrder = await _unitOfWork.ProductionOrders.GetByIdAsync(productionOrderId);
            if (productionOrder == null) throw new InvalidOperationException($"Production order {productionOrderId} not found");

            if (productionOrder.Status != OrderStatus.Approved)
                throw new InvalidOperationException($"Only approved orders can consume materials. Current status: {productionOrder.Status}");

            foreach (var line in productionOrder.Lines)
            {
                var rawMaterial = await _unitOfWork.Products.GetByIdAsync(line.RawMaterialProductId);
                if (rawMaterial == null) throw new InvalidOperationException($"Raw material {line.RawMaterialProductId} not found");

                if (rawMaterial.InventoryQuantity < line.Quantity.Value)
                    throw new InvalidOperationException($"Insufficient {rawMaterial.Name}. Available: {rawMaterial.InventoryQuantity}, Required: {line.Quantity.Value}");

                rawMaterial.DecreaseInventory(line.Quantity.Value);

                var movement = new InventoryMovement(
                    Guid.NewGuid(),
                    line.RawMaterialProductId,
                    line.Quantity,
                    MovementType.ProductionConsumption,
                    DateTime.UtcNow,
                    null,
                    null,
                    null,
                    $"ProdOrder-{productionOrder.Id:N}"
                );

                await _unitOfWork.Products.UpdateAsync(rawMaterial);
                await _unitOfWork.InventoryMovements.AddAsync(movement);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ProduceFinishedGoodsAsync(Guid productionOrderId)
        {
            if (productionOrderId == Guid.Empty) throw new ArgumentException("Invalid production order ID");

            var productionOrder = await _unitOfWork.ProductionOrders.GetByIdAsync(productionOrderId);
            if (productionOrder == null) throw new InvalidOperationException($"Production order {productionOrderId} not found");

            if (productionOrder.Status != OrderStatus.Approved)
                throw new InvalidOperationException($"Only approved orders can produce goods. Current status: {productionOrder.Status}");

            var finishedProduct = await _unitOfWork.Products.GetByIdAsync(productionOrder.ProductId);
            if (finishedProduct == null) throw new InvalidOperationException($"Finished product {productionOrder.ProductId} not found");

            finishedProduct.IncreaseInventory(productionOrder.Quantity.Value);

            var movement = new InventoryMovement(
                Guid.NewGuid(),
                productionOrder.ProductId,
                productionOrder.Quantity,
                MovementType.ProductionOutput,
                DateTime.UtcNow,
                null,
                null,
                null,
                $"ProdOrder-{productionOrder.Id:N}"
            );

            var productionCost = await CalculateProductionCostAsync(productionOrder);

            await GenerateProductionAccountingEntriesAsync(productionOrder, productionCost);

            await _unitOfWork.Products.UpdateAsync(finishedProduct);
            await _unitOfWork.InventoryMovements.AddAsync(movement);
            productionOrder.Complete();
            await _unitOfWork.ProductionOrders.UpdateAsync(productionOrder);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<decimal> CalculateProductionCostAsync(ProductionOrder productionOrder)
        {
            decimal totalCost = 0;

            foreach (var line in productionOrder.Lines)
            {
                var rawMaterial = await _unitOfWork.Products.GetByIdAsync(line.RawMaterialProductId);
                if (rawMaterial == null) continue;

                var movements = await _unitOfWork.InventoryMovements.GetAllAsync();
                var materialMovements = movements
                    .Where(m => m.ProductId == line.RawMaterialProductId && m.Cost != null)
                    .ToList();

                if (materialMovements.Any())
                {
                    var avgCost = materialMovements.Average(m => m.Cost?.Amount ?? 0);
                    totalCost += avgCost * line.Quantity.Value;
                }
            }

            return totalCost;
        }

        private async Task GenerateProductionAccountingEntriesAsync(ProductionOrder productionOrder, decimal totalProductionCost)
        {
            if (totalProductionCost == 0) return;

            var finishedGoodsAccount = await FindOrCreateAccountAsync("Finished Goods Inventory", AccountType.Asset);
            var debitEntry = new AccountingEntry(
                Guid.NewGuid(),
                DateTime.UtcNow,
                finishedGoodsAccount.Id,
                totalProductionCost,
                0,
                $"ProdOrder-{productionOrder.Id:N}"
            );

            var rawMaterialAccount = await FindOrCreateAccountAsync("Raw Material Inventory", AccountType.Asset);
            var creditEntry = new AccountingEntry(
                Guid.NewGuid(),
                DateTime.UtcNow,
                rawMaterialAccount.Id,
                0,
                totalProductionCost,
                $"ProdOrder-{productionOrder.Id:N}"
            );

            await _unitOfWork.AccountingEntries.AddAsync(debitEntry);
            await _unitOfWork.AccountingEntries.AddAsync(creditEntry);
        }

        private async Task<Account> FindOrCreateAccountAsync(string accountName, AccountType type)
        {
            var accounts = await _unitOfWork.Accounts.GetAllAsync();
            var account = accounts.FirstOrDefault(a => a.Name == accountName);

            if (account == null)
            {
                account = new Account(Guid.NewGuid(), accountName, type);
                await _unitOfWork.Accounts.AddAsync(account);
                await _unitOfWork.SaveChangesAsync();
            }

            return account;
        }
    }
}