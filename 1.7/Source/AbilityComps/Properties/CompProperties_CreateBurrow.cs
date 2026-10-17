
using RimWorld;
using Verse;
namespace VFEInsectoids
{

  

    public class CompProperties_CreateBurrow : CompProperties_AbilityEffect
    {
    
        public ThingDef building;
        public ThingDef tunnel;
        public BurrowSize size;

        public CompProperties_CreateBurrow()
        {
            compClass = typeof(CompCreateBurrow);
        }
    }
}