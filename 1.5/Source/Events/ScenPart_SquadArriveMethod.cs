using KCSG;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    public class ScenPart_SquadArriveMethod : ScenPart_PlayerPawnsArriveMethod
    {
        public SettlementLayoutDef layoutDef;

        public override void GenerateIntoMap(Map map)
        {
            if (Find.GameInitData == null)
            {
                return;
            }

            PawnGroupKindWorker_GeneratePawns_Patch.forcedSecondGeneline = VFEI_DefOf.VFEI_Sorne;
            GenStep_InsectGen.Generate(map, layoutDef, 1250);
            RimWorld.BaseGen.BaseGen.Generate();
            PawnGroupKindWorker_GeneratePawns_Patch.forcedSecondGeneline = null;

            var list = new List<List<Thing>>();
            foreach (var startingAndOptionalPawn in Find.GameInitData.startingAndOptionalPawns)
            {
                var list2 = new List<Thing>
                {
                    startingAndOptionalPawn
                };
                list.Add(list2);
            }
            var list3 = new List<Thing>();
            foreach (var allPart in Find.Scenario.AllParts)
            {
                list3.AddRange(allPart.PlayerStartingThings());
            }
            int num = 0;
            foreach (var item in list3)
            {
                if (item.def.CanHaveFaction)
                {
                    item.SetFactionDirect(Faction.OfPlayer);
                }
                list[num].Add(item);
                num++;
                if (num >= list.Count)
                {
                    num = 0;
                }
            }

            DropThingGroupsNear(MapGenerator.PlayerStartSpot, map, list, 0,
                Find.GameInitData.QuickStarted || method != PlayerPawnsArriveMethod.DropPods,
                leaveSlag: true, canRoofPunch: true, forbid: true, allowFogged: false);
        }

        public static void DropThingGroupsNear(IntVec3 dropCenter, Map map, List<List<Thing>> thingsGroups, int openDelay = 110, bool instaDrop = false, bool leaveSlag = false, bool canRoofPunch = true, bool forbid = true, bool allowFogged = true, bool canTransfer = false)
        {
            List<IntVec3> takenCells = new List<IntVec3>();
            foreach (var thingsGroup in thingsGroups)
            {
                if (!TryFindDropSpotNear(dropCenter, map, takenCells, out var result, allowFogged, canRoofPunch))
                {
                    result = dropCenter;
                    Log.Warning(string.Concat("DropThingsNear failed to find a place to drop ", thingsGroup.FirstOrDefault(), " near ", dropCenter, ". Dropping on random square instead."));
                }
                takenCells.Add(result);
                if (forbid)
                {
                    for (int i = 0; i < thingsGroup.Count; i++)
                    {
                        thingsGroup[i].SetForbidden(value: true, warnOnFail: false);
                    }
                }
                if (instaDrop)
                {
                    foreach (var item in thingsGroup)
                    {
                        GenPlace.TryPlaceThing(item, result, map, ThingPlaceMode.Near);
                    }
                    continue;
                }
                var activeDropPodInfo = new ActiveDropPodInfo();
                foreach (var item2 in thingsGroup)
                {
                    activeDropPodInfo.innerContainer.TryAdd(item2);
                }
                activeDropPodInfo.openDelay = openDelay;
                activeDropPodInfo.leaveSlag = leaveSlag;
                MakeHellPod(result, map, activeDropPodInfo);
            }
        }

        public static bool TryFindDropSpotNear(IntVec3 center, Map map, List<IntVec3> takenCells, out IntVec3 result,
            bool allowFogged, bool canRoofPunch)
        {
            if (DebugViewSettings.drawDestSearch)
            {
                map.debugDrawer.FlashCell(center, 1f, "center");
            }
            Room centerRoom = center.GetRoom(map);
            Predicate<IntVec3> validator = delegate (IntVec3 c)
            {
                if (c.DistanceTo(center) < 20)
                {
                    return false;
                }
                foreach (var taken in takenCells)
                {
                    if (taken.DistanceTo(c) < 20)
                    {
                        return false;
                    }
                }
                if (!DropCellFinder.IsGoodDropSpot(c, map, allowFogged, canRoofPunch, true))
                {
                    return false;
                }
                return map.reachability.CanReach(center, c, PathEndMode.OnCell, TraverseMode.PassDoors, Danger.Deadly);
            };

            if (canRoofPunch && centerRoom != null && !centerRoom.PsychologicallyOutdoors)
            {
                Predicate<IntVec3> v2 = (IntVec3 c) => validator(c) && c.GetRoom(map) == centerRoom;
                if (TryFindCell(v2, out result))
                {
                    return true;
                }
                Predicate<IntVec3> v3 = delegate (IntVec3 c)
                {
                    if (!validator(c))
                    {
                        return false;
                    }
                    Room room = c.GetRoom(map);
                    return room != null && !room.PsychologicallyOutdoors;
                };
                if (TryFindCell(v3, out result))
                {
                    return true;
                }
            }
            return TryFindCell(validator, out result);
            bool TryFindCell(Predicate<IntVec3> v, out IntVec3 r)
            {
                int num = 5;
                do
                {
                    if (CellFinder.TryFindRandomCellNear(center, map, num, v, out r))
                    {
                        return true;
                    }
                    num += 3;
                }
                while (num <= 30);
                r = center;
                return false;
            }
        }


        public static void MakeHellPod(IntVec3 c, Map map, ActiveDropPodInfo info)
        {
            var activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(VFEI_DefOf.VFEI_HellpodActiveDropPod);
            activeDropPod.Contents = info;
            SkyfallerMaker.SpawnSkyfaller(VFEI_DefOf.VFEI_HellpodIncoming, activeDropPod, c, map);
            foreach (var item in activeDropPod.Contents.innerContainer)
            {
                if (item is Pawn pawn && pawn.IsWorldPawn())
                {
                    Find.WorldPawns.RemovePawn(pawn);
                    pawn.psychicEntropy?.SetInitialPsyfocusLevel();
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref layoutDef, "layoutDef");
        }
    }
}
