using Microsoft.EntityFrameworkCore;
using SmartParking.Core.Interfaces;
using SmartParkingSystem;
using System.Linq.Expressions;

namespace SmartParking.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ParkingContext _context;

        public GenericRepository(ParkingContext context)
        {
            _context = context;
        }

        public async Task<T> GetByIdAsync(int id) =>
            await _context.Set<T>().FindAsync(id);

        public async Task<IReadOnlyList<T>> GetAllAsync() =>
            await _context.Set<T>().ToListAsync();

        public async Task<IReadOnlyList<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<T> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
                query = query.Include(include);

            // Assuming your entity has a property named Id
            // If your primary key is different, adjust this
            var keyProperty = typeof(T).GetProperties().FirstOrDefault(p => p.Name.EndsWith("Id"));
            if (keyProperty == null)
                throw new Exception("No Id property found on entity");

            // Build expression dynamically to filter by id
            query = query.Where(e => EF.Property<int>(e, keyProperty.Name) == id);

            return await query.FirstOrDefaultAsync();
        }

    }
}
