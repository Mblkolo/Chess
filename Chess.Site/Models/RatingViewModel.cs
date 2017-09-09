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

        public Rating[] Rating { get; set; }

        public ResultViewModel[] LatestResults { get; set; }
    }

    public class ResultViewModel
    {
        public string WhitePlayer { get; set; }
        public string BlackPlayer { get; set; }

        public GameResult Result { get; set; }
    }

    public class Rating
    {
        public decimal Points { get; set; }
        public string Name { get; set; }
    }


    public enum GameResult
    {
        [Display(Name = "Белые победили")]
        WhiteWin,

        [Display(Name = "Ничья")]
        NoWins,

        [Display(Name = "Чёрные победили")]
        BlackWin
    }
}
