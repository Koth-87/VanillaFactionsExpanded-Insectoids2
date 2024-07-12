using HarmonyLib;
using System;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    public class VFEInsectoidsMod : Mod
    {
        public VFEInsectoidsSettings settings;
        public VFEInsectoidsMod(ModContentPack pack) : base(pack)
        {
            new Harmony("VFEInsectoidsMod").PatchAll();
            settings = GetSettings<VFEInsectoidsSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            VFEInsectoidsSettings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }
    }

    public class VFEInsectoidsSettings : ModSettings
    {
        public static float minHiveStabilityDistance = 10.9f;
        public static float stabilityHiveMaintenancePenalty = 0.25f;

        public static void DoSettingsWindowContents(Rect inRect)
        {
            var ls = new Listing_Standard();
            ls.Begin(inRect);
            minHiveStabilityDistance = ls.SliderLabeled("VFEI_MinHiveStabilityDistance".Translate() + ": " + Math.Round(minHiveStabilityDistance, 1).ToString("0.#"), minHiveStabilityDistance, 1, 50);
            stabilityHiveMaintenancePenalty = ls.SliderLabeled("VFEI_StabilityHiveMaintenancePenalty".Translate() + ": " + stabilityHiveMaintenancePenalty.ToStringPercent(), stabilityHiveMaintenancePenalty, 0f, 1f);
            ls.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref minHiveStabilityDistance, "minHiveStabilityDistance", 10.9f);
            Scribe_Values.Look(ref stabilityHiveMaintenancePenalty, "stabilityHiveMaintenancePenalty", 0.25f);
        }
    }
}
