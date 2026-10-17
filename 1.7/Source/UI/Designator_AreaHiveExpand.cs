using RimWorld;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    public class Designator_AreaHiveExpand : Designator_AreaHive
    {
        public Designator_AreaHiveExpand() : base(DesignateMode.Add)
        {
            defaultLabel = "VFEI_HiveAreaExpand".Translate();
            defaultDesc = "VFEI_HiveAreaExpandDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("UI/HiveZone", true);
            soundDragSustain = SoundDefOf.Designate_DragAreaAdd;
            soundDragChanged = SoundDefOf.Designate_DragZone_Changed;
            soundSucceeded = SoundDefOf.Designate_ZoneAdd_AllowedArea;
        }
    }
}
