using OSM.Models;

namespace OSM.Interfaces;

public interface IOsmClient
{
    
    Task<IEnumerable<Badge>> GetBadgesAsync(string termId, BadgeType type);
    Task<IEnumerable<Term>> GetTermsAsync();
}