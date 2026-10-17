using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    public class CompMaintenableHive : CompMaintainable
    {
        private bool? maintainedByHivenode;
        private int ticksSinceLastCheck;
        public bool MaintainedByHivenode
        {
            get
            {
                if (maintainedByHivenode is null || Find.TickManager.TicksGame > ticksSinceLastCheck + 60)
                {
                    CheckProtectedByHiveNode();
                }
                return maintainedByHivenode.Value;
            }
        }

        public CompHive compHive;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            compHive = parent.GetComp<CompHive>();
        }

        private void CheckProtectedByHiveNode()
        {
            maintainedByHivenode = GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, 7.9f, true)
                    .Any(x => x.def == VFEI_DefOf.VFEI2_Hivenode);
            ticksSinceLastCheck = Find.TickManager.TicksGame;
        }

        public override string CompInspectStringExtra()
        {
            if (MaintainedByHivenode)
            {
                return "VFEI_MaintainedByHivenode".Translate();
            }
            else
            {
                return CurStage switch
                {
                    MaintainableStage.NeedsMaintenance => "VFEI_DueForMaintenance".Translate((TicksNeedsMaintenance - ticksSinceMaintain).ToStringTicksToPeriod()),
                    MaintainableStage.Damaging => "DeterioratingDueToLackOfMaintenance".Translate(),
                    MaintainableStage.Healthy => "VFEI_NeedsToBeMaintainedIn".Translate((TicksHealthy - ticksSinceMaintain).ToStringTicksToPeriod()),
                    _ => null,
                };
            }
        }

        public int TicksHealthy => compHive != null ? (int)compHive.MaintenanceDurationOverride(Props.ticksHealthy) : Props.ticksHealthy;
        public int TicksNeedsMaintenance => compHive != null ? (int)compHive.MaintenanceDurationOverride(Props.ticksHealthy + Props.ticksNeedsMaintenance) : Props.ticksHealthy + Props.ticksNeedsMaintenance;

        [HarmonyPatch(typeof(CompMaintainable), "CheckTakeDamage")]
        public static class CompMaintainable_CheckTakeDamage_Patch
        {
            public static void Postfix(CompMaintainable __instance)
            {
                if (__instance is CompMaintenableHive compMaintenableHive
                    && __instance.CurStage == MaintainableStage.Damaging)
                {
                    VFEI_DefOf.VFEI_MaintenanceDamage.Spawn(__instance.parent, __instance.parent.Map);
                }
            }
        }

        [HarmonyPatch(typeof(CompMaintainable), "CurStage", MethodType.Getter)]
        public static class CompMaintainable_CurStage_Patch
        {
            public static bool Prefix(CompMaintainable __instance, ref MaintainableStage __result)
            {
                if (__instance is CompMaintenableHive compMaintenableHive)
                {
                    if (compMaintenableHive.MaintainedByHivenode)
                    {
                        __result = MaintainableStage.Healthy;
                        return false;
                    }
                    if (__instance.ticksSinceMaintain < compMaintenableHive.TicksHealthy)
                    {
                        __result = MaintainableStage.Healthy;
                    }
                    else if (__instance.ticksSinceMaintain < compMaintenableHive.TicksNeedsMaintenance)
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
}
