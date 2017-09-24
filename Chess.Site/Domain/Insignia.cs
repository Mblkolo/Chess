using System;
using System.Collections.Generic;

namespace Chess.Site.Domain
{
    public class Insignia
    {
        public string Emoji { get; set; }
        public string SlackEmoji { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Func<GameResult, Player, Player, List<GameResult>, bool> Func { get; set; }

        public string GetTitle()
        {
            var title = $"«{Name}»";

            if (string.IsNullOrEmpty(Description) == false && Description != Name)
                title += $" — {Description}";
                
            return title;
        }
    }
}