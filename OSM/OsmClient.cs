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

    public OsmClient(IConfiguration configuration) : this(new OsmOptions(configuration))
    { }

    public OsmClient(OsmOptions options)
    {
        _options = options;
        
        var restClientOptions = new RestClientOptions(_options.ClientEndpoint)
        {
            MaxTimeout = 3000,
            Authenticator = new OsmAuthenticator(_options)
        };

        _client = new RestClient(restClientOptions);
    } 

    public async Task<IList<Badge>> GetBadgesAsync(string termId, BadgeType type)
    {
        var request = new RestRequest("/ext/badges/records/");
        request.Parameters.AddParameter(new QueryParameter("action", "getBadgeStructureByType"));
        request.Parameters.AddParameter(new QueryParameter("section", _options.Section));
        request.Parameters.AddParameter(new QueryParameter("section_id", _options.SectionId.ToString()));
        request.Parameters.AddParameter(new QueryParameter("term_id", termId));
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
            var coreCriteria = JsonDocumentExtensions.ToJsonDocument(criteriaNodes.Matches[1].Value).RootElement.ToJsonString();
            badge.Criteria = JsonSerializer.Deserialize<List<Criteria>>(coreCriteria);
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

    public async Task<IList<Member>> GetMembersAsync(string termId)
    {
        var request = new RestRequest("/ext/members/contact/");
        request.Parameters.AddParameter(new QueryParameter("action", "getListOfMembers"));
        request.Parameters.AddParameter(new QueryParameter("sort", "dob"));
        request.Parameters.AddParameter(new QueryParameter("sectionid", _options.SectionId.ToString()));
        request.Parameters.AddParameter(new QueryParameter("termid", termId));
        request.Parameters.AddParameter(new QueryParameter("section", _options.Section));
        var response = await _client.ExecuteGetAsync(request);
        var jsonString = response.Content ?? string.Empty;
        var node = JsonNode.Parse(jsonString);
        var memberNodes = JsonPath.Parse($"$['items']").Evaluate(node);

        if(memberNodes.Matches == null || !memberNodes.Matches.Any()) return Array.Empty<Member>();
        
        return memberNodes.Matches.SelectMany(match =>
            JsonSerializer.Deserialize<IEnumerable<Member>>(match.Value.ToJsonString())).ToList();
    }

    public async Task<IList<BadgeCompletion>> GetBadgeCompletion(string termId, string badgeId, string badgeVersion)
    {
        var request = new RestRequest("/ext/badges/records/");
        request.Parameters.AddParameter(new QueryParameter("action", "getBadgeRecords"));
        request.Parameters.AddParameter(new QueryParameter("sectionid", _options.SectionId.ToString()));
        request.Parameters.AddParameter(new QueryParameter("termid", termId));
        request.Parameters.AddParameter(new QueryParameter("section", _options.Section));
        request.Parameters.AddParameter(new QueryParameter("badge_id", badgeId));
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

    public async Task<IList<Member?>> GetPersonBadgeSummaryAsync(string termId)
    {
        var request = new RestRequest("/ext/badges/badgesbyperson/");
        request.Parameters.AddParameter(new QueryParameter("action", "loadBadgesByMember"));
        request.Parameters.AddParameter(new QueryParameter("section", _options.Section));
        request.Parameters.AddParameter(new QueryParameter("sectionid", _options.SectionId.ToString()));
        request.Parameters.AddParameter(new QueryParameter("term_id", termId));
        
        var response = await _client.ExecuteGetAsync(request);
        if(string.IsNullOrEmpty(response.Content)) return Array.Empty<Member>();
        var node = JsonNode.Parse(response.Content!);
        
        var evaluateResult = JsonPath.Parse($"$['data'][*]").Evaluate(node);
        
        if(evaluateResult.Matches == null || !evaluateResult.Matches.Any()) return Array.Empty<Member>();
        return evaluateResult.Matches.Select(match => JsonSerializer.Deserialize<Member>(match.Value.AsJsonString())).ToList();
    }
}