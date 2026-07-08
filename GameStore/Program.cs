using GameStore;
using GameStore.Endpoints;
using GameStore.Repository;
using GameStore.Repository.Interface;
using GameStore.Services;
using GameStore.Services.Interface;
using Microsoft.AspNetCore.Diagnostics;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//Data Validation available in our Dtos
builder.Services.AddValidation();
builder.Services.AddOpenApi();
builder.AddGameStoreDB();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

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
app.MapUserEndponts(); 
app.MigrateDB();
app.Run();
