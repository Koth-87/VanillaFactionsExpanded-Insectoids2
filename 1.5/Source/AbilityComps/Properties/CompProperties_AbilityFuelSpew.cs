
using RimWorld;
using Verse;
namespace VFEInsectoids
{
    public class CompProperties_AbilityFuelSpew : CompProperties_AbilityEffect
    {
        public float range;

        public float lineWidthEnd;

        public ThingDef filthDef;

        public int damAmount = -1;

 

        public bool canHitFilledCells;

        public CompProperties_AbilityFuelSpew()
        {
            compClass = typeof(CompAbilityFuelSpew);
        }
    }
}