using NBAMetrics.Core.Domain.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBAMetrics.Web.Models
{
    public class TeamAllTimePointsViewModel
    {
        public List<Season> Season { get; set; }
        public List<int> Points { get; set; }

        public TeamAllTimePointsViewModel(List<Season> season, List<int> points)
        {
            Season = season;
            Points = points;
        }
    }
}