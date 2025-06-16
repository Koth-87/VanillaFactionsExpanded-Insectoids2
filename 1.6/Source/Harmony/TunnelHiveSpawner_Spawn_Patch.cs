using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(TunnelHiveSpawner), "Spawn")]
    public static class TunnelHiveSpawner_Spawn_Patch
    {
        public static void Prefix(TunnelHiveSpawner __instance, Map map, out float __state)
        {
            __state = __instance.insectsPoints;
            __instance.insectsPoints = 0;
            if (map.IsInfested() && HiveUtility.TotalSpawnedHivesCount(map) >= 30)
            {
                __instance.spawnHive = false;
            }
        }

        public static void Postfix(TunnelHiveSpawner __instance, float __state, Map map, IntVec3 loc)
        {
            if (__state > 0)
            {
                var parms = new PawnGroupMakerParms
                {
                    points = __state,
                    faction = Faction.OfInsects,
                    groupKind = PawnGroupKindDefOf.Combat,
                    tile = __instance.Tile
                };
                var pawns = PawnGroupKindWorker_GeneratePawns_Patch.GenerateInsectPawns(parms);
                foreach (var pawn in pawns)
                {
                    GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(loc, map, 2), map);
                    pawn.mindState.spawnedByInfestationThingComp = __instance.spawnedByInfestationThingComp;
                    if (ModsConfig.BiotechActive)
                    {
                        PollutionUtility.Notify_TunnelHiveSpawnedInsect(pawn);
                    }
                }
                if (pawns.Any())
                {
                    LordMaker.MakeNewLord(Faction.OfInsects, new LordJob_AssaultColony(Faction.OfInsects, canKidnap: true, canTimeoutOrFlee: false), map, pawns);
                }
            }
        }
    }
}
