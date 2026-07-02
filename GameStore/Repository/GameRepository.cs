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

        public async Task<PageResponseOffsetDto<Game>> GetPaginatedOffsetGame(int pageNumber=1, int pageSize=5)
        {
            var games =  await _dbContext
            .Games
            .OrderBy(g => g.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();  

            var total = await _dbContext.Games.CountAsync();

            PageResponseOffsetDto<Game> response = new(pageNumber, pageSize,total, games);

            return response;

        }
    }
}