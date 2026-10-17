using HarmonyLib;
using Verse;

namespace VFEInsectoids
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            new Harmony("VFEInsectoidsMod").PatchAll();
        }
    }
}
