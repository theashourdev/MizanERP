using System;
using System.Threading.Tasks;
using MizanERP.Application.Repositories;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly MizanERPDbContext _context;

        public SupplierRepository(MizanERPDbContext context)
        {
            _context = context;
        }

        public async Task<Supplier?> GetByIdAsync(Guid id)
        {
            return await _context.Suppliers.FindAsync(id);
        }

        public async Task<List<Supplier>> GetAllAsync()
        {
            return await Task.FromResult(_context.Suppliers.ToList());
        }

        public async Task AddAsync(Supplier entity)
        {
            await _context.Suppliers.AddAsync(entity);
        }

        public Task UpdateAsync(Supplier entity)
        {
            _context.Suppliers.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Supplier entity)
        {
            entity.MarkDeleted();
            _context.Suppliers.Update(entity);
            return Task.CompletedTask;
        }
    }
}