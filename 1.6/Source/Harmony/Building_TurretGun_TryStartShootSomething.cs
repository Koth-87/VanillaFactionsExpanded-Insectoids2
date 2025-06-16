using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Sound;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(Building_TurretGun), nameof(Building_TurretGun.TryStartShootSomething))]
    public static class VFEInsectoids_Building_TurretGun_TryStartShootSomething_Patch
    {
        public static void Postfix(Building_TurretGun __instance)
        {
          if(__instance?.def == VFEI_DefOf.VFEI2_Vilelobber)
            {
                if (__instance.currentTargetInt.IsValid)
                {
                    VFEI_DefOf.VFEI_InsectoidTurretTargetAcquired.PlayOneShot(new TargetInfo(__instance.Position, __instance.Map));
                }

            }
        }
    }
}