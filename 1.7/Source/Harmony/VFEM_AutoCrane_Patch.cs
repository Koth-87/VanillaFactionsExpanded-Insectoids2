using HarmonyLib;
using System.Reflection;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch]
    public static class VFEM_AutoCrane_Patch
    {
        public static MethodBase targetMethod;
        public static bool Prepare() => ModsConfig.IsActive("OskarPotocki.VFE.Mechanoid");
        public static MethodBase TargetMethod() => AccessTools.Method("VFEMech.Building_Autocrane:BaseValidator");
        public static void Postfix(Thing x, ref bool __result)
        {
            if (__result)
            {
                var extension = (x.def.entityDefToBuild ?? x.def).GetModExtension<InsectBuilding>();
                if (extension != null && extension.nonInsectCanBuildIt is false)
                {
                    __result = false;
                }
            }
        }
    }
}
