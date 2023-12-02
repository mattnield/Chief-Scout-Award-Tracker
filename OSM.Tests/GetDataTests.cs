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
        var badges = _client.GetBadgesByType(BadgeType.Challenge);
        
        Assert.NotNull(badges);
        Assert.NotEmpty(badges);

        var skillsBadge = badges.First(b => b.Name == "Skills");
        
        
        Assert.NotNull(skillsBadge);
        Assert.NotEmpty(skillsBadge.Criteria);
    }
    
    
    [Fact]
    public async Task GetBadgeSummaries()
    {
        var badges = await _client.GetPersonBadgeSummaryAsync();
        var challengeBadges = _client.GetBadgesByType(BadgeType.Challenge);
        
        Assert.NotNull(badges);
        Assert.NotEmpty(badges);

        var member = badges.First();
        var memberBadges = member.Badges.Where(b => b.BadgeType == BadgeType.Challenge);
        
        // Assert.Equal(challengeBadges.Count, memberBadges.Count()); // This is only equal when _all_ badges of the type are complete 
        Assert.NotNull(member);
        
        Assert.NotNull(member.Badges);
        Assert.NotEmpty(member.Badges);
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
        var members = (await _client.GetMembersAsync()).ToArray();
        Assert.NotNull(members);
        Assert.NotEmpty(members);

        var me = members.First(m => m.LastName == "Nield");
        Assert.NotNull(me);
    }
    
    
    [Fact]
    public async Task GetPatrolsWithMembers()
    {
        var patrols = (await _client.GetPatrols()).ToArray();
        Assert.NotNull(patrols);
        Assert.NotEmpty(patrols);
    }
    
    [Fact]
    public async Task BadgeCompletion()
    {
        var skillsBadge =
            _client.GetBadgesByType(BadgeType.Challenge).First(b => b.Name == "Skills");

        var completion = await _client.GetBadgeCompletion(skillsBadge.Id, skillsBadge.Version);
        
        Assert.NotNull(completion);
        Assert.NotEmpty(completion);
    }

    [Fact]
    public async Task GetMember()
    {
        var member = await _client.GetMemberAsync(756599);
        
        Assert.NotNull(member);
    }
}