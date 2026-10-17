
using RimWorld;
using Verse;
namespace VFEInsectoids
{
    public class CompProperties_AbilityAcidSpew : CompProperties_AbilityEffect
    {
        public float range;

        public float lineWidthEnd;

        public ThingDef filthDef;

        public int damAmount = -1;

        public EffecterDef effecterDef;

        public bool canHitFilledCells;

        public CompProperties_AbilityAcidSpew()
        {
            compClass = typeof(CompAbilityAcidSpew);
        }
    }
}