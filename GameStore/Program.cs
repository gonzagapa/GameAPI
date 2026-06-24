using GameStore;
using GameStore.Data;
using GameStore.Endpoints;
using GameStore.Models;

var builder = WebApplication.CreateBuilder(args);

//Data Validation available in our Dtos
builder.Services.AddValidation();

var connString = "Data Source=GameStore.db";
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

var app = builder.Build();
app.MapGamesEndpoints();
app.MigrateDB();
app.Run();
