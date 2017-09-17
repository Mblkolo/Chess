using System;
using System.Collections.Generic;

namespace Chess.Site.Controllers
{
    using System.Diagnostics;
    using System.Linq;
    using Dal;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Domain;
    using Integration;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Infrastructure;

    public class RatingController : Controller
    {
        private readonly SessionFactory sessionFactory;
        private readonly SlackService slackService;
        private readonly RatingRepository ratingRepository;

        private Dictionary<string, Insignia> Insignias = new List<Insignia>()
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

        public RatingController(SessionFactory sessionFactory, SlackService slackService, RatingRepository ratingRepository)
        {
            this.sessionFactory = sessionFactory;
            this.slackService = slackService;
            this.ratingRepository = ratingRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = sessionFactory.Execute(s =>
            {
                var players = ratingRepository.GetPlayers(s);

                return new RatingViewModel
                {
                    Players = players.Select(x => new SelectListItem {Text = x.Name, Value = x.Id.ToString()}).ToArray(),
                    Rating = players.OrderByDescending(x => x.Decipoints)
                        .ThenBy(x => x.Id)
                        .Select(x => new Rating
                            {
                                Name = x.Name,
                                Points = x.Points
                            }
                        )
                        .ToArray(),
                    LatestResults = ratingRepository.GetGameResults(s)
                        .Select(x => new ResultViewModel
                            {
                                Winner = x.Winner,
                                BlackPlayer = players.Single(p => p.Id == x.BlackPlayerId).Name,
                                WhitePlayer = players.Single(p => p.Id == x.WhitePlayerId).Name,
                                PlayedAt = x.CreatedAt,
                                BlackDeltaPoints = x.BlackDeltaPoints,
                                WhiteDeltaPoints = x.WhiteDeltaPoints
                            }
                        )
                        .ToArray()
                };
            });

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(CreateGameResultDto dto)
        {
            if (dto.BlackPlayerId == dto.WhitePlayerId)
                return RedirectToAction("Index");

            string message = null;
            sessionFactory.Execute(s =>
            {
                var whitePlayer = ratingRepository.GetPlayerById(s, dto.WhitePlayerId);
                var blackPlayer = ratingRepository.GetPlayerById(s, dto.BlackPlayerId);
                var whiteRating = whitePlayer.Points;
                var blackRating = blackPlayer.Points;


                var result = new GameResult(whitePlayer, blackPlayer, dto.Winner);
                ratingRepository.SaveGameResult(s, result);

                ratingRepository.UpdatePlayerDecipoints(s, whitePlayer);
                ratingRepository.UpdatePlayerDecipoints(s, blackPlayer);

                var games = ratingRepository.GetGameResults(s)
                .Where(x=>x.WhitePlayerId == whitePlayer.Id && x.BlackPlayerId == blackPlayer.Id ||
                          x.WhitePlayerId == blackPlayer.Id && x.BlackPlayerId == whitePlayer.Id
                )
                .ToList();
                
                message = $@"{whitePlayer.Name} vs {blackPlayer.Name}... {dto.Winner.EnumDisplayNameFor()}! Личный счёт {games.Sum(x=>x.GetPlayerScore(whitePlayer.Id))}:{games.Sum(x => x.GetPlayerScore(blackPlayer.Id))}
{whitePlayer.Name} {whiteRating} -> {whitePlayer.Points}
{blackPlayer.Name} {blackRating} -> {blackPlayer.Points} ";

                var players = new[] {whitePlayer, blackPlayer};
                foreach (var player in players)
                {
                    if (player.Insignias == null)
                        player.Insignias = "";
                    foreach (var insignia in Insignias)
                    {
                        if (player.Insignias.Contains(insignia.Key) == false && insignia.Value.Func(result, player, players.Single(x => x != player)))
                        {
                            player.Insignias += insignia.Key;
                            message += $@"
{player.Name} получает орден {insignia.Key} «{insignia.Value.Name}» {insignia.Value.SlackEmoji}! ";
                            ratingRepository.UpdatePlayerInsignias(s, player);
                        }
                    }
                }
            });

            slackService.SendMessage(message);

            return RedirectToAction("Index");
        }

        private class Insignia
        {
            public string Emoji { get; set; }
            public string SlackEmoji { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public Func<GameResult, Player, Player, bool> Func { get; set; }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Players()
        {
            var players = sessionFactory.Execute(s => ratingRepository.GetPlayers(s));

            return View(new PlayersViewModel
            {
                Players = players
            });
        }

        [HttpPost]
        public IActionResult Players(Player dto)
        {
            if (string.IsNullOrEmpty(dto.Name))
                return RedirectToAction("Players");

            sessionFactory.Execute(s =>
            {
                var palyer = new Player
                {
                    Name = dto.Name,
                    Decipoints = GameResult.StartDecipoints,
                    SlackNickname = dto.SlackNickname
                };
                ratingRepository.SavePlayer(s, palyer);
            });

            return RedirectToAction("Players");
        }
    }
}