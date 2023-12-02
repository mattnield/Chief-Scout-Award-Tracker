using System.Security.AccessControl;
using System.Text;
using Microsoft.Extensions.Configuration;
using OSM;
using OSM.Configuration;
using OSM.Models;
using Spectre.Console;

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory()) 
    .AddJsonFile("appsettings.json") 
    .Build();
        
var _client = new OsmClient(config);

// Get current challenge badges
var badges = _client.GetBadgesByType(BadgeType.Challenge);

foreach (var badge in badges)
{
    Console.WriteLine($"- {badge.Id}_{badge.Version}:{badge.Name}");
}
// Get current members
var members = (await _client.GetMembersAsync()).Where(m => m.PatrolId >= 0);
var table = new Table();
table.AddColumns("Badge", "Module", "Task");
table.AddColumns(members.Select(member => string.Join(' ', member.FirstName, member.Initial)).ToArray());


StringBuilder csvStringBuilder = new StringBuilder();
csvStringBuilder.AppendFormat("Badge,Module,Task,{0}\n", string.Join(',', members.Select(member => string.Join(' ', member.FirstName, member.Initial)).ToArray()));

foreach (var badge in badges)
{
    var badgeCompletion = await _client.GetBadgeCompletion(badge.Id, badge.Version);
    foreach (var criteria in badge.Criteria)
    {
        csvStringBuilder.AppendJoin(',', $"\"{badge.Name}\"", $"\"{criteria.Module}\"", $"\"{criteria.Name}\"");
        
        List<String> rowValues = new() {
            badge.Name,
            criteria.Module,
            criteria.Name
        };

        foreach (var scout in members)
        {
            var completionRecord = badgeCompletion
                .FirstOrDefault(bc => bc.ScoutId == scout.Id)
                ?.Achievements
                .FirstOrDefault(a => a.Key == criteria.Id)
                .Value ?? string.Empty;

            csvStringBuilder.Append(string.IsNullOrEmpty(completionRecord) ? ",\"\"" : ",\"x\"");
            rowValues.Add(string.IsNullOrEmpty(completionRecord) ? " ": "x");
        }

        csvStringBuilder.Append('\n');

        table.AddRow(rowValues.ToArray());
    }
}

AnsiConsole.Write(table);

// Build CSV output
Console.WriteLine(csvStringBuilder.ToString());

table.Rows.Clear();


