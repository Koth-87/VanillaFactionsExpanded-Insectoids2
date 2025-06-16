using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(GenConstruct), "CanPlaceBlueprintAt_NewTemp")]
    public static class GenConstruct_CanPlaceBlueprintAt_NewTemp_Patch
    {
        public static TaggedString GetAffordanceString(TaggedString originalString, BuildableDef entDef, ThingDef stuffDef)
        {
            var affordance = ThingUtility.GetTerrainAffordanceNeed(entDef, stuffDef);
            if (affordance != null && (affordance == VFEI_DefOf.VFEI2_CreepAffordance || affordance == VFEI_DefOf.VFEI2_JellyFloorAffordance))
            {
                return "VFEI_RequiresFloor".Translate(entDef, affordance.label);
            }
            return originalString;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var helperMethod = AccessTools.Method(typeof(GenConstruct_CanPlaceBlueprintAt_NewTemp_Patch), 
                nameof(GetAffordanceString));
            var translateMethod = AccessTools.Method(typeof(TranslatorFormattedStringExtensions), "Translate", 
                new Type[] { typeof(string), typeof(NamedArgument), typeof(NamedArgument) });
            var originalString = "TerrainCannotSupport_TerrainAffordance";

            bool foundTranslate = false;
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                yield return code;
                if (!foundTranslate && code.Calls(translateMethod) && codes[i - 7].OperandIs(originalString))
                {
                    foundTranslate = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_S, 7);
                    yield return new CodeInstruction(OpCodes.Call, helperMethod);
                }
            }
        }
    }
}
