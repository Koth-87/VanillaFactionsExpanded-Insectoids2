using HarmonyLib;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(PawnRenderNode), "EnsureInitialized")]
    public static class PawnRenderNode_EnsureInitialized_Patch
    {
        public static void Postfix(PawnRenderNode __instance)
        {
            if (__instance.tree?.pawn is null) return;
            if (__instance.graphics != null)
            {
                for (var i = __instance.graphics.Count - 1; i >= 0; i--)
                {
                    var graphic = __instance.graphics[i];
                    PawnRenderNode_AnimalPart_GraphicFor_Patch.Postfix(__instance.tree.pawn, ref graphic);
                    __instance.graphics[i] = graphic;
                }
            }
            if (__instance.graphicStateLookup != null)
            {
                var keys = __instance.graphicStateLookup.Keys.ToList();
                for (var i = keys.Count - 1; i >= 0; i--)
                {
                    var key = keys[i];
                    var graphic = __instance.graphicStateLookup[key];
                    PawnRenderNode_AnimalPart_GraphicFor_Patch.Postfix(__instance.tree.pawn, ref graphic);
                    __instance.graphicStateLookup[key] = graphic;
                }
            }
        }
    }
}
