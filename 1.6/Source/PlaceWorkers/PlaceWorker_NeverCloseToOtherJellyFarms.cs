using Verse;

namespace VFEInsectoids
{
    public class PlaceWorker_NeverCloseToOtherJellyFarms : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            foreach (var nearby in GenRadial.RadialDistinctThingsAround(center, map, 3.9f, true))
            {
                if (nearby.def == def || nearby.def.entityDefToBuild == def)
                {
                    return "VFEI_CannotPlaceCloseToOtherJellyFarm".Translate();
                }
            }
            return true;
        }
    }
}
