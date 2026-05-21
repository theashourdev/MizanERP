using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MizanERP.Application.Repositories;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly MizanERPDbContext _context;

        public PurchaseOrderRepository(MizanERPDbContext context)
        {
            _context = context;
        }

        public async Task<PurchaseOrder?> GetByIdAsync(Guid id)
        {
            return await _context.PurchaseOrders
                .Include(p => p.Lines)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<PurchaseOrder>> GetAllAsync()
        {
            return await _context.PurchaseOrders
                .Include(p => p.Lines)
                .ToListAsync();
        }

        public async Task AddAsync(PurchaseOrder entity)
        {
            await _context.PurchaseOrders.AddAsync(entity);
        }

        public Task UpdateAsync(PurchaseOrder entity)
        {
            _context.PurchaseOrders.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(PurchaseOrder entity)
        {
            entity.MarkDeleted();
            _context.PurchaseOrders.Update(entity);
            return Task.CompletedTask;
        }
    }
}