using HarmonyLib;
using RimWorld;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(TunnelHiveSpawner), "Spawn")]
    public static class TunnelHiveSpawner_Spawn_Patch
    {
        public static void Prefix()
        {

        }
    }
}
