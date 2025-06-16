using HarmonyLib;
using RimWorld;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(SitePartDef), "FactionCanOwn")]
    public static class SitePartDef_FactionCanOwn_Patch
    {
        public static void Postfix(SitePartDef __instance, Faction faction, ref bool __result)
        {
            if (__result && faction != null && faction.def == FactionDefOf.Insect 
                && __instance.Worker is not SitePartWorker_InsectHive)
            {
                __result = false;
            }
        }
    }
}
