using RimWorld;
using Verse;

namespace VFEInsectoids
{
    public class StaticPod : Building
    {

        public override void TickRare()
        {
            base.TickRare();
            
            FleckMaker.Static(this.Position, this.Map, VFEI_DefOf.BlastEMP);
           
        }


    }

}