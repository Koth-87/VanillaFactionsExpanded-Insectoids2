using HarmonyLib;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(PawnRenderNode_AnimalPart), "GraphicFor")]
    public static class PawnRenderNode_AnimalPart_GraphicFor_Patch
    {
        public static void Postfix(Pawn pawn, ref Graphic __result)
        {
            if (__result != null && pawn.IsColonyInsect(out var hediff))
            {
                if (pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Fresh)
                {
                    __result = __result.GetColoredVersion(__result.Shader, hediff.CompHive.insectColor, __result.colorTwo);
                }
                else if (pawn.Drawer.renderer.CurRotDrawMode == RotDrawMode.Rotting)
                {
                    __result = __result.GetColoredVersion(__result.Shader, PawnRenderUtility.GetRottenColor(hediff.CompHive.insectColor), __result.colorTwo);
                }
            }
        }
    }
}
