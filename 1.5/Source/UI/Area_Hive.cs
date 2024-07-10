using RimWorld;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    public class Area_Hive : Area
    {
        public Area_Hive()
        {
        }

        public Area_Hive(AreaManager areaManager) : base(areaManager)
        {
        }

        public override string Label => "VFEI_HiveArea".Translate();

        public override Color Color => new ColorInt(139, 162, 72).ToColor;

        public override int ListPriority => 6969;

        public override string GetUniqueLoadID()
        {
            return $"VFEI_HiveArea_{ID}";
        }
    }
}
