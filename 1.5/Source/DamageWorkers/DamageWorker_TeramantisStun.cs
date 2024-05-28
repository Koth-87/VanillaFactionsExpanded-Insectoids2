
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
namespace VFEInsectoids
{
    public class DamageWorker_TeramantisStun : DamageWorker_Cut
    {
       

        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            DamageResult damageResult = base.Apply(dinfo, victim);

            Pawn pawn = victim as Pawn;
            if (pawn?.health?.hediffSet?.HasHediff(VFEI_DefOf.VFEI2_TeramantisStun)==false)
            {
                pawn.health.AddHediff(VFEI_DefOf.VFEI2_TeramantisStun);
                pawn.stances.stunner.StunFor(280, dinfo.Instigator);

            }
            
            return damageResult;
        }
    }
}