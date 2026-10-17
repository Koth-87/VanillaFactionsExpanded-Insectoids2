using HarmonyLib;
using Verse;

namespace VFEInsectoids
{

    [HarmonyPatch(typeof(MapComponentUtility), "MapGenerated")]
    public static class MapComponentUtility_GenerateMap_Patch
    {
        public static void Postfix(Map map)
        {
            if (map != null && map.IsPocketMap is false && map.Tile.IsInfestedTile())
            {
                var mapGenDef = DefDatabase<InsectMapGenDef>.GetRandom();
                DeepProfiler.Start("Map gen");
                mapGenDef.DoMapGen(map);
                DeepProfiler.End();
            }
        }
    }
}
