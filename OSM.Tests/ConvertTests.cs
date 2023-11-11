using System.Text.Json;
using NuGet.Frameworks;
using OSM.Models;
using OSM.Models.Converters;

namespace OSM.Tests;

public class ConvertTests
{
    [Fact]
    public void DeserializeBadgeCompletionRecord()
    {
        var json = "{\"scoutid\":2147236,\"firstname\":\"John\",\"lastname\":\"Doe\",\"awarded\":\"0\",\"completed\":\"0\",\"awardeddate\":\"0000-00-00\",\"photo_guid\":\"32563a02-35ec-421a-b2eb-a2a904b76164\",\"_114263\":\"YorkshireDalesTrip2022\",\"_114264\":\"SummerCamp2022-KentJamboree\",\"_114265\":\"GroupCamp2023\",\"_114267\":\"YorkshireDalesTrip2022\",\"_114268\":\"YorkshireDalesTrip2022\",\"_114269\":\"KentJamboree2022\",\"read_only\":true,\"eligibilty\":true,\"_filterString\":\"johndoe\",\"emailable\":true}";
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.Converters.Add(new BadgeAwardConverter());
        var badgeCompletion = JsonSerializer.Deserialize<BadgeCompletion>(json, options);
        
        Assert.NotNull(badgeCompletion);
        Assert.NotNull(badgeCompletion.Achievements);
        Assert.NotEmpty(badgeCompletion.Achievements);
        Assert.Equal(6, badgeCompletion.Achievements.Count);
    }

    [Fact]
    public void SerializeBadgeCompletion()
    {
        var badgeCompletion = new BadgeCompletion()
        {
            ScoutId = 123456,
            Achievements = new List<KeyValuePair<string, string>>()
            {
                new ("0234567","First Value"),
                new ("0345678", "Second Value")
            }
        };

        var scoutJson = JsonSerializer.Serialize(badgeCompletion);
        
        Assert.NotEqual(string.Empty, scoutJson);

    }
}