
using RimWorld;
using Verse;
namespace VFEInsectoids
{
    public class JobGiver_AICastAbilityOnPosition : JobGiver_AICastAbility
    {
        public override LocalTargetInfo GetTarget(Pawn caster, Ability ability)
        {
            LocalTargetInfo localTargetInfo = new LocalTargetInfo(caster.Position);
            if (ability.CanApplyOn(localTargetInfo))
            {
                return localTargetInfo;
            }
            return LocalTargetInfo.Invalid;
        }
    }
}