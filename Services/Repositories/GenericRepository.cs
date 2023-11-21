using ApiCos.Data;
using ApiCos.Services.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace ApiCos.Services.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected ApiDbContext _context;
        protected DbSet<T> dbSet;
        private ApiDbContext context;
        protected readonly ILogger _logger;

        public GenericRepository(ApiDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            this.dbSet = _context.Set<T>();
        }


        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public virtual async Task<T> GetById(int id)
        {
            try
            {
                return await dbSet.FindAsync(id);
            } catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(GetById)}: " + ex.Message);
                return null;
            }
        }

        public virtual async Task<T> Add(T entity)
        {
            try
            {
                await dbSet.AddAsync(entity);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(Add)}: " + ex.Message);
                return null;
            }
        }

        public virtual async Task<T> Delete(int id)
        {
            try
            {
                var entity = await dbSet.FindAsync(id);
                if (entity != null)
                {
                    dbSet.Remove(entity);
                    return entity;
                }
                else
                {
                    _logger.LogError($"Error in {nameof(Delete)}: entity with id {id} not found.");
                    return null;
                }
            } catch (Exception ex)
            {
                _logger.LogError($"Error in {nameof(Delete)}: " + ex.Message);
                return null;
            }
        }

        public Task<T> Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
