using Microsoft.Extensions.Configuration;

namespace OSM.Configuration;

public class OsmOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string ClientEndpoint { get; set; } = "https://www.onlinescoutmanager.co.uk";
    public long SectionId { get; set; }
    public string Section { get; set; } = string.Empty;
    public IEnumerable<Permission> Permissions { get; set; } = new List<Permission>();

    public OsmOptions(IConfiguration configuration)
    {
        configuration.GetSection("OsmOptions").Bind(this);
    }
}

public class Permission
{
    public string Section { get; set; } = string.Empty;
    public AccessType AccessType { get; set; } = AccessType.Read;
    public override string ToString()
    {
        return $"section:{Section.ToLower()}:{AccessType.ToString().ToLower()}";
    }
}

public enum AccessType
{
    Read,Write
}