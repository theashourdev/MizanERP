using System;
using System.Threading.Tasks;
using MizanERP.Application.Repositories;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence.Repositories
{
    public class InventoryMovementRepository : IInventoryMovementRepository
    {
        private readonly MizanERPDbContext _context;

        public InventoryMovementRepository(MizanERPDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryMovement?> GetByIdAsync(Guid id)
        {
            return await _context.InventoryMovements.FindAsync(id);
        }

        public async Task<List<InventoryMovement>> GetAllAsync()
        {
            return await Task.FromResult(_context.InventoryMovements.ToList());
        }

        public async Task AddAsync(InventoryMovement entity)
        {
            await _context.InventoryMovements.AddAsync(entity);
        }

        public Task UpdateAsync(InventoryMovement entity)
        {
            _context.InventoryMovements.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(InventoryMovement entity)
        {
            entity.MarkDeleted();
            _context.InventoryMovements.Update(entity);
            return Task.CompletedTask;
        }
    }
}