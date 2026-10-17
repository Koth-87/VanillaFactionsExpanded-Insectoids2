using RimWorld;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    public class JobGiver_InsectHunt : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            var prey = FoodUtility.BestPawnToHuntForPredator(pawn, true);
            if (prey != null && pawn.CanReserveAndReach(prey, PathEndMode.Touch, Danger.Deadly))
            {
                return JobMaker.MakeJob(VFEI_DefOf.VFEI_InsectHunt, prey);
            }
            return null;
        }
    }

}
