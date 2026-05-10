using System;
using System.Linq;
using System.Threading.Tasks;
using MizanERP.Application.DTOs;
using MizanERP.Domain.Entities;
using MizanERP.Domain.Enums;

namespace MizanERP.Application.Services
{
    public class AccountingService : IAccountingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task GenerateJournalEntriesAsync(AccountingTransactionDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.AccountId == Guid.Empty)
                throw new ArgumentException("Invalid account ID");

            var account = await _unitOfWork.Accounts.GetByIdAsync(dto.AccountId);
            if (account == null)
                throw new InvalidOperationException($"Account {dto.AccountId} not found");

            if (dto.DebitAmount > 0 || dto.CreditAmount > 0)
            {
                var entry = new AccountingEntry(
                    Guid.NewGuid(),
                    dto.TransactionDate,
                    dto.AccountId,
                    dto.DebitAmount,
                    dto.CreditAmount,
                    dto.Reference ?? dto.Description
                );

                await _unitOfWork.AccountingEntries.AddAsync(entry);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task PostEntriesToLedgerAsync()
        {
            // Implementation: typically marks entries as posted/locked
            // For now, this is a placeholder
            await Task.CompletedTask;
        }

        public async Task<object> GenerateTrialBalanceAsync()
        {
            var entries = await _unitOfWork.AccountingEntries.GetAllAsync();
            var groupedByAccount = entries.GroupBy(e => e.AccountId);
            
            var trialBalance = new List<dynamic>();
            foreach (var group in groupedByAccount)
            {
                var totalDebit = group.Sum(e => e.Debit);
                var totalCredit = group.Sum(e => e.Credit);
                var balance = totalDebit - totalCredit;

                if (balance != 0)
                {
                    trialBalance.Add(new 
                    { 
                        AccountId = group.Key, 
                        TotalDebit = totalDebit,
                        TotalCredit = totalCredit,
                        Balance = balance
                    });
                }
            }

            var totalDebitSum = trialBalance.Sum(x => (decimal)x.TotalDebit);
            var totalCreditSum = trialBalance.Sum(x => (decimal)x.TotalCredit);

            return new 
            { 
                GeneratedAt = DateTime.UtcNow,
                Entries = trialBalance,
                TotalDebit = totalDebitSum,
                TotalCredit = totalCreditSum
            };
        }

        public async Task GenerateJournalEntriesForPurchaseAsync(Guid purchaseOrderId)
        {
            if (purchaseOrderId == Guid.Empty) throw new ArgumentException("Invalid purchase order ID");

            var purchaseOrder = await _unitOfWork.PurchaseOrders.GetByIdAsync(purchaseOrderId);
            if (purchaseOrder == null) throw new InvalidOperationException($"Purchase order {purchaseOrderId} not found");

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(purchaseOrder.SupplierId);
            if (supplier == null) throw new InvalidOperationException($"Supplier {purchaseOrder.SupplierId} not found");

            var totalAmount = purchaseOrder.GetTotal();

            var rawMaterialAccount = await FindOrCreateAccountAsync("Raw Material Inventory", AccountType.Asset);
            var debitEntry = new AccountingEntry(
                Guid.NewGuid(),
                DateTime.UtcNow,
                rawMaterialAccount.Id,
                totalAmount.Amount,
                0,
                $"PO-{purchaseOrder.Id:N}"
            );

            var payableAccount = await FindOrCreateAccountAsync($"Payable - {supplier.Name}", AccountType.Liability);
            var creditEntry = new AccountingEntry(
                Guid.NewGuid(),
                DateTime.UtcNow,
                payableAccount.Id,
                0,
                totalAmount.Amount,
                $"PO-{purchaseOrder.Id:N}"
            );

            await _unitOfWork.AccountingEntries.AddAsync(debitEntry);
            await _unitOfWork.AccountingEntries.AddAsync(creditEntry);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task GenerateJournalEntriesForSaleAsync(Guid salesOrderId)
        {
            if (salesOrderId == Guid.Empty) throw new ArgumentException("Invalid sales order ID");

            var salesOrder = await _unitOfWork.SalesOrders.GetByIdAsync(salesOrderId);
            if (salesOrder == null) throw new InvalidOperationException($"Sales order {salesOrderId} not found");

            var customer = await _unitOfWork.Customers.GetByIdAsync(salesOrder.CustomerId);
            if (customer == null) throw new InvalidOperationException($"Customer {salesOrder.CustomerId} not found");

            var totalRevenue = salesOrder.GetTotal();

            var receivableAccount = await FindOrCreateAccountAsync($"Receivable - {customer.Name}", AccountType.Asset);
            var debitReceivable = new AccountingEntry(
                Guid.NewGuid(),
                DateTime.UtcNow,
                receivableAccount.Id,
                totalRevenue.Amount,
                0,
                $"SO-{salesOrder.Id:N}"
            );

            var revenueAccount = await FindOrCreateAccountAsync("Sales Revenue", AccountType.Revenue);
            var creditRevenue = new AccountingEntry(
                Guid.NewGuid(),
                DateTime.UtcNow,
                revenueAccount.Id,
                0,
                totalRevenue.Amount,
                $"SO-{salesOrder.Id:N}"
            );

            await _unitOfWork.AccountingEntries.AddAsync(debitReceivable);
            await _unitOfWork.AccountingEntries.AddAsync(creditRevenue);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task GenerateJournalEntriesForProductionAsync(Guid productionOrderId)
        {
            if (productionOrderId == Guid.Empty) throw new ArgumentException("Invalid production order ID");

            var productionOrder = await _unitOfWork.ProductionOrders.GetByIdAsync(productionOrderId);
            if (productionOrder == null) throw new InvalidOperationException($"Production order {productionOrderId} not found");

            var productionCost = await CalculateProductionCostAsync(productionOrder);
            if (productionCost == 0) return;

            var finishedGoodsAccount = await FindOrCreateAccountAsync("Finished Goods Inventory", AccountType.Asset);
            var debitEntry = new AccountingEntry(
                Guid.NewGuid(),
                DateTime.UtcNow,
                finishedGoodsAccount.Id,
                productionCost,
                0,
                $"ProdOrder-{productionOrder.Id:N}"
            );

            var rawMaterialAccount = await FindOrCreateAccountAsync("Raw Material Inventory", AccountType.Asset);
            var creditEntry = new AccountingEntry(
                Guid.NewGuid(),
                DateTime.UtcNow,
                rawMaterialAccount.Id,
                0,
                productionCost,
                $"ProdOrder-{productionOrder.Id:N}"
            );

            await _unitOfWork.AccountingEntries.AddAsync(debitEntry);
            await _unitOfWork.AccountingEntries.AddAsync(creditEntry);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task GenerateJournalEntriesForInventoryAdjustmentAsync(Guid inventoryTransactionId)
        {
            if (inventoryTransactionId == Guid.Empty) throw new ArgumentException("Invalid inventory transaction ID");

            var movement = await _unitOfWork.InventoryMovements.GetByIdAsync(inventoryTransactionId);
            if (movement == null) throw new InvalidOperationException($"Inventory movement {inventoryTransactionId} not found");

            if (movement.Cost == null || movement.Cost.Amount == 0) return;

            var totalAmount = movement.Cost.Amount * movement.Quantity.Value;

            var inventoryAccount = await FindOrCreateAccountAsync("Inventory Adjustment", AccountType.Asset);
            var expenseAccount = await FindOrCreateAccountAsync("Inventory Variance", AccountType.Expense);

            if (movement.MovementType == MovementType.In)
            {
                var debitEntry = new AccountingEntry(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    inventoryAccount.Id,
                    totalAmount,
                    0,
                    $"InventAdj-{movement.Id:N}"
                );

                var creditEntry = new AccountingEntry(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    expenseAccount.Id,
                    0,
                    totalAmount,
                    $"InventAdj-{movement.Id:N}"
                );

                await _unitOfWork.AccountingEntries.AddAsync(debitEntry);
                await _unitOfWork.AccountingEntries.AddAsync(creditEntry);
            }
            else
            {
                var creditEntry = new AccountingEntry(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    inventoryAccount.Id,
                    0,
                    totalAmount,
                    $"InventAdj-{movement.Id:N}"
                );

                var debitEntry = new AccountingEntry(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    expenseAccount.Id,
                    totalAmount,
                    0,
                    $"InventAdj-{movement.Id:N}"
                );

                await _unitOfWork.AccountingEntries.AddAsync(debitEntry);
                await _unitOfWork.AccountingEntries.AddAsync(creditEntry);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<decimal> CalculateProductionCostAsync(ProductionOrder productionOrder)
        {
            decimal totalCost = 0;

            foreach (var line in productionOrder.Lines)
            {
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