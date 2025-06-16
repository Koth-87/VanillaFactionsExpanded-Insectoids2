using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(WorkGiver_RemoveBuilding), "HasJobOnThing")]
    public static class WorkGiver_RemoveBuilding_HasJobOnThing_Patch
    {
        public static void Postfix(Pawn pawn, Thing t, ref bool __result)
        {
            if (__result)
            {
                var extension = (t.def.entityDefToBuild ?? t.def).GetModExtension<InsectBuilding>();
                if (extension is null && pawn.IsColonyInsect())
                {
                    __result = false;
                }
            }
        }
    }
}
