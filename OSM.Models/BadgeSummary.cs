namespace OSM.Models;

public record BadgeSummary(
    string completed,
    string awarded,
    int awarded_date,
    string badge,
    string badge_shortname,
    string badge_group,
    double status,
    string picture,
    string badge_identifier,
    string badge_id,
    object level
);