using NBAMetrics.Core.Domain.Entities.Enum;
using NBAMetrics.Core.Domain.Entities.Identity;
using NBAMetrics.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace NBAMetrics.Core.Drawing
{
    public class DrawChart
    {
        public string GenerateChart(List<Teams> teams, List<int[]> points, Season season)
        {
            string check = checkIfExist(season);
            if (!string.IsNullOrEmpty(check)) return check;
            string path = string.Empty;
            using (var chart = new Chart())
            {
                chart.ChartAreas.Add(new ChartArea("Top 5 zespołów"));
                int[] minAndMax = GetMinAndMax(points);
                chart.ChartAreas[0].AxisY.Minimum = minAndMax[0] - 100;
                chart.ChartAreas[0].AxisY.Maximum = minAndMax[1] + 100;
                chart.Legends.Add("Legenda");
                chart.Legends[0].Docking = Docking.Bottom;
                chart.Legends[0].LegendStyle = LegendStyle.Row;
                chart.Width = 1000;
                chart.Height = 600;
                List<Series> series = GenerateSeries(teams, points);
                foreach (Series ser in series)
                {
                    chart.Series.Add(ser);
                }
                path = SaveChart(chart, season);
            }
            return path;
        }

        private List<Series> GenerateSeries(List<Teams> teams, List<int[]> points)
        {
            List<Series> seriesList = new List<Series>();
            Series series;
            for (int i = 0; i < teams.Count(); i++)
            {
                series = new Series(teams[i].Name);
                series.ChartType = SeriesChartType.Line;
                series.Points.DataBindXY(new List<string>() { "Listopad", "Grudzień", "Styczeń", "Luty", "Marzec", "Kwiecień", "PlayOff" }, points[i]);
                series.IsVisibleInLegend = true;
                series.IsValueShownAsLabel = true;
                series.BorderWidth = 4;
                seriesList.Add(series);
            }
            seriesList.ElementAt(0).Color = Color.Blue;
            seriesList.ElementAt(1).Color = Color.Green;
            seriesList.ElementAt(2).Color = Color.Yellow;
            seriesList.ElementAt(3).Color = Color.Black;
            seriesList.ElementAt(4).Color = Color.Red;
            return seriesList;
        }

        public string CompareTeamsDraw(List<Teams> teams, List<int[]> points)
        {
            string id = string.Empty;
            using (var chart = new Chart())
            {
                chart.ChartAreas.Add(new ChartArea("Porównanie zespołów"));
                int[] minAndMax = GetMinAndMax(points);
                chart.ChartAreas[0].AxisY.Minimum = 5;// minAndMax[0] - 100;
                chart.ChartAreas[0].AxisY.Maximum = minAndMax[1] + 100;
                chart.Legends.Add("Legenda");
                chart.Legends[0].Docking = Docking.Bottom;
                chart.Legends[0].LegendStyle = LegendStyle.Row;
                chart.Width = 1000;
                chart.Height = 600;
                List<Series> series = GenerateCompareSeries(teams, points);
                foreach (Series ser in series)
                {
                    chart.Series.Add(ser);
                }
                id = Guid.NewGuid().ToString().Replace("-", "");
                string path = SaveChart(chart, id);
            }
            return id;
        }

        List<Series> GenerateCompareSeries(List<Teams> teams, List<int[]> points)
        {
            List<Series> seriesList = new List<Series>();
            Series series;
            for (int i = 0; i < teams.Count(); i++)
            {
                series = new Series(teams[i].Name);
                series.ChartType = SeriesChartType.Line;
                List<string> s = new List<string>();
                foreach (Season season in Enum.GetValues(typeof(Season)))
                {
                    var member = typeof(Season).GetMember(season.ToString());
                    DisplayAttribute displayName = (DisplayAttribute)member[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
                    s.Add(displayName.Name);
                }
                series.Points.DataBindXY(s, points[i]);
                series.IsVisibleInLegend = true;
                series.IsValueShownAsLabel = false;
                series.BorderWidth = 4;
                seriesList.Add(series);
                if (i == 0) seriesList.ElementAt(0).Color = Color.Blue;
                if (i == 1) seriesList.ElementAt(1).Color = Color.Green;
                if (i == 2) seriesList.ElementAt(2).Color = Color.Yellow;
                if (i == 3) seriesList.ElementAt(3).Color = Color.Black;
                if (i == 4) seriesList.ElementAt(4).Color = Color.Red;
            }
            return seriesList;
        }

        int[] GetMinAndMax(List<int[]> points)
        {
            int min = 100000;
            int max = 1000;
            foreach (int[] array  in points)
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

        string checkIfExist(Season season)
        {
            string path = string.Format("{0}\\Data\\Charts\\{1}.png", Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), season);
            if (File.Exists(path)) return path;
            else return string.Empty;
        }

        string SaveChart(Chart chart, Season season)
        {
            string path = string.Format("{0}\\Data\\Charts\\{1}.png", Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), season);
            chart.SaveImage(path, ChartImageFormat.Jpeg);
            return path;
        }

        string SaveChart(Chart chart, string compared)
        {
            string path = string.Format("{0}\\Data\\Charts\\{1}.png", Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), compared);
            chart.SaveImage(path, ChartImageFormat.Jpeg);
            return path;
        }
	}  
}
