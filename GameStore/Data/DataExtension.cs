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
                    new Genre{Name = "Sports"},
                    new Genre{Name = "Action"},
                    new Genre{Name = "Adventure"},
                    new Genre{Name = "Shooter"},
                    new Genre{Name = "Strategy"},
                    new Genre{Name = "Simulation"},
                    new Genre{Name = "Puzzle"},
                    new Genre{Name = "Horror"},
                    new Genre{Name = "Survival"},
                    new Genre{Name = "MMO"},
                    new Genre{Name = "Sandbox"}
                ); 

                context.SaveChanges();
            }

            if (!context.Set<Game>().Any())
            {
                context.Set<Game>().AddRange(
                    new Game {Name ="Street Figher II", GenreId = 1, Price= 19.99M, ReleaseDate= new DateOnly(1992,7,15)},
                    new Game {Name = "Final Fantasy VII", GenreId = 2, Price = 69.99M, ReleaseDate= new DateOnly(2024,2,29)}, 
                    new Game {Name = "Astro Bot",GenreId =3, Price = 50.00M, ReleaseDate = new DateOnly(2000,1,30)},
                    new Game {Name = "Super Mario 64", GenreId = 3, Price = 59.99M, ReleaseDate = new DateOnly(1996, 9, 29)},
                    new Game {Name = "Crash Bandicoot", GenreId = 3, Price = 39.99M, ReleaseDate = new DateOnly(1996, 9, 9)},
                    new Game {Name = "Donkey Kong Country", GenreId = 3, Price = 49.99M, ReleaseDate = new DateOnly(1994, 11, 21)},
                    new Game {Name = "Sonic the Hedgehog", GenreId = 3, Price = 19.99M, ReleaseDate = new DateOnly(1991, 6, 23)},
                    new Game {Name = "Rayman Legends", GenreId = 3, Price = 29.99M, ReleaseDate = new DateOnly(2013, 8, 29)},
                    new Game {Name = "Super Meat Boy", GenreId = 3, Price = 14.99M, ReleaseDate = new DateOnly(2010, 10, 20)},
                    new Game {Name = "Celeste", GenreId = 3, Price = 19.99M, ReleaseDate = new DateOnly(2018, 1, 25)},
                    new Game {Name = "Hollow Knight", GenreId = 3, Price = 14.99M, ReleaseDate = new DateOnly(2017, 2, 24)},
                    new Game {Name = "Tekken 3", GenreId = 1, Price = 49.99M, ReleaseDate = new DateOnly(1997, 3, 20)},
                    new Game {Name = "Mortal Kombat 11", GenreId = 1, Price = 59.99M, ReleaseDate = new DateOnly(2019, 4, 23)},
                    new Game {Name = "Super Smash Bros. Melee", GenreId = 1, Price = 59.99M, ReleaseDate = new DateOnly(2001, 11, 21)},
                    new Game {Name = "Guilty Gear Strive", GenreId = 1, Price = 59.99M, ReleaseDate = new DateOnly(2021, 6, 11)},
                    new Game {Name = "Soulcalibur V", GenreId = 1, Price = 39.99M, ReleaseDate = new DateOnly(2012, 1, 31)},
                    new Game {Name = "Injustice 2", GenreId = 1, Price = 49.99M, ReleaseDate = new DateOnly(2017, 5, 11)},
                    new Game {Name = "Dragon Ball FighterZ", GenreId = 1, Price = 59.99M, ReleaseDate = new DateOnly(2018, 1, 26)},
                    new Game {Name = "The Witcher 3", GenreId = 2, Price = 39.99M, ReleaseDate = new DateOnly(2015, 5, 19)},
                    new Game {Name = "Skyrim", GenreId = 2, Price = 39.99M, ReleaseDate = new DateOnly(2011, 11, 11)},
                    new Game {Name = "Persona 5", GenreId = 2, Price = 59.99M, ReleaseDate = new DateOnly(2016, 9, 15)},
                    new Game {Name = "Chrono Trigger", GenreId = 2, Price = 14.99M, ReleaseDate = new DateOnly(1995, 3, 11)},
                    new Game {Name = "Mass Effect 2", GenreId = 2, Price = 29.99M, ReleaseDate = new DateOnly(2010, 1, 26)},
                    new Game {Name = "Fallout: New Vegas", GenreId = 2, Price = 9.99M, ReleaseDate = new DateOnly(2010, 10, 19)},
                    new Game {Name = "Baldur's Gate 3", GenreId = 2, Price = 59.99M, ReleaseDate = new DateOnly(2023, 8, 3)},
                    new Game {Name = "Cyberpunk 2077", GenreId = 2, Price = 59.99M, ReleaseDate = new DateOnly(2020, 12, 10)},
                    new Game {Name = "Gran Turismo 4", GenreId = 4, Price = 49.99M, ReleaseDate = new DateOnly(2004, 12, 28)},
                    new Game {Name = "Forza Horizon 5", GenreId = 4, Price = 59.99M, ReleaseDate = new DateOnly(2021, 11, 9)},
                    new Game {Name = "Mario Kart 8 Deluxe", GenreId = 4, Price = 59.99M, ReleaseDate = new DateOnly(2017, 4, 28)},
                    new Game {Name = "Need for Speed: Most Wanted", GenreId = 4, Price = 19.99M, ReleaseDate = new DateOnly(2005, 11, 11)},
                    new Game {Name = "Burnout 3: Takedown", GenreId = 4, Price = 19.99M, ReleaseDate = new DateOnly(2004, 9, 7)},
                    new Game {Name = "Dirt Rally 2.0", GenreId = 4, Price = 29.99M, ReleaseDate = new DateOnly(2019, 2, 26)},
                    new Game {Name = "F1 23", GenreId = 4, Price = 69.99M, ReleaseDate = new DateOnly(2023, 6, 16)},
                    new Game {Name = "Assetto Corsa", GenreId = 4, Price = 19.99M, ReleaseDate = new DateOnly(2014, 12, 19)},
                    new Game {Name = "FIFA 23", GenreId = 5, Price = 59.99M, ReleaseDate = new DateOnly(2022, 9, 27)},
                    new Game {Name = "NBA 2K23", GenreId = 5, Price = 59.99M, ReleaseDate = new DateOnly(2022, 9, 8)},
                    new Game {Name = "Madden NFL 24", GenreId = 5, Price = 69.99M, ReleaseDate = new DateOnly(2023, 8, 15)},
                    new Game {Name = "Tony Hawk's Pro Skater 2", GenreId = 5, Price = 39.99M, ReleaseDate = new DateOnly(2000, 9, 19)},
                    new Game {Name = "Wii Sports", GenreId = 5, Price = 49.99M, ReleaseDate = new DateOnly(2006, 11, 19)},
                    new Game {Name = "Rocket League", GenreId = 5, Price = 19.99M, ReleaseDate = new DateOnly(2015, 7, 7)},
                    new Game {Name = "MLB The Show 23", GenreId = 5, Price = 59.99M, ReleaseDate = new DateOnly(2023, 3, 28)},
                    new Game {Name = "PGA Tour 2K23", GenreId = 5, Price = 59.99M, ReleaseDate = new DateOnly(2022, 10, 11)},
                    new Game {Name = "NHL 23", GenreId = 5, Price = 59.99M, ReleaseDate = new DateOnly(2022, 10, 11)}
                );

                context.SaveChanges();
            }
        }));
    }
}
