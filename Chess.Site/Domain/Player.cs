namespace Chess.Site.Domain
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SlackNickname { get; set; }
        public decimal Points => Decipoints / 100m;

        public int Decipoints { get; set; }
        public string Insignias { get; set; }

        public string GetSlackName()
        {
            if (string.IsNullOrEmpty(SlackNickname))
                return Name;

            if (SlackNickname.StartsWith('@'))
                return SlackNickname;

            return "@" + SlackNickname;
        }
    }
}