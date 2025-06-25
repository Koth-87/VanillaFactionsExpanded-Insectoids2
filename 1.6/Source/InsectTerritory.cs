using System.Collections.Generic;
using RimWorld.Planet;
using Verse;

namespace VFEInsectoids
{

    public class InsectTerritory : IExposable
    {
        public HashSet<PlanetTile> tiles = new HashSet<PlanetTile>();

        public void ExposeData()
        {
            Scribe_Collections.Look(ref tiles, "tiles", LookMode.Value);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                tiles ??= new HashSet<PlanetTile>();
            }
        }
    }
}
