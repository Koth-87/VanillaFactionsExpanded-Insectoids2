using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(PathFinder), nameof(PathFinder.FindPath), new Type[] { typeof(IntVec3), typeof(LocalTargetInfo), typeof(TraverseParms), typeof(PathEndMode), typeof(PathFinderCostTuning) })]
    public static class PathFinder_FindPath_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var found = false;
            for (int i = 0; i < codes.Count; i++)
            {
                yield return codes[i];
                if (!found && codes[i].opcode == OpCodes.Stloc_S && codes[i].operand is LocalBuilder lb && lb.LocalIndex == 53)
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 41);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 42);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 46);
                    yield return new CodeInstruction(OpCodes.Call, typeof(PathFinder_FindPath_Patch).GetMethod(nameof(PathFinder_FindPath_Patch.ChangePathCostIfNeeded)));
                    yield return new CodeInstruction(OpCodes.Stloc_S, 46);
                }
            }
            if (!found)
            {
                Log.Error("[VFE Insectoids] PathFinder.FindPath Transpiler failed. The code won't work.");
            }
        }

        static public float ChangePathCostIfNeeded(Pawn pawn, int xCell, int zCell, float cost)
        {
            if (pawn != null)
            {
                var cell = new IntVec3(xCell, 0, zCell);
                var terrain = cell.GetTerrain(pawn.Map);
                cost = pawn.TryChangePathCost(cost, terrain);
            }
            return cost;
        }

        public static float TryChangePathCost(this Pawn pawn, float cost, TerrainDef terrain)
        {
            if (terrain == VFEI_DefOf.VFEI2_Creep)
            {
                if (pawn.RaceProps.Insect || 
                    (pawn.genes != null && pawn.genes.GenesListForReading.Any(x => x.def.defName == "VRE_InsectFlesh" && x.Active)))
                {
                    cost /= 1.06f;
                }
                else
                {
                    cost /= 0.22f;
                }
            }
            return cost;
        }
    }
}
