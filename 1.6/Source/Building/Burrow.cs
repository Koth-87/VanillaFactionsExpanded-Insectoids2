
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;
using Verse.Noise;
using static HarmonyLib.Code;

namespace VFEInsectoids
{
    public enum BurrowSize
    {
        Small, Medium, Large

    }


    public class Burrow : Building

    {

        public BurrowSize size;
     

        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {

            if (dinfo.Def == DamageDefOf.Bomb)
            {
                this.HitPoints -= (int)dinfo.Amount;
                if (this.HitPoints <= 0)
                {
                    this.Destroy();
                }
            }

            base.PreApplyDamage(ref dinfo, out absorbed);
        }

       

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
           
            VFEI_DefOf.VFEI_DustCloud.Spawn(Position, Map).Cleanup();
            int rubbleNumber = 1;
            switch (size) { case BurrowSize.Small: rubbleNumber = 8; break;
                case BurrowSize.Medium: rubbleNumber = 16; break;
                case BurrowSize.Large: rubbleNumber = 24; break;
            }

            for (var i = 0; i < rubbleNumber; i++)
            {
                var randomCell = GenRadial.RadialCellsAround(Position, 3, true).RandomElement();
                if (randomCell.InBounds(Map) && GenSight.LineOfSight(randomCell, Position, Map))
                {
                    FilthMaker.TryMakeFilth(randomCell, Map, ThingDefOf.Filth_RubbleRock);
                }
            }
           
            Messages.Message("VFEI_BurrowCollapsed".Translate(), new LookTargets(Position, Map), MessageTypeDefOf.NeutralEvent);
            base.DeSpawn(mode);
        }
        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref size, "size");
        
        }

    }
}
