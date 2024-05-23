using RimWorld;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    public class CompProperties_OnDestroyWall : CompProperties
    {
        public IntRange filthSpawnAmount;
        public float radius;
        public ThingDef filth;
        public bool chaining;
        public CompProperties_OnDestroyWall()
        {
            this.compClass = typeof(CompOnDestroyWall);
        }
    }

    public class CompOnDestroyWall : ThingComp
    {
        public static bool DoNotChain;
        public CompProperties_OnDestroyWall Props => base.props as CompProperties_OnDestroyWall;
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            LongEventHandler.toExecuteWhenFinished.Add(delegate
            {
                FilthMaker.TryMakeFilth(parent.Position, previousMap, Props.filth);
                var filthSpawning = Props.filthSpawnAmount.RandomInRange;
                for (var i = 0; i < filthSpawning; i++)
                {
                    var randomCell = GenRadial.RadialCellsAround(parent.Position, Props.radius, true).RandomElement();
                    if (randomCell.InBounds(previousMap) && GenSight.LineOfSight(randomCell, parent.Position, previousMap))
                    {
                        FilthMaker.TryMakeFilth(randomCell, previousMap, Props.filth);
                    }
                }
            });

            if (Props.chaining)
            {
                if (DoNotChain)
                {
                    return;
                }
                DoNotChain = true;
                var info = new TargetInfo(parent.Position, previousMap);
                foreach (var thing in GenAdj.CellsAdjacent8Way(info).SelectMany(x => x.GetThingList(previousMap)
                    .Where(x => x != parent && x.def == parent.def)).ToList())
                {
                    thing.Destroy(mode);
                }
                DoNotChain = false;
            }
        }
    }
}
