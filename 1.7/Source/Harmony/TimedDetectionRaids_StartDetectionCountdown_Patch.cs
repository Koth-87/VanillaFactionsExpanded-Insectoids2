using HarmonyLib;
using RimWorld.Planet;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(TimedDetectionRaids), nameof(TimedDetectionRaids.StartDetectionCountdown))]
    public static class TimedDetectionRaids_StartDetectionCountdown_Patch
    {
        public static bool Prefix(TimedDetectionRaids __instance)
        {
            if (__instance.parent is Site site && site.parts != null)
            {
                foreach (var part in site.parts)
                {
                    if (part.def.Worker is SitePartWorker_InsectHive)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
