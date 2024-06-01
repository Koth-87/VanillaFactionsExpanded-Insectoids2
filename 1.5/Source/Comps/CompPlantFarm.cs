using RimWorld;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    public class CompProperties_PlantFarm : CompProperties_AOE
    {
        public ThingDef plant;
        public TerrainDef requiredTerrain;
        public CompProperties_PlantFarm()
        {
            this.compClass = typeof(CompPlantFarm);
        }
    }
    public class CompPlantFarm : CompAOE
    {
        public CompProperties_PlantFarm Props => base.props as CompProperties_PlantFarm;

        protected override bool CellValidator(IntVec3 cell)
        {
            return (Props.requiredTerrain is null || cell.GetTerrain(parent.Map) == Props.requiredTerrain)
                                    && cell.GetThingList(parent.Map).OfType<Plant>().Any(thing => Props.plant == thing.def) is false;
        }

        protected override void DoEffect(IntVec3 cell)
        {
            var plant = GenSpawn.Spawn(Props.plant, cell, parent.Map) as Plant;
            plant.Growth = 0.05f;
        }
    }
}
