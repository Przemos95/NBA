using NBAMetrics.Core.Domain.Entities.Identity;
using NBAMetrics.DataAccess;
using NBAMetrics.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NBAMetrics.Web.Controllers
{
    public class NavController : Controller
    {
        private ITeamsRepository teamsRepository;

        public NavController(ITeamsRepository repo)
        {
            teamsRepository = repo;
        }
        // GET: NAv
        public PartialViewResult Menu()
        {
            List<Teams> teams = teamsRepository.Where(x => x.TeamID > 0).ToList();
            return PartialView(teams);
        }

        public PartialViewResult Seasons()
        {
            return PartialView();
        }
    }
}