using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(GenLeaving), nameof(GenLeaving.GetBuildingResourcesLeaveCalculator))]
    public static class VFEInsectoids_GenLeaving_GetBuildingResourcesLeaveCalculator_Patch
    {
        private static void Postfix(ref Func<int, int> __result, Thing destroyedThing, DestroyMode mode)
        {
            if (destroyedThing?.def != VFEI_DefOf.VFEI2_InfestedShipChunk)
            {
                return;
            }
            switch (mode)
            {
                case DestroyMode.KillFinalize:
                    __result = (int count) => GenMath.RoundRandom(count);
                    break;
                case DestroyMode.Deconstruct:
                    __result = (int count) => GenMath.RoundRandom(count);
                    break;
                case DestroyMode.FailConstruction:
                    __result = (int count) => GenMath.RoundRandom(count);
                    break;
                case DestroyMode.KillFinalizeLeavingsOnly:
                    break;
            }
        }
    }
}
