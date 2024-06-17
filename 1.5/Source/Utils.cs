using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    public static class Utils
    {
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static bool IsInfestedTile(this int tile)
        {
            if (tile <= 0) return false;
            foreach (var insectData in GameComponent_Insectoids.Instance.insectTiles)
            {
                if (insectData.Value.tiles.Contains(tile))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsInfested(this Map map)
        {
            return map.Tile.IsInfestedTile();
        }
    }
}
