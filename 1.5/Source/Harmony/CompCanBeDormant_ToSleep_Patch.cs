using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(CompCanBeDormant), "ToSleep")]
    public static class CompCanBeDormant_ToSleep_Patch
    {
        public static bool Prefix(CompCanBeDormant __instance)
        {
            if (__instance.parent is Pawn pawn && pawn.IsColonyInsect())
            {
                return false;
            }
            return true;
        }
    }
}
