using Microsoft.AspNetCore.Mvc;

namespace OSM.Web.ViewComponents.Badges;

public class BadgeListViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(IList<OSM.Models.BadgeSummary> badges)
    {
        return View(badges);
    }
}