
using HarmonyLib;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(AreaManager), "AddStartingAreas")]
    public static class AreaManager_AddStartingAreas_Patch
    {
        public static void Postfix(AreaManager __instance)
        {
            __instance.areas.Add(new Area_Hive(__instance));
        }
    }
}