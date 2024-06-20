using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(TunnelHiveSpawner), "Spawn")]
    public static class TunnelHiveSpawner_Spawn_Patch
    {
        public static void Prefix(TunnelHiveSpawner __instance, Map map)
        {
            if (map.IsInfested() && HiveUtility.TotalSpawnedHivesCount(map) >= 30)
            {
                __instance.spawnHive = false;
            }
        }
    }
}
