using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(PawnGroupMakerUtility), "GeneratePawns")]
    public static class PawnGroupMakerUtility_GeneratePawns
    {
        public static IEnumerable<Pawn> Postfix(IEnumerable<Pawn> __result,
            PawnGroupMakerParms parms, bool warnOnZeroResults)
        {
            if (parms.faction == Faction.OfInsects)
            {
                foreach (var insect in PawnGroupKindWorker_GeneratePawns_Patch.GenerateInsectPawns(parms))
                {
                    yield return insect;
                }
            }
            else
            {
                foreach (var r in __result)
                {
                    yield return r;
                }
            }
        } 
    }
    }

