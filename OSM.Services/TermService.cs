using OSM.Models;

namespace OSM.Services;

public interface ITermService
{
    IEnumerable<Term> GetTerms();
}

public class TermService : ITermService
{
    public IEnumerable<Term> GetTerms()
    {
        throw new NotImplementedException();
    }
}