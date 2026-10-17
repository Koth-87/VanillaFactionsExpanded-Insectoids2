using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(Site), "ShouldRemoveMapNow")]
    public static class Site_ShouldRemoveMapNow_Patch
    {
        public static void Postfix(Site __instance, ref bool __result, ref bool alsoRemoveWorldObject)
        {
            if (__result && __instance.parts != null)
            {
                foreach (var part in __instance.parts)
                {
                    if (part.def.Worker is SitePartWorker_InsectHive)
                    {
                        if (__instance.Map.listerThings.AllThings.Any(x => x is Hive))
                        {
                            List<Quest> quests = Find.QuestManager.QuestsListForReading;
                            for (var j = 0; j < quests.Count; j++)
                            {
                                Quest quest = quests[j];
                                foreach (var worldTimeout in quest.PartsListForReading.OfType<QuestPart_WorldObjectTimeout>())
                                {
                                    if (worldTimeout.worldObject == __instance)
                                    {
                                        if (worldTimeout.State == QuestPartState.Disabled)
                                        {
                                            worldTimeout.Enable(new SignalArgs("wtfIshouldSentIt"));
                                        }
                                    }
                                }
                            }
                            __result = alsoRemoveWorldObject = false;
                            return;
                        }
                    }
                }
            }
        }
    }
}
