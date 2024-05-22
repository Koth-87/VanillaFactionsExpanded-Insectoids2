using System.Collections.Generic;
using Verse;

namespace VFEInsectoids
{
    public class InsectTerritory : IExposable
    {
        public HashSet<int> tiles = new HashSet<int>();

        public void ExposeData()
        {
            Scribe_Collections.Look(ref tiles, "tiles", LookMode.Value);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                tiles ??= new HashSet<int>();
            }
        }
    }
}
