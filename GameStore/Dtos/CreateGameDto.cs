using System.ComponentModel.DataAnnotations;

namespace GameStore.Dtos;
public record CreateGameDto(
    [Required][StringLength(50)] string Name, 
    [Range(1,50)] int GenreId, 
    [Range(1,1000)] decimal Price, 
    DateOnly ReleaseDate
);