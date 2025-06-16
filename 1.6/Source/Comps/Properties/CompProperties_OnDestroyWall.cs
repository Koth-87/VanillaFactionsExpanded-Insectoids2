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
}