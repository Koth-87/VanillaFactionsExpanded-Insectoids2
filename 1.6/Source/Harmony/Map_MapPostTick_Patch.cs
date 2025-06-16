using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(Map), "MapPostTick")]
    public static class Map_MapPostTick_Patch
    {
        public static void Postfix(Map __instance)
        {
            if (__instance.IsHashIntervalTick(60) && __instance.IsInfested())
            {
                var infestationMtb = GameComponent_Insectoids.Instance.InfestationMtbDays(__instance.Tile);
                if (infestationMtb > 0 && Rand.MTBEventOccurs(infestationMtb, 60000f, 60f))
                {
                    var parms = StorytellerUtility.DefaultParmsNow(IncidentDefOf.Infestation.category, __instance);
                    IncidentDefOf.Infestation.Worker.TryExecute(parms);
                }
            }
        }
    }
}
