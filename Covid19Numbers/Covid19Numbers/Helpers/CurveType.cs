using System;
namespace Covid19Numbers
{
    public enum CurveType
    {
        Unknown,
        Cases,
        Deaths,
        Recovered,
        NewCasesByDay,
        NewDeathsByDay,
        NewRecoveredByDay
    }
}
