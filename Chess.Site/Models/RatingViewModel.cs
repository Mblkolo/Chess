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
        public string Insignias { get; set; }

        public int Games { get; set; }
        public int WhiteGames { get; set; }
        public int BlackGames { get; set; }
        public int Wins { get; set; }
        public int WhiteWins { get; set; }
        public int BlackWins { get; set; }
        public int Loses { get; set; }
        public int BlackLoses{ get; set; }
        public int WhiteLoses { get; set; }
        public int Draws { get; set; }
        public int WhiteDraws { get; set; }
        public int BlackDraws { get; set; }
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
