using Verse;

namespace VFEInsectoids
{
    public class Hediff_ToBeCocooned : HediffWithComps
    {
        public Thing hive;

        public override void Tick()
        {
            base.Tick();
            if (pawn.Spawned)
            {
                var comp = hive.TryGetComp<CompHive>();
                comp.SpawnCocoon(pawn);
                pawn.health.RemoveHediff(this);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref hive, "hive");
        }
    }
}
