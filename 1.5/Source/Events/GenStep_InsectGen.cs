using KCSG;
using RimWorld.BaseGen;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using Verse;

namespace VFEInsectoids
{
    public class GenStep_InsectGen : GenStep
    {
        public override int SeedPart => 916595355;
        public SettlementLayoutDef layoutDef;

        public override void Generate(Map map, GenStepParams parms)
        {
            BaseGen.globalSettings.map = map;
            var rect = CellRect.WholeMap(map);
            var site = map.Parent as Site;
            var rp = new ResolveParams
            {
                faction = map.ParentFaction,
                rect = rect,
                settlementPawnGroupPoints = site.ActualThreatPoints,
            };
            GenOption.settlementLayout = layoutDef;
            GenOption.GetAllMineableIn(rect, map);
            SettlementGenUtils.Generate(rp, map, layoutDef);
            // Flood refog
            if (map.mapPawns.FreeColonistsSpawned.Count > 0)
            {
                FloodFillerFog.DebugRefogMap(map);
            }
        }
    }
}
