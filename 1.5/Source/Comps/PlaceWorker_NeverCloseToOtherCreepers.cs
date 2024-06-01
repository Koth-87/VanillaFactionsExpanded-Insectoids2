using Verse;

namespace VFEInsectoids
{
    public class PlaceWorker_NeverCloseToOtherCreepers : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            foreach (var item in map.listerThings.ThingsOfDef(def as ThingDef))
            {
                if (item.Position.DistanceTo(center) <= 6.9f)
                {
                    return "VFEI_CannotPlaceCloseToOtherCreeper".Translate();
                }
            }
            return true;
        }
    }
}
