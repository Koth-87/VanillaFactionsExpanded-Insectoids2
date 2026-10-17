using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    public class ArtificialHive : ThingWithComps, IAttackTarget, ILoadReferenceable
    {
        Thing IAttackTarget.Thing => this;

        public float TargetPriorityFactor => 0.4f;

        public LocalTargetInfo TargetCurrentlyAimingAt => LocalTargetInfo.Invalid;

        public bool ThreatDisabled(IAttackTargetSearcher disabledFor)
        {
            return false;
        }
    }
}
