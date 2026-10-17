using Verse;

namespace VFEInsectoids
{
    public class CompProperties_SwarmlingToCocoon : CompProperties
    {

        public int ticksBeforeBecomingCocoon;



        public CompProperties_SwarmlingToCocoon()
        {
            this.compClass = typeof(CompSwarmlingToCocoon);
        }



    }
}