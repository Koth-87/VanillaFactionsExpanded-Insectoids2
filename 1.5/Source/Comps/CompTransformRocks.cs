using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    public class CompProperties_TransformRocks : CompProperties_AOE
    {
        public ThingDef thingToSet;
        public CompProperties_TransformRocks()
        {
            this.compClass = typeof(CompTransformRocks);
        }
    }

    public class CompTransformRocks : CompAOE_Thing
    {
        public CompProperties_TransformRocks Props => base.props as CompProperties_TransformRocks;

        protected override bool TryGetThing(List<Thing> things, out Thing thing)
        {
            thing = things.OrderBy(x => x.Position.DistanceTo(parent.Position)).FirstOrDefault();
            return thing != null;
        }

        protected override List<Thing> GetThings()
        {
            var things = base.GetThings();
            compSpawner.canSpawn = things.Any() is false;
            return things;
        }

        protected override void DoEffect(Thing thing)
        {
            var pos = thing.Position;
            thing.DeSpawn();
            var rock = GenSpawn.Spawn(Props.thingToSet, pos, parent.Map);
            if (parent.Faction != null && rock.def.CanHaveFaction)
            {
                rock.SetFaction(parent.Faction);
            }
        }

        protected override bool ThingValidator(Thing thing)
        {
            return thing.def.IsNonResourceNaturalRock;
        }
    }
}
