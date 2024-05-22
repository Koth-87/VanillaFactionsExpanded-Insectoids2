using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(Page_CreateWorldParams), "DoWindowContents")]
    public static class Page_CreateWorldParams_DoWindowContents_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var endGroup = AccessTools.Method(typeof(Widgets), "EndGroup");
            foreach (var instruction in codeInstructions)
            {
                if (instruction.Calls(endGroup))
                {
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 7).MoveLabelsFrom(instruction);
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 8);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(Page_CreateWorldParams_DoWindowContents_Patch),
                        nameof(AddInsectTerritoryScaleSlider)));
                }
                yield return instruction;
            }
        }

        public static void AddInsectTerritoryScaleSlider(ref float num2, float width)
        {
            num2 += 40f;
            Widgets.Label(new Rect(0f, num2, 200f, 30f), "VFEI_InsectTerritoryScale".Translate());
            Rect rect = new Rect(200f, num2, width, 30f);
            GameComponent_Insectoids.Instance.insectTerritoryScale = Widgets.HorizontalSlider(rect, 
                GameComponent_Insectoids.Instance.insectTerritoryScale, 0f, 2f, middleAlignment: true,
                GameComponent_Insectoids.Instance.insectTerritoryScale.ToStringPercent(), null, null, 0.05f);
        }
    }
}
