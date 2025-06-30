using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    public class JobDriver_UpgradeHive : JobDriver
    {
        private Thing Hive => job.GetTarget(TargetIndex.A).Thing;

        private Thing Item => job.GetTarget(TargetIndex.B).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (pawn.Reserve(Hive, job, 1, -1, null, errorOnFailed))
            {
                return pawn.Reserve(Item, job, 1, -1, null, errorOnFailed);
            }
            return false;
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.B).FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
            Toil toil = Toils_General.Wait(600);
            toil.WithProgressBarToilDelay(TargetIndex.A);
            toil.FailOnDespawnedOrNull(TargetIndex.A);
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return toil;
            yield return Toils_General.Do(Upgrade);
        }

        private void Upgrade()
        {
            var compHive = Hive.TryGetComp<CompHive>();
            compHive.additionalCapacity += 2;
            while (compHive.CanSpawn())
            {
                compHive.DoSpawn(sendMessage: false);
            }
            Item.SplitOff(1).Destroy();
        }
    }
}
