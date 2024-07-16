using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(Pawn_PathFollower), "CostToMoveIntoCell", new Type[] { typeof(Pawn), typeof(IntVec3) })]
    public static class Pawn_PathFollower_CostToMoveIntoCell_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var method = AccessTools.Method(typeof(PathGrid), "CalculatedCostAt");
            foreach (var instruction in codeInstructions)
            {
                yield return instruction;
                if (instruction.Calls(method))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(
                        typeof(Pawn_PathFollower_CostToMoveIntoCell_Patch),
                        "TryChangeCost"));
                }
            }
        }

        public static int TryChangeCost(int __result, Pawn pawn, IntVec3 c)
        {
            if (pawn.Map != null)
            {
                var terrain = c.GetTerrain(pawn.Map);
                __result = (int)pawn.TryChangePathCost(__result, terrain);
            }
            return __result;
        }
    }
}
