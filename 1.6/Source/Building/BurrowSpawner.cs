using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace VFEInsectoids
{
    public class BurrowSpawner : GroundSpawner
    
    {
        public List<ThingDef> thingsToSpawn = new List<ThingDef>();
        public FactionDef faction;
        public BurrowSize size;
        public int radiusOfSpawn = 2;
        public override void Spawn(Map map, IntVec3 loc)
        {
            base.Spawn(map, loc);
            foreach (var def in thingsToSpawn)
            {
                var pos = CellFinder.RandomClosewalkCellNear(loc, map, radiusOfSpawn,
                    (IntVec3 c) => c.GetThingList(map).Any(x => x is Hive || x is TunnelHiveSpawner || x is Building
                    || x is Cocoon || x is BurrowSpawner) is false
                 
                    && GenSpawn.WouldWipeAnythingWith(c, def.defaultPlacingRot, def, map, (Thing x) => true) is false);
                var thing = ThingMaker.MakeThing(def);
                if (def.CanHaveFaction)
                {
                    thing.SetFaction(Find.FactionManager.FirstFactionOfDef(faction));
                }
                GenPlace.TryPlaceThing(thing, pos, map, ThingPlaceMode.Near);
                if (thing is Burrow burrow)
                {
                    burrow.size = size;   
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref thingsToSpawn, "thingsToSpawn", LookMode.Def);
            Scribe_Defs.Look(ref faction, "faction");
            Scribe_Values.Look(ref size, "size");
            Scribe_Values.Look(ref radiusOfSpawn, "radiusOfSpawn", 2);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                thingsToSpawn ??= new List<ThingDef>();
              
            }
        }
    }
}
