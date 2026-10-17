using HarmonyLib;
using RimWorld.Planet;
using UnityEngine;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(WorldObject), "ExpandingIconColor", MethodType.Getter)]
    public static class WorldObject_ExpandingIconColor_Patch
    {
        public static void Postfix(WorldObject __instance, ref Color __result)
        {
            if (__instance is Site site && site.parts != null)
            {
                foreach (var part in site.parts)
                {
                    if (part.def.Worker is SitePartWorker_InsectHive)
                    {
                        __result = __instance.Faction.Color;
                        return;
                    }
                }
            }
        }
    }
}
