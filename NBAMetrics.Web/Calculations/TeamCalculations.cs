using NBAMetrics.Core.Domain.Entities.Enum;
using NBAMetrics.Core.Domain.Entities.Identity;
using NBAMetrics.DataAccess;
using NBAMetrics.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NBAMetrics.Core.Calculations
{
    public class TeamCalculations
    {
        ITeamsRepository _teamsRepository;
        IActivityRepository _activityRepository;
        IRankingsRepository _rankingRepository;
        IPositionsRepository _positionsRepository;
        IEstimatedRankingRepository _estimatedRankingRepository;
        public Teams _team;

        public TeamCalculations(ITeamsRepository teamsRepo, IActivityRepository activityRepo,
            IRankingsRepository rankingRepo, IPositionsRepository positionsRepo, IEstimatedRankingRepository estimatedRepo, Teams team)
        {
            _teamsRepository = teamsRepo;
            _activityRepository = activityRepo;
            _rankingRepository = rankingRepo;
            _positionsRepository = positionsRepo;
            _estimatedRankingRepository = estimatedRepo;
            _team = team;
        }

        public static int changeInMonth(Teams team, List<Match> matches, Rankings teamsStrength)
        {
            double opponentsStrength = 0;
            double opponentsNumber = 0;
            double numberOfWins = 0;
            double pointsDifference = 0;

            foreach (Match match in matches)
            {
                if (match.Team1 == team)
                {
                    PropertyInfo propInfo = teamsStrength.GetType().GetProperty(match.Team2.Name.Replace(' ','_').Replace('.','_'));
                    opponentsStrength += Convert.ToInt32(propInfo.GetValue(teamsStrength));
                    if (match.PointsTeam1 > match.PointsTeam2) numberOfWins++;
                    pointsDifference += match.PointsTeam1 - match.PointsTeam2;
                }
                else if (match.Team2 == team)
                {
                    PropertyInfo propInfo = teamsStrength.GetType().GetProperty(match.Team1.Name.Replace(' ', '_').Replace('.', '_'));
                    opponentsStrength += Convert.ToInt32(propInfo.GetValue(teamsStrength));
                    if (match.PointsTeam1 < match.PointsTeam2) numberOfWins++;
                    pointsDifference += match.PointsTeam2 - match.PointsTeam1;
                }
                opponentsNumber++;
            }
            opponentsStrength = opponentsStrength / opponentsNumber;
            numberOfWins = numberOfWins / opponentsNumber;
            double performanceRating = opponentsStrength + (numberOfWins - 0.5) * 1000 + pointsDifference;
            performanceRating = (performanceRating * opponentsNumber + 1000 * 5) / (opponentsNumber + 5);

            return (int)performanceRating;
        }

        public int[] GetSeasonRanking(PropertyInfo propInfo, Season season)
        {
            if (propInfo == null) propInfo = new EstimatedRankings().GetType().GetProperty(_team.Name.Replace(' ', '_').Replace('.', '_'));
            int[] rankings = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
            string novId = string.Format("{0} {1}", season, Month.November);
            string decId = string.Format("{0} {1}", season, Month.December);
            string janId = string.Format("{0} {1}", season, Month.January);
            string febId = string.Format("{0} {1}", season, Month.February);
            string marId = string.Format("{0} {1}", season, Month.March);
            string aprId = string.Format("{0} {1}", season, Month.April);
            string poId = string.Format("{0} {1}", season, Month.PlayOffs);
            try
            {
                rankings[0] = Convert.ToInt32(propInfo.GetValue(_estimatedRankingRepository.First(x => x.ID == novId)));
                rankings[1] = Convert.ToInt32(propInfo.GetValue(_estimatedRankingRepository.First(x => x.ID == decId)));
                rankings[2] = Convert.ToInt32(propInfo.GetValue(_estimatedRankingRepository.First(x => x.ID == janId)));
                rankings[3] = Convert.ToInt32(propInfo.GetValue(_estimatedRankingRepository.First(x => x.ID == febId)));
                rankings[4] = Convert.ToInt32(propInfo.GetValue(_estimatedRankingRepository.First(x => x.ID == marId)));
                rankings[5] = Convert.ToInt32(propInfo.GetValue(_estimatedRankingRepository.First(x => x.ID == aprId)));
                rankings[6] = Convert.ToInt32(propInfo.GetValue(_estimatedRankingRepository.First(x => x.ID == poId)));
            }
            catch { }
            return rankings;
        }

        public Dictionary<Season, int> GetAllTimeRanking()
        {
            Dictionary<Season, int[]> pointsAllTime = new Dictionary<Season, int[]>();
            Dictionary<Season, int> estimatedRanking = EstimateAllTimeRanking();
            return estimatedRanking;
        }

        public Dictionary<Season, bool> CheckWhenTeamWasActive()
        {
            Dictionary<Season, bool> activity = new Dictionary<Season, bool>();
            bool isActive;
            PropertyInfo propInfo = new Activity().GetType().GetProperty(_team.Name.Replace(' ', '_').Replace('.', '_'));
            foreach (Season season in Enum.GetValues(typeof(Season)))
            {
                isActive = CheckIfTeamWasActive(propInfo, season);
                activity.Add(season, isActive);
            }
            return activity;
        }

        public Dictionary<Season, int[]> GetHistory(Teams team, Season season)
        {
            List<Season> seasons = new List<Season>();
            if ((int)season > 1)
            {                
                if ((int)season > 2)
                {                    
                    if ((int)season > 3)
                    {                        
                        if ((int)season > 4)
                        {                         
                            if ((int)season > 5)
                            {
                                seasons.Add(season - 5);
                            }
                            seasons.Add(season - 4);
                        }
                        seasons.Add(season - 3);
                    }
                    seasons.Add(season - 2);
                }
                seasons.Add(season - 1);
            }
            seasons.Add(season);
            Dictionary<Season, int[]> history = new Dictionary<Season, int[]>();
            int[] values;
            foreach (Season seas in seasons)
            {
                values = GetTeamPointsOfSeason(seas);
                history.Add(seas, values);
            }
            return history;
        }

        private Dictionary<Season, int> EstimateAllTimeRanking()
        {
            PropertyInfo propInfo = new EstimatedRankings().GetType().GetProperty(_team.Name.Replace(' ', '_').Replace('.', '_'));
            Dictionary<Season, int> estimatedRanking = new Dictionary<Season, int>();
            foreach (Season season in Enum.GetValues(typeof(Season)))
            {
                string poId = string.Format("{0} {1}", season, Month.PlayOffs);
                int score = 0;
                try
                {
                    score = Convert.ToInt32(propInfo.GetValue(_estimatedRankingRepository.First(x => x.ID == poId)));
                }
                catch { }
                estimatedRanking.Add(season, score);
            }
            return estimatedRanking;
        }

        #region Teams
        public bool CheckIfTeamWasActive(PropertyInfo propInfo, Season season)
        {
            IEnumerable<Activity> activityList = _activityRepository.AsQueryable().Where(x => x.Id == season.ToString());
            if (!activityList.Any()) return false;
            Activity activity = activityList.First();
            return Convert.ToBoolean(propInfo.GetValue(activity));
        }

        public int[] GetTeamPointsOfSeason(Season season)
        {
            PropertyInfo propInfo = new Rankings().GetType().GetProperty(_team.Name.Replace(' ', '_'));
            int[] points = new int[7];
            points[0] = getTeamPointsOfMonth(propInfo, season, Month.November);
            points[1] = getTeamPointsOfMonth(propInfo, season, Month.December);
            points[2] = getTeamPointsOfMonth(propInfo, season, Month.January);
            points[3] = getTeamPointsOfMonth(propInfo, season, Month.February);
            points[4] = getTeamPointsOfMonth(propInfo, season, Month.March);
            points[5] = getTeamPointsOfMonth(propInfo, season, Month.April);
            points[6] = getTeamPointsOfMonth(propInfo, season, Month.PlayOffs);
            return points;
        }

        private int getTeamPointsOfMonth(PropertyInfo propInfo, Season season, Month month)
        {
            string id = string.Format("{0} {1}", season, month);
            IEnumerable<Rankings> rankingList = _rankingRepository.AsQueryable().Where(x => x.ID.Contains(id));
            if (!rankingList.Any()) return 0;
            Rankings ranking = rankingList.First();
            return Convert.ToInt32(propInfo.GetValue(ranking));
        }
        #endregion
    }
}
