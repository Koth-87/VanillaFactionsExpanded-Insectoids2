using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(InfestationUtility), "FindRootTunnelLoc")]
    public static class InfestationUtility_FindRootTunnelLoc_Patch
    {
        public static void Prefix(Map map, ref bool spawnAnywhereIfNoGoodCell, ref bool ignoreRoofIfNoGoodCell)
        {
            if (map.IsInfested())
            {
                spawnAnywhereIfNoGoodCell = ignoreRoofIfNoGoodCell = true;
            }
        }
    }
}
