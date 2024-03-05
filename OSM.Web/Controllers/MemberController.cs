using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using OSM.Interfaces;
using OSM.Models;
using OSM.Web.Models;

namespace OSM.Web.Controllers;

public class MemberController : Controller
{
    private IOsmClient _client { get; set; }
    public MemberController(IOsmClient client)
    {
        _client = client;
    }
    // GET
    public async Task<IActionResult> Detail(int id)
    {
        // Get member details
        var member = (await _client.GetMemberAsync(id));
        var patrols = await _client.GetPatrols();
        var patrol = patrols.First(p => p.Members.Any(m => m.Id == member.Id));
        var badges = (await _client.GetPersonBadgeSummaryAsync()).First(m => m.Id == id).Badges;

        var challengeAwards = (await Task.WhenAll(
            _client.GetBadgesByType(BadgeType.Challenge)
                .Where(badge => badge.Id != 1539)
                .Select(async badge => 
                    new KeyValuePair<Badge, IList<BadgeCompletion>>(
                        badge, 
                        (await _client.GetBadgeCompletion(badge.Id, badge.Version))
                        .Where(c => c.ScoutId == id)
                        .ToList()
                    )
                )
        )).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        
        var model = new MemberDetailViewModel()
        {
            Member = member!,
            Patrol = patrol,
            BadgeSummaries = badges,
            BadgesThisTerm = GetBadgesThisTerm(badges),
            BadgesLastTerm = GetBadgesLastTerm(badges),
            BadgesInProgress = GetBadgesInProgress(badges),
            ChiefScoutProgress = challengeAwards,
            SectionName = _client.SectionName,
            SectionShortName = _client.SectionShortName
        };
        
        return View(model);
    }

    private static List<BadgeSummary> GetBadgesInProgress(BadgeSummary[] badges)
    {
        return badges.Where(b =>
            b.CompletedLevel <= 0).ToList();
    }

    private List<BadgeSummary> GetBadgesLastTerm(BadgeSummary[] badges)
    {
        return badges.Where(b =>
            (b.Awarded &&  b.AwardedDate.Value < _client.CurrentTerm.StartDate)).ToList();
    }

    private List<BadgeSummary> GetBadgesThisTerm(BadgeSummary[] badges)
    {
        return badges.Where(b =>
            (b.IsComplete) 
            &&
            (
                !b.Awarded
                ||
                (b.Awarded && b.AwardedDate.Value >= _client.CurrentTerm.StartDate))
        ).ToList();
    }
}