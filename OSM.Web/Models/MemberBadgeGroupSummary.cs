namespace OSM.Web.Models;

public class MemberBadgeGroupSummary : BaseViewModel
{
    public string Name { get; set; }
    public int Id { get; set; }
    public Dictionary<int, BadgeProgress> Badges { get; set; }
}

public enum BadgeProgress
{
    NotStarted,
    Started,
    Complete,
}