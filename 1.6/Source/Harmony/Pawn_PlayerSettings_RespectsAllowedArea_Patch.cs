using HarmonyLib;
using RimWorld;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(Pawn_PlayerSettings), "RespectsAllowedArea", MethodType.Getter)]
    public static class Pawn_PlayerSettings_RespectsAllowedArea_Patch
    {
        public static void Postfix(Pawn_PlayerSettings __instance, ref bool __result)
        {
            if (__result == false && __instance.pawn.IsColonyInsect())
            {
                __result = true;
            }
        }
    }
}
