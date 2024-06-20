using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(IncidentWorker_Infestation), "CanFireNowSub")]
    public static class IncidentWorker_Infestation_CanFireNowSub_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var totalSpawnedHivesCountInfo = AccessTools.Method(typeof(HiveUtility), "TotalSpawnedHivesCount");
            foreach (var codeInstruction in codeInstructions)
            {
                yield return codeInstruction;
                if (codeInstruction.Calls(totalSpawnedHivesCountInfo))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(IncidentWorker_Infestation_CanFireNowSub_Patch), "TryOverrideHiveCount"));
                }
            }
        }

        public static int TryOverrideHiveCount(int count, Map map)
        {
            if (map.IsInfested())
            {
                return 0;
            }
            return count;
        }
    }
}
