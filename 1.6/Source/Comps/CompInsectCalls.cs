﻿using Verse;
using Verse.Sound;
namespace VFEInsectoids
{
    public class CompInsectCalls : ThingComp
    {
        public int tickCounter = 0;
        public int nextTick;

        public CompProperties_InsectCalls Props => (CompProperties_InsectCalls)props;

        public override void PostExposeData()
        {
            base.PostExposeData();


        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);

            nextTick = Props.interval.RandomInRange * 2000;
        }

        public override void CompTick()
        {
            if (tickCounter>nextTick)
            {
                Props.soundDefs.RandomElement().PlayOneShot(new TargetInfo(this.parent.Position, this.parent.Map));
                tickCounter = 0;
                nextTick = Props.interval.RandomInRange * 2000;
            }
            tickCounter++;
        }



    }
}