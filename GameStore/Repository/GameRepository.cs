using GameStore.Data;
using GameStore.Dtos;
using GameStore.Models;
using GameStore.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Repository
{
    public class GameRepository:Repository<Game>, IGameRepository
    {
        public GameRepository(GameStoreContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<GameSummaryDto>> GetAllGamesSummary()
        {
            return 
            await _dbContext
            .Games.Include(g => g.Genre)
                .Select( g=> new GameSummaryDto(g.Id, g.Name, g.Genre!.Name, g.Price, g.ReleaseDate))
                .AsNoTracking()
                .ToListAsync(); 
        }
    }
}