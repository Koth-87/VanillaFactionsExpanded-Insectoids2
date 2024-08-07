using RimWorld;
using System.Collections.Generic;
using System.Linq;
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

    public class CompTerraform : CompAOE_Cell
    {
        public CompProperties_Terraform Props => base.props as CompProperties_Terraform;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                parent.Map.terrainGrid.SetTerrain(parent.Position, Props.terrainToSet);
            }
        }

        protected override bool CellValidator(IntVec3 cell)
        {
            var result = cell.GetTerrain(parent.Map) is TerrainDef terrain && terrain != Props.terrainToSet
                && TerrainValidator(terrain) && cell.GetEdifice(parent.Map) is null;
            //Log.Message(cell + " - result: " + result);
            return result;
        }

        public static bool TerrainValidator(TerrainDef terrain)
        {
            return terrain.IsWater is false && terrain.layerable is false &&
                (terrain.natural || terrain.affordances != null
                && (terrain.affordances.Contains(TerrainAffordanceDefOf.SmoothableStone)
                || terrain.affordances.Contains(VFEI_DefOf.Diggable)))
                && terrain.HasTag("Road") is false;
        }

        protected override List<IntVec3> GetCells()
        {
            var cells = base.GetCells();
            compSpawner.canSpawn = cells.Any() is false;
            return cells;
        }

        protected override bool TryGetCell(List<IntVec3> cells, out IntVec3 cell)
        {
            cell = cells.OrderBy(x => x.DistanceTo(parent.Position)).FirstOrDefault();
            return cell != default;
        }

        protected override void DoEffect(IntVec3 cell)
        {
            parent.Map.terrainGrid.SetTerrain(cell, Props.terrainToSet);
        }
    }
}
