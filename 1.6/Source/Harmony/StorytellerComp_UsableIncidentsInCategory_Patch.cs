
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(StorytellerComp), "UsableIncidentsInCategory", new Type[] { typeof(IncidentCategoryDef), 
        typeof(Func<IncidentDef, IncidentParms>) })]
    public static class StorytellerComp_UsableIncidentsInCategory_Patch
    {
        public static IEnumerable<IncidentDef> Postfix(IEnumerable<IncidentDef> __result, StorytellerComp __instance)
        {
            if (__instance is not StorytellerComp_ExclusiveIncidents && Find.Storyteller?.def == VFEI_DefOf.VFEI_EmpressEvil)
            {
                var comp = Find.Storyteller.storytellerComps.OfType<StorytellerComp_ExclusiveIncidents>().FirstOrDefault();
                foreach (var r in __result)
                {
                    if (comp.Props.incidents.Contains(r) is false)
                    {
                        yield return r;
                    }
                }
            }
            else
            {
                foreach (var r in __result)
                {
                    yield return r;
                }
            }
        }
    }
}