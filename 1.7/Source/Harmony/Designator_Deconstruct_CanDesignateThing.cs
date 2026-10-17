using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(Designator_Deconstruct), nameof(Designator_Deconstruct.CanDesignateThing))]
    public static class VFEInsectoids_Designator_Deconstruct_CanDesignateThing_Patch
    {
        public static void Postfix(Thing t, ref AcceptanceReport __result)
        {
            if (__result.Accepted && t.def == VFEI_DefOf.VFEI2_InfestedShipChunk)
            {
                CompInsectSpawner compInsectSpawner = t.TryGetComp<CompInsectSpawner>();
                if (compInsectSpawner != null && compInsectSpawner.nextPawnSpawnTick != -1)
                {
                    __result = false;
                }
            }
        }
    }
}
