namespace Chess.Site.Domain
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SlackNickname { get; set; }
        public decimal Points => Decipoints / 100m;

        public int Decipoints { get; set; }
    }
}