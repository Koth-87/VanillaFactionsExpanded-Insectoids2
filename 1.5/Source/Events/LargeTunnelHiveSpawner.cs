using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace VFEInsectoids
{
    public class LargeTunnelHiveSpawner : TunnelHiveSpawner
    {
        public List<ThingDef> thingsToSpawn = new List<ThingDef>();
        public List<PawnKindDef> pawnsToSpawn = new List<PawnKindDef>();
        public List<LargeTunnelHiveSpawner> otherHives = new List<LargeTunnelHiveSpawner>();
        public override void Spawn(Map map, IntVec3 loc)
        {
            base.Spawn(map, loc);
            foreach (var def in thingsToSpawn)
            {
                var pos = CellFinder.RandomClosewalkCellNear(loc, map, 6,
                    (IntVec3 c) => c.GetThingList(map).Any(x => x is Hive || x is TunnelHiveSpawner) is false
                    && otherHives.Any(x => x.OccupiedRect().ExpandedBy(2).Contains(c)) is false
                    && GenSpawn.WouldWipeAnythingWith(c, def.defaultPlacingRot, def, map, (Thing x) => true) is false);
                var thing = ThingMaker.MakeThing(def);
                GenPlace.TryPlaceThing(thing, pos, map, ThingPlaceMode.Near);
            }
            List<Pawn> list = new List<Pawn>();
            foreach (var result in pawnsToSpawn)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(result, Faction.OfInsects);
                GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(loc, map, 2), map);
                list.Add(pawn);
                if (ModsConfig.BiotechActive)
                {
                    PollutionUtility.Notify_TunnelHiveSpawnedInsect(pawn);
                }
            }
            if (list.Any())
            {
                LordMaker.MakeNewLord(Faction.OfInsects, new LordJob_AssaultColony(Faction.OfInsects, canKidnap: true, canTimeoutOrFlee: false), map, list);
                Log.Message("Spawned: " + list.Select(x => x.def.label).ToStringSafeEnumerable());
            }
            var thumpers = loc.GetThingList(map).Where(x => x.TryGetComp<CompThumper>() != null).ToList();
            thumpers.ForEach(x => x.Destroy(DestroyMode.KillFinalizeLeavingsOnly));
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref thingsToSpawn, "thingsToSpawn", LookMode.Def);
            Scribe_Collections.Look(ref pawnsToSpawn, "pawnsToSpawn", LookMode.Def);
            Scribe_Collections.Look(ref otherHives, "otherHives", LookMode.Reference);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                thingsToSpawn ??= new List<ThingDef>();
                pawnsToSpawn ??= new List<PawnKindDef>();
                otherHives ??= new List<LargeTunnelHiveSpawner>();
            }
        }
    }
}
