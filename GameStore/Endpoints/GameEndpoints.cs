using GameStore.Data;
using GameStore.Dtos;
using GameStore.Mapping;
using GameStore.Models;
using GameStore.Repository;
using GameStore.Repository.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Endpoints;

public static class GameEndpoints
{
    const string GetGameEndpointName = "GetGame";

    //WebApplication is the object to be extended
    public static void MapGamesEndpoints(this WebApplication app)
    {
            //Grouping paths with a fixed prefix
            var group = app.MapGroup("/games").WithTags("Games");

            //GET /games
            // We limit the amount of items to return by 30
            group.MapGet("/", async Task<Ok<PageResponseOffsetDto<Game>>> (
                IGameRepository repository,
                PaginationParamsDto pagination) =>
            {
                
                var games = await repository.GetPaginatedOffsetEntity(pagination.PageNumber, pagination.PageSize);

                return TypedResults.Ok(games);
            });

            // GET /games/{id:int} -> route constraint 
            group.MapGet("/{id:int}",async  Task<Results<Ok<GameDetailsDto>, NotFound>> (
                int id, 
                IGameRepository repository) =>
            {
                var game = await repository.GetByIdAsync(id);

                if(game is null)
                {
                    return TypedResults.NotFound();
                }
                GameDetailsDto gameDetails = game.MapGameDetailsDto();
                return TypedResults.Ok(gameDetails); 

            }).WithName(GetGameEndpointName);

            // POST /games
            group.MapPost("/",async Task<CreatedAtRoute<GameDetailsDto>> (
                CreateGameDto newGame, 
                IGameRepository repository) =>
            {
                Game game = new()
                {
                    Name = newGame.Name,
                    GenreId = newGame.GenreId,
                    Price = newGame.Price,
                    ReleaseDate = newGame.ReleaseDate
                } ;

               await repository.AddAsync(game);
                
                var dto = game.MapGameDetailsDto();

                return TypedResults.CreatedAtRoute(dto, GetGameEndpointName, new { id = game.Id });
            }).RequireAuthorization();

            // PUT /games/{id}
            group.MapPut("/{id:int}", async Task<Results<NotFound,NoContent>> (
                int id, 
                UpdateGameDto updatedGame, 
                IGameRepository repository) =>
            {
                var games = await repository.GetByIdAsync(id);

                if(games is null)
                {
                    return TypedResults.NotFound();
                }

               games.GenreId = updatedGame.GenreId; 
               games.Name = updatedGame.Name; 
               games.ReleaseDate = updatedGame.ReleaseDate; 
               games.Price = updatedGame.Price;


                await repository.UpdateAsync(games);
                return TypedResults.NoContent();  
            }).RequireAuthorization();

            // DELETE /games/{id}
            group.MapDelete("/{id:int}", async Task<NoContent> (
                int id, IGameRepository repository) =>
            {
             
                await repository.DeleteByIdAsync(id);
                return TypedResults.NoContent();
            }).RequireAuthorization();
    }


}
