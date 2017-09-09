namespace Chess.Site.Controllers
{
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
            return View(new RatingViewModel
            {
                Players = new[]
                {
                    new SelectListItem {Text = "Первый", Value = "first"},
                    new SelectListItem {Text = "Второй", Value = "second"},
                },
                Rating = new[]
                {
                    new Rating {Name = "Вася", Points = 234},
                    new Rating {Name = "Коля", Points = 231},
                    new Rating {Name = "Маша", Points = 134},
                },
                LatestResults = new[]
                {
                    new ResultViewModel
                    {
                        WhitePlayer = "Вася",
                        BlackPlayer = "Коля",
                        Result = GameResult.FirstWin
                    }
                }
            });
        }

        [HttpGet]
        public IActionResult Players()
        {
            ViewResult result = null;
            sessionFactory.Execute(s =>
            {
                var players = s.Query<Player>("SELECT * FROM players ORDER BY id");

                result = View(new PlayersViewModel
                {
                    Players = players
                });
            });

            return result;
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