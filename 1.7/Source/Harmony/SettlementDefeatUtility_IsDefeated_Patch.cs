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
            if (__result && faction == Faction.OfInsects)
            {
                foreach (var hiveDef in Utils.allHiveDefs)
                {
                    if (map.listerThings.ThingsOfDef(hiveDef).Any(x => x.Faction == faction))
                    {
                        __result = false;
                    }
                }
            }
        }
    }

}
