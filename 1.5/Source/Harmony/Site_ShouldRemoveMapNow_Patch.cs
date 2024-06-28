using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(Site), "ShouldRemoveMapNow")]
    public static class Site_ShouldRemoveMapNow_Patch
    {
        public static void Postfix(Site __instance, ref bool __result)
        {
            if (__result && __instance.parts != null)
            {
                foreach (var part in __instance.parts)
                {
                    if (part.def.Worker is SitePartWorker_InsectHive)
                    {
                        if (__instance.Map.mapPawns.AllPawns.Where(x => x.RaceProps.Insect 
                            && x.HostileTo(Faction.OfPlayer) && !x.Dead && !x.Destroyed).Any()
                            || __instance.Map.listerThings.AllThings.Any(x => x is Hive))
                        {
                            __result = false;
                            return;
                        }
                    }
                }
            }
        }
    }
}
