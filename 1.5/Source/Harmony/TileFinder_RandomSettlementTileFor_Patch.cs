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

        public static bool Prefix(ref float __result, int x)
        {
            if (RandomSettlementTileFor_Patch.factionToCheck?.def == FactionDefOf.Insect)
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

        [HarmonyPatch(typeof(TileFinder), nameof(TileFinder.RandomSettlementTileFor))]
        public static class RandomSettlementTileFor_Patch
        {
            public static Faction factionToCheck;
            public static void Prefix(Faction faction)
            {
                factionToCheck = faction;
            }

            public static void Postfix()
            {
                factionToCheck = null;
            }
        }
    }
}
