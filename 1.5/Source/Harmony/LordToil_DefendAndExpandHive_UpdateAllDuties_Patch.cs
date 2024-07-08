using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(LordToil_DefendAndExpandHive), "UpdateAllDuties")]
    public static class LordToil_DefendAndExpandHive_UpdateAllDuties_Patch
    {
        public static bool Prefix(LordToil_DefendAndExpandHive __instance)
        {
            if (__instance.Map.IsInfested())
            {
                __instance.FilterOutUnspawnedHives();
                for (int i = 0; i < __instance.lord.ownedPawns.Count; i++)
                {
                    Hive hiveFor = __instance.GetHiveFor(__instance.lord.ownedPawns[i]);
                    if (hiveFor != null)
                    {
                        PawnDuty duty = new PawnDuty(VFEI_DefOf.VFEI_DefendAndExpandHive,
                            hiveFor, __instance.distToHiveToAttack);
                        __instance.lord.ownedPawns[i].mindState.duty = duty;
                    }
                    else
                    {
                        PawnDuty duty = new PawnDuty(VFEI_DefOf.VFEI_DefendAndExpandHive);
                        __instance.lord.ownedPawns[i].mindState.duty = duty;
                    }
                }
                return false;
            }
            return true;
        }
    }
}
