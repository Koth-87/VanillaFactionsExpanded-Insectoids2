using RimWorld;

namespace VFEInsectoids
{
    public class RaidStrategyWorker_HiveRaid : RaidStrategyWorker_ImmediateAttack
    {
        public override bool CanUseWith(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            return parms?.faction == Faction.OfInsects && base.CanUseWith(parms, groupKind);
        }
    }
}
