using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BISTickerAPI.Controllers
{
    public class PresentationController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}