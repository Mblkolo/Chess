namespace Chess.Site.Domain
{
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SlackNickname { get; set; }
        public decimal Points => Decipoints / 10m;

        public int Decipoints { get; set; }
    }
}