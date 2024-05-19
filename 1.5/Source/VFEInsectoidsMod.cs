using HarmonyLib;
using Verse;

namespace VFEInsectoids
{
    public class VFEInsectoidsMod : Mod
    {
        public VFEInsectoidsMod(ModContentPack pack) : base(pack)
        {
            new Harmony("VFEInsectoidsMod").PatchAll();
        }
    }
}
