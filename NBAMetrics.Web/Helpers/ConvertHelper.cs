using NBAMetrics.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBAMetrics.Web.Helpers
{
    public static class ConvertHelper
    {
        public static EstimatedRankings ConvertPositionsToRanking(Positions positions)
        {
            EstimatedRankings ranking = new EstimatedRankings()
            {
                Anderson_Packers = positions.Anderson_Packers,
                Atlanta_Hawks = positions.Atlanta_Hawks,
                Baltimore_Bullets = positions.Baltimore_Bullets,
                Boston_Celtics = positions.Boston_Celtics,
                Brooklyn_Nets = positions.Brooklyn_Nets,
                Charlotte_Hornets = positions.Charlotte_Hornets,
                Chicago_Stags = positions.Chicago_Stags,
                Chicago_Bulls = positions.Chicago_Bulls,
                Cleveland_Cavaliers = positions.Cleveland_Cavaliers,
                Cleveland_Rebels = positions.Cleveland_Rebels,
                Dallas_Mavericks = positions.Dallas_Mavericks,
                Denver_Nuggets = positions.Denver_Nuggets,
                Detroit_Pistons = positions.Detroit_Pistons,
                Detroit_Falcons = positions.Detroit_Falcons,
                Golden_State_Warriors = positions.Golden_State_Warriors,
                Houston_Rockets = positions.Houston_Rockets,
                ID = positions.ID,
                Indianapolis_Jets = positions.Indianapolis_Jets,
                Indianapolis_Olympians = positions.Indianapolis_Olympians,
                Indiana_Pacers = positions.Indiana_Pacers,
                LA_Clipers = positions.LA_Clipers,
                Los_Angeles_Lakers = positions.Los_Angeles_Lakers,
                Memphis_Grizzlies = positions.Memphis_Grizzlies,
                Miami_Heat = positions.Miami_Heat,
                Milwaukee_Bucks = positions.Milwaukee_Bucks,
                Minnesota_Timberwolves = positions.Minnesota_Timberwolves,
                New_Orleans_Pelicans = positions.New_Orleans_Pelicans,
                New_York_Knicks = positions.New_York_Knicks,
                Oklahoma_City_Thunder = positions.Oklahoma_City_Thunder,
                Orlando_Magic = positions.Orlando_Magic,
                Philadelphia_76ers = positions.Philadelphia_76ers,
                Phoenix_Suns = positions.Phoenix_Suns,
                Portland_Trail_Blazers = positions.Portland_Trail_Blazers,
                Providence_Steam_Rollers = positions.Providence_Steam_Rollers,
                Sacramento_Kings = positions.Sacramento_Kings,
                San_Antonio_Spurs = positions.San_Antonio_Spurs,
                Sheboygan_Red_Skins = positions.Sheboygan_Red_Skins,
                St__Louis_Bombers = positions.St__Louis_Bombers,
                Toronto_Huskies = positions.Toronto_Huskies,
                Toronto_Raptors = positions.Toronto_Raptors,
                Utah_Jazz = positions.Utah_Jazz,
                Washington_Capitols = positions.Washington_Capitols,
                Washington_Wizards = positions.Washington_Wizards,
                Waterloo_Hawks = positions.Waterloo_Hawks,
                Pittsburgh_Ironmen = positions.Pittsburgh_Ironmen
            };
            return ranking;
        }
    }
}