using GameStore.Data;
using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore;

public static class DataExtension
{
    //Execute pending migrations
    public static void MigrateDB(this WebApplication app)
    {
        //Create an temporal service scope to retrieve your db context
        using var scope = app.Services.CreateScope(); 

        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>(); 
        dbContext.Database.Migrate();
    }

    public static void AddGameStoreDB(this WebApplicationBuilder builder)
    {
        var connString = builder.Configuration.GetConnectionString("GameStore");

        //Here we register our DBContext to the IServiceProvider container with Scoped lifetime
        //Why?: To prevent memory leaks, Db connections are expensive resource, isnt thread-safe and avoid concurrency issues.
        builder.Services.AddSqlite<GameStoreContext>(connString, optionsAction:options => options.UseSeeding((context, _) =>
        {
            if (!context.Set<Genre>().Any())
            {
                context.Set<Genre>().AddRange(
                    new Genre{Name = "Fighting"}, 
                    new Genre{Name = "RPG"},
                    new Genre{Name = "Platformer"},
                    new Genre{Name = "Racing"},
                    new Genre{Name = "Sports"}
                ); 

                context.SaveChanges();
            }

            if (!context.Set<Game>().Any())
            {
                context.Set<Game>().AddRange(
                    new Game {Name ="Street Figher II", GenreId = 1, Price= 19.99M, ReleaseDate= new DateOnly(1992,7,15)},
                    new Game {Name = "Final Fantasy VII", GenreId = 2, Price = 69.99M, ReleaseDate= new DateOnly(2024,2,29)}, 
                    new Game {Name = "Astro Bot",GenreId =3, Price = 50.00M, ReleaseDate = new DateOnly(2000,1,30)}
                );

                context.SaveChanges();
            }
        }));
    }
}
