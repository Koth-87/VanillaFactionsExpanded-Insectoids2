
using RimWorld;
using Verse;
namespace VFEInsectoids
{
    public class DamageWorker_AcidBite : DamageWorker_Bite
    {
        public override BodyPartRecord ChooseHitPart(DamageInfo dinfo, Pawn pawn)
        {
            return pawn.health.hediffSet.GetRandomNotMissingPart(dinfo.Def, dinfo.Height, BodyPartDepth.Outside);
        }

        public override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            base.ApplySpecialEffectsToPart(pawn, totalDamage, dinfo, result);
            
            pawn.TakeDamage(new DamageInfo(DamageDefOf.AcidBurn, 6, 0f, -1f, dinfo.Instigator, dinfo.HitPart, null, DamageInfo.SourceCategory.ThingOrUnknown));

            

        }
    }
}
