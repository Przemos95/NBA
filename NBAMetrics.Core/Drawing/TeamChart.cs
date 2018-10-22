using NBAMetrics.Core.Domain.Entities.Enum;
using NBAMetrics.Core.Domain.Entities.Identity;
using NBAMetrics.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace NBAMetrics.Core.Drawing
{
    public class TeamChart
    {
        Teams _team;
        public TeamChart(Teams team)
        {
            _team = team;
        }

        public string GenerateAllTimeChart(List<Season> seasons, List<int> points)
        {
            string path = string.Empty;
            string check = string.Format("{0}\\Data\\Charts\\{1}\\AllTime.png", Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), _team.Name);
            if (File.Exists(check))
            {
                return check;
            }
            using (var chart = new Chart())
            {
                chart.ChartAreas.Add(new ChartArea());
                int[] minAndMax = GetMinAndMax(new List<int[]>() { points.ToArray() });
                chart.ChartAreas[0].AxisY.Minimum = minAndMax[0] - 100;
                chart.ChartAreas[0].AxisY.Maximum = minAndMax[1] + 100;
                chart.Legends.Add("Legenda");
                chart.Legends[0].Docking = Docking.Bottom;
                chart.Legends[0].LegendStyle = LegendStyle.Row;
                chart.Width = 1000;
                chart.Height = 600;
                Series series = GenerateAllTimeSeries(seasons, points);
                chart.Series.Add(series);
                path = SaveAllTimeChart(chart);
            }
            return path;
        }
        public string GenerateSeasonChart(int[] points, Season season)
        {
            string path = string.Empty;
            string check = string.Format("{0}\\Data\\Charts\\{1}\\{2}.png", Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), _team.Name, season);
            if (File.Exists(check))
            {
                return check;
            }
            using (var chart = new Chart())
            {
                chart.ChartAreas.Add(new ChartArea());
                int[] minandmax = GetMinAndMax(new List<int[]>() { points });
                chart.ChartAreas[0].AxisY.Minimum = minandmax[0] - 100;
                chart.ChartAreas[0].AxisY.Maximum = minandmax[1] + 100;
                chart.Legends.Add("legenda");
                chart.Legends[0].Docking = Docking.Bottom;
                chart.Legends[0].LegendStyle = LegendStyle.Row;
                chart.Width = 1000;
                chart.Height = 600;
                Series series = GenerateSeries(new List<string>() { "listopad", "grudzień", "styczeń", "luty", "marzec", "kwiecień", "playoffs" }, points.ToList());
                chart.Series.Add(series);
                path = SaveSeasonChart(chart, season);
            }
            return path;
        }

        private string SaveAllTimeChart(Chart chart)
        {
            string path = string.Format("{0}\\Data\\Charts\\{1}", Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), _team.Name);
            DirectoryInfo di = Directory.CreateDirectory(path);
            string fullPath = string.Format("{0}\\AllTime.png", path);
            chart.SaveImage(fullPath, ChartImageFormat.Jpeg);
            return fullPath;
        }
        private string SaveSeasonChart(Chart chart, Season season)
        {
            string path = string.Format("{0}\\Data\\Charts\\{1}", Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), _team.Name);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fullPath = string.Format("{0}\\{1}.png", path, season);           
            chart.SaveImage(fullPath, ChartImageFormat.Jpeg);
            return fullPath;
        }

        Series GenerateAllTimeSeries(List<Season> seasons, List<int> points)
        {
            List<string> s = new List<string>();
            foreach (Season season in seasons)
            {
                var member = typeof(Season).GetMember(season.ToString());
                DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
                s.Add(displayName.Name);
            }
            Series series = new Series(_team.Name);
            series.ChartType = SeriesChartType.Line;
            series.Points.DataBindXY(s, points);
            series.ToolTip = "Data Point Y Value: #VALY{6}";
            series.IsVisibleInLegend = true;
            series.IsValueShownAsLabel = true;
            series.BorderWidth = 4;
            return series;
        }
        Series GenerateSeries(List<string> xValues, List<int> points)
        {
            Series series;
            series = new Series(_team.Name);
            series.ChartType = SeriesChartType.Line;
            series.Points.DataBindXY(xValues, points);
            series.ToolTip = "Data Point Y Value: #VALY{6}";
            series.IsVisibleInLegend = true;
            series.IsValueShownAsLabel = true;
            series.BorderWidth = 4;
            return series;
        }

        int[] GetMinAndMax(List<int[]> points)
        {
            int min = 100000;
            int max = 1000;
            foreach (int[] array in points)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] > max)
                    {
                        max = array[i];
                    }
                    if (array[i] < min)
                    {
                        min = array[i];
                    }
                }
            }
            return new int[] { min, max };
        }
    }
}
