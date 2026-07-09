using GameStore.Data;
using GameStore.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Endpoints;

public static class GenreEndpoints
{
    public static void MapGenreEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/genre"); 

        //TODO: implement in-memory cache: https://dotnettutorials.net/lesson/how-to-implement-in-memory-caching-in-asp-net-core-web-api/
        //GET /genre
        group.MapGet("/",async(GameStoreContext dbContext)=> 
            await dbContext.Genres
            .Select(genre => new GenreDto(genre.Id,genre.Name))
            .AsNoTracking()
            .ToListAsync()
        );
    }
}
