using HarmonyLib;
using RimWorld;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(CompMaintainable), "CurStage", MethodType.Getter)]
    public static class CompMaintainable_CurStage_Patch
    {
        public static bool Prefix(CompMaintainable __instance, ref MaintainableStage __result)
        {
            var compHive = __instance.parent.GetComp<CompHive>();
            if (compHive != null)
            {
                if (__instance.ticksSinceMaintain < 
                    compHive.MaintenanceDurationOverride(__instance.Props.ticksHealthy))
                {
                    __result = MaintainableStage.Healthy;
                }
                else if (__instance.ticksSinceMaintain < compHive.MaintenanceDurationOverride(__instance.Props.ticksHealthy + __instance.Props.ticksNeedsMaintenance))
                {
                    __result = MaintainableStage.NeedsMaintenance;
                }
                else
                {
                    __result = MaintainableStage.Damaging;
                }
                return false;
            }
            return true;
        }
    }
}
