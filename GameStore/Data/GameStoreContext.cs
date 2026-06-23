using GameStore.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data;

//Establish a DB session between your API and SQLite database
//Estamos utilizando el patron "repository"
public class GameStoreContext(DbContextOptions<GameStoreContext> options)
    : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();

    public DbSet<Genre> Genres => Set<Genre>();
}
