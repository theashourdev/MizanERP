using System;
using System.Threading.Tasks;
using MizanERP.Application.Repositories;
using MizanERP.Domain.Entities;

namespace MizanERP.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly MizanERPDbContext _context;

        public RoleRepository(MizanERPDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByIdAsync(Guid id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<List<Role>> GetAllAsync()
        {
            return await Task.FromResult(_context.Roles.ToList());
        }

        public async Task AddAsync(Role entity)
        {
            await _context.Roles.AddAsync(entity);
        }

        public Task UpdateAsync(Role entity)
        {
            _context.Roles.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Role entity)
        {
            entity.MarkDeleted();
            _context.Roles.Update(entity);
            return Task.CompletedTask;
        }
    }
}