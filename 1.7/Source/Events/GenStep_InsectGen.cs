using KCSG;
using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using Verse;
using Verse.AI.Group;

namespace VFEInsectoids
{
    public class GenStep_InsectGen : GenStep
    {
        public override int SeedPart => 916595355;
        public SettlementLayoutDef layoutDef;

        public override void Generate(Map map, GenStepParams parms)
        {
            var site = map.Parent as Site;
            Generate(map, layoutDef, site.ActualThreatPoints);
        }

        public static void Generate(Map map, SettlementLayoutDef layoutDef, float enemyPoints)
        {
            var oldFaction = map.Parent.Faction;
            if (oldFaction != Faction.OfInsects)
            {
                map.Parent.SetFaction(Faction.OfInsects);
            }
            BaseGen.globalSettings.map = map;
            var rect = CellRect.WholeMap(map);
            var rp = new ResolveParams
            {
                faction = map.ParentFaction,
                rect = rect,
            };
            GenOption.settlementLayout = layoutDef;
            GenOption.GetAllMineableIn(rect, map);
            SettlementGenUtils.Generate(rp, map, layoutDef);
            AddHostilePawnGroup(Faction.OfInsects, map, rp, PawnGroupKindDefOf.Combat, enemyPoints, layoutDef);
            if (map.mapPawns.FreeColonistsSpawned.Count > 0)
            {
                FloodFillerFog.DebugRefogMap(map);
            }
            if (oldFaction != Faction.OfInsects)
            {
                map.Parent.SetFaction(oldFaction);
            }
        }

        public static void AddHostilePawnGroup(Faction faction, Map map, ResolveParams parms, 
            PawnGroupKindDef pawnGroup, float points, SettlementLayoutDef layoutDef)
        {
            Lord singlePawnLord = LordMaker.MakeNewLord(faction, new LordJob_DefendAndExpandHive
            {
                aggressive = true
            }, map, null);
            TraverseParms tp = TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false);
            ResolveParams rp = parms;
            rp.rect = CellRect.CenteredOn(rp.rect.CenterCell, layoutDef.centerBuildings.centerSize.x, layoutDef.centerBuildings.centerSize.z); ;
            rp.faction = faction;
            rp.singlePawnLord = singlePawnLord;
            rp.pawnGroupKindDef = pawnGroup;
            rp.singlePawnSpawnCellExtraPredicate = parms.singlePawnSpawnCellExtraPredicate ?? ((IntVec3 x) => map.reachability.CanReachMapEdge(x, tp));
            rp.pawnGroupMakerParams = new PawnGroupMakerParms
            {
                tile = map.Tile,
                faction = faction,
                points = points,
                inhabitants = true,
                seed = parms.settlementPawnGroupSeed
            };
            BaseGen.symbolStack.Push("pawnGroup", rp, null);

        }
    }
}
