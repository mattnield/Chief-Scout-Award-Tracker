using System.Text.Json;
using System.Text.Json.Nodes;
using Json.More;
using Json.Path;
using Microsoft.Extensions.Configuration;
using OSM.Configuration;
using OSM.Interfaces;
using OSM.Models;
using OSM.Models.Converters;
using RestSharp;

namespace OSM;

public class OsmClient : IOsmClient
{
    private readonly RestClient _client;
    private readonly OsmOptions _options;
    private readonly Term _currentTerm;
    private readonly IList<Badge> _badges;

    public string SectionName { get; set; }
    public string SectionShortName { get; set; }

    public Term CurrentTerm
    {
        get => _currentTerm;
    }

    public OsmClient(IConfiguration configuration) : this(new OsmOptions(configuration))
    { }

    public OsmClient(OsmOptions options)
    {
        SectionName = options.SectionName;
        SectionShortName = options.SectionShortName;
        
        _options = options;
        
        var restClientOptions = new RestClientOptions(_options.ClientEndpoint)
        {
            MaxTimeout = 3000,
            Authenticator = new OsmAuthenticator(_options)
        };

        _client = new RestClient(restClientOptions);
        var allTerms = GetTermsAsync().Result.ToArray();
        _currentTerm = allTerms.Any(t => t.Current) ? allTerms.First(t => t.Current) : allTerms.OrderByDescending(t => t.StartDate).First();

        _badges = GetBadgesAsync(BadgeType.Challenge).Result
            .Concat(GetBadgesAsync(BadgeType.Activity).Result)
            .Concat(GetBadgesAsync(BadgeType.Staged).Result)
            .Concat(GetBadgesAsync(BadgeType.Core).Result).ToArray();
    }

