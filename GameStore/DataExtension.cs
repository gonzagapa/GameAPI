using GameStore.Data;
using Microsoft.EntityFrameworkCore;

namespace GameStore;

public static class DataExtension
{
    //Execute pending migrations
    public static void MigrateDB(this WebApplication app)
    {
        using var scope = app.Services.CreateScope(); 
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>(); 
        dbContext.Database.Migrate();
    }
}
