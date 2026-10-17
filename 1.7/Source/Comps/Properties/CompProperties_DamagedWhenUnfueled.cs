using Verse;

namespace VFEInsectoids
{
    public class CompProperties_DamagedWhenUnfueled : CompProperties
    {
        public int interval;
        public int damage;

        public CompProperties_DamagedWhenUnfueled()
        {
            this.compClass = typeof(CompDamagedWhenUnfueled);
        }
    }
}