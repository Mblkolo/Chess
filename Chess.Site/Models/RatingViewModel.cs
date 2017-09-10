﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Chess.Site.Models
{
    public class RatingViewModel
    {
        public SelectListItem[] Players { get; set; }
        public string WhitePlayer { get; set; }
        public string BlackPlayer { get; set; }

        public Winner Winner { get; set; }

        public Rating[] Rating { get; set; }

        public ResultViewModel[] LatestResults { get; set; }
    }

    public class ResultViewModel
    {
        public string WhitePlayer { get; set; }
        public string BlackPlayer { get; set; }

        public Winner Result { get; set; }
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
