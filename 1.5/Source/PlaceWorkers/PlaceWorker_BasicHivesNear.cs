using Verse;
namespace VFEInsectoids
{
    public class PlaceWorker_BasicHivesNear : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            foreach (var nearby in GenRadial.RadialDistinctThingsAround(center, map, 5, true))
            {
                if (nearby.def == VFEI_DefOf.VFEI2_ArtificialBasicHive || nearby.def.entityDefToBuild == VFEI_DefOf.VFEI2_ArtificialBasicHive)
                {
                    return "VFEI_CannotPlaceCloseToOtherBasicHive".Translate();
                }
            }
            return true;
        }
    }
}
