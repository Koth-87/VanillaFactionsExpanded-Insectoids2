using RimWorld;
using Verse;

namespace VFEInsectoids
{
    public class HellpodIncoming : DropPodIncoming
    {
        public override void Tick()
        {
            base.Tick();
            if (this.IsHashIntervalTick(1))
            {
                FleckMaker.ThrowFireGlow(DrawPos, Map, 1);
                FleckMaker.ThrowSmoke(DrawPos, Map, 1);
            }
        }
    }
}
