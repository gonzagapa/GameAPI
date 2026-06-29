using GameStore;
using GameStore.Endpoints;
using GameStore.Repository;
using GameStore.Repository.Interface;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

//Data Validation available in our Dtos
builder.Services.AddValidation();
builder.AddGameStoreDB();
builder.Services.AddScoped<IGameRepository, GameRepository>();

var app = builder.Build();

//Global exceptions handler
app.UseExceptionHandler(appException =>
{
   appException.Run(async context =>
   {
       context.Response.StatusCode = StatusCodes.Status500InternalServerError;  
       context.Response.ContentType = "application/json"; 

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>(); 
        if(contextFeature is not null)
       {
        Console.WriteLine($"Error:{contextFeature.Error}");    
        await context.Response.WriteAsJsonAsync(new
        {
            StatusCode = context.Response.StatusCode, 
            Message = "Internal Server Error"
        });
       }
   }) ;
});

app.MapGamesEndpoints();
app.MapGenreEndpoints();
app.MigrateDB();
app.Run();
