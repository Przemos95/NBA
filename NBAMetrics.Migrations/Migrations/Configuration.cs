namespace NBAMetrics.Migrations.Migrations
{
    using Core.Domain.Entities.Identity;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<NBAMetrics.Migrations.DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(NBAMetrics.Migrations.DbContext context)
        {
            context.Teams.AddOrUpdate(
                new Team() { TeamID = 1, Name = "Atlanta Hawks", OldNames = "St. Louis Hawks, Milwaukee Hawks, Tri-Cities Blackhawks", IsActive = true },
                new Team() { TeamID = 2, Name = "Boston Celtics", OldNames = "", IsActive = true },
                new Team() { TeamID = 3, Name = "Brooklyn Nets", OldNames = "New Jersey Nets, New York Nets", IsActive = true },
                new Team() { TeamID = 4, Name = "Charlotte Hornets", OldNames = "Charlotte Bobcats", IsActive = true },
                new Team() { TeamID = 5, Name = "Chicago Bulls", OldNames = "", IsActive = true },
                new Team() { TeamID = 6, Name = "Cleveland Cavaliers", OldNames = "", IsActive = true },
                new Team() { TeamID = 7, Name = "Dallas Mavericks", OldNames = "", IsActive = true },
                new Team() { TeamID = 8, Name = "Denver Nuggets", OldNames = "", IsActive = true },
                new Team() { TeamID = 9, Name = "Detroit Pistons", OldNames = "Ft. Wayne Zollner Pistons", IsActive = true },
                new Team() { TeamID = 10, Name = "Golden State Warriors", OldNames = "San Francisco Warriors, Philadelphia Warriors" , IsActive = true },
                new Team() { TeamID = 11, Name = "Houston Rockets", OldNames = "San Diego Rockets" , IsActive = true },
                new Team() { TeamID = 12, Name = "Indiana Pacers", OldNames = "", IsActive = true },
                new Team() { TeamID = 13, Name = "LA Clipers", OldNames = "Los Angeles Clippers, San Diego Clippers, Buffalo Braves" , IsActive = true },
                new Team() { TeamID = 14, Name = "Los Angeles Lakers", OldNames = "Minneapolis Lakers" , IsActive = true },
                new Team() { TeamID = 15, Name = "Memphis Grizzlies", OldNames = "Vancouver Grizzlies" , IsActive = true },
                new Team() { TeamID = 16, Name = "Miami Heat", OldNames = "", IsActive = true },
                new Team() { TeamID = 17, Name = "Milwaukee Bucks", OldNames = "", IsActive = true },
                new Team() { TeamID = 18, Name = "Minnesota Timberwolves", OldNames = "", IsActive = true },
                new Team() { TeamID = 19, Name = "New Orleans Pelicans", OldNames = "New Orleans Hornets, New Orleans, Oklahoma City Hornets" , IsActive = true },
                new Team() { TeamID = 20, Name = "New York Knicks", OldNames = "", IsActive = true },
                new Team() { TeamID = 21, Name = "Oklahoma City Thunder", OldNames = "Seattle SuperSonics" , IsActive = true },
                new Team() { TeamID = 22, Name = "Orlando Magic", OldNames = "", IsActive = true },
                new Team() { TeamID = 23, Name = "Philadelphia 76ers", OldNames = "Syracuse Nationals", IsActive = true },
                new Team() { TeamID = 24, Name = "Phoenix Suns", OldNames = "", IsActive = true },
                new Team() { TeamID = 25, Name = "Portland Trail Blazers", OldNames = "", IsActive = true },
                new Team() { TeamID = 26, Name = "Sacramento Kings", OldNames = "Kansas City Kings, Kansas City-Omaha Kings, Cincinnati Royals, Rochester Royals", IsActive = true },
                new Team() { TeamID = 27, Name = "San Antonio Spurs", OldNames = "", IsActive = true },
                new Team() { TeamID = 28, Name = "Toronto Raptors", OldNames = "", IsActive = true },
                new Team() { TeamID = 29, Name = "Utah Jazz", OldNames = "New Orleans Jazz", IsActive = true },
                new Team() { TeamID = 30, Name = "Washington Wizards", OldNames = "Washington Bullets, Capital Bullets, Baltimore Bullets, Chicago Zephyrs, Chicago Packers", IsActive = true },
                new Team() { TeamID = 31, Name = "Anderson Packers", OldNames = "", IsActive = false },
                new Team() { TeamID = 32, Name = "Baltimore Bullets", OldNames = "", IsActive = false },
                new Team() { TeamID = 33, Name = "Chciago Stags", OldNames = "", IsActive = false },
                new Team() { TeamID = 34, Name = "Cleveland Rebels", OldNames = "", IsActive = false },
                new Team() { TeamID = 35, Name = "Denver Nuggets", OldNames = "", IsActive = false },
                new Team() { TeamID = 36, Name = "Detroid Falcons", OldNames = "", IsActive = false },
                new Team() { TeamID = 37, Name = "Indianapolis Jets", OldNames = "", IsActive = false },
                new Team() { TeamID = 38, Name = "Indianapolis Olympians", OldNames = "", IsActive = false },
                new Team() { TeamID = 39, Name = "Pittsburgh Ironmen", OldNames = "", IsActive = false },
                new Team() { TeamID = 40, Name = "Indianapolis Olympians", OldNames = "", IsActive = false },
                new Team() { TeamID = 41, Name = "Providence Steamrollers", OldNames = "", IsActive = false },
                new Team() { TeamID = 42, Name = "Sheboygan Redskins", OldNames = "", IsActive = false },
                new Team() { TeamID = 43, Name = "St. Louis Bombers", OldNames = "", IsActive = false },
                new Team() { TeamID = 44, Name = "Toronto Huskies", OldNames = "", IsActive = false },
                new Team() { TeamID = 45, Name = "Washington Capitols", OldNames = "", IsActive = false },
                new Team() { TeamID = 46, Name = "Waterloo Hawks", OldNames = "", IsActive = false });
        }
    }
}
