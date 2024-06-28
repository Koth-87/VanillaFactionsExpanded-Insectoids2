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
            Widgets.DefIcon(curPawnKindIconRect, compHive.chosenKind);
            if (Widgets.ButtonInvisible(curPawnKindIconRect))
            {
                var floatList = new List<FloatMenuOption>();
                FillFloatList(floatList);
                Find.WindowStack.Add(new FloatMenu(floatList));
            }

            curPawnKindIconRect = new Rect(curPawnKindIconRect.xMax + 5, labelRect.y, 32, 32).ContractedBy(3);
            var record = compHive.Props.insectTypes.FirstOrDefault(x => x.insect == compHive.chosenKind);
            GUI.DrawTexture(curPawnKindIconRect, record.insectType.GetInsectTypeTexture());
            TooltipHandler.TipRegion(curPawnKindIconRect, ("VFEI_" + record.insectType.ToString() + "Desc").Translate());
            curPawnKindIconRect = new Rect(curPawnKindIconRect.xMax + 5, labelRect.y, 32, 32).ContractedBy(3);
            GUI.color = compHive.insectColor;
            GUI.DrawTexture(curPawnKindIconRect, HiveIcon);
            GUI.color = Color.white;

            if (Widgets.ButtonInvisible(curPawnKindIconRect))
            {
                var dialog = new Dialog_ChooseColor("VFEI_ChooseInsectoidColor".Translate
                    (compHive.chosenKind.label), compHive.insectColor, AllColors, delegate (Color x)
                    {
                        compHive.insectColor = x;
                        foreach (var insect in compHive.insects)
                        {
                            insect.Drawer.renderer.SetAllGraphicsDirty();
                        }
                    });
                dialog.forcePause = true;
                Find.WindowStack.Add(dialog);
            }

            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect2 = new Rect(rect.x + 7, labelRect.yMax - 5, rect.width - 14, rect.height - labelRect.height + 5);
            DrawInsectBlocks(rect2.ContractedBy(3));
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
                if (compHive.chosenKind != pawnkind)
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
            int maxBlocks = Mathf.Max(usedBandwidth, totalBandwidth);
            int row = 1;
            float blockSize = rect.height;
            int blocksPerWidth = (int)(rect.width / blockSize);
            while (row * blocksPerWidth < maxBlocks)
            {
                if (blockSize > 26)
                {
                    blockSize *= 0.99f;
                }
                else
                {
                    row++;
                    blockSize = (int)(rect.height / (float)row);
                }
                blocksPerWidth = (int)(rect.width / blockSize);
            }
            int column = (int)(rect.width / blockSize);
            int curBlock = 0;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    curBlock++;
                    Rect rect5 = new Rect(rect.x + (float)(j * blockSize), (rect.y + (float)(i * blockSize)), blockSize, blockSize).ContractedBy(2f);
                    if (curBlock <= maxBlocks)
                    {
                        if (curBlock <= usedBandwidth)
                        {
                            Widgets.DrawRectFast(rect5, (curBlock <= totalBandwidth) ? FilledBlockColor : ExcessBlockColor);
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
}
