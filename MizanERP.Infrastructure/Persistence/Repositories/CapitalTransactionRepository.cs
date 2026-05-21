using Microsoft.EntityFrameworkCore;
using MizanERP.Application.Repositories;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence.Repositories
{
    public class CapitalTransactionRepository : ICapitalTransactionRepository
    {
        private readonly MizanERPDbContext _context;

        public CapitalTransactionRepository(MizanERPDbContext context)
        {
            _context = context;
        }

        public async Task<CapitalTransaction?> GetByIdAsync(Guid id)
        {
            return await _context.CapitalTransactions.FindAsync(id);
        }

        public async Task<List<CapitalTransaction>> GetAllAsync()
        {
            return await _context.CapitalTransactions.ToListAsync();
        }

        public async Task AddAsync(CapitalTransaction entity)
        {
            await _context.CapitalTransactions.AddAsync(entity);
        }

        public Task UpdateAsync(CapitalTransaction entity)
        {
            _context.CapitalTransactions.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(CapitalTransaction entity)
        {
            entity.MarkDeleted();
            _context.CapitalTransactions.Update(entity);
            return Task.CompletedTask;
        }

        public async Task<decimal?> GetLatestAmountAsync()
        {
            var latestAmout = await _context.CapitalTransactions.OrderByDescending(c => c.CreatedAt)
                .Select(c => c.Amount)
                .FirstOrDefaultAsync();

            return latestAmout;
        }
    }
}
