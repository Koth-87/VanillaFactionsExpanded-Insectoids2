using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(WorldObjectsHolder), "Remove")]
    public static class WorldObjectsHolder_Remove_Patch
    {
        public static void Postfix(WorldObject o)
        {
            if (o is Settlement settlement && settlement.Faction?.def == VFEI_DefOf.VFEI2_Hive)
            {
                Find.World.renderer.SetDirty<WorldLayer_Insects>();
            }
        }
    }
}
