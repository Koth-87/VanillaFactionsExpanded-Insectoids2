using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    public abstract class IncidentWorker_InfestedCrashBase : IncidentWorker
    {
        public abstract ThingDef MainPartDef { get;}
        public abstract int MainPartPoints { get; }

        public override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<TargetInfo> list = new List<TargetInfo>();
            var chunkDef = MainPartDef;
            var takenCells = new HashSet<IntVec3>();
            IntVec3 intVec = FindDropPodLocation(map, (IntVec3 spot) => CanPlaceAt(spot));
            if (intVec == IntVec3.Invalid)
            {
                return false;
            }
            var center = intVec;
            var points = parms.points - MainPartPoints;
            var count = (int)(Mathf.Max(1, points / 250f));
            SpawnCrashPart(map, list, takenCells, ref intVec, center, count);
            if (points >= 250f)
            {
                chunkDef = VFEI_DefOf.VFEI2_InfestedShipChunk;
                for (var i = 0; i < count; i++)
                {
                    if (SpawnCrashPart(map, list, takenCells, ref intVec, center, count) is false)
                    {
                        break;
                    }
                }
            }
            SendStandardLetter(parms, list);
            return true;
            bool CanPlaceAt(IntVec3 loc)
            {
                CellRect cellRect = GenAdj.OccupiedRect(loc, chunkDef.defaultPlacingRot, chunkDef.Size);
                if (loc.Fogged(map) || !cellRect.InBounds(map))
                {
                    return false;
                }
                if (!DropCellFinder.SkyfallerCanLandAt(loc, map, chunkDef.Size))
                {
                    return false;
                }
                foreach (IntVec3 item2 in cellRect)
                {
                    RoofDef roof = item2.GetRoof(map);
                    if (roof != null && roof.isNatural)
                    {
                        return false;
                    }
                }

                if (cellRect.Any(x => takenCells.Contains(x)))
                {
                    return false;
                }
                return GenConstruct.CanBuildOnTerrain(chunkDef, loc, map, Rot4.North);
            }
            bool SpawnCrashPart(Map map, List<TargetInfo> list, HashSet<IntVec3> takenCells, 
                ref IntVec3 intVec, IntVec3 center, int count)
            {
                Thing thing = ThingMaker.MakeThing(chunkDef);
                var faction = GameComponent_Insectoids.HiveFaction;
                thing.SetFaction(faction);
                GenSpawn.Spawn(SkyfallerMaker.MakeSkyfaller(ThingDefOf.ShipChunkIncoming, thing), intVec, map);
                takenCells.AddRange(GenAdj.OccupiedRect(intVec, thing.def.defaultPlacingRot, thing.def.Size).ExpandedBy(3));
                list.Add(thing);
                if (CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.ShipChunkIncoming, map, out intVec, 10,
                    nearLoc: center, nearLocMaxDist: 2 * count, extraValidator: CanPlaceAt) is false)
                {
                    return false;
                }
                return true;
            }
            IntVec3 FindDropPodLocation(Map map, Predicate<IntVec3> validator)
            {
                for (int i = 0; i < 200; i++)
                {
                    IntVec3 intVec = RCellFinder.FindSiegePositionFrom(DropCellFinder.FindRaidDropCenterDistant(map, allowRoofed: true), map, allowRoofed: true);
                    if (validator(intVec))
                    {
                        return intVec;
                    }
                }
                return IntVec3.Invalid;
            }
        }
    }
}
