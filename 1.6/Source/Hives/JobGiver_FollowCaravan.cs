using RimWorld;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    public class JobGiver_FollowCaravan : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            var pawnHive = pawn.mindState.duty.focus.Pawn;
            if (pawnHive.CurJob is Job job && job.def == JobDefOf.Goto && job.jobGiver is JobGiver_GotoTravelDestination)
            {
                return JobMaker.MakeJob(JobDefOf.Goto, job.targetA);
            }
            return null;
        }
    }
}
