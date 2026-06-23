using GameStore.Endpoints;

var builder = WebApplication.CreateBuilder(args);

//Data Validation available in our Dtos
builder.Services.AddValidation();

var app = builder.Build();
app.MapGamesEndpoints();
app.Run();