    public IList<Badge> GetBadgesByType(BadgeType badgeType)
    {
        return _badges.Where(b => b.BadgeType == badgeType).ToList();
    }
    private async Task<IList<Badge>> GetBadgesAsync(BadgeType type)
    {
        var request = new RestRequest("/ext/badges/records/");
        request.Parameters.AddParameter(new QueryParameter("action", "getBadgeStructureByType"));
        request.Parameters.AddParameter(new QueryParameter("section", _options.Section));
        request.Parameters.AddParameter(new QueryParameter("section_id", _options.SectionId.ToString()));
        request.Parameters.AddParameter(new QueryParameter("term_id", _currentTerm.Id));
        request.Parameters.AddParameter(new QueryParameter("type_id", ((int) type).ToString()));
        var response = await _client.ExecuteGetAsync(request);
        var jsonString = response.Content ?? string.Empty;
        var node = JsonNode.Parse(jsonString);
        var badgeNodes = JsonPath.Parse($"$['details'][?(@.type_id=='{(int)type}')]").Evaluate(node);

        if(badgeNodes.Matches == null || !badgeNodes.Matches.Any()) return Array.Empty<Badge>();
        
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonConverters.EnumStringConverter<BadgeType>() }
        };

        var badges = new List<Badge>();
        foreach (var match in badgeNodes.Matches.Where(m => m.Value != null))
        {
            var badge = JsonSerializer.Deserialize<Badge>(match.Value!.ToJsonString(), options);
            var criteriaNodes = JsonPath.Parse($"$['structure']['{badge.Id}_{badge.Version}']..['rows']").Evaluate(node);
            if(criteriaNodes.Matches.Count > 1)
            {
                var coreCriteria = JsonDocumentExtensions.ToJsonDocument(criteriaNodes.Matches[1].Value).RootElement
                    .ToJsonString();
                badge.Criteria = JsonSerializer.Deserialize<List<Criteria>>(coreCriteria);
            }
            badges.Add(badge);
        }
        
        return badges;
    }

    public async Task<IList<Term>> GetTermsAsync()
    {
        var request = new RestRequest("/ext/generic/startup/");
        request.Parameters.AddParameter(new QueryParameter("action", "getData"));
        var response = await _client.ExecuteGetAsync(request);
        var jsonString = response.Content?[18..] ?? string.Empty;
        var node = JsonNode.Parse(jsonString);
        var termsNodes = JsonPath.Parse($"$..['terms']['{_options.SectionId}']").Evaluate(node);

        if(termsNodes.Matches == null || !termsNodes.Matches.Any()) return Array.Empty<Term>();
        var terms = termsNodes.Matches.SelectMany(match =>
            JsonSerializer.Deserialize<IEnumerable<Term>>(match.Value.ToJsonString()));
        return terms.ToList();
    }

    public async Task<Member?> GetMemberAsync(int memberId)
    {
        
        var request = new RestRequest("/ext/members/contact/");
        request.Parameters.AddParameter(new QueryParameter("action", "getIndividual"));
        request.Parameters.AddParameter(new QueryParameter("scoutid", memberId.ToString()));
        request.Parameters.AddParameter(new QueryParameter("sectionid", _options.SectionId.ToString()));
        request.Parameters.AddParameter(new QueryParameter("termid", _currentTerm.Id));
        request.Parameters.AddParameter(new QueryParameter("context", "members"));
        var response = await _client.ExecuteGetAsync(request);
        var jsonString = response.Content ?? string.Empty;
        var node = JsonNode.Parse(jsonString);
        var memberNodes = JsonPath.Parse($"$['data']").Evaluate(node);
        
        if(memberNodes.Matches == null || !memberNodes.Matches.Any()) return null;
        var s = memberNodes.Matches.First().Value.ToJsonString();
        return JsonSerializer.Deserialize<Member>(s);
    }

    public async Task<IList<Member>> GetMembersAsync()
    {
        var request = new RestRequest("/ext/members/contact/");
        request.Parameters.AddParameter(new QueryParameter("action", "getListOfMembers"));
        request.Parameters.AddParameter(new QueryParameter("sort", "dob"));
        request.Parameters.AddParameter(new QueryParameter("sectionid", _options.SectionId.ToString()));
        request.Parameters.AddParameter(new QueryParameter("termid", _currentTerm.Id));
        request.Parameters.AddParameter(new QueryParameter("section", _options.Section));
        var response = await _client.ExecuteGetAsync(request);
        var jsonString = response.Content ?? string.Empty;
        var node = JsonNode.Parse(jsonString);
        var memberNodes = JsonPath.Parse($"$['items']").Evaluate(node);

        if(memberNodes.Matches == null || !memberNodes.Matches.Any()) return Array.Empty<Member>();
        
        return memberNodes.Matches.SelectMany(match =>
            JsonSerializer.Deserialize<IEnumerable<Member>>(match.Value.ToJsonString())).ToList();
    }

    public async Task<IList<BadgeCompletion>> GetBadgeCompletion(int badgeId, string badgeVersion)
    {
        var request = new RestRequest("/ext/badges/records/");
        request.Parameters.AddParameter(new QueryParameter("action", "getBadgeRecords"));
        request.Parameters.AddParameter(new QueryParameter("sectionid", _options.SectionId.ToString()));
        request.Parameters.AddParameter(new QueryParameter("termid", _currentTerm.Id));
        request.Parameters.AddParameter(new QueryParameter("section", _options.Section));
        request.Parameters.AddParameter(new QueryParameter("badge_id", badgeId.ToString()));
        request.Parameters.AddParameter(new QueryParameter("badge_version", badgeVersion));
        request.Parameters.AddParameter(new QueryParameter("underscores", null));
        
        var response = await _client.ExecuteGetAsync(request);
        if(string.IsNullOrEmpty(response.Content)) return Array.Empty<BadgeCompletion>();
        var node = JsonNode.Parse(response.Content!);
        
        var evaluateResult = JsonPath.Parse($"$['items'][*]").Evaluate(node);

        if(evaluateResult.Matches == null || !evaluateResult.Matches.Any()) return Array.Empty<BadgeCompletion>();
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.Converters.Add(new BadgeAwardConverter());
        var output = evaluateResult.Matches
            .Select(match =>
                JsonSerializer.Deserialize<BadgeCompletion>(match.Value.AsJsonString(), options))
            .Where(result => result != null);

        return output.ToList();
    }

    public async Task<IList<Member?>> GetPersonBadgeSummaryAsync()
    {
        var request = new RestRequest("/ext/badges/badgesbyperson/");
        request.Parameters.AddParameter(new QueryParameter("action", "loadBadgesByMember"));
        request.Parameters.AddParameter(new QueryParameter("section", _options.Section));
        request.Parameters.AddParameter(new QueryParameter("sectionid", _options.SectionId.ToString()));
        request.Parameters.AddParameter(new QueryParameter("term_id", _currentTerm.Id));
        
        var response = await _client.ExecuteGetAsync(request);
        if(string.IsNullOrEmpty(response.Content)) return Array.Empty<Member>();
        var node = JsonNode.Parse(response.Content!);
        
        var evaluateResult = JsonPath.Parse($"$['data'][*]").Evaluate(node);
        
        if(evaluateResult.Matches == null || !evaluateResult.Matches.Any()) return Array.Empty<Member>();
        return evaluateResult.Matches.Select(match => JsonSerializer.Deserialize<Member>(match.Value.AsJsonString())).ToList();
    }

    public async Task<IList<Patrol>> GetPatrols(bool includeNoPatrol = true)
    {

        var request = new RestRequest("/ext/members/patrols/");
        request.Parameters.AddParameter(new QueryParameter("action", "getPatrolsWithPeople"));
        request.Parameters.AddParameter(new QueryParameter("sectionid", _options.SectionId.ToString()));
        request.Parameters.AddParameter(new QueryParameter("termid", _currentTerm.Id));
        request.Parameters.AddParameter(new QueryParameter("include_no_patrol", includeNoPatrol ? "y" : "n"));

        var response = await _client.ExecuteGetAsync(request);
        if (string.IsNullOrEmpty(response.Content)) return new List<Patrol>();
        var node = JsonNode.Parse(response.Content!);

        var evaluateResult = JsonPath.Parse($"$[*]").Evaluate(node);

        if (evaluateResult.Matches == null || !evaluateResult.Matches.Any()) return Array.Empty<Patrol>();
        return evaluateResult.Matches.Select(match => JsonSerializer.Deserialize<Patrol>(match.Value.AsJsonString()))
            .ToList();
    }
}