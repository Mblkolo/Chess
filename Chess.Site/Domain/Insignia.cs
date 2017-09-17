using System;

namespace Chess.Site.Domain
{
    public class Insignia
    {
        public string Emoji { get; set; }
        public string SlackEmoji { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Func<GameResult, Player, Player, bool> Func { get; set; }
    }
}