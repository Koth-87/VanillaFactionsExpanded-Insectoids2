using HarmonyLib;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(WITab_Terrain), "FillTab")]
    public static class WITab_Terrain_FillTab_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            foreach (var instruction in codeInstructions)
            {
                yield return instruction;
                if (instruction.OperandIs("AverageDiseaseFrequency"))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 3);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(WITab_Terrain_FillTab_Patch), "AddInfestationLabel"));
                }
            }
        }

        public static void AddInfestationLabel(int tileInt, Listing_Standard listing_Standard)
        {
            if (tileInt.IsInfestedTile())
            {
                listing_Standard.LabelDouble("VFEI_Infestation".Translate(), "VFEI_Present".Translate(), "VFEI_InfestationTooltip".Translate());
            }
        }
    }
}
