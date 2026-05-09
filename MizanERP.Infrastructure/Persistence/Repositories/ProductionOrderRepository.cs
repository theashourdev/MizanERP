using System;
using System.Threading.Tasks;
using MizanERP.Application.Repositories;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence.Repositories
{
    public class ProductionOrderRepository : IProductionOrderRepository
    {
        private readonly MizanERPDbContext _context;

        public ProductionOrderRepository(MizanERPDbContext context)
        {
            _context = context;
        }

        public async Task<ProductionOrder?> GetByIdAsync(Guid id)
        {
            return await _context.ProductionOrders.FindAsync(id);
        }

        public async Task<List<ProductionOrder>> GetAllAsync()
        {
            return await Task.FromResult(_context.ProductionOrders.ToList());
        }

        public async Task AddAsync(ProductionOrder entity)
        {
            await _context.ProductionOrders.AddAsync(entity);
        }

        public Task UpdateAsync(ProductionOrder entity)
        {
            _context.ProductionOrders.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(ProductionOrder entity)
        {
            entity.MarkDeleted();
            _context.ProductionOrders.Update(entity);
            return Task.CompletedTask;
        }
    }
}