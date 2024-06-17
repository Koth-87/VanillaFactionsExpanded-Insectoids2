using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(WildAnimalSpawner), "CommonalityOfAnimalNow")]
    public static class WildAnimalSpawner_CommonalityOfAnimalNow_Patch
    { 
        public static void Postfix(ref float __result, PawnKindDef def, WildAnimalSpawner __instance)
        {
            if (__instance.map.IsInfestedTile())
            {
                var record = VFEI_DefOf.VFEI_Sorne.insects.FirstOrDefault(x => x.kind == def);
                if (record != null)
                {
                    __result = record.selectionWeight;
                }
            }
        }
    }
}
