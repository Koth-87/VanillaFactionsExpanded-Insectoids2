using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Linq;
using System.Reflection;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch]
    public static class TileFinder_RandomSettlementTileFor_Patch
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return typeof(TileFinder).GetNestedTypes(AccessTools.all)
                .SelectMany(x => x.GetMethods(AccessTools.all))
                .FirstOrDefault(x => x.Name.Contains("<RandomSettlementTileFor>") && x.ReturnType == typeof(float));
        }

        public static bool Prefix(ref float __result, int x, Faction ___faction)
        {
            if (___faction?.def == FactionDefOf.Insect)
            {
                Tile tile = Find.WorldGrid[x];
                if (!tile.biome.canBuildBase || !tile.biome.implemented || tile.hilliness == Hilliness.Impassable)
                {
                    __result = 0f;
                }
                else
                {
                    __result = tile.biome.settlementSelectionWeight;
                }
                return false;
            }
            return true;
        }
    }
}
