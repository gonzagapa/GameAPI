using GameStore.Dtos;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<GameDto> games = [
  new (1,"Street Figher II", "Fighting", 19.99M,new DateOnly(1992,7,15)),
  new (2, "Final Fantasy VII", "RPG", 69.99M, new DateOnly(2024,2,29)), 
  new(3, "Astro Bot","Fantasy", 50.00M, new DateOnly(2000,1,30))  
]; 


//GET /games
app.MapGet("/games", () => games);

app.Run();
