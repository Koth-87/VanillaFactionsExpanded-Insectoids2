using HarmonyLib;
using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VFEInsectoids
{
    public class VFEInsectoidsMod : Mod
    {
        public VFEInsectoidsSettings settings;
        public VFEInsectoidsMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<VFEInsectoidsSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            VFEInsectoidsSettings.DoSettingsWindowContents(inRect);
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            SettingsApplier.ApplySettings();
        }

        public override string SettingsCategory() => "VFEI_ModNameForSettings".Translate();
    }

    [StaticConstructorOnStartup]
    public static class SettingsApplier
    {
        static SettingsApplier()
        {
            ApplySettings();
        }

        public static void ApplySettings()
        {
            var spawnerProps = VFEI_DefOf.VFEI2_JellyFarm.GetCompProperties<CompProperties_Spawner>();
            spawnerProps.spawnCount = VFEInsectoidsSettings.jellyFarmProductionAmount;
            spawnerProps.spawnMaxAdjacent = spawnerProps.spawnCount * 4;

            var terraformProps = VFEI_DefOf.VFEI2_Creeper.GetCompProperties<CompProperties_Terraform>();
            terraformProps.spawnTickRate = new IntRange(VFEInsectoidsSettings.creeperSpawnSpeed, VFEInsectoidsSettings.creeperSpawnSpeed);
        
            var plantFarmProps = VFEI_DefOf.VFEI2_TendrilFarm.GetCompProperties<CompProperties_PlantFarm>();
            plantFarmProps.spawnTickRate = VFEInsectoidsSettings.tendrilmossSpawnSpeed;
        }
    }

    public class VFEInsectoidsSettings : ModSettings
    {
        public static float minHiveStabilityDistance = 10.9f;
        public static float stabilityHiveMaintenancePenalty = 0.25f;
        public static int jellyFarmProductionAmount = 10;
        public static int creeperSpawnSpeed = 45000;
        public static IntRange tendrilmossSpawnSpeed = new IntRange(15000, 25000);
        public static int baseArtificalInsectCount = 2;
        public static float baseTimeBetweenHanHordeWaves = 5f;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            var ls = new Listing_Standard();
            ls.Begin(inRect);
            minHiveStabilityDistance = ls.SliderLabeled("VFEI_MinHiveStabilityDistance".Translate() + ": " + Math.Round(minHiveStabilityDistance, 1).ToString("0.#"), minHiveStabilityDistance, 1, 50);
            stabilityHiveMaintenancePenalty = ls.SliderLabeled("VFEI_StabilityHiveMaintenancePenalty".Translate() + ": " + stabilityHiveMaintenancePenalty.ToStringPercent(), stabilityHiveMaintenancePenalty, 0f, 1f);
            jellyFarmProductionAmount = (int)ls.SliderLabeled("VFEI_JellyFarmProductionAmount".Translate() + ": " + jellyFarmProductionAmount.ToString(), jellyFarmProductionAmount, 1, 100);
            creeperSpawnSpeed = (int)ls.SliderLabeled("VFEI_CreeperSpawnSpeed".Translate() + ": " + creeperSpawnSpeed.ToStringTicksToPeriod(), creeperSpawnSpeed, 2500, 120000);
            IntRange(ls, "VFEI_TendrilmossSpawnSpeed".Translate(), ref tendrilmossSpawnSpeed, 2500, 120000);
            baseArtificalInsectCount = (int)ls.SliderLabeled("VFEI_BaseArtificalInsectCount".Translate() + ": " + baseArtificalInsectCount.ToString(), baseArtificalInsectCount, 1, 5);
            baseTimeBetweenHanHordeWaves = ls.SliderLabeled("VFEI_BaseTimeBetweenHanHordeWaves".Translate() + ": " + ((int)(baseTimeBetweenHanHordeWaves * GenDate.TicksPerDay)).ToStringTicksToPeriod(), baseTimeBetweenHanHordeWaves, 0.1f, 10f);
            ls.End();
        }

        public static void IntRange(Listing_Standard ls, string label, ref IntRange range, int min, int max)
        {
            Rect rect = ls.GetRect(30f);
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect.LeftPart(0.5f), label);
            Text.Anchor = TextAnchor.UpperLeft;
            rect = rect.RightPart(1f - 0.5f);
            if (!ls.BoundingRectCached.HasValue || rect.Overlaps(ls.BoundingRectCached.Value))
            {
                IntRange(rect, (int)ls.CurHeight, ref range, min, max);
            }
            ls.Gap(ls.verticalSpacing);
        }

        public static void IntRange(Rect rect, int id, ref IntRange range, int min = 0, int max = 100, string labelKey = null, int minWidth = 0)
        {
            Rect rect2 = rect;
            rect2.xMin += 8f;
            rect2.xMax -= 8f;
            GUI.color = Widgets.RangeControlTextColor;
            string text = range.min.ToStringTicksToPeriod() + " - " + range.max.ToStringTicksToPeriod();
            if (labelKey != null)
            {
                text = labelKey.Translate(text);
            }
            GameFont font = Text.Font;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperCenter;
            Rect rect3 = rect2;
            rect3.yMin -= 2f;
            Widgets.Label(rect3, text);
            Text.Anchor = TextAnchor.UpperLeft;
            Rect position = new Rect(rect2.x, rect2.yMax - 8f - 1f, rect2.width, 2f);
            GUI.DrawTexture(position, BaseContent.WhiteTex);
            float num = rect2.x + rect2.width * (float)(range.min - min) / (float)(max - min);
            float num2 = rect2.x + rect2.width * (float)(range.max - min) / (float)(max - min);
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(num, rect2.yMax - 8f - 2f, num2 - num, 4f), BaseContent.WhiteTex);
            float num3 = num;
            float num4 = num2;
            Rect position2 = new Rect(num3 - 16f, position.center.y - 8f, 16f, 16f);
            GUI.DrawTexture(position2, Widgets.FloatRangeSliderTex);
            Rect position3 = new Rect(num4 + 16f, position.center.y - 8f, -16f, 16f);
            GUI.DrawTexture(position3, Widgets.FloatRangeSliderTex);
            if (Widgets.curDragEnd != 0 && (Event.current.type == EventType.MouseUp || Event.current.rawType == EventType.MouseDown))
            {
                Widgets.draggingId = 0;
                Widgets.curDragEnd = Widgets.RangeEnd.None;
                SoundDefOf.DragSlider.PlayOneShotOnCamera();
            }
            bool flag = false;
            if (Mouse.IsOver(rect) || Widgets.draggingId == id)
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && id != Widgets.draggingId)
                {
                    Widgets.draggingId = id;
                    float x = Event.current.mousePosition.x;
                    if (x < position2.xMax)
                    {
                        Widgets.curDragEnd = Widgets.RangeEnd.Min;
                    }
                    else if (x > position3.xMin)
                    {
                        Widgets.curDragEnd = Widgets.RangeEnd.Max;
                    }
                    else
                    {
                        float num5 = Mathf.Abs(x - position2.xMax);
                        float num6 = Mathf.Abs(x - (position3.x - 16f));
                        Widgets.curDragEnd = ((num5 < num6) ? Widgets.RangeEnd.Min : Widgets.RangeEnd.Max);
                    }
                    flag = true;
                    Event.current.Use();
                    SoundDefOf.DragSlider.PlayOneShotOnCamera();
                }
                if (flag || (Widgets.curDragEnd != 0 && UnityGUIBugsFixer.MouseDrag()))
                {
                    int num7 = Mathf.RoundToInt(Mathf.Clamp((Event.current.mousePosition.x - rect2.x) / rect2.width * (float)(max - min) + (float)min, min, max));
                    if (Widgets.curDragEnd == Widgets.RangeEnd.Min)
                    {
                        if (num7 != range.min)
                        {
                            range.min = num7;
                            if (range.min > max - minWidth)
                            {
                                range.min = max - minWidth;
                            }
                            int num8 = Mathf.Max(min, range.min + minWidth);
                            if (range.max < num8)
                            {
                                range.max = num8;
                            }
                            Widgets.CheckPlayDragSliderSound();
                        }
                    }
                    else if (Widgets.curDragEnd == Widgets.RangeEnd.Max && num7 != range.max)
                    {
                        range.max = num7;
                        if (range.max < min + minWidth)
                        {
                            range.max = min + minWidth;
                        }
                        int num9 = Mathf.Min(max, range.max - minWidth);
                        if (range.min > num9)
                        {
                            range.min = num9;
                        }
                        Widgets.CheckPlayDragSliderSound();
                    }
                    if (Event.current.type == EventType.MouseDrag)
                    {
                        Event.current.Use();
                    }
                }
            }
            Text.Font = font;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref minHiveStabilityDistance, "minHiveStabilityDistance", 10.9f);
            Scribe_Values.Look(ref stabilityHiveMaintenancePenalty, "stabilityHiveMaintenancePenalty", 0.25f);
            Scribe_Values.Look(ref jellyFarmProductionAmount, "jellyFarmProductionAmount", 10);
            Scribe_Values.Look(ref creeperSpawnSpeed, "stabcreeperSpawnSpeedilityHiveMaintenancePenalty", 45000);
            Scribe_Values.Look(ref tendrilmossSpawnSpeed, "tendrilmossSpawnSpeed", new IntRange(15000, 25000));
            Scribe_Values.Look(ref baseArtificalInsectCount, "baseArtificalInsectCount", 2);
            Scribe_Values.Look(ref baseTimeBetweenHanHordeWaves, "baseTimeBetweenHanHordeWaves", 5f);
        }
    }
}
