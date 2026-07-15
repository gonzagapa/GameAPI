using GameStore.Data;
using GameStore.Dtos;
using GameStore.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GameStore.Endpoints;

public static class GenreEndpoints
{
    public static void MapGenreEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/genre").WithTags("Genre"); 

        //GET /genre
        group.MapGet("/",async Task<Ok<List<GenreDto>>> (GameStoreContext dbContext, IMemoryCache _cache)=>
        {
            var cacheKey = "/genre";
            if(!_cache.TryGetValue(cacheKey, out List<GenreDto>? genrers))
            {
                genrers = await dbContext.Genres.Select(genre => new GenreDto(genre.Id,genre.Name))
                .AsNoTracking()
                .ToListAsync();

    
                //configure absolute expiration: expires after fixed time
                var cacheEntryOption = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                _cache.Set(cacheKey, genrers, cacheEntryOption);
            }
             return TypedResults.Ok(genrers);   
            
        }
        ).RequireAuthorization();
    }
}
