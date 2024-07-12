
using RimWorld;
using Verse;
namespace VFEInsectoids
{
    public class DeathActionProperties_VanishInsect : DeathActionProperties
    {
        public FleckDef fleck;

        public ThingDef filth;

        public IntRange filthCountRange = IntRange.one;

      

        public DeathActionProperties_VanishInsect()
        {
            workerClass = typeof(DeathActionWorker_VanishInsect);
        }
    }
}
