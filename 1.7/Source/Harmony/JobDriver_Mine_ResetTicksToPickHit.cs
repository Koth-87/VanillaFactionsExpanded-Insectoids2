using HarmonyLib;
using RimWorld;

namespace VFEInsectoids
{

    [HarmonyPatch(typeof(JobDriver_Mine), "ResetTicksToPickHit")]
    public static class VFEInsectoids_JobDriver_Mine_ResetTicksToPickHit_Patch
    {
        public static void Postfix(JobDriver_Mine __instance,ref int ___ticksToPickHit)
        {
            if ( __instance.pawn.IsColonyInsect())
            {
                ___ticksToPickHit *= 2;
            }
        }
    }
}
