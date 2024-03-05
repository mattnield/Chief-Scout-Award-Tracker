using OSM.Models;

namespace OSM.Web.Models;

public class MemberDetailViewModel : BaseViewModel
{
    public Member Member { get; set; }
    public Patrol Patrol { get; set; }
    public IList<BadgeSummary> BadgeSummaries { get; set; } = new List<BadgeSummary>();
    public IList<BadgeSummary> BadgesThisTerm { get; set; } = new List<BadgeSummary>();
    public IList<BadgeSummary> BadgesLastTerm { get; set; } = new List<BadgeSummary>();
    public IList<BadgeSummary> BadgesInProgress { get; set; } = new List<BadgeSummary>();

    public IDictionary<Badge, IList<BadgeCompletion>> ChiefScoutProgress { get; set; } =
        new Dictionary<Badge, IList<BadgeCompletion>>();
}

public class BadgeStatus
{
    public string DisplayName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool Complete { get; set; } = false;
    public bool Awarded { get; set; } = false;
    public DateTime? AwardedDate { get; set; }
    public BadgeType BadgeType { get; set; }
}