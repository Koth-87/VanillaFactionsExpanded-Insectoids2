using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(LordToil_HiveRelated), "FindClosestHive")]
    public static class LordToil_HiveRelated_FindClosestHive_Patch
    {
        public static bool Prefix(ref Hive __result, Pawn pawn)
        {
            __result = (Hive)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, 
                ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.Touch, TraverseParms.For(pawn), 30f, 
                (Thing x) => x is Hive && x.Faction == pawn.Faction, null, 0, 30);
            return false;
        }
    }
}
