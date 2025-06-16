using RimWorld;
using System.Linq;
using Verse;
using System.Collections.Generic;
using static VFEInsectoids.Cocoon;

namespace VFEInsectoids
{
    public class CompProperties_InsectBurrow : CompProperties
    {

        public List<WeightedAnimals> insectoidsToSpawn;
        public int spawningStartingTimer;
        public float spawningDelayMultiplier;
        public int maxWavesToSpawn;

        public CompProperties_InsectBurrow()
        {
            this.compClass = typeof(CompInsectBurrow);
        }
    }
}