using GameStore.Data;
using GameStore.Dtos;
using GameStore.Models;

namespace GameStore.Endpoints;

public static class GameEndpoints
{
    const string GetGameEndpointName = "GetGame";
    static readonly List<GameDto> games = [
        new (1,"Street Figher II", "Fighting", 19.99M,new DateOnly(1992,7,15)),
        new (2, "Final Fantasy VII", "RPG", 69.99M, new DateOnly(2024,2,29)), 
        new(3, "Astro Bot","Fantasy", 50.00M, new DateOnly(2000,1,30))  
    ];

    //WebApplication is the object to be extended
    public static void MapGamesEndpoints(this WebApplication app)
    {
            //Grouping paths with a fixed prefix
            var group = app.MapGroup("/games");

            //GET /games
            group.MapGet("/", () =>
            {
                
            });


            // GET /games/{id} 
            group.MapGet("/{id}",(int id) =>
            {
                var game = games.Find(game => game.Id == id);

                return game is null ? Results.NotFound(game) : Results.Ok(game); 

            }).WithName(GetGameEndpointName);

            // POST /games
            group.MapPost("/",(CreateGameDto newGame, GameStoreContext dbContext) =>
            {
                Game game = new()
                {
                    Name = newGame.Name,
                    GenreId = newGame.GenreId,
                    Price = newGame.Price,
                    ReleaseDate = newGame.ReleaseDate
                } ;

                dbContext.Games.Add(game);
                dbContext.SaveChanges(); 

                GameDetailsDto gameDetails = new(game.Name,game.GenreId,game.Price, game.ReleaseDate);

                //NEVER EXPOSE THE INTERNAL DETAILS to clients, return the DTO
                return Results.CreatedAtRoute(GetGameEndpointName, new {id = game.Id},gameDetails);
            });

            // PUT /games/{id}
            group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
            {
                var index = games.FindIndex(game => game.Id == id); 

                if(index == -1)
                {
                    return Results.NotFound();
                }

                games[index] = new(id,
                updatedGame.Name, 
                updatedGame.Genre, 
                updatedGame.Price, 
                updatedGame.ReleaseDate); 

                return Results.NoContent();  
            });

            // DELETE /games/{id}
            group.MapDelete("/{id}", (int id) =>
            {
            games.RemoveAll(game => game.Id == id); 

            return Results.NoContent();
            });
    }


}
