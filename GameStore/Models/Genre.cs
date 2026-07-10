namespace GameStore.Models;

public class Genre : IEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }
}
