namespace Chess.Site.Dal
{
    using System;
    using Microsoft.Data.Sqlite;
    using Microsoft.Extensions.Options;

    public class SessionFactory
    {
        private readonly DbConnectionOptions dbConnectionOptions;

        public SessionFactory(IOptions<DbConnectionOptions> dbConnectionOption)
        {
            if (dbConnectionOption?.Value?.ConnectionString == null)
                throw new ArgumentNullException(nameof(dbConnectionOption));

            this.dbConnectionOptions = dbConnectionOption.Value;
        }

        public void Execute(Action<Session> action)
        {
            Execute<object>(x =>
            {
                action(x);
                return null;
            });
        }

        public T Execute<T>(Func<Session, T> action)
        {
            using (var connection = new SqliteConnection(dbConnectionOptions.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var result = action(new Session(connection, transaction));
                    transaction.Commit();

                    return result;
                }
            }
        }
    }
}