using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    public class LargeTunnelHiveSpawner : TunnelHiveSpawner
    {
        public List<ThingDef> thingsToSpawn = new List<ThingDef>();
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
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref thingsToSpawn, "thingsToSpawn", LookMode.Def);
            Scribe_Collections.Look(ref otherHives, "otherHives", LookMode.Reference);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                thingsToSpawn ??= new List<ThingDef>();
                otherHives ??= new List<LargeTunnelHiveSpawner>();
            }
        }
    }
}
