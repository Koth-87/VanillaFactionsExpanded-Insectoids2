using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace VFEInsectoids
{
    public class InsectBuilding : DefModExtension
    {
        public bool nonInsectCanBuildIt;
    }

    [HarmonyPatch(typeof(GenConstruct), "CanConstruct", new Type[] { typeof(Thing), typeof(Pawn), typeof(bool), typeof(bool), typeof(JobDef) })]
    public static class GenConstruct_CanConstruct_Patch
    {
        public static void Postfix(ref bool __result, Thing t, Pawn p)
        {
            if (__result)
            {
                var extension = (t.def.entityDefToBuild ?? t.def).GetModExtension<InsectBuilding>();
                if (extension != null)
                {
                    if (extension.nonInsectCanBuildIt)
                    {
                        return;
                    }
                    else if (p.IsColonyInsect() is false
                        && (p.genes is null || p.genes.GenesListForReading.Any(x => x.def.defName == "VRE_Hiveglands" && x.Active) is false))
                    {
                        __result = false;
                    }
                }
                else if (p.IsColonyInsect())
                {
                    __result = false;
                }
            }
        }
    }
}
