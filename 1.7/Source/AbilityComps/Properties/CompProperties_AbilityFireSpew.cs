using RimWorld;
using Verse;

namespace VFEInsectoids
{
    public class CompProperties_AbilityFireSpew : CompProperties_AbilityEffect
    {
        public float range;

        public float lineWidthEnd;

        public ThingDef filthDef;

        public float filthChance;

        public int damAmount = -1;

        public EffecterDef effecterDef;

        public bool canHitFilledCells;

        public CompProperties_AbilityFireSpew()
        {
            compClass = typeof(CompAbilityFireSpew);
        }
    }
}
