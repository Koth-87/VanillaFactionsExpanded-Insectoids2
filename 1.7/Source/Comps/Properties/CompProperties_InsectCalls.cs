using Verse;
using System.Collections.Generic;

namespace VFEInsectoids
{
    public class CompProperties_InsectCalls : CompProperties
    {
        public IntRange interval;
        public List<SoundDef> soundDefs;

        public CompProperties_InsectCalls()
        {
            this.compClass = typeof(CompInsectCalls);
        }
    }
}