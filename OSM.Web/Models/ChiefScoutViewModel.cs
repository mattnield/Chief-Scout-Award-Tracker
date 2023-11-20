using OSM.Models;

namespace OSM.Web.Models;

public class ChiefScoutViewModel
{
    public IList<Member> Members { get; set; } = new List<Member>();
    public IList<Badge> Badges { get; set; } = new List<Badge>();

    public Dictionary<string, IList<BadgeCompletion>> Completion { get; set; } =
        new();
}