using NBAMetrics.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBAMetrics.Web.Controllers
{
    public class HomeController : Controller
    {
        private ITeamsRepository _teams;
        public HomeController(ITeamsRepository teamsParam)
        {
            _teams = teamsParam;
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}