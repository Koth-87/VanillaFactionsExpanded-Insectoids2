
using RimWorld;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    public class CompAbilityInsectGlide : AbilityComp
    {
        public override void CompTick()
        {
            base.CompTick();
            if (parent.pawn.IsHashIntervalTick(60) && parent.pawn.CurJob is Job job 
                && job.jobGiver is JobGiver_AIGotoNearestHostile && job.targetA.HasThing && parent.CanCast)
            {
                if (JobGiver_AIGotoNearestHostile_TryGiveJob_Patch.TryGetTargetCell(parent.pawn, 
                    job.targetA.Thing, this.parent.verb, out var result))
                {
                    if (result != default)
                    {
                        Job jumpJob = parent.GetJob(result.cell, result.cell);
                        parent.pawn.jobs.StartJob(jumpJob, JobCondition.InterruptForced, resumeCurJobAfterwards: true);
                    }
                }
            }
        }
    }
}