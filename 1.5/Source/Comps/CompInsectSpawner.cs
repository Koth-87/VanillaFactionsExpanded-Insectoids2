using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
            if (nextPawnSpawnTick != -1)
            {
                SpawnInsects();
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                CalculateNextPawnSpawnTick();
            }
        }

        public override void CompTick()
        {
            if (nextPawnSpawnTick != -1 && parent.Spawned && Find.TickManager.TicksGame >= nextPawnSpawnTick)
            {
                SpawnInsects();
            }
        }

        public void SpawnInsects()
        {
            nextPawnSpawnTick = -1;
            SpawnPawnsUntilPoints(Props.maxSpawnedPawnsPoints);
        }

        [HarmonyPatch(typeof(Thing), "Destroy")]
        public static class Thing_Destroy_Patch
        {
            public static void Prefix(Thing __instance, DestroyMode mode)
            {
                if (mode == DestroyMode.Deconstruct)
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
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
            {
                var field = AccessTools.Field(typeof(CompProperties_SpawnerPawn), "spawnablePawnKinds");
                foreach (var instruction in codeInstructions)
                {
                    yield return instruction;
                    if (instruction.LoadsField(field))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(CompSpawnerPawn_RandomPawnKindDef_Patch), "GetFilteredList"));
                    }
                }
            }

            public static List<PawnKindDef> GetFilteredList(List<PawnKindDef> list, CompSpawnerPawn compSpawnerPawn)
            {
                if (compSpawnerPawn is CompInsectSpawner insectSpawner)
                {
                    return insectSpawner.Props.geneline.insects.Select(x => x.kind).ToList();
                }
                return list;
            }
        }
    }
}
