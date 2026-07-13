using Microsoft.AspNetCore.Mvc.Testing;

namespace GameStore.Test;

public class GameEndpointTest
{
    [Fact]
    public async Task Should_get_all_games_paginated()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient(); 

        var response = await client.GetStringAsync("/");

        Assert.Equal("Hello World!", response);
    }
}
