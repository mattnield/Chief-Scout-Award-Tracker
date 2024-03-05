using OSM.Models;

namespace OSM.Web.Models;

public class ChiefScoutViewModel : BaseViewModel
{
    public IList<Member> Members { get; set; } = new List<Member>();
    public IList<Badge> Badges { get; set; } = new List<Badge>();

    public Dictionary<int, IList<BadgeCompletion>> Completion { get; set; } =
        new();
}