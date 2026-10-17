using RimWorld;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    public class CompTargetEffect_UpgradeHive : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            if (user.IsColonistPlayerControlled)
            {
                Job job = JobMaker.MakeJob(VFEI_DefOf.VFEI_UpgradeHive, target, parent);
                job.count = 1;
                job.playerForced = true;
                user.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }
        }
    }
}
