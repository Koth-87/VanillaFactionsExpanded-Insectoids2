
using RimWorld;
using Verse;
namespace VFEInsectoids
{
    public class CompDamagedWhenUnfueled : ThingComp
    {


        public CompProperties_DamagedWhenUnfueled Props => (CompProperties_DamagedWhenUnfueled)props;

        private CompRefuelable refuelComp;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            refuelComp = parent.GetComp<CompRefuelable>();
        }

        public override void CompTick()
        {
            if (this.parent.IsHashIntervalTick(Props.interval))
            {
                if (refuelComp != null && !refuelComp.HasFuel)
                {

                    parent.HitPoints--;
                    if (parent.HitPoints <= 0) { parent.Kill(null); }

                }

            }
        }


       
    }
}