
using HarmonyLib;
using KCSG;
using RimWorld;
using RimWorld.BaseGen;
using Verse;
using Verse.AI.Group;

namespace VFEInsectoids
{
    [HarmonyPatch("KCSG.SymbolResolver_Settlement", "AddHostilePawnGroup")]
    public static class SymbolResolver_Settlement_AddHostilePawnGroup_Patch
    {
        public static SimpleCurve scaleByWealth = new SimpleCurve
        {
            new CurvePoint(25000, 0.25f),
            new CurvePoint(50000, 0.5f),
            new CurvePoint(100000, 1f),
            new CurvePoint(200000, 2f),
            new CurvePoint(300000, 3f),
            new CurvePoint(400000, 4f),
            new CurvePoint(500000, 5f),
            new CurvePoint(1000000, 10f),
        };

        public static bool Prefix(Faction faction, Map map, ResolveParams parms, PawnGroupKindDef pawnGroup)
        {
            if (faction == Faction.OfInsects)
            {
                AddHostilePawnGroup(faction, map, parms, pawnGroup);
                return false;
            }
            return true;
        }

        private static void AddHostilePawnGroup(Faction faction, Map map, ResolveParams parms, PawnGroupKindDef pawnGroup)
        {
            Lord singlePawnLord = parms.singlePawnLord ?? LordMaker.MakeNewLord(faction, new LordJob_DefendBase(faction, parms.rect.CenterCell), map, null);
            TraverseParms tp = TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false);
            ResolveParams rp = parms;
            rp.rect = parms.rect;
            rp.faction = faction;
            rp.singlePawnLord = singlePawnLord;
            rp.pawnGroupKindDef = pawnGroup;
            rp.singlePawnSpawnCellExtraPredicate = parms.singlePawnSpawnCellExtraPredicate ?? ((IntVec3 x) => map.reachability.CanReachMapEdge(x, tp));
            if (rp.pawnGroupMakerParams == null && faction.def.pawnGroupMakers.Any(pgm => pgm.kindDef == PawnGroupKindDefOf.Settlement))
            {
                rp.pawnGroupMakerParams = new PawnGroupMakerParms
                {
                    tile = map.Tile,
                    faction = faction,
                    points = parms.settlementPawnGroupPoints ?? SymbolResolver_Settlement.DefaultPawnsPoints.RandomInRange,
                    inhabitants = true,
                    seed = parms.settlementPawnGroupSeed
                };
            }
            if (GenOption.settlementLayout != null)
            {
                rp.pawnGroupMakerParams.points *= GenOption.settlementLayout.defenseOptions.pawnGroupMultiplier;
                var scale = scaleByWealth.Evaluate(WealthUtility.PlayerWealth);
                rp.pawnGroupMakerParams.points *= scale;
            }

            BaseGen.symbolStack.Push("pawnGroup", rp, null);
        }
    }
}