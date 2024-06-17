﻿using HarmonyLib;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(SettlementProximityGoodwillUtility), "GetConfirmationDescriptions")]
    public static class SettlementProximityGoodwillUtility_GetConfirmationDescriptions_Patch
    {
        public static IEnumerable<string> Postfix(IEnumerable<string> result, int tile)
        {
            foreach (var item in result)
            {
                yield return item;
            }
            if (tile.IsInfestedTile())
            {
                yield return "VFEI_SettleWarning".Translate(GameComponent_Insectoids.Instance.InfestationMtbDays(tile));
            }
        }
    }
}
