using HarmonyLib;
using RimWorld;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(Designator_Slaughter), nameof(Designator_Slaughter.CanDesignateThing))]
    public static class Designator_Slaughter_CanDesignateThing_Patch
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
