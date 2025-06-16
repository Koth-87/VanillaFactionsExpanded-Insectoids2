using HarmonyLib;
using RimWorld;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(ITab_Pawn_Training), "IsVisible", MethodType.Getter)]
    public static class ITab_Pawn_Training_IsVisible_Patch
    {
        public static void Postfix(ITab_Pawn_Training __instance, ref bool __result)
        {
            if (__result && __instance.SelPawn.IsColonyInsect())
            {
                __result = false;
            }
        }
    }
}
