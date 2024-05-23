using RimWorld;
using Verse;
using Verse.Sound;
using VFEInsectoids;

namespace VFEInsectoids
{
    internal class CompSwarmlingToCocoon : ThingComp
    {
        private int timeBeforeTransform;

        private CompProperties_SwarmlingToCocoon Props
        {
            get
            {
                return (CompProperties_SwarmlingToCocoon)this.props;
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.timeBeforeTransform = Find.TickManager.TicksGame + Props.ticksBeforeBecomingCocoon;
        }

        public override void CompTick()
        {
            base.CompTick();
            if (Find.TickManager.TicksGame == this.timeBeforeTransform && this.parent.Map != null)
            {
                IntVec3 pos = this.parent.Position;
                Map map = this.parent.Map;

                Thing thing = ThingMaker.MakeThing(VFEI_DefOf.VFEI2_InsectoidCocoon);
                if (this.parent.Faction != null) thing.SetFaction(this.parent.Faction);

                GenSpawn.Spawn(thing, pos, map);
                for (int i = 0; i < 5; i++)
                {
                    CellFinder.TryFindRandomReachableCellNearPosition(pos, pos, map, 1, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out IntVec3 c);
                    FilthMaker.TryMakeFilth(c, map, ThingDefOf.Filth_Slime);
                }
                VFEI_DefOf.Hive_Spawn.PlayOneShot(new TargetInfo(pos, map));
                this.parent.Destroy();
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.timeBeforeTransform, "timeBeforeTransform");
        }
    }
}