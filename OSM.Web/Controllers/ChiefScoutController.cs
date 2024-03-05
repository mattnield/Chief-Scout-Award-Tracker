using Microsoft.AspNetCore.Mvc;
using OSM.Interfaces;
using OSM.Models;
using OSM.Web.Models;

namespace OSM.Web.Controllers;

public class ChiefScoutController : Controller
{
    private IOsmClient _client { get; set; }

    public ChiefScoutController(IOsmClient client)
    {
        _client = client;
    }
    
    // GET
    public async Task<IActionResult> Index()
    {;
        var model = new ChiefScoutViewModel()
        {
            SectionName = _client.SectionName,
            SectionShortName = _client.SectionShortName
        };
        model.Badges = _client.GetBadgesByType(BadgeType.Challenge).Where(badge => badge.Id!=1539).ToList();
        foreach (var badge in model.Badges)
        {
            model.Completion.Add(
                badge.Id, 
                await _client.GetBadgeCompletion(badge.Id, badge.Version)
            );
        }
        model.Members = (await _client.GetMembersAsync()).Where(m => m.PatrolId >= 0).ToList();
        return View(model);
    }
}