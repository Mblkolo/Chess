using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Chess.Site.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Chess.Site.Controllers
{
    public class RatingController : Controller
    {
        public IActionResult Index()
        {
            return View(new RatingViewModel
            {
                Players = new []
                {
                    new SelectListItem{Text = "Первый", Value = "first"},
                    new SelectListItem{Text = "Второй", Value = "second"},
                }
            });
        }
    }
}
