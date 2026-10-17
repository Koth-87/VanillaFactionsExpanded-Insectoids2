using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(Designator_ReleaseAnimalToWild), nameof(Designator_ReleaseAnimalToWild.CanDesignateThing))]
    public static class Designator_ReleaseAnimalToWild_CanDesignateThing_Patch
    {
        public static bool Prefix(Thing t)
        {
            if (t is Pawn pawn && pawn.IsColonyInsect())
            {
                return false;
            }
            return true;
        }
    }
}
