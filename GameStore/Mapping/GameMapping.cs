using GameStore.Dtos;
using GameStore.Models;

namespace GameStore.Mapping
{
    public static class GameMapping
    {
        public static GameDetailsDto MapGameDetailsDto(this Game game)
        {
            return new(game.Name,game.GenreId,game.Price, game.ReleaseDate);
        }
    }
}