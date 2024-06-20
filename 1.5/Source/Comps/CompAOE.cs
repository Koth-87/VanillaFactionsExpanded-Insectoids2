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

    public abstract class CompAOE : ThingComp
    {
        public CompProperties_AOE Props => base.props as CompProperties_AOE;
        public int nextTickEffect;

        public int NextTickEffect => Find.TickManager.TicksGame + Props.spawnTickRate.RandomInRange;

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
                    if (cells.TryRandomElement(out var cell))
                    {
                        DoEffect(cell);
                    }
                    nextTickEffect = NextTickEffect;
                }
            }
        }

        protected virtual bool Active => parent.Spawned;

        protected virtual List<IntVec3> GetCells()
        {
            return GenRadial.RadialCellsAround(parent.Position, Props.radius, true)
                .Where(cell => cell.InBounds(parent.Map) & CellValidator(cell)).ToList();
        }

        protected abstract void DoEffect(IntVec3 cell);

        protected abstract bool CellValidator(IntVec3 cell);

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref nextTickEffect, "nextPlantSpawn");
        }
    }
}
