using System;
using System.Threading.Tasks;
using MizanERP.Application.Repositories;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence.Repositories
{
    public class AccountingEntryRepository : IAccountingEntryRepository
    {
        private readonly MizanERPDbContext _context;

        public AccountingEntryRepository(MizanERPDbContext context)
        {
            _context = context;
        }

        public async Task<AccountingEntry?> GetByIdAsync(Guid id)
        {
            return await _context.AccountingEntries.FindAsync(id);
        }

        public async Task<List<AccountingEntry>> GetAllAsync()
        {
            return await Task.FromResult(_context.AccountingEntries.ToList());
        }

        public async Task AddAsync(AccountingEntry entity)
        {
            await _context.AccountingEntries.AddAsync(entity);
        }

        public Task UpdateAsync(AccountingEntry entity)
        {
            _context.AccountingEntries.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(AccountingEntry entity)
        {
            entity.MarkDeleted();
            _context.AccountingEntries.Update(entity);
            return Task.CompletedTask;
        }
    }
}