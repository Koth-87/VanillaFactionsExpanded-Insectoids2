using HarmonyLib;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(Pawn), "Destroy")]
    public static class Pawn_Destroy_Patch
    {
        public static void Prefix(Pawn __instance)
        {
            if (__instance.IsColonyInsect(out var hediff))
            {
                __instance.health.RemoveHediff(hediff);
            }
        }
    }
}
