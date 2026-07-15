using System.Collections;
using GameStore.Dtos;
using GameStore.Models;
using GameStore.Dtos;

namespace GameStore.Repository.Interface
{
    public interface IGameRepository: IRepository<Game>
    {
        Task<IEnumerable<GameSummaryDto>> GetAllGamesSummary();
    }    
}
