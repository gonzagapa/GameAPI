namespace GameStore.Dtos;

public record GameDetailsDto
(
    string Name, 
     int GenreId, 
     decimal Price, 
    DateOnly ReleaseDate
);
