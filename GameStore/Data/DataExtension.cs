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
        }));
    }
}
