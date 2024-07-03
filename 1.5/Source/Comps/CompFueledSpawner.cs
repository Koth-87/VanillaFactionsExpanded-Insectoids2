
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace VFEInsectoids
{
    public class CompFueledSpawner : CompSpawner
    {
       

        public new CompProperties_FueledSpawner PropsSpawner => (CompProperties_FueledSpawner)props;

        private bool Fueled => parent.GetComp<CompRefuelable>()?.HasFuel ?? false;

        public override void CompTick()
        {
            TickInterval(1);
        }

        public override void CompTickRare()
        {
            TickInterval(250);
        }

        private new void TickInterval(int interval)
        {
            if (!parent.Spawned)
            {
                return;
            }
            CompCanBeDormant comp = parent.GetComp<CompCanBeDormant>();
            if (comp != null)
            {
                if (!comp.Awake)
                {
                    return;
                }
            }
            else if (parent.Position.Fogged(parent.Map))
            {
                return;
            }
            if (!PropsSpawner.requiresFuel || Fueled)
            {
                ticksUntilSpawn -= interval;
                CheckShouldSpawn();
            }
        }           

        public override string CompInspectStringExtra()
        {
            if (PropsSpawner.writeTimeLeftToSpawn && (!PropsSpawner.requiresPower || PowerOn) && (!PropsSpawner.requiresFuel || Fueled))
            {
                return "NextSpawnedItemIn".Translate(GenLabel.ThingLabel(PropsSpawner.thingToSpawn, null, PropsSpawner.spawnCount)).Resolve() + ": " + ticksUntilSpawn.ToStringTicksToPeriod().Colorize(ColoredText.DateTimeColor);
            }
            return null;
        }
    }
}