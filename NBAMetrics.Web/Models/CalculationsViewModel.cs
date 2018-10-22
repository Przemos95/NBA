using NBAMetrics.Core.Domain.Entities.Identity;
using NBAMetrics.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBAMetrics.Web.Models
{
    public class CalculationsViewModel
    {
        public Teams Team { get; set; }
        public int[] Points { get; set; }

        public int CurrentPosition { get; set; }

        public int PreviewPosition { get; set; }

        public CalculationsViewModel(Teams team, int[] points)
        {
            Team = team;
            Points = points;
        }

        public CalculationsViewModel(Teams team, int[] points, int currentPosition, int previewPosition)
        {
            Team = team;
            Points = points;
            CurrentPosition = currentPosition;
            PreviewPosition = previewPosition;
        }
    }
}