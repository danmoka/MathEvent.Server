using System.Collections.Generic;

namespace MathEvent.Models.Others
{
    public static class ChartTypes
    {
        public static string Pie
        {
            get
            {
                return "pie";
            }
        }
        public static string Bar
        {
            get
            {
                return "bar";
            }
        }
    }

    /// <summary>
    /// Класс, описывающий статистику, для отображения в виде графика
    /// </summary>
    public class ChartData
    {
        public string Title { get; set; }

        public string Type { get; set; }

        public string ValueField { get; set; } = "y";

        public string ArgumentField { get; set; } = "x";

        public ICollection<ChartDataPiece> Data { get; set; }
    }
}
