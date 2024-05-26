using RimWorld;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    public class CompProperties_PlantFarm : CompProperties
    {
        public float radius;
        public ThingDef plant;
        public TerrainDef requiredTerrain;
        public IntRange spawnTickRate;
        public CompProperties_PlantFarm()
        {
            this.compClass = typeof(CompPlantFarm);
        }
    }
    public class CompPlantFarm : ThingComp
    {
        public CompProperties_PlantFarm Props => base.props as CompProperties_PlantFarm;
        public int nextPlantSpawn;

        public override void CompTick()
        {
            base.CompTick();
            if (parent.Spawned)
            {
                if (nextPlantSpawn == 0)
                {
                    nextPlantSpawn = NextPlantSpawn;
                }
                if (Find.TickManager.TicksGame >= nextPlantSpawn)
                {
                    var cells = GenRadial.RadialCellsAround(parent.Position, Props.radius, true)
                        .Where(cell => (Props.requiredTerrain is null || cell.GetTerrain(parent.Map) == Props.requiredTerrain)
                        && cell.GetThingList(parent.Map).OfType<Plant>().Any(thing => Props.plant == thing.def) is false);
                    if (cells.TryRandomElement(out var cell))
                    {
                        var plant = GenSpawn.Spawn(Props.plant, cell, parent.Map) as Plant;
                        plant.Growth = 0.05f;
                    }
                    nextPlantSpawn = NextPlantSpawn;
                }
            }
        }

        public int NextPlantSpawn => Find.TickManager.TicksGame + Props.spawnTickRate.RandomInRange;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref nextPlantSpawn, "nextPlantSpawn");
        }
    }
}
