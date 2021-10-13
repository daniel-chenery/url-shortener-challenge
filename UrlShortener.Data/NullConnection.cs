using System;
using System.Data;

namespace UrlShortener.Data
{
    public class NullConnection : IDbConnection
    {
        public NullConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).

        public string ConnectionString { get; set; }

#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).

        public int ConnectionTimeout => throw new NotImplementedException();

        public string Database => throw new NotImplementedException();

        public ConnectionState State => throw new NotImplementedException();

        public IDbTransaction BeginTransaction() => throw new NotImplementedException();

        public IDbTransaction BeginTransaction(IsolationLevel il) => throw new NotImplementedException();

        public void ChangeDatabase(string databaseName) => throw new NotImplementedException();

        public void Close() => throw new NotImplementedException();

        public IDbCommand CreateCommand() => throw new NotImplementedException();

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

        public void Dispose()
        {
        }

#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize

        public void Open() => throw new NotImplementedException();
    }
}