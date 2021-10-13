using Microsoft.Extensions.Options;
using System;
using System.Data;
using UrlShortener.Core.Configuration;

namespace UrlShortener.Data.Factories
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly IOptions<DatabaseOptions> _options;

        public ConnectionFactory(IOptions<DatabaseOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Ordinarily, this would return a MSSQL / MySQl / SQLite or any other connection
        /// Each Type (MSSQL, MySQL) would be it's own class
        /// For the purposes of this task, a database will not be implemented
        /// </summary>
        /// <returns></returns>
#pragma warning disable CS8604 // Possible null reference argument.

        public IDbConnection GetConnection() => new NullConnection(_options.Value.ConnectionString);

#pragma warning restore CS8604 // Possible null reference argument.
    }
}