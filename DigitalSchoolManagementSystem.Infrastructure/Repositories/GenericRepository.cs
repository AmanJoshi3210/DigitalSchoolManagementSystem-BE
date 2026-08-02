using System.Linq.Expressions;
using DigitalSchoolManagementSystem.Application.Interfaces;
using DigitalSchoolManagementSystem.Domain.Common;
using DigitalSchoolManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext Context;
        protected readonly DbSet<T> DbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

        public async Task<IReadOnlyList<T>> GetAllAsync() => await DbSet.ToListAsync();

        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await DbSet.Where(predicate).ToListAsync();

        public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
            await DbSet.SingleOrDefaultAsync(predicate);

        public IQueryable<T> Query() => DbSet.AsQueryable();

        public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities) => await DbSet.AddRangeAsync(entities);

        public void Update(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            DbSet.Update(entity);
        }

        public void Remove(T entity) => DbSet.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) => DbSet.RemoveRange(entities);

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) => await DbSet.AnyAsync(predicate);

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null) =>
            predicate is null ? await DbSet.CountAsync() : await DbSet.CountAsync(predicate);
    }
}
