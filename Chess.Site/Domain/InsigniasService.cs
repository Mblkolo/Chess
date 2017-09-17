using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Site.Domain
{
    public static class InsigniasService
    {
        public static Dictionary<string, Insignia> Insignias = new List<Insignia>()
            {
                new Insignia
                {
                    Name = "Ранняя пташка",
                    Emoji = "🐔",
                    SlackEmoji = ":chicken:",
                    Description = "Сыграть партию до 9 утра",
                    Func = (result, player, opponent) =>
                    {
                        var cheTime = DateTime.UtcNow.AddHours(5);
                        return cheTime.Hour < 9 && cheTime.Hour >= 6;
                    }
                },
                new Insignia
                {
                    Name = "Сова",
                    Emoji = "🦉",
                    SlackEmoji = ":coffee:",
                    Description = "Сыграть партию после 8 вечера",
                    Func = (result, player, opponent) =>
                    {
                        var cheTime = DateTime.UtcNow.AddHours(5);
                        return cheTime.Hour >= 20;
                    }
                },
                new Insignia
                {
                    Name = "Гроза Ансара",
                    Emoji = "🌩",
                    SlackEmoji = ":ewok:",
                    Description = "Выиграть Ансара",
                    Func = (result, player, opponent) => result.GetPlayerScore(player.Id) == 1 && opponent.Name == "Ансар"
                },
            }
            .ToDictionary(x => x.Emoji);
    }
}