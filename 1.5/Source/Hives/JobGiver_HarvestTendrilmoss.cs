using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    public class JobGiver_HarvestTendrilmoss : ThinkNode_JobGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            Job job = JobMaker.MakeJob(JobDefOf.Harvest);
            float num = 0f;
            foreach (var plant in pawn.Map.listerThings.ThingsOfDef(VFEI_DefOf.VFEI2_TendrilmossVines)
                .OfType<Plant>().OrderBy(x => x.Position.DistanceTo(pawn.Position)))
            {
                if (HasJobOnCell(pawn, plant))
                {
                    num += plant.def.plant.harvestWork;
                    if (num > 2400f)
                    {
                        break;
                    }
                    job.AddQueuedTarget(TargetIndex.A, plant);
                }
            }
            if (job.targetQueueA != null && job.targetQueueA.Count >= 3)
            {
                job.targetQueueA.SortBy((LocalTargetInfo targ) => targ.Cell.DistanceToSquared(pawn.Position));
            }
            if (job.targetQueueA != null && job.targetQueueA.Any())
            {
                return job;
            }
            return null;
        }

        public bool HasJobOnCell(Pawn pawn, Plant plant)
        {
            if (plant.IsForbidden(pawn))
            {
                return false;
            }
            if (!plant.HarvestableNow || plant.LifeStage != PlantLifeStage.Mature)
            {
                return false;
            }
            if (!plant.CanYieldNow())
            {
                return false;
            }
            if (!pawn.CanReserve(plant, 1, -1, null))
            {
                return false;
            }
            return true;
        }
    }
}
