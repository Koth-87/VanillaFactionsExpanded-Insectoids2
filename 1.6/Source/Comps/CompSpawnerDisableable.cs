using RimWorld;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    public class CompSpawnerDisableable : CompSpawner
    {
        public bool canSpawn;

        public override void CompTick()
        {
            if (canSpawn)
            {
                base.CompTick();
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref canSpawn, "canSpawn");
        }
    }
}
