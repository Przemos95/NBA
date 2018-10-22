using NBAMetrics.Core.Calculations;
using NBAMetrics.Core.Domain.Entities.Enum;
using NBAMetrics.Core.Domain.Entities.Identity;
using NBAMetrics.Core.Drawing;
using NBAMetrics.DataAccess;
using NBAMetrics.DataAccess.Interfaces;
using NBAMetrics.Web.Helpers;
using NBAMetrics.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace NBAMetrics.Web.Controllers
{
    public class CalculationController : Controller
    {
        Teams[] allTeams;
        ITeamsRepository _teamsRepository;
        IActivityRepository _activityRepository;
        IRankingsRepository _rankingRepository;
        IPositionsRepository _positionsRepository;
        IEstimatedRankingRepository _estimatedRankingRepository;
        public CalculationController(ITeamsRepository teamsRepo, IActivityRepository activityRepo,
            IRankingsRepository rankingRepo, IPositionsRepository positionsRepo, IEstimatedRankingRepository estimatedRepo)
        {
            _teamsRepository = teamsRepo;
            _activityRepository = activityRepo;
            _rankingRepository = rankingRepo;
            _positionsRepository = positionsRepo;
            _estimatedRankingRepository = estimatedRepo;
            allTeams = teamsRepo.AsQueryable().ToArray();
        }
        // GET: Calculation
        public ActionResult Index(int id = 1)
        {
            SeasonCalculations calc = new SeasonCalculations((Season)id, _teamsRepository, _activityRepository,
                _rankingRepository, _positionsRepository, _estimatedRankingRepository);
            Activity isActive;
            EstimatedRankings[] points;
            calc.GetRanking(out points, out isActive);
            List<CalculationsViewModel> viewModel = new List<CalculationsViewModel>();
            EstimatedRankings currentRanking = ConvertHelper.ConvertPositionsToRanking(calc.GetPosition((Season)id));
            Positions previousRanking = calc.GetPosition((Season)id - 1);
            EstimatedRankings prevRanking = null;
            if (previousRanking != null) { prevRanking = ConvertHelper.ConvertPositionsToRanking(previousRanking); }
            foreach (Teams team in allTeams)
            {
                PropertyInfo propInfo = isActive.GetType().GetProperty(team.Name.Replace(' ','_').Replace('.', '_'));
                if (Convert.ToBoolean(propInfo.GetValue(isActive)))
                {
                    propInfo = points[0].GetType().GetProperty(team.Name.Replace(' ', '_').Replace('.', '_'));
                    int prevRank = prevRanking == null ? 0 : Convert.ToInt32(propInfo.GetValue(prevRanking));
                    int[] res = new int[7];
                    viewModel.Add(new CalculationsViewModel(team, new int[] { Convert.ToInt32(propInfo.GetValue(points[0])), Convert.ToInt32(propInfo.GetValue(points[1])),
                        Convert.ToInt32(propInfo.GetValue(points[2])), Convert.ToInt32(propInfo.GetValue(points[3])), Convert.ToInt32(propInfo.GetValue(points[4])),
                        Convert.ToInt32(propInfo.GetValue(points[5])), Convert.ToInt32(propInfo.GetValue(points[6]))}, Convert.ToInt32(propInfo.GetValue(currentRanking)), prevRank));
                }
            }
            viewModel = viewModel.OrderByDescending(x => x.Points[6]).ToList();
            List<Teams> drawTeams = new List<Teams>();
            List<int[]> drawPoints = new List<int[]>();
            foreach (CalculationsViewModel vm in viewModel.Take(5))
            {
                drawTeams.Add(vm.Team);
                drawPoints.Add(vm.Points);
            }
            DrawChart drawChart = new DrawChart();
            string path = drawChart.GenerateChart(drawTeams, drawPoints, (Season)id);
            ViewBag.Season = (Season)id;
            ViewBag.SeasonDisplayName = Helpers.Helper.GetSeasonDisplayName((Season)id);
            ViewBag.Path = path;
            return View(viewModel);
        }
    }
}