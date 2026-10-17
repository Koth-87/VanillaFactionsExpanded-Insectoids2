using HarmonyLib;
using RimWorld;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(PawnsArrivalModeWorker), "CanUseWith")]
    public static class PawnsArrivalModeWorker_CanUseWith_Patch
    {
        public static void Postfix(PawnsArrivalModeWorker __instance, ref bool __result, IncidentParms parms)
        {
            if (parms.faction == Faction.OfInsects &&
                (__instance is PawnsArrivalModeWorker_EdgeDrop
                || __instance is PawnsArrivalModeWorker_CenterDrop 
                || __instance is PawnsArrivalModeWorker_EdgeDropGroups
                || __instance is PawnsArrivalModeWorker_RandomDrop))
            {
                __result = false;
            }
        }
    }
}
