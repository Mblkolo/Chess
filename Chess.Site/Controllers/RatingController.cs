namespace Chess.Site.Controllers
{
    using System.Linq;
    using Dal;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Domain;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class RatingController : Controller
    {
        private readonly SessionFactory sessionFactory;

        public RatingController(SessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public IActionResult Index()
        {
            var viewModel = sessionFactory.Execute(s =>
            {
                var players = GetPlayers(s);

                return new RatingViewModel
                {
                    Players = players.Select(x => new SelectListItem { Text = x.Name, Value = x.Id }).ToArray(),
                    Rating = players.OrderByDescending(x => x.Decipoints)
                                    .ThenBy(x => x.Id)
                                    .Select(x => new Rating
                                                 {
                                                     Name = x.Name,
                                                     Points = x.Points
                                                 }
                                    )
                                    .ToArray()
                    ,
                    LatestResults = new[]
                    {
                        new ResultViewModel
                        {
                            WhitePlayer = "Вася",
                            BlackPlayer = "Коля",
                            Result = GameResult.WhiteWin
                        }
                    }
                };
            });

            return View(viewModel);
        }

        private Player[] GetPlayers(Session session)
        {
            return session.Query<Player>("SELECT * FROM players ORDER BY id");
        }

        [HttpGet]
        public IActionResult Players()
        {
            var players = sessionFactory.Execute(s => GetPlayers(s));

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
                s.Execute("INSERT INTO players(name, slackNickname) VALUES(@Name, @SlackNickname)",
                    new {dto.Name, dto.SlackNickname});
            });

            return RedirectToAction("Players");
        }
    }
}