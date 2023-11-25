using OSM.Models;

namespace OSM.Web.Models;

public class HomepageViewModel
{
    public IList<Badge> Badges { get; set; } = new List<Badge>();
    public IList<MemberBadgeGroupSummary> MemberProgress { get; set; } = new List<MemberBadgeGroupSummary>();
    
}

