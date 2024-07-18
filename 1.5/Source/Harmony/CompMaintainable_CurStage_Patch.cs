using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(CompMaintainable), "CurStage", MethodType.Getter)]
    public static class CompMaintainable_CurStage_Patch
    {
        public static bool Prefix(CompMaintainable __instance, ref MaintainableStage __result)
        {
            if (GenRadial.RadialDistinctThingsAround(__instance.parent.Position, __instance.parent.Map, 7.9f, true)
                .Any(x => x.def == VFEI_DefOf.VFEI2_Hivenode))
            {
                __result = MaintainableStage.Healthy;
                return false;
            }
            var compHive = __instance.parent.GetComp<CompHive>();
            if (compHive != null)
            {
                if (__instance.ticksSinceMaintain < compHive.MaintenanceDurationOverride(__instance.Props.ticksHealthy))
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
