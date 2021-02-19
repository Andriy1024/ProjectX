using System.Data;

namespace ProjectX.Core.DataAccess
{
    /// <summary>
    /// Service with scoped lifetime in DI container.
    /// </summary>
    public interface ICurrentSqlConnection
    {
        IDbConnection GetConnection();
    }
}
