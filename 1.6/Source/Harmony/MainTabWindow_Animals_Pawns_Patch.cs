using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(MainTabWindow_Animals), "Pawns", MethodType.Getter)]
    public static class MainTabWindow_Animals_Pawns_Patch
    {
        public static IEnumerable<Pawn> Postfix(IEnumerable<Pawn> result)
        {
            foreach (var p in result)
            {
                if (p.IsColonyInsect() is false)
                {
                    yield return p;
                }
            }
        }
    }
    }

