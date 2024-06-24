using System.Collections.Generic;
using Verse;

namespace VFEInsectoids
{

    public class CompProperties_Terraform : CompProperties_AOE
    {
        public TerrainDef terrainToSet;
        public CompProperties_Terraform()
        {
            this.compClass = typeof(CompTerraform);
        }
    }

    public class CompTerraform : CompAOE
    {
        public CompProperties_Terraform Props => base.props as CompProperties_Terraform;
        public CompInsectJellySpawner insectSpawner;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                parent.Map.terrainGrid.SetTerrain(parent.Position, Props.terrainToSet);
            }
            insectSpawner = parent.GetComp<CompInsectJellySpawner>();
        }

        protected override bool CellValidator(IntVec3 cell)
        {
            return cell.GetTerrain(parent.Map) != Props.terrainToSet;
        }

        protected override List<IntVec3> GetCells()
        {
            var cells = base.GetCells();
            insectSpawner.canSpawn = cells.Any() is false;
            return cells;
        }

        protected override void DoEffect(IntVec3 cell)
        {
            parent.Map.terrainGrid.SetTerrain(cell, Props.terrainToSet);
        }
    }
}
