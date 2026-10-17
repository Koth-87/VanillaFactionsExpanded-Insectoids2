using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(RepairUtility), "PawnCanRepairEver")]
    public static class RepairUtility_PawnCanRepairEver_Patch
    {
        public static void Postfix(ref bool __result, Pawn pawn, Thing t)
        {
            if (__result)
            {
                pawn.CheckCanWorkOnIt(t, ref __result);
            }
        }
    }
}
