using Verse;

namespace VFEInsectoids
{
    public class PlaceWorker_NeverCloseToOtherCreepers : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            foreach (var nearby in GenRadial.RadialDistinctThingsAround(center, map, VFEI_DefOf.VFEI2_Creeper.specialDisplayRadius, true))
            {
                if (nearby.def == VFEI_DefOf.VFEI2_Creeper || nearby.def.entityDefToBuild == VFEI_DefOf.VFEI2_Creeper)
                {
                    return "VFEI_CannotPlaceCloseToOtherCreeper".Translate();
                }
            }
            return true;
        }
    }
}
