using NBAMetrics.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBAMetrics.Web.Models
{
    public class CompareTeamsViewModel
    {
        public Dictionary<Teams, int[]> Points { get; set; }
        public string Path { get; set; }
    }
}