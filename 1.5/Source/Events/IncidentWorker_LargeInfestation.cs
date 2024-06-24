using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    public class IncidentWorker_LargeInfestation : IncidentWorker
    {
        public const float HivePoints = 220f;

        public static readonly SimpleCurve PointsFactorCurve = new SimpleCurve
        {
            new CurvePoint(0f, 0.7f),
            new CurvePoint(5000f, 0.45f)
        };

        public override float ChanceFactorNow(IIncidentTarget target)
        {
            var mult = target is Map map && map.IsInfested() ? 2 : 1;
            return base.ChanceFactorNow(target) * mult;
        }

        public override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (base.CanFireNowSub(parms) && Faction.OfInsects != null 
                && IncidentWorker_Infestation_CanFireNowSub_Patch.TryOverrideHiveCount
                (HiveUtility.TotalSpawnedHivesCount(map), map) < 30)
            {
                return true;
            }
            return false;
        }

        public override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            var additionalStructuresCount = (int)new SimpleCurve
            {
                new CurvePoint(800, 2),
                new CurvePoint(10000f, 14)
            }.Evaluate(parms.points);
            parms.points *= PointsFactorCurve.Evaluate(parms.points);
            Thing thing = SpawnTunnels(Mathf.Max(GenMath.RoundRandom(parms.points / 220f), 1), map, additionalStructuresCount);
            SendStandardLetter(parms, thing);
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }

        public static Thing SpawnTunnels(int hiveCount, Map map, int additionalStructuresCount)
        {
            if (RCellFinder.TryFindRandomPawnEntryCell(out var loc, map, 0) && !loc.IsValid)
            {
                return null;
            }
            var hives = new List<LargeTunnelHiveSpawner>();
            Thing thing = SpawnTunnels(hiveCount, map, loc, hives);
            SpawnHiveThings(additionalStructuresCount, hives);
            return thing;
        }

        public static Thing SpawnTunnels(int hiveCount, Map map, IntVec3 loc, List<LargeTunnelHiveSpawner> hives)
        {
            var tunnelHiveSpawner = (LargeTunnelHiveSpawner)ThingMaker.MakeThing(VFEI_DefOf.VFEI2_LargeTunnelHiveSpawner);
            hives.Add(tunnelHiveSpawner);
            Thing thing = GenSpawn.Spawn(tunnelHiveSpawner, loc, map, WipeMode.FullRefund);
            for (int i = 0; i < hiveCount - 1; i++)
            {
                loc = CompSpawnerHives.FindChildHiveLocation(thing.Position, map, ThingDefOf.Hive,
                    ThingDefOf.Hive.GetCompProperties<CompProperties_SpawnerHives>(), true, allowUnreachable: true);
                if (loc.IsValid)
                {
                    tunnelHiveSpawner = (LargeTunnelHiveSpawner)ThingMaker.MakeThing(VFEI_DefOf.VFEI2_LargeTunnelHiveSpawner);
                    hives.Add(tunnelHiveSpawner);
                    thing = GenSpawn.Spawn(tunnelHiveSpawner, loc, map, WipeMode.FullRefund);
                }
            }
            return thing;
        }

        public static void SpawnHiveThings(int additionalStructuresCount, List<LargeTunnelHiveSpawner> hives)
        {
            for (var i = 0; i < additionalStructuresCount; i++)
            {
                var thing = thingWeights.RandomElementByWeight(x => x.Item2).Item1;
                var hive = hives.RandomElement();
                hive.thingsToSpawn.Add(thing);
                hive.otherHives = hives.Where(x => x != hive).ToList();
            }
        }

        public static List<(ThingDef, float)> thingWeights = new List<(ThingDef, float)>
        {
            (VFEI_DefOf.VFEI2_LargeGlowPod, 3),
            (VFEI_DefOf.VFEI2_GlowPodFormation, 2),
            (VFEI_DefOf.VFEI2_FoamPod, 1),
            (VFEI_DefOf.VFEI2_TendrilFarm, 1),
            (VFEI_DefOf.VFEI2_JellyFarm, 1),
            (VFEI_DefOf.VFEI2_Creeper, 1),
            (VFEI_DefOf.VFEI2_InsectoidCocoon, 6),
        };
    }
}
