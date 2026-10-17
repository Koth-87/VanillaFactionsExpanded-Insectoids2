using Verse;

namespace VFEInsectoids
{
    public abstract class CompAOE : ThingComp
    {
        public CompProperties_AOE Props => base.props as CompProperties_AOE;
        public int nextTickEffect;
        protected virtual bool Active => parent.Spawned;
        public int NextTickEffect => Find.TickManager.TicksGame + Props.spawnTickRate.RandomInRange;
        public CompSpawnerDisableable compSpawner;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            compSpawner = parent.GetComp<CompSpawnerDisableable>();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref nextTickEffect, "nextPlantSpawn");
        }
    }
}
