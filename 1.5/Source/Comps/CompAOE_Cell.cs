using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    public class CompProperties_AOE : CompProperties
    {
        public float radius;
        public IntRange spawnTickRate;
    }

    public abstract class CompAOE_Cell : CompAOE
    {
        public override void CompTick()
        {
            base.CompTick();
            if (Active)
            {
                if (nextTickEffect == 0)
                {
                    nextTickEffect = NextTickEffect;
                }
                if (Find.TickManager.TicksGame >= nextTickEffect)
                {
                    var cells = GetCells();
                    if (TryGetCell(cells, out var cell))
                    {
                        DoEffect(cell);
                    }
                    nextTickEffect = NextTickEffect;
                }
            }
        }

        protected virtual bool TryGetCell(List<IntVec3> cells, out IntVec3 cell)
        {
            return cells.TryRandomElement(out cell);
        }

        protected virtual List<IntVec3> GetCells()
        {
            return GenRadial.RadialCellsAround(parent.Position, Props.radius, true)
                .Where(cell => cell.InBounds(parent.Map) && CellValidator(cell)).ToList();
        }

        protected abstract void DoEffect(IntVec3 cell);

        protected abstract bool CellValidator(IntVec3 cell);
    }
}
