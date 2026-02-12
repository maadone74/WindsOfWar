using System.Collections.Generic;

namespace WindsOfWar
{
    public static class UnitLibrary
    {
        public static List<UnitData> GetAllUnits()
        {
            var units = new List<UnitData>();
            units.AddRange(AmericanUnits.GetUnits());
            units.AddRange(GermanUnits.GetUnits());
            return units;
        }
    }
}
