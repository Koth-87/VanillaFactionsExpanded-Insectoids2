using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace VFEInsectoids
{
    public class IncidentWorker_InfestedChunkCrash : IncidentWorker
    {
        public override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            List<TargetInfo> list = new List<TargetInfo>();
            ThingDef shipPartDef = def.mechClusterBuilding;
            var takenCells = new HashSet<IntVec3>();
            IntVec3 intVec = FindDropPodLocation(map, (IntVec3 spot) => CanPlaceAt(spot));
            if (intVec == IntVec3.Invalid)
            {
                return false;
            }
            var center = intVec;
            var count = (int)(Mathf.Max(1, parms.points / 250f));
            for (var i = 0; i < count; i++)
            {
                Thing thing = ThingMaker.MakeThing(shipPartDef);
                var faction = GameComponent_Insectoids.HiveFaction;
                thing.SetFaction(faction);
                GenSpawn.Spawn(SkyfallerMaker.MakeSkyfaller(ThingDefOf.ShipChunkIncoming, thing), intVec, map);
                takenCells.AddRange(GenAdj.OccupiedRect(intVec, thing.def.defaultPlacingRot, thing.def.Size).ExpandedBy(1));
                list.Add(thing);
                if (CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.ShipChunkIncoming, map, out intVec, 10,
                    nearLoc: center, nearLocMaxDist: 2 * count, extraValidator: CanPlaceAt) is false)
                {
                    break;
                }
            }
            SendStandardLetter(parms, list);
            return true;
            bool CanPlaceAt(IntVec3 loc)
            {
                CellRect cellRect = GenAdj.OccupiedRect(loc, Rot4.North, shipPartDef.Size);
                if (loc.Fogged(map) || !cellRect.InBounds(map))
                {
                    return false;
                }
                if (!DropCellFinder.SkyfallerCanLandAt(loc, map, shipPartDef.Size))
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
                if (takenCells.Contains(loc))
                {
                    return false;
                }
                return GenConstruct.CanBuildOnTerrain(shipPartDef, loc, map, Rot4.North);
            }
        }

        private static IntVec3 FindDropPodLocation(Map map, Predicate<IntVec3> validator)
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
