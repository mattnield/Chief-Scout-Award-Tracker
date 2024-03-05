using OSM.Models;

namespace OSM.Interfaces;

public interface IOsmClient
{
    string SectionName { get; set; }
    string SectionShortName { get; set; }
    Term CurrentTerm { get; }
    IList<Badge> GetBadgesByType(BadgeType badgeType);
    Task<IList<Term>> GetTermsAsync();
    Task<IList<Member>> GetMembersAsync();
    Task<IList<BadgeCompletion>> GetBadgeCompletion(int badgeId, string badgeVersion);
    Task<IList<Member>> GetPersonBadgeSummaryAsync();
    Task<IList<Patrol>> GetPatrols(bool includeNoPatrol = true);
    Task<Member?> GetMemberAsync(int memberId);
    
    
}