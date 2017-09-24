using System;
using System.Collections.Generic;
using System.Linq;
using Chess.Site.Models;

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
                    Func = (result, player, opponent, games) =>
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
                    Func = (result, player, opponent, games) =>
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
                    Func = (result, player, opponent, games) => result.GetPlayerScore(player.Id) == 1 && opponent.Name == "Ансар"
                },
                new Insignia
                {
                    Name = "Серия побед (3)",
                    Emoji = "⭐️",
                    SlackEmoji = ":star:",
                    Description = "3 победы подряд",
                    Func = (result, player, opponent, games) => LastWinsCount(games, player) >= 3
                },
                new Insignia
                {
                    Name = "Серия побед (5)",
                    Emoji = "🌟",
                    SlackEmoji = ":star2:",
                    Description = "5 побед подряд",
                    Func = (result, player, opponent, games) => LastWinsCount(games, player) >= 5
                },
                new Insignia
                {
                    Name = "5 побед",
                    Emoji = "👶🏻",
                    SlackEmoji = ":baby:",
                    Description = "5 побед",
                    Func = (result, player, opponent, games) => WinsCount(result, player, games, 5)
                },
                new Insignia
                {
                    Name = "10 побед",
                    Emoji = "👦🏻",
                    SlackEmoji = ":boy:",
                    Description = "10 побед",
                    Func = (result, player, opponent, games) => WinsCount(result, player, games, 10)
                },
                new Insignia
                {
                    Name = "Буквоед",
                    Emoji = "🅰",
                    SlackEmoji = ":a:",
                    Description = "Победа над игроком, имя которого начинается с той же буквы",
                    Func = (result, player, opponent, games) => result.GetPlayerScore(player.Id) == 1 && opponent.Name.ToLower()[0] == player.Name.ToLower()[0]
                },
                new Insignia
                {
                    Name = "Первая победа месяца",
                    Emoji = "📆",
                    SlackEmoji = ":date:",
                    Func = (result, player, opponent, games) => result.GetPlayerScore(player.Id) == 1 &&
                                                                games.Any(g => g.CreatedAt.Year == result.CreatedAt.Year && g.CreatedAt.Month == result.CreatedAt.Month) == false
                },
                new Insignia
                {
                    Name = "Первая победа года",
                    Emoji = "🗓",
                    SlackEmoji = ":calendar:",
                    Func = (result, player, opponent, games) => result.GetPlayerScore(player.Id) == 1 &&
                                                                games.Any(g => g.CreatedAt.Year == result.CreatedAt.Year) == false
                },
                new Insignia
                {
                    Name = "Белый конь",
                    Emoji = "🦄",
                    SlackEmoji = ":unicorn_face:",
                    Description = "Первая победа белых после серии побед чёрных",
                    Func = (result, player, opponent, games) => result.GetPlayerScore(player.Id) == 1 && result.Winner == Winner.White &&
                                                                LastWinsCount(games.OrderByDescending(x=>x.CreatedAt).Skip(1).ToList(), Winner.Black) >= 3
                },
                new Insignia
                {
                    Name = "Чёрный слон",
                    Emoji = "🦍",
                    SlackEmoji = ":monkey:",
                    Description = "Первая победа чёрных после серии побед белых",
                    Func = (result, player, opponent, games) => result.GetPlayerScore(player.Id) == 1 && result.Winner == Winner.Black &&
                                                                LastWinsCount(games.OrderByDescending(x=>x.CreatedAt).Skip(1).ToList(), Winner.White) >= 3
                }
            }
            .ToDictionary(x => x.Emoji);

        private static bool WinsCount(GameResult result, Player player, List<GameResult> games, int value)
        {
            return result.GetPlayerScore(player.Id) == 1 && games
                       .Where(x => x.WithPlayer(player.Id))
                       .Count(x => x.GetPlayerScore(player.Id) == 1) >= value;
        }

        private static int LastWinsCount(List<GameResult> games, Player player)
        {
            return games
                .Where(x => x.WithPlayer(player.Id))
                .OrderByDescending(x => x.CreatedAt)
                .TakeWhile(x => x.GetPlayerScore(player.Id) == 1)
                .Count();
        }

        private static int LastWinsCount(List<GameResult> games, Winner winner)
        {
            return games
                .OrderByDescending(x => x.CreatedAt)
                .TakeWhile(x => x.Winner == winner)
                .Count();
        }
    }
}