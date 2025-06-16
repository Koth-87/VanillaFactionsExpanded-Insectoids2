using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(InfestationCellFinder), "GetScoreAt")]
    public static class InfestationCellFinder_GetScoreAt_Patch
    {
        public static bool Prefix(ref float __result, IntVec3 cell, Map map)
        {
            if (map.IsInfested())
            {
                __result = GetScoreAt(cell, map);
                return false;
            }
            return true;
        }

        private static float GetScoreAt(IntVec3 cell, Map map)
        {
            if ((float)(int)CellFinderUtility.DistToColonyBuilding[cell] > 30f)
            {
                return 0f;
            }
            if (!cell.Walkable(map))
            {
                return 0f;
            }
            if (cell.Fogged(map))
            {
                return 0f;
            }
            if (InfestationCellFinder.CellHasBlockingThings(cell, map))
            {
                return 0f;
            }
            Region region = cell.GetRegion(map);
            if (region == null)
            {
                return 0f;
            }
            if (InfestationCellFinder.closedAreaSize[cell] < 2)
            {
                return 0f;
            }
            float temperature = cell.GetTemperature(map);
            if (temperature < -17f)
            {
                return 0f;
            }
            return 1;
        }
    }
}
