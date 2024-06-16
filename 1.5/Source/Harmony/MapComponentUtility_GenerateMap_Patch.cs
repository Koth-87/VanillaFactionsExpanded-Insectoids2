using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(MapComponentUtility), "MapGenerated")]
    public static class MapComponentUtility_GenerateMap_Patch
    {
        public static void Postfix(Map map)
        {
            if (map != null && map.IsPocketMap is false && GameComponent_Insectoids.Instance.IsInfested(map.Tile))
            {
                var mapGenDef = DefDatabase<InsectMapGenDef>.GetRandom();
                mapGenDef.DoMapGen(map);
            }
        }
    }
}
