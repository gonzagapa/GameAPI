using GameStore.Dtos;
using GameStore.Models;

namespace GameStore.Repository.Interface
{
    public interface IRepository<T> where T: class, IEntity
    {
        Task<IEnumerable<T>> GetAllAsync(); 

        Task<T> GetByIdAsync(int id); 

        Task AddAsync(T entity); 

        Task UpdateAsync(T entity);

        Task DeleteByIdAsync(int id);

        Task<PageResponseOffsetDto<T>> GetPaginatedOffsetEntity(int pageNumber=1, int pageSize=5);
    }
}