using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(WorldObjectsHolder), "Add")]
    public static class WorldObjectsHolder_Add_Patch
    {
        public static void Postfix(WorldObject o)
        {
            if (o is Settlement settlement && settlement.Faction?.def == VFEI_DefOf.VFEI2_Hive)
            {
                GameComponent_Insectoids.Instance.AddInsectHive(settlement);
                Find.World.renderer.SetDirty<WorldLayer_Insects>();
            }
        }
    }
}
