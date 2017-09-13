namespace Chess.Site.Dal
{
    using System.Linq;
    using Domain;

    public class RatingRepository
    {
        public GameResult[] GetGameResults(Session session)
        {
            return session.Query<GameResult>("SELECT * FROM gameResults ORDER BY createdAt DESC");
        }

        public void SaveGameResult(Session s, GameResult result)
        {
            s.Execute(@"INSERT INTO gameResults(whitePlayerId, blackPlayerId, whiteDeltaDecipoints, blackDeltaDecipoints, winner, createdAt)
                        VALUES(@WhitePlayerId, @BlackPlayerId, @WhiteDeltaDecipoints, @BlackDeltaDecipoints, @Winner, @CreatedAt)", result);

        }

        public void UpdatePlayerDecipoints(Session session, Player player)
        {
            session.Execute("UPDATE players SET decipoints=@Decipoints WHERE id=@Id", player);
        }

        public Player GetPlayerById(Session session, int playerId)
        {
            return session.Query<Player>("SELECT * FROM players WHERE id=@playerId", new { playerId }).Single();
        }

        public Player[] GetPlayers(Session session)
        {
            return session.Query<Player>("SELECT * FROM players ORDER BY id");
        }

        public void SavePlayer(Session session, Player player)
        {
            session.Execute(
                "INSERT INTO players(name, slackNickname, decipoints) VALUES(@Name, @SlackNickname, @Decipoints)",
                player);
        }
    }
}
