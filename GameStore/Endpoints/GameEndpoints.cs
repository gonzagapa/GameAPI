using GameStore.Data;
using GameStore.Dtos;
using GameStore.Models;
using GameStore.Repository;
using GameStore.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Endpoints;

public static class GameEndpoints
{
    const string GetGameEndpointName = "GetGame";
    const int MAX_PAGE_SIZE = 30;
    const int MIN_PAGE_SIZE = 5;

    //WebApplication is the object to be extended
    public static void MapGamesEndpoints(this WebApplication app)
    {
            //Grouping paths with a fixed prefix
            var group = app.MapGroup("/games");

            //GET /games
            // We limit the amount of items to return by 30
            group.MapGet("/", async (IGameRepository repository, int? pageNumber, int? pageSize) =>
            {
                pageSize = pageSize > MAX_PAGE_SIZE ? MIN_PAGE_SIZE: pageSize;

                var games = await repository.GetPaginatedOffsetEntity(pageNumber ?? 1, pageSize ?? 5);
                
                return Results.Ok(games);
            });

            //ALWAYS RETURN A DTO, VOID RETURNING INTERNAL REPRESENTATION
            // GET /games/{id} 
            group.MapGet("/{id}",async (int id, IGameRepository repository) =>
            {
                var game = await repository.GetByIdAsync(id);

                if(game is null)
                {
                    return Results.NotFound();
                }
                GameDetailsDto gameDetails = new(game.Name,game.GenreId,game.Price, game.ReleaseDate);
                return Results.Ok(gameDetails); 

            }).WithName(GetGameEndpointName);

            // POST /games
            group.MapPost("/",async (CreateGameDto newGame, IGameRepository repository) =>
            {
                Game game = new()
                {
                    Name = newGame.Name,
                    GenreId = newGame.GenreId,
                    Price = newGame.Price,
                    ReleaseDate = newGame.ReleaseDate
                } ;

               await repository.AddAsync(game);
                GameDetailsDto gameDetails = new(game.Name,game.GenreId,game.Price, game.ReleaseDate);

                return Results.CreatedAtRoute(GetGameEndpointName, new {id = game.Id},gameDetails);
            }).RequireAuthorization();

            // PUT /games/{id}
            group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, IGameRepository repository) =>
            {
                var games = await repository.GetByIdAsync(id);

                if(games is null)
                {
                    return Results.NotFound();
                }

               games.GenreId = updatedGame.GenreId; 
               games.Name = updatedGame.Name; 
               games.ReleaseDate = updatedGame.ReleaseDate; 
               games.Price = updatedGame.Price;


                await repository.UpdateAsync(games);
                return Results.NoContent();  
            }).RequireAuthorization();

            // DELETE /games/{id}
            group.MapDelete("/{id}", async (int id, IGameRepository repository) =>
            {
             
                await repository.DeleteByIdAsync(id);
                return Results.NoContent();
            }).RequireAuthorization();
    }


}
