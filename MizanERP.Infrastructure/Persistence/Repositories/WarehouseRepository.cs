using System;
using System.Threading.Tasks;
using MizanERP.Application.Repositories;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly MizanERPDbContext _context;

        public WarehouseRepository(MizanERPDbContext context)
        {
            _context = context;
        }

        public async Task<Warehouse?> GetByIdAsync(Guid id)
        {
            return await _context.Warehouses.FindAsync(id);
        }

        public async Task<List<Warehouse>> GetAllAsync()
        {
            return await Task.FromResult(_context.Warehouses.ToList());
        }

        public async Task AddAsync(Warehouse entity)
        {
            await _context.Warehouses.AddAsync(entity);
        }

        public Task UpdateAsync(Warehouse entity)
        {
            _context.Warehouses.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Warehouse entity)
        {
            entity.MarkDeleted();
            _context.Warehouses.Update(entity);
            return Task.CompletedTask;
        }
    }
}