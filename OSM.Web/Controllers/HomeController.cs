using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using OSM.Interfaces;
using OSM.Models;
using OSM.Web.Models;

namespace OSM.Web.Controllers;

public class HomeController : Controller
{
    private readonly IOsmClient _client;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, IOsmClient client)
    {
        _client = client;
        _logger = logger;
    }

    private BadgeProgress GetBadgeProgress(string badgeId, BadgeSummary[] badges)
    {
        if (!badges.Any(b => b.badge_id == badgeId)) return BadgeProgress.NotStarted;

        return (badges.First(b => b.badge_id == badgeId).completed == "1")
            ? BadgeProgress.Complete
            : BadgeProgress.Started;
    }
    public async Task<IActionResult> Index()
    {
        var term = (await _client.GetTermsAsync()).First(t => t.Current);
        var members = await _client.GetPersonBadgeSummaryAsync(term.Id);
        var model = new HomepageViewModel()
        {
            Badges = await _client.GetBadgesAsync(term.Id, BadgeType.Challenge)
        };

        foreach (var member in members)
        {
            model.MemberProgress.Add(new MemberBadgeGroupSummary()
            {
                Name = string.Join(' ', member.FirstName, member.Initial),
                Id = member.Id,
                Badges = model.Badges.Select(badge => new KeyValuePair<string,BadgeProgress>(badge.Id, GetBadgeProgress(badge.Id, member.Badges))).ToList().ToDictionary(o => o.Key, o=> o.Value)
            });
        }
        
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}