using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Chess.Site.Models
{
    public class RatingViewModel
    {
        public SelectListItem[] Players { get; set; }
        public string FirstPlayer { get; set; }
        public string SecondPlayer { get; set; }

        public GameResult Result { get; set; }
    }

    public enum GameResult
    {
        [Display(Name = "Первый победил")]
        FirstWin,

        [Display(Name = "Ничья")]
        NoWins,

        [Display(Name = "Второй победил")]
        LastWin
    }
}
