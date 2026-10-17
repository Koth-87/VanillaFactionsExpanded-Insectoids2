using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    public class IncidentWorker_BurrowSiege : IncidentWorker
    {
        public const float SmallBurrowPoints = 400f;
        public const float InsectBurrowPoints = 1000f;
        public const float LargeBurrowPoints = 1600f;

        public override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            return base.CanFireNowSub(parms) && Faction.OfInsects != null;
        }

        public override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            float points = parms.points;
            Thing thing = SpawnBurrowSpawners(parms.points, map);
            SendStandardLetter(parms, thing);
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }

        public static Thing SpawnBurrowSpawners(float totalPoints, Map map)
        {
            int smallBurrowCount = Mathf.CeilToInt(totalPoints * 0.50f / SmallBurrowPoints);
            int mediumBurrowCount = Mathf.CeilToInt(totalPoints * 0.25f / InsectBurrowPoints);
            int largeBurrowCount = Mathf.CeilToInt(totalPoints * 0.25f / LargeBurrowPoints);

            if (!RCellFinder.TryFindRandomPawnEntryCell(out var loc, map, 0) || !loc.IsValid)
            {
                return null;
            }

            List<BurrowSpawner> spawners = new List<BurrowSpawner>();

            spawners.Add(SpawnBurrowSpawner(map, VFEI_DefOf.VFEI2_SmallBurrowSpawner, VFEI_DefOf.VFEI2_SmallBurrow, BurrowSize.Small, loc));

            for (int i = 0; i < smallBurrowCount - 1; i++)
            {
                loc = FindNearbyLocation(spawners.First().Position, map);
                if (loc.IsValid)
                {
                    spawners.Add(SpawnBurrowSpawner(map, VFEI_DefOf.VFEI2_SmallBurrowSpawner, VFEI_DefOf.VFEI2_SmallBurrow, BurrowSize.Small, loc));
                }
            }

            for (int i = 0; i < mediumBurrowCount; i++)
            {
                loc = FindNearbyLocation(spawners.First().Position, map);
                if (loc.IsValid)
                {
                    spawners.Add(SpawnBurrowSpawner(map, VFEI_DefOf.VFEI2_MediumBurrowSpawner, VFEI_DefOf.VFEI2_MediumBurrow, BurrowSize.Medium, loc));
                }
            }

            for (int i = 0; i < largeBurrowCount; i++)
            {
                loc = FindNearbyLocation(spawners.First().Position, map);
                if (loc.IsValid)
                {
                    spawners.Add(SpawnBurrowSpawner(map, VFEI_DefOf.VFEI2_LargeBurrowSpawner, VFEI_DefOf.VFEI2_LargeBurrow, BurrowSize.Large, loc));
                }
            }
            SpawnBurrowThings(spawners, totalPoints);
            return spawners.FirstOrDefault();
        }

        public static BurrowSpawner SpawnBurrowSpawner(Map map, ThingDef burrowSpawnerDef, ThingDef burrowDef, BurrowSize size,
            IntVec3 loc)
        {
            if (loc.IsValid)
            {
                var spawner = (BurrowSpawner)ThingMaker.MakeThing(burrowSpawnerDef);
                spawner.faction = Faction.OfInsects.def;
                spawner.size = size;
                spawner.thingsToSpawn.Add(burrowDef);
                spawner.radiusOfSpawn = (int)(size + 1) * 2;
                return (BurrowSpawner)GenSpawn.Spawn(spawner, loc, map, WipeMode.FullRefund);
            }
            return null;
        }

        private static IntVec3 FindNearbyLocation(IntVec3 origin, Map map)
        {
            return CompSpawnerHives.FindChildHiveLocation(origin, map, ThingDefOf.Hive, ThingDefOf.Hive.GetCompProperties<CompProperties_SpawnerHives>(), true, allowUnreachable: true);
        }

        public static void SpawnBurrowThings(List<BurrowSpawner> spawners, float points)
        {
            var randomStructureWeights = new List<(ThingDef, float)>
            {
                (ThingDefOf.GlowPod, 3f),
                (VFEI_DefOf.VFEI2_InsectoidCocoon, 6f),
            };
            var additionalStructuresCount = (int)new SimpleCurve
            {
                new CurvePoint(800, 2),
                new CurvePoint(10000f, 14)
            }.Evaluate(points);
            for (var i = 0; i < additionalStructuresCount; i++)
            {
                var thingToSpawn = randomStructureWeights.RandomElementByWeight(x => x.Item2).Item1;
                spawners.RandomElement().thingsToSpawn.Add(thingToSpawn);
            }
        }
    }
}