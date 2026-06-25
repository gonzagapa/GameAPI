using GameStore.Data;
using GameStore.Dtos;
using GameStore.Models;
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
            group.MapGet("/", async (GameStoreContext dbContext) =>
            {
                var games = await dbContext.Games
                .Include(g => g.Genre)
                //Proyection of Data
                .Select( g=> new GameSummaryDto(g.Id, g.Name, g.Genre!.Name, g.Price, g.ReleaseDate))
                .AsNoTracking()
                .ToListAsync(); 


                return games;
            });

            //ALWAYS RETURN A DTO, VOID RETURNING INTERNAL REPRESENTATION
            // GET /games/{id} 
            group.MapGet("/{id}",async (int id, GameStoreContext dbContext) =>
            {
                var game = await dbContext.Games.FindAsync(id);

                if(game is null)
                {
                    return Results.NotFound();
                }
                GameDetailsDto gameDetails = new(game.Name,game.GenreId,game.Price, game.ReleaseDate);
                return Results.Ok(gameDetails); 

            }).WithName(GetGameEndpointName);

            // POST /games
            group.MapPost("/",async (CreateGameDto newGame, GameStoreContext dbContext) =>
            {
                Game game = new()
                {
                    Name = newGame.Name,
                    GenreId = newGame.GenreId,
                    Price = newGame.Price,
                    ReleaseDate = newGame.ReleaseDate
                } ;

                await dbContext.Games.AddAsync(game);
                await dbContext.SaveChangesAsync(); 

                GameDetailsDto gameDetails = new(game.Name,game.GenreId,game.Price, game.ReleaseDate);

                return Results.CreatedAtRoute(GetGameEndpointName, new {id = game.Id},gameDetails);
            });

            // PUT /games/{id}
            group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
            {
                var games = await dbContext.Games.FindAsync(id);

                if(games is null)
                {
                    return Results.NotFound();
                }

               games.GenreId = updatedGame.GenreId; 
               games.Name = updatedGame.Name; 
               games.ReleaseDate = updatedGame.ReleaseDate; 
               games.Price = updatedGame.Price;


                await dbContext.SaveChangesAsync();

                return Results.NoContent();  
            });

            // DELETE /games/{id}
            group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
            {
             
             await dbContext.Games.Where( game => game.Id == id)
             .ExecuteDeleteAsync(); 

             await dbContext.SaveChangesAsync();

            return Results.NoContent();
            });
    }


}
