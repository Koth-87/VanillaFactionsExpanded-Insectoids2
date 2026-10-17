using HarmonyLib;
using RimWorld;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(Faction), "HasGoodwill", MethodType.Getter)]
    public static class Faction_HasGoodwill_Patch
    {
        public static void Postfix(Faction __instance, ref bool __result)
        {
            if (__result && __instance == Faction.OfInsects)
            {
                __result = false;
            }
        }
    }
}
