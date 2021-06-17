using System.Collections.Generic;

namespace MathEvent.Converters.Others
{
    /// <summary>
    /// Типы графиков
    /// Pie - круговая
    /// Bar - гистограмма
    /// </summary>
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
    public class SimpleStatistics
    {
        public string Title { get; set; }

        public string Type { get; set; }

        public string ValueField { get; set; } = "y";

        public string ArgumentField { get; set; } = "x";

        public ICollection<ChartDataPiece> Data { get; set; }
    }
}
