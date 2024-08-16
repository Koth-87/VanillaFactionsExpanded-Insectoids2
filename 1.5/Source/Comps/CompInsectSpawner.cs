using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    public class CompProperties_InsectSpawner : CompProperties_SpawnerPawn
    {
        public InsectGenelineDef geneline;

        public CompProperties_InsectSpawner()
        {
            this.compClass = typeof(CompInsectSpawner);
        }
    }

    public class CompInsectSpawner : CompSpawnerPawn
    {
        public CompProperties_InsectSpawner Props => base.props as CompProperties_InsectSpawner;

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
            if (nextPawnSpawnTick != -1 && parent is not Hive)
            {
                SpawnInsects();
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                if (parent is not Hive || parent.Tile.IsInfestedTile())
                {
                    CalculateNextPawnSpawnTick();
                }
            }
        }

        public override void CompTick()
        {
            if (parent is not Hive)
            {
                if (nextPawnSpawnTick != -1 && parent.Spawned && Find.TickManager.TicksGame >= nextPawnSpawnTick)
                {
                    SpawnInsects();
                }
            }
            else
            {
                base.CompTick();
            }
        }

        public void SpawnInsects()
        {
            SpawnPawnsUntilPoints(Props.maxSpawnedPawnsPoints); 
            nextPawnSpawnTick = -1;
        }


        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (previousMap.Parent is Site && parent is Hive)
            {
                if (previousMap.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial).Any(x => x.HasComp<CompInsectSpawner>()) is false)
                {
                    QuestUtility.SendQuestTargetSignals(previousMap.Parent.questTags, "AllHivesAreDestroyed", parent.Named("SUBJECT"));
                }
            }
        }

        [HarmonyPatch(typeof(Thing), "Destroy")]
        public static class Thing_Destroy_Patch
        {
            public static void Prefix(Thing __instance, DestroyMode mode)
            {
                if (mode == DestroyMode.Deconstruct && __instance is not Hive)
                {
                    var comp = __instance.TryGetComp<CompInsectSpawner>();
                    if (comp != null && comp.nextPawnSpawnTick != -1)
                    {
                        comp.SpawnInsects();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CompSpawnerPawn), "RandomPawnKindDef")]
        public static class CompSpawnerPawn_RandomPawnKindDef_Patch
        {
            public static bool Prefix(ref PawnKindDef __result, CompSpawnerPawn __instance)
            {
                if (__instance is CompInsectSpawner insectSpawner)
                {
                    __result = RandomPawnKindDef(insectSpawner);
                    return false;
                }
                return true;
            }

            private static PawnKindDef RandomPawnKindDef(CompInsectSpawner insectSpawner)
            {
                float curPoints = insectSpawner.SpawnedPawnsPoints;
                var source = insectSpawner.Props.geneline.insects.Where(x => x.kind != VFEI_DefOf.VFEI2_Queen);
                if (insectSpawner.Props.maxSpawnedPawnsPoints > -1f)
                {
                    source = source.Where(x => curPoints + x.kind.combatPower <= insectSpawner.Props.maxSpawnedPawnsPoints).ToList();
                }
                if (source.TryRandomElementByWeight(x => x.selectionWeight, out var result))
                {
                    return result.kind;
                }
                return null;
            }
        }
    }
}
