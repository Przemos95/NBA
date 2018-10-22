using NBAMetrics.Core.Domain.Entities.Enum;
using NBAMetrics.Core.Domain.Entities.Identity;
using NBAMetrics.DataAccess;
using NBAMetrics.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NBAMetrics.Core.Calculations
{
    public class SeasonCalculations
    {
        Teams[] allTeams;
        IActivityRepository _activityRepository;
        IRankingsRepository _rankingRepository;
        IPositionsRepository _positionsRepository;
        IEstimatedRankingRepository _estimatedRankingRepository;
        Season _currentSeason;
        public SeasonCalculations(Season season, ITeamsRepository teamsRepo, IActivityRepository activityRepo,
            IRankingsRepository rankingRepo, IPositionsRepository positionsRepo, IEstimatedRankingRepository estimatedRepo)
        {
            allTeams = teamsRepo.AsQueryable().ToArray();
            _activityRepository = activityRepo;
            _rankingRepository = rankingRepo;
            _positionsRepository = positionsRepo;
            _estimatedRankingRepository = estimatedRepo;
            _currentSeason = season;          
        }

        #region public

        public void GetRanking(out EstimatedRankings[] points, out Activity isActive)
        {
            bool fileExists = CheckIfRankingExist();
            if (!fileExists)
            {
                CalculateRanking();
            }
            points = EstimateRanking(out isActive);
            SavePositions(points[6], isActive);
        }

        public Positions GetPosition(Season season)
        {
            List<Positions> positions = _positionsRepository.AsQueryable().Where(x => x.ID == season.ToString()).ToList();
            if (positions.Any()) return positions.First();
            return null;
        }

        public void CalculateRanking()
        {
            Rankings teamsStrength = calculateTeamsStrength();
            Rankings[] _pointsInRegularSeason = changeInRegularSeason(teamsStrength);
            Activity isActive = new Activity();
            foreach (Teams team in allTeams)
            {
                PropertyInfo propInfoRanking = _pointsInRegularSeason[0].GetType().GetProperty(team.Name.Replace(' ', '_').Replace('.','_'));
                PropertyInfo propInfoActivity = isActive.GetType().GetProperty(team.Name.Replace(' ','_').Replace('.', '_'));
                if (Convert.ToInt32(propInfoRanking.GetValue(_pointsInRegularSeason[4])) == 0)
                    propInfoActivity.SetValue(isActive, false);
                else
                    propInfoActivity.SetValue(isActive, true);
            }         
            Rankings _pointsInPlayoffs = changeInPlayoffs(_pointsInRegularSeason);
            SaveRanking(_pointsInRegularSeason, _pointsInPlayoffs, isActive);
        }

        public bool CheckIfRankingExist()
        {
            Rankings[] points = GetSeasonRanking(_currentSeason);
            if (points != null) return true;
            return false;
        }
         
        #endregion public

        #region changeInRegularSeason/PlayOff

        private Rankings changeInPlayoffs(Rankings[] pointsRegularSeason)
        {
            //Calculate strength --start--
            Rankings teamStrength = new Rankings();
            foreach (Rankings item in pointsRegularSeason)
            {
                foreach (Teams team in allTeams)
                {
                    PropertyInfo propInfo = teamStrength.GetType().GetProperty(team.Name.Replace(' ','_').Replace('.', '_'));
                    propInfo.SetValue(teamStrength, Convert.ToInt32(propInfo.GetValue(teamStrength)) + Convert.ToInt32(propInfo.GetValue(item)));
                }
            }
            foreach (Teams team in allTeams)
            {
                PropertyInfo propInfo = teamStrength.GetType().GetProperty(team.Name.Replace(' ', '_').Replace('.', '_'));
                propInfo.SetValue(teamStrength, (Convert.ToInt32(propInfo.GetValue(teamStrength)) / 5));
            }
            //--end--
            List<Match> matches = getMatchesFromFile(_currentSeason.ToString()).ElementAt(6);
            Rankings points = MonthCalculations.changeInMonth(matches, allTeams, teamStrength);         
            return points;
        }

        private Rankings[] changeInRegularSeason(Rankings teamsStrength)
        {
            Rankings[] points = new Rankings[6];
            List<List<Match>> matches = getMatchesFromFile(_currentSeason.ToString());
            matches.RemoveAt(6); //remove PlayOffs
            Rankings pointsInMonth;
            int i = 0;
            foreach (List<Match> month in matches)
            {
                pointsInMonth = MonthCalculations.changeInMonth(month, allTeams, teamsStrength);
                points[i] = pointsInMonth;
                i++;
            }
            return points;
        }
        #endregion changeInRegularSeason/PlayOff

        #region Streams/Get/Save

        /// <summary>
        /// Zapisuje rankingi w kolejnych miesiącach
        /// </summary>
        /// <param name="points"></param>
        /// <param name="playoff"></param>
        /// <param name="isActive"></param>
        private void SaveRanking(Rankings[] points, Rankings playoff, Activity isActive)
        {
            Rankings november = new Rankings(),
                december = new Rankings(),
                january = new Rankings(),
                february = new Rankings(),
                march = new Rankings(),
                april = new Rankings();

            foreach (Teams team in allTeams)
            {
                PropertyInfo propInfo = points[0].GetType().GetProperty(team.Name.Replace(' ','_').Replace('.','_'));
                propInfo.SetValue(november, propInfo.GetValue(points[0]));
                propInfo.SetValue(december, propInfo.GetValue(points[1]));
                propInfo.SetValue(january, propInfo.GetValue(points[2]));
                propInfo.SetValue(february, propInfo.GetValue(points[3]));
                propInfo.SetValue(march, propInfo.GetValue(points[4]));
                propInfo.SetValue(april, propInfo.GetValue(points[5]));
            }
            november.ID =  string.Format("{0} {1}", _currentSeason.ToString(), Month.November.ToString());
            _rankingRepository.Add(november);
            december.ID = string.Format("{0} {1}", _currentSeason.ToString(), Month.December.ToString());
            _rankingRepository.Add(december);
            january.ID = string.Format("{0} {1}", _currentSeason.ToString(), Month.January.ToString());
            _rankingRepository.Add(january);
            february.ID = string.Format("{0} {1}", _currentSeason.ToString(), Month.February.ToString());
            _rankingRepository.Add(february);
            march.ID = string.Format("{0} {1}", _currentSeason.ToString(), Month.March.ToString());
            _rankingRepository.Add(march);
            april.ID = string.Format("{0} {1}", _currentSeason.ToString(), Month.April.ToString());
            _rankingRepository.Add(april);
            playoff.ID = string.Format("{0} {1}", _currentSeason.ToString(), Month.PlayOffs.ToString());
            _rankingRepository.Add(playoff);
            _rankingRepository.SaveChanges();

            isActive.Id  = _currentSeason.ToString();
            _activityRepository.Add(isActive);
            _activityRepository.SaveChanges();
        }

        private EstimatedRankings[] EstimateRanking(out Activity isActive)
        {
            string currentSeasonId = _currentSeason.ToString();
            if (_estimatedRankingRepository.Where(x => x.ID.StartsWith(currentSeasonId)).Any())
            {
                EstimatedRankings[] estimatedRng = GetEstimatedRanking();
                isActive = _activityRepository.First(x => x.Id.StartsWith(_currentSeason.ToString()));
                return estimatedRng;
            }

            //read month by month rankings from last 5 years
            Rankings[] rankingSeason_0 = GetSeasonRanking(_currentSeason);
            Rankings[] rankingSeason_1 = GetSeasonRanking(_currentSeason - 1); if (rankingSeason_1 == null) rankingSeason_1 = new Rankings[7];
            Rankings[] rankingSeason_2 = GetSeasonRanking(_currentSeason - 2); if (rankingSeason_2 == null) rankingSeason_2 = new Rankings[7];
            Rankings[] rankingSeason_3 = GetSeasonRanking(_currentSeason - 3); if (rankingSeason_3 == null) rankingSeason_3 = new Rankings[7];
            Rankings[] rankingSeason_4 = GetSeasonRanking(_currentSeason - 4); if (rankingSeason_4 == null) rankingSeason_4 = new Rankings[7];
            Rankings[] rankingSeason_5 = GetSeasonRanking(_currentSeason - 5); if (rankingSeason_5 == null) rankingSeason_5 = new Rankings[7] { new Rankings(), new Rankings(), new Rankings(), new Rankings(), new Rankings(), new Rankings(), new Rankings() };

            //estimated ranking -> ranking from last 5 year (dec_5 - nov_0 or apr_5-march_0)
            Rankings[] estimatedRanking = new Rankings[7] { new Rankings(), new Rankings(), new Rankings(), new Rankings(), new Rankings(), new Rankings(), new Rankings() };
            Rankings rankingSeasonFrom_4_To_1 = new Rankings();

            List<Rankings[]> rankingsToAdd = new List<Rankings[]>();
            rankingsToAdd.Add(rankingSeason_1);
            rankingsToAdd.Add(rankingSeason_2);
            rankingsToAdd.Add(rankingSeason_3);
            rankingsToAdd.Add(rankingSeason_4);

            //foreach ranking (rankingSeason_1, rankingSeason_2, etc)
            foreach (var rankingSeason in rankingsToAdd)
            {
                //foreach month in rankingSeason
                foreach (var month in rankingSeason)
                {
                    if (month != null)
                    {
                        //foreach team in month
                        foreach (Teams team in allTeams)
                        {
                            PropertyInfo propInfo = month.GetType().GetProperty(team.Name.Replace(' ', '_').Replace('.','_'));
                            propInfo.SetValue(rankingSeasonFrom_4_To_1, Convert.ToInt32(propInfo.GetValue(rankingSeasonFrom_4_To_1)) + Convert.ToInt32(propInfo.GetValue(month)));
                        }
                    }
                }
            }
            string id = string.Format("{0}", _currentSeason);
            isActive = _activityRepository.First(x => x.Id == id);
            //november
            foreach (Teams team in allTeams)
            {
                PropertyInfo activityInfo = isActive.GetType().GetProperty(team.Name.Replace(' ', '_').Replace('.', '_'));
                PropertyInfo propInfo = rankingSeason_0[0].GetType().GetProperty(team.Name.Replace(' ', '_').Replace('.', '_'));
                if ((bool)activityInfo.GetValue(isActive))
                {
                    propInfo.SetValue(estimatedRanking[0], (Convert.ToInt32(propInfo.GetValue(rankingSeason_0[0]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeasonFrom_4_To_1) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[1]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[2]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[3]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[4]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[5]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[6]) ?? 0)));
                    //december
                    propInfo.SetValue(estimatedRanking[1], (Convert.ToInt32(propInfo.GetValue(rankingSeason_0[1]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[0]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeasonFrom_4_To_1) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[2]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[3]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[4]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[5]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[6]) ?? 0)));
                    //january
                    propInfo.SetValue(estimatedRanking[2], (Convert.ToInt32(propInfo.GetValue(rankingSeason_0[2]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[1]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[0]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeasonFrom_4_To_1) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[3]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[4]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[5]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[6]) ?? 0)));
                    //february
                    propInfo.SetValue(estimatedRanking[3], (Convert.ToInt32(propInfo.GetValue(rankingSeason_0[3]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[2]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[1]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[0]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeasonFrom_4_To_1) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[4]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[5]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[6]) ?? 0)));
                    //march
                    propInfo.SetValue(estimatedRanking[4], (Convert.ToInt32(propInfo.GetValue(rankingSeason_0[4]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[3]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[2]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[1]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[0]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeasonFrom_4_To_1) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[5]) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[6]) ?? 0)));
                    //april
                    propInfo.SetValue(estimatedRanking[5], (Convert.ToInt32(propInfo.GetValue(rankingSeason_0[5]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[4]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[3]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[2]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[1]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[0]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeasonFrom_4_To_1) ?? 0)
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_5[6]) ?? 0)));
                    //playoff
                    propInfo.SetValue(estimatedRanking[6], (Convert.ToInt32(propInfo.GetValue(rankingSeason_0[6]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[5]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[4]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[3]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[2]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[1]))
                        + Convert.ToInt32(propInfo.GetValue(rankingSeason_0[0]))
                           + Convert.ToInt32(propInfo.GetValue(rankingSeasonFrom_4_To_1) ?? 0)));
                }
                else
                {
                    propInfo.SetValue(estimatedRanking[0], 0);
                    propInfo.SetValue(estimatedRanking[1], 0);
                    propInfo.SetValue(estimatedRanking[2], 0);
                    propInfo.SetValue(estimatedRanking[3], 0);
                    propInfo.SetValue(estimatedRanking[4], 0);
                    propInfo.SetValue(estimatedRanking[5], 0);
                    propInfo.SetValue(estimatedRanking[6], 0);
                }
            }
            estimatedRanking[0].ID = string.Format("{0} {1}", _currentSeason, Month.November);
            estimatedRanking[1].ID = string.Format("{0} {1}", _currentSeason, Month.December);
            estimatedRanking[2].ID = string.Format("{0} {1}", _currentSeason, Month.January);
            estimatedRanking[3].ID = string.Format("{0} {1}", _currentSeason, Month.February);
            estimatedRanking[4].ID = string.Format("{0} {1}", _currentSeason, Month.March);
            estimatedRanking[5].ID = string.Format("{0} {1}", _currentSeason, Month.April);
            estimatedRanking[6].ID = string.Format("{0} {1}", _currentSeason, Month.PlayOffs);
            //string id = string.Format("{0}", _currentSeason);
            //isActive = _activityRepository.First(x => x.Id == id);
            return SaveEstimatedRanking(estimatedRanking);
        }

        /// <summary>
        /// Returns list of months. Every month have matches list.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private List<List<Match>> getMatchesFromFile(string folder)
        {
            List<List<Match>> months = new List<List<Match>>();
            string path = string.Format("{0}\\Data\\Scores\\{1}.txt", Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), folder);
            StreamReader sr;
            string content;
            using (sr = new StreamReader(path))
            {
                content = sr.ReadToEnd();
                sr.Close();
            }
            string[] matches = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<Match> matchesList = new List<Match>();
            foreach (string match in matches)
            {
                if (match.Equals("Listopad")) { }
                else if (match.Equals("Grudzien") || match.Equals("Styczen") || match.Equals("Luty") 
                    || match.Equals("Marzec") || match.Equals("Kwiecien") || match.Equals("PlayOffs"))
                {
                    months.Add(matchesList);
                    matchesList = new List<Match>();
                }
                else
                    matchesList.Add(recognizeMatch(match));
            }
            months.Add(matchesList);
            return months;
        }

        private void SavePositions(EstimatedRankings points, Activity isActive)
        {
            if (!_positionsRepository.AsQueryable().Where(x => x.ID == _currentSeason.ToString()).Any())
            {
                KeyValuePair<Teams, int?>[] teamPoints = new KeyValuePair<Teams, int?>[44];
                for (int i = 0; i < 44; i++)
                {
                    PropertyInfo propInfo = points.GetType().GetProperty(allTeams[i].Name.Replace(' ','_').Replace('.','_'));
                    teamPoints[i] = new KeyValuePair<Teams, int?>(allTeams[i], Convert.ToInt32(propInfo.GetValue(points)));
                }
                teamPoints = teamPoints.OrderByDescending(m => m.Value).ToArray();
                for (int i = 0; i < teamPoints.Length; i++)
                {
                    if (teamPoints[i].Value == 0)
                        teamPoints[i] = new KeyValuePair<Teams, int?>(teamPoints[i].Key, null);
                    else
                        teamPoints[i] = new KeyValuePair<Teams, int?>(teamPoints[i].Key, i + 1);
                }
                Positions positions = new Positions();
                positions.ID = _currentSeason.ToString();
                for (int i = 0; i < allTeams.Length; i++)
                {
                    int? pkt = teamPoints.Where(m => m.Key == allTeams[i]).First().Value;
                    PropertyInfo propInfo = new Positions().GetType().GetProperty(allTeams[i].Name.Replace(' ', '_').Replace('.','_'));
                    if (pkt != null && pkt != 0)
                        propInfo.SetValue(positions, Convert.ToInt32(teamPoints.Where(m => m.Key == allTeams[i]).First().Value));
                    else
                        propInfo.SetValue(positions, null);
                }
                _positionsRepository.Add(positions);
                _positionsRepository.SaveChanges();
            }
        }

        private EstimatedRankings[] SaveEstimatedRanking(Rankings[] points)
        {
            EstimatedRankings[] estimatedRanking = new EstimatedRankings[7];
            for (int i = 0; i < points.Length; i++)
            {
                estimatedRanking[i] = new EstimatedRankings()
                {
                    Anderson_Packers = points[i].Anderson_Packers, Atlanta_Hawks = points[i].Atlanta_Hawks, Baltimore_Bullets = points[i].Baltimore_Bullets, Boston_Celtics = points[i].Boston_Celtics,
                    Brooklyn_Nets = points[i].Brooklyn_Nets, Charlotte_Hornets = points[i].Charlotte_Hornets, Chicago_Stags = points[i].Chicago_Stags, Chicago_Bulls = points[i].Chicago_Bulls,
                    Cleveland_Cavaliers = points[i].Cleveland_Cavaliers, Cleveland_Rebels = points[i].Cleveland_Rebels, Dallas_Mavericks = points[i].Dallas_Mavericks, Denver_Nuggets = points[i].Denver_Nuggets,
                    Detroit_Pistons = points[i].Detroit_Pistons, Detroit_Falcons = points[i].Detroit_Falcons, Golden_State_Warriors = points[i].Golden_State_Warriors, Houston_Rockets = points[i].Houston_Rockets,
                    ID = points[i].ID, Indianapolis_Jets = points[i].Indianapolis_Jets, Indianapolis_Olympians = points[i].Indianapolis_Olympians, Indiana_Pacers = points[i].Indiana_Pacers, LA_Clipers = points[i].LA_Clipers,
                    Los_Angeles_Lakers = points[i].Los_Angeles_Lakers, Memphis_Grizzlies = points[i].Memphis_Grizzlies, Miami_Heat = points[i].Miami_Heat, Milwaukee_Bucks = points[i].Milwaukee_Bucks,
                    Minnesota_Timberwolves = points[i].Minnesota_Timberwolves, New_Orleans_Pelicans = points[i].New_Orleans_Pelicans, New_York_Knicks = points[i].New_York_Knicks, Oklahoma_City_Thunder = points[i].Oklahoma_City_Thunder,
                    Orlando_Magic = points[i].Orlando_Magic, Philadelphia_76ers = points[i].Philadelphia_76ers, Phoenix_Suns = points[i].Phoenix_Suns, Pittsburgh_Ironmen = points[i].Pittsburgh_Ironmen,
                    Portland_Trail_Blazers = points[i].Portland_Trail_Blazers, Providence_Steam_Rollers = points[i].Providence_Steam_Rollers, Sacramento_Kings = points[i].Sacramento_Kings, San_Antonio_Spurs = points[i].San_Antonio_Spurs,
                    Sheboygan_Red_Skins = points[i].Sheboygan_Red_Skins, St__Louis_Bombers = points[i].St__Louis_Bombers, Toronto_Huskies = points[i].Toronto_Huskies, Toronto_Raptors = points[i].Toronto_Raptors,
                    Utah_Jazz = points[i].Utah_Jazz, Washington_Capitols = points[i].Washington_Capitols, Washington_Wizards = points[i].Washington_Wizards, Waterloo_Hawks = points[i].Waterloo_Hawks 
                };
            }

            foreach (var rnk in estimatedRanking)
            {
                _estimatedRankingRepository.Add(rnk);
            }
            _estimatedRankingRepository.SaveChanges();
            return estimatedRanking;
        }

        private EstimatedRankings[] GetEstimatedRanking()
        {
            EstimatedRankings[] rankings = new EstimatedRankings[7];
            string nov = string.Format("{0} {1}", _currentSeason, Month.November);
            string dec = string.Format("{0} {1}", _currentSeason, Month.December);
            string jan = string.Format("{0} {1}", _currentSeason, Month.January);
            string feb = string.Format("{0} {1}", _currentSeason, Month.February);
            string mar = string.Format("{0} {1}", _currentSeason, Month.March);
            string apr = string.Format("{0} {1}", _currentSeason, Month.April);
            string po = string.Format("{0} {1}", _currentSeason, Month.PlayOffs);
            rankings[0] = _estimatedRankingRepository.First(x => x.ID == nov);
            rankings[1] = _estimatedRankingRepository.First(x => x.ID == dec);
            rankings[2] = _estimatedRankingRepository.First(x => x.ID == jan);
            rankings[3] = _estimatedRankingRepository.First(x => x.ID == feb);
            rankings[4] = _estimatedRankingRepository.First(x => x.ID == mar);
            rankings[5] = _estimatedRankingRepository.First(x => x.ID == apr);
            rankings[6] = _estimatedRankingRepository.First(x => x.ID == po);
            return rankings;
        }
        #endregion 

        #region Recognize
        private Match recognizeMatch(string match)
        {
            string[] param = match.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            if (param.Length == 1) return null;
            Teams team1 = recognizeTeam(param[0]);
            Teams team2 = recognizeTeam(param[3]);
            return new Match(team1, team2, int.Parse(param[1]), int.Parse(param[2]));
        }

        private Teams recognizeTeam(string name)
        {
            foreach (Teams team in allTeams)
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
        #endregion Recognize

        #region Helpers

        /// <summary>
        /// Returns list of teams strength based on last 8 seasons.
        /// </summary>
        private Rankings calculateTeamsStrength()
        {
            Rankings teamStrength_01 = calculateTeamPoints(_currentSeason - 1);
            Rankings teamStrength_02 = calculateTeamPoints(_currentSeason - 2);
            Rankings teamStrength_03 = calculateTeamPoints(_currentSeason - 3);
            Rankings teamStrength_04 = calculateTeamPoints(_currentSeason - 4);
            Rankings teamStrength_05 = calculateTeamPoints(_currentSeason - 5);
            Rankings teamStrength_06 = calculateTeamPoints(_currentSeason - 6);
            Rankings teamStrength_07 = calculateTeamPoints(_currentSeason - 7);
            Rankings teamStrength_08 = calculateTeamPoints(_currentSeason - 8);

            Rankings finalRanking = new Rankings(); 
            foreach (Teams team in allTeams)
            {
                PropertyInfo propInfo = finalRanking.GetType().GetProperty(team.Name.Replace(' ', '_').Replace('.', '_'));
                propInfo.SetValue(finalRanking, Convert.ToInt32((Convert.ToDouble(propInfo.GetValue(teamStrength_01)) * 0.3
                    + Convert.ToDouble(propInfo.GetValue(teamStrength_02)) * 0.25
                    + Convert.ToDouble(propInfo.GetValue(teamStrength_03)) * 0.18
                    + Convert.ToDouble(propInfo.GetValue(teamStrength_04)) * 0.12
                    + Convert.ToDouble(propInfo.GetValue(teamStrength_05)) * 0.08
                    + Convert.ToDouble(propInfo.GetValue(teamStrength_06)) * 0.04
                    + Convert.ToDouble(propInfo.GetValue(teamStrength_07)) * 0.02
                    + Convert.ToDouble(propInfo.GetValue(teamStrength_08)) * 0.01)));
            }
            return finalRanking;
        }

        /// <summary>
        /// Returns sum of points for every team in this season.
        /// </summary>
        /// <param name="season"></param>
        private Rankings calculateTeamPoints(Season season)
        {
            Rankings[] rankingSeason = GetSeasonRanking(season);
            Rankings ranking = new Rankings();

            if (rankingSeason == null)
            {
                foreach (Teams team in allTeams)
                {
                    PropertyInfo propInfo = ranking.GetType().GetProperty(team.Name.Replace(' ', '_').Replace('.', '_'));
                    propInfo.SetValue(ranking, 1000);
                }
                return ranking;
            }

            foreach (Teams team in allTeams)
            {
                PropertyInfo propInfo = ranking.GetType().GetProperty(team.Name.Replace(' ','_').Replace('.', '_'));
                //sumowanie wszystkich rankingów dla danej drużyny. Jeżeli nie ma punktów, ustawiamy
                //startowo na 1000
                //foreach (Rankings rnk in rankingSeason)
                //{
                //    propInfo.SetValue(ranking, Convert.ToInt64(propInfo.GetValue(ranking)) + Convert.ToInt64(propInfo.GetValue(rnk)));
                //}
                propInfo.SetValue(ranking, rankingSeason.Sum(x => Convert.ToInt32(propInfo.GetValue(x))));
                if (Convert.ToInt32(propInfo.GetValue(ranking)) == 0)
                {
                    propInfo.SetValue(ranking, 1000);
                }
                else
                {
                    int number = 7;
                    if (Convert.ToInt32(propInfo.GetValue(rankingSeason[0])) == 0) number--;
                    if (Convert.ToInt32(propInfo.GetValue(rankingSeason[1])) == 0) number--;
                    if (Convert.ToInt32(propInfo.GetValue(rankingSeason[2])) == 0) number--;
                    if (Convert.ToInt32(propInfo.GetValue(rankingSeason[5])) ==  0) number--;
                    if (Convert.ToInt32(propInfo.GetValue(rankingSeason[6])) == 0) number--;
                    propInfo.SetValue(ranking, (Convert.ToInt32(propInfo.GetValue(ranking)) / number));
                }
            }
            return ranking;
        }
        #endregion Helpers

        #region Rankings
        public Rankings[] GetSeasonRanking(Season season)
        {
            Rankings[] months = new Rankings[7];
            Rankings november = GetMonthRanking(season, Month.November);
            if (november == null) return null;
            months[0] = november;
            months[1] = GetMonthRanking(season, Month.December);
            months[2] = GetMonthRanking(season, Month.January);
            months[3] = GetMonthRanking(season, Month.February);
            months[4] = GetMonthRanking(season, Month.March);
            months[5] = GetMonthRanking(season, Month.April);
            months[6] = GetMonthRanking(season, Month.PlayOffs);
            return months;
        }

        public Rankings GetMonthRanking(Season season, Month month)
        {
            string id = string.Format("{0} {1}", season.ToString(), month.ToString());
            List<Rankings> rankings = _rankingRepository.AsQueryable().Where(x => x.ID == id).ToList();
            if (rankings.Any()) return rankings.First();
            return null;
        }
        #endregion
    }
}
