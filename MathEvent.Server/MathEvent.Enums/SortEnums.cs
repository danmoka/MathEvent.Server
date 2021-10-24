using System.ComponentModel;

namespace MathEvent.Enums
{
    public enum SortBy
    {
        [Description("От ближайших по дате")]
        Closest,
        [Description("От дальних по дате")]
        NotClosest,
        [Description("От А к Я")]
        AtoZ,
        [Description("От Я к А")]
        ZtoA,
    }
}
