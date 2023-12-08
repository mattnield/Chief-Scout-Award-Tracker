using Microsoft.AspNetCore.Mvc;

namespace OSM.Web.ViewComponents.Member;

public class MemberLinkViewComponent: ViewComponent
{
    public IViewComponentResult Invoke(OSM.Models.Member member)
    {
        return View(member);
    }
}