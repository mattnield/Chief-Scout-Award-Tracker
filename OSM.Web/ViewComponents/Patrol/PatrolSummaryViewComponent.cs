using Microsoft.AspNetCore.Mvc;

namespace OSM.Web.ViewComponents.Patrol;

public class PatrolSummaryViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(OSM.Models.Patrol patrol)
    {
        return View(patrol);
    }
}