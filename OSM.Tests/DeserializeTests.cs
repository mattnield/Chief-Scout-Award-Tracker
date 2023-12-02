using System.Text.Json;
using OSM.Models;

namespace OSM.Tests;

public class DeserializeTests
{
    [Fact]
    public void DeserializeBadge()
    {
        var jsonString = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestData/badge.json"));

        var badge = JsonSerializer.Deserialize<Badge>(jsonString);
        
        Assert.NotNull(badge);
        if (badge == null) return;
        
        Assert.Equal(1539, badge.Id);
        Assert.Equal("Chief Scout's Gold", badge.Name);
    }
}