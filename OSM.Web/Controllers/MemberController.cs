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
        var model = new MemberDetailViewModel()
        {
            Member = member,
            Patrol = patrol,
            BadgeSummaries = badges
        };

        model.BadgesThisTerm = badges.Where(b =>
            (b.IsComplete) 
            &&
            (
                !b.Awarded
                    ||
                (b.Awarded && b.AwardedDate.Value >= _client.CurrentTerm.StartDate))
            ).ToList();
        
        model.BadgesLastTerm = badges.Where(b =>
            (b.Awarded &&  b.AwardedDate.Value < _client.CurrentTerm.StartDate)).ToList();

        model.BadgesInProgress = badges.Where(b =>
            b.CompletedLevel <= 0).ToList();
        
        return View(model);
    }
}