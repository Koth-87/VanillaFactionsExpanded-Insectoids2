using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }
    [HotSwappable]
    [StaticConstructorOnStartup]
    public class Gizmo_Hive : Gizmo
    {
        public CompHive compHive;


        private List<Color> allColors;

        private List<Color> AllColors
        {
            get
            {
                if (allColors == null)
                {
                    allColors = new List<Color>();

                    foreach (ColorDef colDef in DefDatabase<ColorDef>.AllDefs)
                    {
                        if (!allColors.Any((Color x) => x.IndistinguishableFrom(colDef.color)))
                        {
                            allColors.Add(colDef.color);
                        }
                    }
                    allColors.RemoveAll((Color x) => x.IndistinguishableFrom(compHive.insectColor));
                    allColors.Add(compHive.insectColor);
                    allColors.SortByColor((Color x) => x);
                }
                return allColors;
            }
        }

        private static readonly Color EmptyBlockColor = new Color(0.3f, 0.3f, 0.3f, 1f);

        private static readonly Color FilledBlockColor = new ColorInt(85, 124, 60).ToColor;

        private static readonly Color ExcessBlockColor = ColorLibrary.Red;

        private static readonly Texture2D HiveIcon = ContentFinder<Texture2D>.Get("UI/Hive");

        public Gizmo_Hive()
        {
            Order = -100f;
        }

        public override float GetWidth(float maxWidth)
        {
            return 197f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Widgets.DrawWindowBackground(rect);
            Text.Font = GameFont.Small;
            var labelRect = new Rect(rect.x + 10, rect.y, 75, 32);
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(labelRect, "VFEI_Insectoids".Translate());
            var curPawnKindIconRect = new Rect(labelRect.xMax + 5, labelRect.y, 32, 32);
            Widgets.DefIcon(curPawnKindIconRect, compHive.currentPawnKindToSpawn);
            if (Widgets.ButtonInvisible(curPawnKindIconRect))
            {
                var floatList = new List<FloatMenuOption>();
                FillFloatList(floatList);
                Find.WindowStack.Add(new FloatMenu(floatList));
            }

            curPawnKindIconRect = new Rect(curPawnKindIconRect.xMax + 5, labelRect.y, 32, 32).ContractedBy(3);
            var record = compHive.Props.insectTypes.FirstOrDefault(x => x.insect == compHive.currentPawnKindToSpawn);
            GUI.DrawTexture(curPawnKindIconRect, record.insectType.GetInsectTypeTexture());
            TooltipHandler.TipRegion(curPawnKindIconRect, ("VFEI_" + record.insectType.ToString() + "Desc").Translate());
            curPawnKindIconRect = new Rect(curPawnKindIconRect.xMax + 5, labelRect.y, 32, 32).ContractedBy(3);
            GUI.color = compHive.insectColor;
            GUI.DrawTexture(curPawnKindIconRect, HiveIcon);
            GUI.color = Color.white;

            if (Widgets.ButtonInvisible(curPawnKindIconRect))
            {
                var dialog = new Dialog_ChooseColor("VFEI_ChooseInsectoidColor".Translate
                    (compHive.currentPawnKindToSpawn.label), compHive.insectColor, AllColors, delegate (Color x)
                    {
                        compHive.insectColor = x;
                    });
                dialog.forcePause = true;
                Find.WindowStack.Add(dialog);
            }

            Rect rect2 = new Rect(rect.x + 5, labelRect.yMax + 5, rect.width, rect.height - labelRect.height).ContractedBy(3f);
            Text.Anchor = TextAnchor.UpperLeft;
            DrawInsectBlocks(rect2);
            return new GizmoResult(GizmoState.Clear);
        }

        private void FillFloatList(List<FloatMenuOption> floatList)
        {
            foreach (var pawnkind in compHive.Props.insectTypes)
            {
                AddToList(floatList, pawnkind.insect, pawnkind.insectType.GetInsectTypeTexture());
            }
        }

        private void AddToList(List<FloatMenuOption> floatList, PawnKindDef pawnkind, Texture2D icon)
        {
            floatList.Add(new FloatMenuOption(pawnkind.LabelCap, delegate
            {
                if (compHive.currentPawnKindToSpawn != pawnkind)
                {
                    compHive.ChangePawnKind(pawnkind);
                }
            }, pawnkind.race, extraPartWidth: 50 + 30, extraPartOnGUI: (Rect rect) =>
            {
                var drawRect = new Rect(rect.x + 60, rect.y, 30, 30).ContractedBy(3);
                GUI.DrawTexture(drawRect, icon);
                var record = compHive.Props.insectTypes.FirstOrDefault(x => x.insect == pawnkind);
                TooltipHandler.TipRegion(drawRect, ("VFEI_" + record.insectType.ToString() + "Desc").Translate());
                return false;
            }));
        }

        private void DrawInsectBlocks(Rect rect)
        {
            var totalBandwidth = compHive.InsectCapacity;
            var usedBandwidth = compHive.insects.Count;
            var blocksUsed = Mathf.Max(usedBandwidth, totalBandwidth);
            var blockWidth = 26;
            var pos = new Vector2(rect.x, rect.y);
            var curBlock = 0;
            for (int i = 0; i < blocksUsed; i++)
            {
                curBlock++;
                Rect rect5 = new Rect(pos.x, pos.y, blockWidth, blockWidth).ContractedBy(2f);
                pos.x += blockWidth + 5;
                if (curBlock <= blocksUsed)
                {
                    if (curBlock <= usedBandwidth)
                    {
                        Widgets.DrawRectFast(rect5, (curBlock <= totalBandwidth) ? FilledBlockColor : ExcessBlockColor);
                        if (Widgets.ButtonInvisible(rect5))
                        {
                            CameraJumper.TryJumpAndSelect(compHive.insects[i]);
                        }
                    }
                    else
                    {
                        Widgets.DrawRectFast(rect5, EmptyBlockColor);
                    }
                }
            }
        }
    }
}
