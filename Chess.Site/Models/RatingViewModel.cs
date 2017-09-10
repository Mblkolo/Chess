using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Chess.Site.Models
{
    using System;

    public class RatingViewModel
    {
        public SelectListItem[] Players { get; set; }
        public string WhitePlayer { get; set; }
        public string BlackPlayer { get; set; }

        public Winner Winner { get; set; }

        public Rating[] Rating { get; set; }

        public ResultViewModel[] LatestResults { get; set; }
    }

    public class CreateGameResultDto
    {
        public int WhitePlayerId { get; set; }
        public int BlackPlayerId { get; set; }

        public Winner Winner { get; set; }
    }

    public class ResultViewModel
    {
        public DateTime PlayedAt { get; set; }

        public string WhitePlayer { get; set; }
        public string BlackPlayer { get; set; }

        public decimal WhiteDeltaPoints { get; set; }
        public decimal BlackDeltaPoints { get; set; }

        public Winner Winner { get; set; }
    }

    public class Rating
    {
        public decimal Points { get; set; }
        public string Name { get; set; }
    }


    public enum Winner
    {
        [Display(Name = "Белые победили")]
        White,

        [Display(Name = "Чёрные победили")]
        Black,

        [Display(Name = "Ничья")]
        Nobody
    }
}
