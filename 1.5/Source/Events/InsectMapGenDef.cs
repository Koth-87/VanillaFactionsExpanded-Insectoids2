using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Verse;
using Verse.AI.Group;

namespace VFEInsectoids
{
    public class ThingCommonality
    {
        public ThingDef thing;

        public float commonality;

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "thing", xmlRoot);
            commonality = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
        }
    }

    public enum SpawnMethod { Scattered, Clumped }

    public class SpawnSettings
    {
        public IntRange insectSpawnThingsAmount;
        public List<ThingCommonality> thingCommonalities;
        public SpawnMethod spawnMethod;
        public float chance = 1f;
        public IntRange loopCount = IntRange.one;
    }

    public class HiveSurroundings
    {
        public float maxSpawnCreepRadius;
        public float alwaysSpawnCreepInRadius;
        public float creepSpawnChancePerDistanceToHive;
        public List<SpawnSettings> spawnSettings;
    }

    public class InsectMapGenDef : Def
    {
        public IntRange hivesToSpawn;
        public List<InsectGenelineDef> hiveGenelinesToSpawn;
        public HiveSurroundings hiveSurroundings;
        public static HashSet<IntVec3> badCells;
        public void DoMapGen(Map map)
        {
            LongEventHandler.SetCurrentEventText("Spawning insect stuff");
            badCells = new HashSet<IntVec3>();
            var mapScale = ((map.Size.x + map.Size.z) / 2f) / 250f;
            var hiveSpawnAmount = hivesToSpawn.RandomInRange * mapScale;
            var hives = new List<Thing>();
            var mapValidCells = map.AllCells.Where(x => BaseValidator(x, false)).ToHashSet();
            for (var i = 0; i < hiveSpawnAmount; i++)
            {
                var def = hiveGenelinesToSpawn.RandomElementByWeight(x => x.spawnWeight).hive;
                DeepProfiler.Start("GetSpawnPosition");
                var cell = GetSpawnPosition(mapValidCells);
                DeepProfiler.End();
                var hive = SpawnThing(map, def, cell);
                if (hive != null)
                {
                    hives.Add(hive);
                    DeepProfiler.Start("AddCreepAround");
                    var creepCells = AddCreepAround(hive);
                    DeepProfiler.End();
                    DeepProfiler.Start("SpawnHiveSurroundings");
                    SpawnHiveSurroundings(creepCells);
                    DeepProfiler.End();
                }
            }
            var creepCount = map.AllCells.Count(x => x.GetTerrain(map) == VFEI_DefOf.VFEI2_Creep);
            var nonCreepCount = map.AllCells.Count(x => x.GetTerrain(map) != VFEI_DefOf.VFEI2_Creep);
            Log.Message("mapScale: " + mapScale + " - hivesToSpawn: " + hiveSpawnAmount 
                + " - hives: " + hives.Count
                + " - creep: " + creepCount + " - non creep: " + nonCreepCount
                + " - rate: " + (creepCount / (float)nonCreepCount));
            badCells.Clear();
            Log.Message("Adding lords for insects");
            if (map.Parent is Settlement settlement && settlement.Faction == Faction.OfInsects)
            {
                Log.Message("Spawned insects: " + map.mapPawns.AllPawns.Where(x => x.RaceProps.Insect).Select(x => x.def.defName + " - " + x.GetLord()?.LordJob).ToStringSafeEnumerable());
            }
            foreach (var insect in map.mapPawns.AllPawns.Where(x => x.RaceProps.Insect && x.Faction is null))
            {
                WildAnimalSpawner_SpawnRandomWildAnimalAt_Patch.TryAddLordJob(insect, null);
            }

            foreach (var lord in map.lordManager.lords.Where(x => x.LordJob is LordJob_DefendAndExpandHive))
            {
                var toils = lord.graph.lordToils.OfType<LordToil_DefendAndExpandHive>();
                foreach (var toil in toils)
                {
                    toil.distToHiveToAttack = 30;
                }
            }

            IntVec3 GetSpawnPosition(HashSet<IntVec3> cells)
            {
                foreach (var cell in cells.InRandomOrder())
                {
                    if (IsGoodCell(cell))
                    {
                        var cellrect = CellRect.CenteredOn(cell, 3);
                        if (IsGoodPlace(cellrect))
                        {
                            return cell;
                        }
                    }
                }
                return IntVec3.Invalid;
            }

            bool IsGoodPlace(CellRect cellrect)
            {
                foreach (var c in cellrect)
                {
                    if (IsGoodCell(c) is false)
                    {
                        return false;
                    }
                }
                return true;
            }

            bool IsGoodCell(IntVec3 c, bool checkForRocksOnly = false)
            {
                if (mapValidCells.Contains(c) is false)
                {
                    return false;
                }
                if (BaseValidator(c, checkForRocksOnly: checkForRocksOnly) is false)
                {
                    return false;
                }
                return true;
            }

            bool BaseValidator(IntVec3 c, bool checkForRocksOnly)
            {
                if (badCells.Contains(c))
                {
                    return false;
                }
                if (c.InBounds(map) is false)
                {
                    badCells.Add(c);
                    return false;
                }
                var terrain = c.GetTerrain(map);
                if (terrain.IsWater)
                {
                    badCells.Add(c);
                    return false;
                }
                var building = c.GetFirstBuilding(map);
                if (building != null && (checkForRocksOnly is false 
                    || checkForRocksOnly && (building.def.building.isNaturalRock || building.def.building.isResourceRock)))
                {
                    return false;
                }
                return true;
            }

            HashSet<IntVec3> AddCreepAround(Thing hive)
            {
                var cells = new HashSet<IntVec3>();
                var radius = hiveSurroundings.maxSpawnCreepRadius;
                hive.Map.floodFiller.FloodFill(hive.Position, (IntVec3 curCell) =>
                curCell.DistanceTo(hive.Position) <= radius && (curCell == hive.Position 
                || IsGoodCell(curCell, checkForRocksOnly: true)),
                delegate (IntVec3 curCell)
                {
                    var dist = curCell.DistanceTo(hive.Position);
                    if (dist <= hiveSurroundings.alwaysSpawnCreepInRadius)
                    {
                        cells.Add(curCell);
                    }
                    else if (Rand.Chance(hiveSurroundings.creepSpawnChancePerDistanceToHive / dist))
                    {
                        var neighbors = GenAdj.CellsAdjacent8Way(new TargetInfo(curCell, hive.Map));
                        if (neighbors.Any(x => cells.Contains(x)) && HasCloseCells(curCell, cells))
                        {
                            cells.Add(curCell);
                        }
                    }
                    return false;
                });

                foreach (var cell in cells)
                {
                    hive.Map.terrainGrid.SetTerrain(cell, VFEI_DefOf.VFEI2_Creep);
                    var plants = cell.GetThingList(hive.Map).OfType<Plant>().ToList();
                    foreach (var plant in plants)
                    {
                        plant.Destroy();
                    }
                }
                return cells;
            }

            void SpawnHiveSurroundings(HashSet<IntVec3> creepCells)
            {
                foreach (var spawnSettings in hiveSurroundings.spawnSettings)
                {
                    if (Rand.Chance(spawnSettings.chance))
                    {
                        var loopCount = spawnSettings.loopCount.RandomInRange;
                        for (var i = 0; i <= loopCount; i++)
                        {
                            var thingAmount = spawnSettings.insectSpawnThingsAmount.RandomInRange;
                            DeepProfiler.Start("GetSpawnPosition");
                            var center = GetSpawnPosition(creepCells.ToHashSet());
                            var takenCells = new HashSet<IntVec3>();
                            DeepProfiler.End();
                            for (var j = 0; j <= thingAmount; j++)
                            {
                                var thingToSpawn = spawnSettings.thingCommonalities.RandomElementByWeight(x => x.commonality).thing;
                                var cell = GetCell();
                                if (cell.IsValid)
                                {
                                    var thing = SpawnThing(map, thingToSpawn, cell, spawnSettings.spawnMethod == SpawnMethod.Clumped ? ThingPlaceMode.Near : ThingPlaceMode.Direct); ;
                                    takenCells.AddRange(thing.OccupiedRect());
                                }
                                IntVec3 GetCell()
                                {
                                    if (spawnSettings.spawnMethod == SpawnMethod.Clumped)
                                    {
                                        if (takenCells.Count >= thingAmount / 2)
                                        {
                                            var cellCandidates = new HashSet<IntVec3>();
                                            foreach (var cell in takenCells)
                                            {
                                                var adjCells = GenAdj.CellsAdjacentCardinal(cell, thingToSpawn.defaultPlacingRot, thingToSpawn.size)
                                                    .Where(x => IsGoodCell(x) && takenCells.Contains(x) is false);
                                                if (adjCells.Any())
                                                {
                                                    cellCandidates.AddRange(adjCells);
                                                }
                                            }
                                            if (cellCandidates.TryRandomElement(out var result))
                                            {
                                                return result;
                                            }
                                            return IntVec3.Invalid;
                                        }
                                        else
                                        {
                                            return center;
                                        }
                                    }
                                    else if (creepCells.Where(x => IsGoodCell(x, false) 
                                    && GenAdj.OccupiedRect(x, thingToSpawn.defaultPlacingRot, thingToSpawn.Size).InBounds(map))
                                        .TryRandomElement(out var cell))
                                    {
                                        return cell;
                                    }
                                    return IntVec3.Invalid;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool HasCloseCells(IntVec3 curCell, HashSet<IntVec3> cells)
        {
            var num = 0;
            foreach (var cell in cells)
            {
                if (cell.DistanceTo(curCell) <= 2)
                {
                    num++;
                }
                if (num >= 3)
                {
                    return true;
                }
            }
            return false;
        }

        private static Thing SpawnThing(Map map, ThingDef thingToSpawn, IntVec3 cell, ThingPlaceMode mode = ThingPlaceMode.Direct)
        {
            var thing = ThingMaker.MakeThing(thingToSpawn);
            if (GenPlace.TryPlaceThing(thing, cell, map, mode))
            {
                badCells.AddRange(thing.OccupiedRect());
                return thing;
            }
            return null;
        }
    }
}
