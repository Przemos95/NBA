using NBAMetrics.Core.Domain.Entities.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NBAMetrics.Web.Helpers
{
    public static class Helper
    {
        public static string GetSeasonDisplayName(Season season)
        {
            var member = typeof(Season).GetMember(season.ToString());
            DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            return displayName.Name;
        }
    }
}