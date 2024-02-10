using Microsoft.AspNetCore.Mvc;
using OSM.Models;
using OSM.Web.Models;

namespace OSM.Web.ViewComponents.ChiefScout;

public class ChiefScoutMemberProgressViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(IDictionary<Badge, IList<BadgeCompletion>> progress)
    {
        return View(progress);
    }
}