using GameStore.Data;
using GameStore.Dtos;
using GameStore.Models;
using GameStore.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Repository
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly GameStoreContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(GameStoreContext dbContext)
        {
            _dbContext = dbContext;  
            _dbSet = dbContext.Set<T>();  
        }   
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity); 
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var item = await _dbSet.FindAsync(id);
            if(item is not null)
            {
                _dbSet.Remove(item);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync( id );
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
        } 

        public async Task<PageResponseOffsetDto<T>> GetPaginatedOffsetEntity(int pageNumber=1, int pageSize=5)
        {
            var entitiesPaginated =  await _dbSet
            .OrderBy(entity => entity.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();  

            var total = await _dbSet.CountAsync();

            PageResponseOffsetDto<T> response = new(pageNumber, pageSize,total, entitiesPaginated);

            return response;

        }
    }
}