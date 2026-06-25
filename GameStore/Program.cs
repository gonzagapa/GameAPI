using GameStore;
using GameStore.Endpoints;

var builder = WebApplication.CreateBuilder(args);

//Data Validation available in our Dtos
builder.Services.AddValidation();
builder.AddGameStoreDB();


var app = builder.Build();
app.MapGamesEndpoints();
app.MapGenreEndpoints();
app.MigrateDB();
app.Run();
