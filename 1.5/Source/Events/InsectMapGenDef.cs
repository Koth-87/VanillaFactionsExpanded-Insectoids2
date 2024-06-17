using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Verse;

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

        public void DoMapGen(Map map)
        {
            var mapScale = ((map.Size.x + map.Size.z) / 2f) / 250f;
            var hiveSpawnAmount = hivesToSpawn.RandomInRange * mapScale;
            var hives = new List<Thing>();
            for (var i = 0; i < hiveSpawnAmount; i++)
            {
                var def = hiveGenelinesToSpawn.RandomElementByWeight(x => x.spawnWeight).hive;
                var cell = GetSpawnPosition(map.AllCells.ToList());
                if (cell.IsValid)
                {
                    var hive = GenSpawn.Spawn(def, cell, map);
                    hives.Add(hive);
                    var creepCells = AddCreepAround(hive);
                    SpawnHiveSurroundings(creepCells);
                }
            }

            var creepCount = map.AllCells.Count(x => x.GetTerrain(map) == VFEI_DefOf.VFEI2_Creep);
            var nonCreepCount = map.AllCells.Count(x => x.GetTerrain(map) != VFEI_DefOf.VFEI2_Creep);
            Log.Message("mapScale: " + mapScale + " - hivesToSpawn: " + hiveSpawnAmount + " - hives: " + hives.Count
                + " - creep: " + creepCount + " - non creep: " + nonCreepCount
                + " - rate: " + (creepCount / (float)nonCreepCount));
            IntVec3 GetSpawnPosition(List<IntVec3> cells, Func<IntVec3, bool> validator = null)
            {
                var cellCandidates = new HashSet<IntVec3>();
                var badCells = new HashSet<IntVec3>();
                foreach (var cell in cells.InRandomOrder())
                {
                    if (IsGoodCell(cellCandidates, badCells, cell))
                    {
                        if (validator is null || validator(cell))
                        {
                            var cellrect = CellRect.CenteredOn(cell, 3);
                            if (IsGoodPlace(cellCandidates, badCells, cellrect))
                            {
                                cellCandidates.Add(cell);
                            }
                        }
                    }
                }
                if (cellCandidates.TryRandomElement(out var result))
                {
                    return result;
                }
                return IntVec3.Invalid;
            }

            bool IsGoodPlace(HashSet<IntVec3> cellCandidates, HashSet<IntVec3> badCells, CellRect cellrect)
            {
                foreach (var c in cellrect)
                {
                    if (IsGoodCell(cellCandidates, badCells, c) is false)
                    {
                        return false;
                    }
                }
                return true;
            }

            bool IsGoodCell(HashSet<IntVec3> cellCandidates, HashSet<IntVec3> badCells, IntVec3 c)
            {
                if (cellCandidates.Contains(c) || badCells.Contains(c))
                {
                    return false;
                }
                if (hives.Any(x => x.Position == c))
                {
                    return false;
                }
                return BaseValidator(c, checkForRocksOnly: false);
            }

            bool BaseValidator(IntVec3 c, bool checkForRocksOnly)
            {
                if (c.InBounds(map) is false)
                {
                    return false;
                }
                var terrain = c.GetTerrain(map);
                if (terrain.IsWater)
                {
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

            List<IntVec3> AddCreepAround(Thing hive)
            {
                var cells = new List<IntVec3>();
                var radius = hiveSurroundings.maxSpawnCreepRadius;
                hive.Map.floodFiller.FloodFill(hive.Position, (IntVec3 curCell) =>
                curCell.DistanceTo(hive.Position) <= radius && BaseValidator(curCell, checkForRocksOnly: true),
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
                        if (neighbors.Count(x => cells.Contains(x)) >= 2)
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

            void SpawnHiveSurroundings(List<IntVec3> creepCells)
            {
                var takenCells = new HashSet<IntVec3>();
                foreach (var spawnSettings in hiveSurroundings.spawnSettings)
                {
                    if (Rand.Chance(spawnSettings.chance))
                    {
                        var loopCount = spawnSettings.loopCount.RandomInRange;
                        for (var i = 0; i <= loopCount; i++)
                        {
                            var thingAmount = spawnSettings.insectSpawnThingsAmount.RandomInRange;
                            var center = GetSpawnPosition(creepCells, delegate (IntVec3 x)
                            {
                                if (takenCells.Contains(x))
                                {
                                    return false;
                                }
                                return true;
                            });

                            for (var j = 0; j <= thingAmount; j++)
                            {
                                var thingToSpawn = spawnSettings.thingCommonalities.RandomElementByWeight(x => x.commonality).thing;
                                var cell = GetCell();
                                var thing = GenSpawn.Spawn(thingToSpawn, cell, map);
                                takenCells.AddRange(thing.OccupiedRect());
                                IntVec3 GetCell()
                                {
                                    if (spawnSettings.spawnMethod == SpawnMethod.Clumped)
                                    {
                                        var radius = 1f;
                                        while (true)
                                        {
                                            foreach (var cell in 
                                                GenRadial.RadialCellsAround(center, radius, true).InRandomOrder())
                                            {
                                                if (takenCells.Contains(cell) 
                                                    || BaseValidator(cell, false) is false
                                                    || GenAdj.OccupiedRect(cell, thingToSpawn.defaultPlacingRot,
                                                    thingToSpawn.Size).InBounds(map) is false)
                                                {
                                                    continue;
                                                }
                                                return cell;
                                            }
                                            radius += 0.5f;
                                            if (radius >= 50)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else if (creepCells.Where(x => takenCells.Contains(x) is false 
                                        && BaseValidator(x, false) && GenAdj.OccupiedRect(x, 
                                        thingToSpawn.defaultPlacingRot, thingToSpawn.Size).InBounds(map))
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
    }
}
