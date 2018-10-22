using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NBAMetrics.Core.Domain.Entities.Identity;
using NBAMetrics.Core.Calculations;
using NBAMetrics.Web.Models;
using NBAMetrics.Core.Domain.Entities.Enum;
using NBAMetrics.Core.Drawing;
using NBAMetrics.DataAccess.Interfaces;
using NBAMetrics.DataAccess;
using System.Reflection;

namespace NBAMetrics.Web.Controllers
{
    public class TeamsController : Controller
    {
        ITeamsRepository _teamsRepository;
        IActivityRepository _activityRepository;
        IRankingsRepository _rankingRepository;
        IPositionsRepository _positionsRepository;
        IEstimatedRankingRepository _estimatedRankingRepository;
        TeamCalculations teamCalc;
        public TeamsController(ITeamsRepository teamsRepo, IActivityRepository activityRepo,
            IRankingsRepository rankingRepo, IPositionsRepository positionsRepo, IEstimatedRankingRepository estimatedRepo)
        {
            _teamsRepository = teamsRepo;
            _activityRepository = activityRepo;
            _rankingRepository = rankingRepo;
            _positionsRepository = positionsRepo;
            teamCalc = new TeamCalculations(_teamsRepository, _activityRepository,
                _rankingRepository, _positionsRepository, estimatedRepo, null);
        }
        // GET: Teams
        public ActionResult Index(int id = 1, Season season = (Season)1)
        {
            Teams team = _teamsRepository.AsQueryable().Where(s => s.TeamID == id).FirstOrDefault();
            teamCalc._team = team;
            PropertyInfo activityPropInfo = new Activity().GetType().GetProperty(team.Name.Replace(' ', '_').Replace('.', '_'));
            bool isActive = teamCalc.CheckIfTeamWasActive(activityPropInfo, season);
            int[] points = new int[7];//null;
            TeamChart chart = new TeamChart(team);
            if (isActive)
            {
                    points = teamCalc.GetSeasonRanking(null, season);
                    ViewBag.ChartPath = chart.GenerateSeasonChart(points, season);
            }
            CalculationsViewModel viewModel = new CalculationsViewModel(team, points);
            ViewBag.Season = (Season)season;
            ViewBag.SeasonDisplayName = Helpers.Helper.GetSeasonDisplayName(season);
            return View(viewModel);
        }

        public ActionResult AllTime(int id = 1)
        {
            Teams team = _teamsRepository.AsQueryable().Where(s => s.TeamID == id).FirstOrDefault();
            teamCalc._team = team;
            Dictionary<Season, bool> whenWasActive = teamCalc.CheckWhenTeamWasActive();
            Dictionary<Season, int> pointsAllTime = teamCalc.GetAllTimeRanking();
            List<Season> seasons = new List<Season>();
            List<int> points = new List<int>();
            foreach (Season season in Enum.GetValues(typeof(Season)))
            {
                if (whenWasActive[season])
                {
                    seasons.Add(season);
                    points.Add(pointsAllTime[season]);
                }
            }
            TeamChart chart = new TeamChart(team);
            ViewBag.Path = chart.GenerateAllTimeChart(seasons, points);
            ViewBag.TeamName = team.Name;
            ViewBag.TeamID = team.TeamID;
            TeamAllTimePointsViewModel viewModel = new TeamAllTimePointsViewModel(seasons, points);
            return View(viewModel);
        }

        public PartialViewResult Activity(int id)
        {
            Teams team = _teamsRepository.AsQueryable().Where(s => s.TeamID == id).FirstOrDefault();
            teamCalc._team = team;
            Dictionary<Season, bool> whenWasActive = teamCalc.CheckWhenTeamWasActive();
            ViewBag.TeamId = id;
            return PartialView(whenWasActive);
        }

        public PartialViewResult History(int teamId, Season season, int[] points)
        {
            Teams team = _teamsRepository.AsQueryable().Where(s => s.TeamID == teamId).FirstOrDefault();
            teamCalc._team = team;
            Dictionary<Season, int[]> Points = teamCalc.GetHistory(team, season);
            ViewBag.Points = points;
            return PartialView(Points);
        }

        public ActionResult CompareTeams()
        {
            List<Teams> teams = _teamsRepository.GetAll().ToList();
            return View(teams);
        }

        public ActionResult CompareSelected(string selected)
        {
            CompareTeamsViewModel viewModel = new CompareTeamsViewModel();
            viewModel.Points = new Dictionary<Teams, int[]>();
            if (selected == null) return RedirectToAction("CompareTeams");
            string[] teams = selected.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<Teams> teamList = new List<Teams>();
            foreach (var team in teams)
            {
                Teams recognized = recognizeTeam(team);
                if (recognized != null) teamList.Add(recognizeTeam(team));
            }

            List<int[]> points = new List<int[]>();
            foreach (Teams team in teamList)
            {
                teamCalc._team = team;
                Dictionary<Season, int> pts = teamCalc.GetAllTimeRanking();
                int[] drawPts = new int[71];
                int i = 0;
                foreach (KeyValuePair<Season, int> season in pts)
                {
                    drawPts[i] = season.Value;
                    i++;
                }
                points.Add(drawPts);
                viewModel.Points.Add(team, drawPts);
            }
            DrawChart draw = new DrawChart();
            viewModel.Path = draw.CompareTeamsDraw(teamList, points);
            return View(viewModel);
        }

        #region Helpers
        private Teams recognizeTeam(string name)
        {
            foreach (Teams team in _teamsRepository.AsQueryable())
            {
                List<string> names = (team.OldNames.Split(new string[] { ", " }, StringSplitOptions.None)).ToList();
                names.Add(team.Name);
                foreach (string item in names)
                {
                    if (item == name)
                    {
                        return team;
                    }
                }
            }
            return null;
        }

        #endregion Helpers
    }
}