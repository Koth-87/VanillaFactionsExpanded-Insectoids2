using RimWorld;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    public class Designator_AreaHiveClear : Designator_AreaHive
    {
        public Designator_AreaHiveClear() : base(DesignateMode.Remove)
        {
            defaultLabel = "VFEI_HiveAreaClear".Translate();
            defaultDesc = "VFEI_HiveAreaClearDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/ShrinkHiveZone", true);
            soundDragSustain = SoundDefOf.Designate_DragAreaDelete;
            soundDragChanged = null;
            soundSucceeded = SoundDefOf.Designate_ZoneDelete;
        }
    }
}
