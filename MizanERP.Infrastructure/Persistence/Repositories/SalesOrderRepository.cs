using System;
using System.Threading.Tasks;
using MizanERP.Application.Repositories;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence.Repositories
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly MizanERPDbContext _context;

        public SalesOrderRepository(MizanERPDbContext context)
        {
            _context = context;
        }

        public async Task<SalesOrder?> GetByIdAsync(Guid id)
        {
            return await _context.SalesOrders.FindAsync(id);
        }

        public async Task<List<SalesOrder>> GetAllAsync()
        {
            return await Task.FromResult(_context.SalesOrders.ToList());
        }

        public async Task AddAsync(SalesOrder entity)
        {
            await _context.SalesOrders.AddAsync(entity);
        }

        public Task UpdateAsync(SalesOrder entity)
        {
            _context.SalesOrders.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(SalesOrder entity)
        {
            entity.MarkDeleted();
            _context.SalesOrders.Update(entity);
            return Task.CompletedTask;
        }
    }
}