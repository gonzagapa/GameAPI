using GameStore;
using GameStore.Data;
using GameStore.Endpoints;
using GameStore.Models;

var builder = WebApplication.CreateBuilder(args);

//Data Validation available in our Dtos
builder.Services.AddValidation();
builder.AddGameStoreDB();


var app = builder.Build();
app.MapGamesEndpoints();
app.MigrateDB();
app.Run();
