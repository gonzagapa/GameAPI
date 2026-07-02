using GameStore.Dtos;

namespace GameStore.Repository.Interface
{
    public interface IRepository<TEntity> where TEntity: class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(); 

        Task<TEntity> GetByIdAsync(int id); 

        Task AddAsync(TEntity entity); 

        Task UpdateAsync(TEntity entity);

        Task DeleteByIdAsync(int id);
    }
}