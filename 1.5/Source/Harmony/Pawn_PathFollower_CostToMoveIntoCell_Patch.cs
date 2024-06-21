using HarmonyLib;
using System;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(Pawn_PathFollower), "CostToMoveIntoCell", new Type[] { typeof(Pawn), typeof(IntVec3) })]
    public static class Pawn_PathFollower_CostToMoveIntoCell_Patch
    {
        public static void Postfix(Pawn pawn, IntVec3 c, ref float __result)
        {
            if (pawn.Map != null)
            {
                var terrain = c.GetTerrain(pawn.Map);
                __result = pawn.TryChangePathCost(__result, terrain);
            }
        }
    }
}
