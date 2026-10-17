using RimWorld;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    public class JobGiver_HiveDefense : JobGiver_AIFightEnemies
    {
        public override IntVec3 GetFlagPosition(Pawn pawn)
        {
            if (pawn.mindState.duty.focus.Thing is Thing hive)
            {
                return hive.Position;
            }
            return pawn.Position;
        }

        public override float GetFlagRadius(Pawn pawn)
        {
            if (pawn.IsColonyInsect(out var hediff))
            {
                if (hediff.InsectType == InsectType.Defender)
                {
                    return 50;
                }
            }
            return 30;
        }

        public override Job MeleeAttackJob(Pawn pawn, Thing enemyTarget)
        {
            Job job = base.MeleeAttackJob(pawn, enemyTarget);
            job.attackDoorIfTargetLost = true;
            return job;
        }
    }
}
