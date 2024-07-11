using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(InspectPaneFiller), "DrawAreaAllowed")]
    public static class InspectPaneFiller_DrawAreaAllowed_Patch
    {
        public static bool Prefix(WidgetRow row, Pawn pawn)
        {
            if (pawn.IsColonyInsect())
            {
                row.Gap(6f);
                bool flag = pawn.playerSettings?.AreaRestrictionInPawnCurrentMap != null;
                Rect rect = row.FillableBar(fillTex: (!flag) ? BaseContent.GreyTex : pawn.playerSettings.AreaRestrictionInPawnCurrentMap.ColorTexture, width: 93f, height: 16f, fillPct: 1f, label: AreaUtility.AreaAllowedLabel(pawn));
                if (Mouse.IsOver(rect))
                {
                    if (flag)
                    {
                        pawn.playerSettings.AreaRestrictionInPawnCurrentMap.MarkForDraw();
                    }
                    Widgets.DrawBox(rect.ContractedBy(-1f));
                }
                return false;
            }
            return true;
        }
    }
}
