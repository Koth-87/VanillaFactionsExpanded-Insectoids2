
using RimWorld;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    public class CompAbilityInsectGlide : AbilityComp, ICompAbilityEffectOnJumpCompleted
    {
        public override bool GizmoDisabled(out string reason)
        {
            if (parent.pawn.Position.Roofed(parent.pawn.Map))
            {
                reason = "VFEI_CannotUseRoofed".Translate();
                return true;
            }
            return base.GizmoDisabled(out reason);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (parent.pawn.Faction != Faction.OfPlayer && parent.pawn.IsHashIntervalTick(60) 
                && parent.pawn.CurJob is Job job)
            {
                if ((job.jobGiver is JobGiver_AIGotoNearestHostile 
                    || job.jobGiver is JobGiver_AITrashBuildingsDistant
                    || job.jobGiver is JobGiver_AIFightEnemies)
                    && job.targetA.HasThing && parent.CanCast)
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

        public void OnJumpCompleted(IntVec3 origin, LocalTargetInfo target)
        {
            IntVec3 curLoc = CellFinder.StandableCellNear(target.Cell, parent.pawn.Map, 2.9f);
            if (curLoc.IsValid && curLoc != parent.pawn.Position)
            {
                parent.pawn.jobs.StartJob(JobMaker.MakeJob(JobDefOf.Goto, curLoc));
            }
        }
    }
}