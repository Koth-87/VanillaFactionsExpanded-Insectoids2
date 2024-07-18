using Verse;

namespace VFEInsectoids
{
    public class CompProperties_SpawnSwarmlings : CompProperties
    {

        public int ticksBetweenSpawn;

        public IntRange numberToSpawn;


        public CompProperties_SpawnSwarmlings()
        {
            this.compClass = typeof(CompSpawnSwarmlings);
        }



    }
}