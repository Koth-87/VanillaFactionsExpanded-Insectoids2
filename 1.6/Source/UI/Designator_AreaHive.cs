using RimWorld;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    public abstract class Designator_AreaHive : Designator_Cells
    {
        private readonly DesignateMode mode;

        public override bool DragDrawMeasurements => true;
        public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.Areas;

        public Designator_AreaHive(DesignateMode mode)
        {
            this.mode = mode;
            useMouseIcon = true;
        }

        public override void DesignateSingleCell(IntVec3 cell)
        {
            if (mode == DesignateMode.Add)
            {
                Map.areaManager.Get<Area_Hive>()[cell] = true;
            }
            else
            {
                Map.areaManager.Get<Area_Hive>()[cell] = false;
            }
            foreach (var pawn in Map.mapPawns.PawnsInFaction(Faction.OfPlayer))
            {
                if (pawn.IsColonyInsect(out var hediff))
                {
                    hediff.UpdateArea();
                }
            }
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            if (!loc.InBounds(Map))
            {
                return false;
            }
            bool enabled = Map.areaManager.Get<Area_Hive>()[loc];
            if (mode == DesignateMode.Add)
            {
                return !enabled;
            }
            return enabled;
        }

        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
            var area = Map.areaManager.Get<Area_Hive>();
            if (area is null)
            {
                area = new Area_Hive(Map.areaManager);
                Map.areaManager.areas.Add(area);
            }
            area.MarkForDraw();
        }
    }
}
