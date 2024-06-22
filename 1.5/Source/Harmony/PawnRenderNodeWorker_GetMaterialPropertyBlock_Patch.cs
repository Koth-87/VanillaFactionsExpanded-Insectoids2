using HarmonyLib;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(PawnRenderNodeWorker), "GetMaterialPropertyBlock")]
    public static class PawnRenderNodeWorker_GetMaterialPropertyBlock_Patch
    {
        public static void Postfix(ref MaterialPropertyBlock __result, Material material, PawnDrawParms parms)
        {
            if (__result != null && parms.pawn.IsColonyInsect(out var hediff))
            {
                PawnRenderUtility.SetMatPropBlockOverlay(__result, hediff.CompHive.insectColor);
            }
        }
    }
}
