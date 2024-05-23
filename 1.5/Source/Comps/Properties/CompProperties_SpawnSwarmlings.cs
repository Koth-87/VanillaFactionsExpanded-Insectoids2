using Verse;

namespace VFEInsectoids
{
    internal class CompProperties_SpawnSwarmlings : CompProperties
    {

        public int ticksBetweenSpawn;

        public int numberToSpawn;


        public CompProperties_SpawnSwarmlings()
        {
            this.compClass = typeof(CompSpawnSwarmlings);
        }



    }
}