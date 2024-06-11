using RimWorld;

namespace VFEInsectoids
{
    public class RaidStrategyWorker_HiveRaid : RaidStrategyWorker_ImmediateAttack
    {
        public override bool CanUseWith(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            return parms?.faction?.def == VFEI_DefOf.VFEI2_Hive && base.CanUseWith(parms, groupKind);
        }
    }
}
