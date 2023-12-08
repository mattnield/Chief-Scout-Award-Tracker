using Microsoft.AspNetCore.Mvc;
using OSM.Web.Models;

namespace OSM.Web.ViewComponents.ChiefScout;

public class ChiefScoutTableViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ChiefScoutViewModel chiefScout)
    {
        return View(chiefScout);
    }
}