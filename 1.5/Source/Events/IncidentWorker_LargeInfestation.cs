using RimWorld;
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
            parms.points *= PointsFactorCurve.Evaluate(parms.points);
            Thing thing = SpawnTunnels(Mathf.Max(GenMath.RoundRandom(parms.points / 220f), 1), map, true, null, parms.infestationLocOverride);
            SendStandardLetter(parms, thing);
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }

        public static Thing SpawnTunnels(int hiveCount, Map map, bool ignoreRoofedRequirement, string questTag = null, IntVec3? overrideLoc = null, float? insectsPoints = null)
        {
            IntVec3 loc = (overrideLoc.HasValue ? overrideLoc.Value : default(IntVec3));
            if (!overrideLoc.HasValue && RCellFinder.TryFindRandomPawnEntryCell(out loc, map, 0))
            {
            }
            if (!loc.IsValid)
            {
                return null;
            }
            TunnelHiveSpawner tunnelHiveSpawner = (TunnelHiveSpawner)ThingMaker.MakeThing(ThingDefOf.TunnelHiveSpawner);
            Thing thing = GenSpawn.Spawn(tunnelHiveSpawner, loc, map, WipeMode.FullRefund);
            if (insectsPoints.HasValue)
            {
                tunnelHiveSpawner.insectsPoints = insectsPoints.Value;
            }
            QuestUtility.AddQuestTag(thing, questTag);
            for (int i = 0; i < hiveCount - 1; i++)
            {
                loc = CompSpawnerHives.FindChildHiveLocation(thing.Position, map, ThingDefOf.Hive, 
                    ThingDefOf.Hive.GetCompProperties<CompProperties_SpawnerHives>(), ignoreRoofedRequirement, allowUnreachable: true);
                if (loc.IsValid)
                {
                    tunnelHiveSpawner = (TunnelHiveSpawner)ThingMaker.MakeThing(ThingDefOf.TunnelHiveSpawner);
                    thing = GenSpawn.Spawn(tunnelHiveSpawner, loc, map, WipeMode.FullRefund);
                    if (insectsPoints.HasValue)
                    {
                        tunnelHiveSpawner.insectsPoints = insectsPoints.Value;
                    }
                    QuestUtility.AddQuestTag(thing, questTag);
                }
            }
            return thing;
        }
    }
}
