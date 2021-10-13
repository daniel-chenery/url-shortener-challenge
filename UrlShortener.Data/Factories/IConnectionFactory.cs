using System.Data;

namespace UrlShortener.Data.Factories
{
    public interface IConnectionFactory
    {
        IDbConnection GetConnection();
    }
}