﻿namespace Chess.Site.Controllers
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
                var allGames = ratingRepository.GetGameResults(s).ToList();

                return new RatingViewModel
                {
                    Players = players.Select(x => new SelectListItem {Text = x.Name, Value = x.Id.ToString()}).ToArray(),
                    Rating = players.OrderByDescending(x => x.Decipoints)
                        .ThenBy(x => x.Id)
                        .Select(x => new Rating
                            {
                                Name = x.Name,
                                Points = x.Points,
                                Insignias = x.Insignias,
                                Games = allGames.Count(g => g.WithPlayer(x.Id)),
                                WhiteGames = allGames.Count(g => g.WhitePlayerId == x.Id),
                                BlackGames = allGames.Count(g => g.BlackPlayerId == x.Id),
                                Wins = allGames.Count(g => g.BlackPlayerId == x.Id && g.Winner == Winner.Black || g.WhitePlayerId == x.Id && g.Winner == Winner.White),
                                WhiteWins = allGames.Count(g => g.WhitePlayerId == x.Id && g.Winner == Winner.White),
                                BlackWins = allGames.Count(g => g.BlackPlayerId == x.Id && g.Winner == Winner.Black),
                                Loses = allGames.Count(g => g.BlackPlayerId == x.Id && g.Winner == Winner.White || g.WhitePlayerId == x.Id && g.Winner == Winner.Black),
                                WhiteLoses = allGames.Count(g => g.WhitePlayerId == x.Id && g.Winner == Winner.Black),
                                BlackLoses = allGames.Count(g => g.BlackPlayerId == x.Id && g.Winner == Winner.White),
                                Draws = allGames.Count(g => g.WithPlayer(x.Id) && g.Winner == Winner.Nobody),
                                WhiteDraws = allGames.Count(g => g.WhitePlayerId == x.Id && g.Winner == Winner.Nobody),
                                BlackDraws = allGames.Count(g => g.BlackPlayerId == x.Id && g.Winner == Winner.Nobody),
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

                var allGames = ratingRepository.GetGameResults(s).ToList();
                var pairGames = allGames
                .Where(x=>x.WhitePlayerId == whitePlayer.Id && x.BlackPlayerId == blackPlayer.Id ||
                          x.WhitePlayerId == blackPlayer.Id && x.BlackPlayerId == whitePlayer.Id
                )
                .ToList();
                
                message = $@"{whitePlayer.GetSlackName()} vs {blackPlayer.GetSlackName()}... {dto.Winner.EnumDisplayNameFor()}! Личный счёт {pairGames.Sum(x=>x.GetPlayerScore(whitePlayer.Id))}:{pairGames.Sum(x => x.GetPlayerScore(blackPlayer.Id))}
{whitePlayer.GetSlackName()} {whiteRating} -> {whitePlayer.Points}
{blackPlayer.GetSlackName()} {blackRating} -> {blackPlayer.Points} ";

                var players = new[] {whitePlayer, blackPlayer};
                foreach (var player in players)
                {
                    if (player.Insignias == null)
                        player.Insignias = "";
                    foreach (var insignia in InsigniasService.Insignias)
                    {
                        if (player.Insignias.Contains(insignia.Key) == false && insignia.Value.Func(result, player, players.Single(x => x != player), allGames))
                        {
                            player.Insignias += insignia.Key+";";
                            message += $@"
{player.GetSlackName()} получает орден {insignia.Key} «{insignia.Value.Name}» {insignia.Value.SlackEmoji}! ";
                            ratingRepository.UpdatePlayerInsignias(s, player);
                        }
                    }
                }
            });

            slackService.SendMessage(message);

            return RedirectToAction("Index");
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