using HarmonyLib;
using RimWorld;
using Verse.AI;
using Verse.AI.Group;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(Pawn_MindState), "StartFleeingBecauseOfPawnAction")]
    public static class Pawn_MindState_StartFleeingBecauseOfPawnAction_Patch
    {
        public static bool Prefix(Pawn_MindState __instance)
        {
            if (__instance.pawn.RaceProps.Insect && __instance.pawn.Faction == Faction.OfInsects 
                && __instance.pawn.GetLord()?.LordJob is LordJob_AssaultColony)
            {
                return false;
            }
            return true;
        }
    }
}
