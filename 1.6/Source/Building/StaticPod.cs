using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

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