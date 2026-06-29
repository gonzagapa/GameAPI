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

    //WebApplication is the object to be extended
    public static void MapGamesEndpoints(this WebApplication app)
    {
            //Grouping paths with a fixed prefix
            var group = app.MapGroup("/games");

            //GET /games
            group.MapGet("/", async (IGameRepository repository) =>
            {
                var games = await repository.GetAllGamesSummary();
                return games;
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
            });

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
            });

            // DELETE /games/{id}
            group.MapDelete("/{id}", async (int id, IGameRepository repository) =>
            {
             
                await repository.DeleteByIdAsync(id);
                return Results.NoContent();
            });
    }


}
