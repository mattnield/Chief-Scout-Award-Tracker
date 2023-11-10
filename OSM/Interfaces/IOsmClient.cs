using OSM.Models;

namespace OSM.Interfaces;

public interface IOsmClient
{
    
    Task<IList<Badge>> GetBadgesAsync(string termId, BadgeType type);
    Task<IList<Term>> GetTermsAsync();
    Task<IList<Member>> GetMembersAsync(string termId);
}