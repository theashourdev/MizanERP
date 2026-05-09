using System;
using System.Threading.Tasks;
using MizanERP.Application.DTOs;
using MizanERP.Domain.Entities;
using MizanERP.Domain.Enums;
using MizanERP.Domain.ValueObjects;

namespace MizanERP.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task StockInAsync(InventoryTransactionDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Quantity <= 0) throw new ArgumentException("Quantity must be positive");

            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null) throw new InvalidOperationException($"Product {dto.ProductId} not found");

            product.IncreaseInventory(dto.Quantity);

            var movement = new InventoryMovement(
                Guid.NewGuid(),
                dto.ProductId,
                new Quantity(dto.Quantity, dto.Unit),
                MovementType.In,
                dto.Date,
                null,
                dto.ToWarehouseId,
                dto.Cost.HasValue ? new Money(dto.Cost.Value, dto.Currency ?? "USD") : null,
                dto.Reference
            );

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.InventoryMovements.AddAsync(movement);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task StockOutAsync(InventoryTransactionDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Quantity <= 0) throw new ArgumentException("Quantity must be positive");

            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null) throw new InvalidOperationException($"Product {dto.ProductId} not found");

            if (product.InventoryQuantity < dto.Quantity)
                throw new InvalidOperationException($"Insufficient inventory. Available: {product.InventoryQuantity}, Required: {dto.Quantity}");

            product.DecreaseInventory(dto.Quantity);

            var movement = new InventoryMovement(
                Guid.NewGuid(),
                dto.ProductId,
                new Quantity(dto.Quantity, dto.Unit),
                MovementType.Out,
                dto.Date,
                dto.FromWarehouseId,
                null,
                dto.Cost.HasValue ? new Money(dto.Cost.Value, dto.Currency ?? "USD") : null,
                dto.Reference
            );

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.InventoryMovements.AddAsync(movement);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AdjustAsync(InventoryTransactionDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Quantity == 0) throw new ArgumentException("Adjustment quantity cannot be zero");

            var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
            if (product == null) throw new InvalidOperationException($"Product {dto.ProductId} not found");

            if (dto.Quantity > 0)
                product.IncreaseInventory(dto.Quantity);
            else
                product.DecreaseInventory(Math.Abs(dto.Quantity));

            var movementType = dto.Quantity > 0 ? MovementType.In : MovementType.Out;
            var movement = new InventoryMovement(
                Guid.NewGuid(),
                dto.ProductId,
                new Quantity(Math.Abs(dto.Quantity), dto.Unit),
                movementType,
                dto.Date,
                null,
                null,
                null,
                $"Adjustment: {dto.Reference}"
            );

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.InventoryMovements.AddAsync(movement);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}