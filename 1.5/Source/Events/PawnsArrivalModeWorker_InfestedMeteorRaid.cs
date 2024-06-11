using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace VFEInsectoids
{
    public class PawnsArrivalModeWorker_InfestedMeteorRaid : PawnsArrivalModeWorker
    {
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            Map map = (Map)parms.target;
            int numberOfMeteor = (int)(Mathf.Max(1, parms.points / 1000f));
            var takenCells = new HashSet<IntVec3>();
            var chunkDef = VFEI_DefOf.VFEI_InfestedMeteorIncoming;
            var insectsTotal = new List<Pawn>();
            Lord lord = LordMaker.MakeNewLord(parms.faction, new LordJob_AssaultColony(parms.faction, false, false, false, false, false), map);
            var groups = pawns.ChunkBy(numberOfMeteor);
            var intVec = IncidentWorker_InfestedCrashBase.FindDropPodLocation(map, CanPlaceAt);
            var center = intVec;
            parms.spawnCenter = center;
            for (int i = 0; i < numberOfMeteor; i++)
            {
                var insects = groups[i];
                if (CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.ShipChunkIncoming, map, out intVec, 10,
                    nearLoc: center, nearLocMaxDist: 2 * numberOfMeteor, extraValidator: CanPlaceAt))
                {
                    var chunk = DefDatabase<ThingDef>.AllDefsListForReading.Where(x => !x.thingCategories.NullOrEmpty()
                        && x.thingCategories.Contains(ThingCategoryDefOf.StoneChunks)).RandomElement();
                    var skyfaller = SkyfallerMaker.SpawnSkyfaller(VFEI_DefOf.VFEI_InfestedMeteorIncoming, 
                        insects.Concat(ThingMaker.MakeThing(chunk)), intVec, map);
                    takenCells.AddRange(GenAdj.OccupiedRect(intVec, skyfaller.def.defaultPlacingRot, skyfaller.def.Size).ExpandedBy(3));
                }
            }

            bool CanPlaceAt(IntVec3 loc)
            {
                return IncidentWorker_InfestedCrashBase.CanPlaceAt(loc, map, chunkDef, takenCells);
            }
        }


        public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (!parms.spawnCenter.IsValid)
            {
                parms.spawnCenter = CellFinderLoose.RandomCellWith((i) => i.Walkable(map) == true, map);
            }
            parms.spawnRotation = Rot4.Random;
            return true;
        }
    }
}
