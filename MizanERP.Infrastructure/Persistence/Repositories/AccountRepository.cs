using System;
using System.Threading.Tasks;
using MizanERP.Application.Repositories;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly MizanERPDbContext _context;

        public AccountRepository(MizanERPDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await Task.FromResult(_context.Accounts.ToList());
        }

        public async Task AddAsync(Account entity)
        {
            await _context.Accounts.AddAsync(entity);
        }

        public Task UpdateAsync(Account entity)
        {
            _context.Accounts.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Account entity)
        {
            entity.MarkDeleted();
            _context.Accounts.Update(entity);
            return Task.CompletedTask;
        }
    }
}