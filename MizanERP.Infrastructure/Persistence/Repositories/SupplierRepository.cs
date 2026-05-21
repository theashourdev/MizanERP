using System;
using System.Threading.Tasks;
using System.Linq;
using MizanERP.Application.Repositories;
using MizanERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
            // If there's already a tracked instance with the same key, update its current values
            var trackedEntry = _context.ChangeTracker.Entries<Supplier>().FirstOrDefault(e => e.Entity.Id == entity.Id);
            if (trackedEntry != null)
            {
                // Update scalar/primitive properties
                trackedEntry.CurrentValues.SetValues(entity);

                // Ensure owned/complex type (Address) is updated as well
                try
                {
                    var addressRef = _context.Entry(trackedEntry.Entity).Reference(nameof(Supplier.Address));
                    if (addressRef != null)
                    {
                        addressRef.CurrentValue = entity.Address;
                    }
                }
                catch
                {
                    // fallback - ignore if cannot set reference
                }
            }
            else
            {
                _context.Suppliers.Update(entity);
            }

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