using NBAMetrics.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBAMetrics.Core.Domain.Entities.Identity
{
    public class Match
    {
        public Teams Team1 { get; set; }
        public Teams Team2 { get; set; }
        public int PointsTeam1 { get; set; }
        public int PointsTeam2 { get; set; }

        public Match(Teams team1, Teams team2, int pointsTeam1, int pointsTeam2)
        {
            Team1 = team1;
            Team2 = team2;
            PointsTeam1 = pointsTeam1;
            PointsTeam2 = pointsTeam2;
        }
    }
}
