using NBAMetrics.Core.Domain.Entities.Identity;
using NBAMetrics.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NBAMetrics.Core.Calculations
{
    public static class MonthCalculations
    {
        public static Rankings changeInMonth(List<Match> matches, Teams[] teams, Rankings teamsStrength)
        {
            Rankings change = new Rankings();
            for(int i = 0; i < 44; i++)
            {
                List<Match> teamMatches = matches.Where(x => x.Team1 == teams[i] || x.Team2 == teams[i]).ToList();
                PropertyInfo propInfo = change.GetType().GetProperty(teams[i].Name.Replace(' ', '_').Replace('.', '_'));
                if (teamMatches.Count() != 0)
                {
                    propInfo.SetValue(change, TeamCalculations.changeInMonth(teams[i], teamMatches, teamsStrength));
                }
                else
                {
                    propInfo.SetValue(change, 0);
                }
            }
            return change;
        }
    }
}
