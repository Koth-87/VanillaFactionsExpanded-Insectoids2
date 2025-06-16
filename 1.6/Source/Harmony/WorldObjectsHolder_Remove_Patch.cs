using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(WorldObjectsHolder), "Remove")]
    public static class WorldObjectsHolder_Remove_Patch
    {
        public static void Postfix(WorldObject o)
        {
            if (o is Settlement settlement && settlement.Faction == Faction.OfInsects)
            {
                Find.World.renderer.SetDirty<WorldLayer_Insects>();
            }
        }
    }
}
