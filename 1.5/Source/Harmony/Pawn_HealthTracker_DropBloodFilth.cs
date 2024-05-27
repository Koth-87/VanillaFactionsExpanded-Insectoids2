using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VFEInsectoids
{



    [HarmonyPatch(typeof(Pawn_HealthTracker), "DropBloodFilth")]
    public static class VFEInsectoids_Pawn_HealthTracker_DropBloodFilth_Patch
    {
        public static void Postfix(Pawn_HealthTracker __instance)
        {
            
            if(__instance.pawn?.RaceProps.renderTree == VFEI_DefOf.VFEI2_Unarmored) {

                FilthMaker.TryMakeFilth(__instance.pawn.PositionHeld, __instance.pawn.MapHeld, VFEI_DefOf.Filth_RubbleRock, __instance.pawn.LabelIndefinite());

            }


        }
    }

}
