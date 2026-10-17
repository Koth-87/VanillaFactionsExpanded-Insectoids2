
using RimWorld;

namespace VFEInsectoids
{
    public class CompProperties_FueledSpawner : CompProperties_Spawner
    {


        public bool requiresFuel;



        public CompProperties_FueledSpawner()
        {
            compClass = typeof(CompFueledSpawner);
        }
    }
}
