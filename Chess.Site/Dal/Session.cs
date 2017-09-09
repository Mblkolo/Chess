namespace Chess.Site.Dal
{
    using System.Data;
    using System.Linq;
    using Dapper;

    public class Session
    {
        private readonly IDbConnection connection;
        private readonly IDbTransaction transaction;

        public Session(IDbConnection connection, IDbTransaction transaction)
        {
            this.connection = connection;
            this.transaction = transaction;
        }

        public T[] Query<T>(string sql, object param = null)
        {
            return connection.Query<T>(sql, param, transaction).ToArray();
        }

        public T ExecuteScalar<T>(string sql, object param = null)
        {
            return connection.ExecuteScalar<T>(sql, param, transaction);
        }
    }
}