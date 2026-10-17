using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(WITab_Terrain), nameof(WITab_Terrain.ListMiscDetails))]
    public static class WITab_Terrain_ListMiscDetails_Patch
    {
        public static void Postfix(Listing_Standard listing, Tile ws, PlanetTile tile)
        {
            if (tile.IsInfestedTile())
            {
                listing.LabelDouble("VFEI_Infestation".Translate(), "VFEI_Present".Translate(), "VFEI_InfestationTooltip".Translate());
            }
        }
    }
}
