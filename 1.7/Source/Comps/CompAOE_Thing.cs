using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    public abstract class CompAOE_Thing : CompAOE
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
                    var things = GetThings();
                    if (TryGetThing(things, out var thing))
                    {
                        DoEffect(thing);
                    }
                    nextTickEffect = NextTickEffect;
                }
            }
        }

        protected virtual bool TryGetThing(List<Thing> things, out Thing thing)
        {
            return things.TryRandomElement(out thing);
        }

        protected virtual List<Thing> GetThings()
        {
            return GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, Props.radius, true)
                .Where(thing => ThingValidator(thing)).ToList();
        }

        protected abstract void DoEffect(Thing thing);

        protected abstract bool ThingValidator(Thing thing);
    }
}
