using Microsoft.Extensions.Configuration;
using OSM.Configuration;
using OSM.Interfaces;
using OSM.Models;

namespace OSM.Tests;

public class GetDataTests
{
    private readonly OsmOptions _options;
    private readonly IOsmClient _client;
    public GetDataTests()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) 
            .AddJsonFile("appsettings.json") 
            .Build();
        
        _client = new OsmClient(config);
        _options = new OsmOptions(config);
    }
    
    [Fact]
    public void CheckConfigurationOptions()
    {
        Assert.NotNull(_options);
        Assert.NotEqual(string.Empty, _options.ClientId);
        Assert.NotEqual(string.Empty, _options.ClientSecret);
        Assert.NotNull(_options.Permissions);
        Assert.NotEmpty(_options.Permissions);
        Assert.NotNull(_options.Permissions.First());
        Assert.NotEqual(string.Empty, _options.Permissions.First().Section);
        Assert.Equal(AccessType.Read, _options.Permissions.First().AccessType);
    }
    
    [Fact]
    public async Task GetBadges()
    {
        var currentTerm = (await _client.GetTermsAsync()).ToArray().First(t => t.Current);
        var badges = await _client.GetBadgesAsync(currentTerm.Id, BadgeType.Challenge);
        
        Assert.NotNull(badges);
        Assert.NotEmpty(badges);

        var skillsBadge = badges.First(b => b.Name == "Skills");
        
        
        Assert.NotNull(skillsBadge);
        Assert.NotEmpty(skillsBadge.Criteria);
    }

    [Fact]
    public async Task GetTerms()
    {
        var terms = (await _client.GetTermsAsync()).ToArray();
        Assert.NotNull(terms);
        Assert.NotEmpty(terms);
        
        var currentTerm = terms.OrderByDescending(t => t.StartDate).First(t => t.Current);
        Assert.NotNull(currentTerm);
    }

    [Fact]
    public async Task GetMembers()
    {
        var currentTerm = (await _client.GetTermsAsync()).ToArray().First(t => t.Current);
        
        var members = (await _client.GetMembersAsync(currentTerm.Id)).ToArray();
        Assert.NotNull(members);
        Assert.NotEmpty(members);

        var me = members.First(m => m.LastName == "Nield");
        Assert.NotNull(me);
    }
    
    [Fact]
    public void BadgeProgress()
    {
        
    }
}