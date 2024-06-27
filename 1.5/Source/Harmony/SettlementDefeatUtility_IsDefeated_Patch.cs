using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(SettlementDefeatUtility), "IsDefeated")]
    public static class SettlementDefeatUtility_IsDefeated_Patch
    {
        public static void Postfix(Map map, Faction faction, ref bool __result)
        {
            if (__result && faction == Faction.OfInsects && map.listerThings.AllThings.Any(x => x is Hive))
            {
                __result = false;
            }
        }
    }

}
