using Library.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Library.Data
{
    public class MsSqlRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly LibraryDbContext context;
        private readonly DbSet<T> dbSet;

        public MsSqlRepository(LibraryDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public async Task CreateAsync(T item)
        {
            await dbSet.AddAsync(item);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            T item = await dbSet.FindAsync(id);
            context.Entry(item).State = EntityState.Deleted;
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            return await Include(includeProperties).ToListAsync();
        }

        public async Task<T> GetAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await dbSet.AsNoTracking().Where(filter).ToListAsync();
        }

        public async Task<T> GetAsync(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = Include(includeProperties);
            return await query.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = Include(includeProperties);
            return await query.Where(filter).ToListAsync();
        }

        public async Task UpdateAsync(T item)
        {
            context.Entry(item).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }


        private IQueryable<T> Include(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = dbSet.AsNoTracking();
            return includeProperties
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }
    }
}
