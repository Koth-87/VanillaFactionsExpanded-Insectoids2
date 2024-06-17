using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(WildAnimalSpawner), "SpawnRandomWildAnimalAt")]
    public static class WildAnimalSpawner_SpawnRandomWildAnimalAt_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var allWildAnimals = AccessTools.PropertyGetter(typeof(BiomeDef), "AllWildAnimals");
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (instruction.Calls(allWildAnimals))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(WildAnimalSpawner_SpawnRandomWildAnimalAt_Patch), "TryOverrideWildAnimals"));
                }
            }
        }

        public static IEnumerable<PawnKindDef> TryOverrideWildAnimals(IEnumerable<PawnKindDef> wildAnimals, WildAnimalSpawner spawner)
        {
            if (spawner.map.IsInfestedTile())
            {
                return wildAnimals.Concat(VFEI_DefOf.VFEI_Sorne.insects.Select(x => x.kind));
            }
            return wildAnimals;
        }
    }
}
