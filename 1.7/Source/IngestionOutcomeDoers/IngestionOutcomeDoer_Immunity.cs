using RimWorld;
using Verse;

namespace VFEInsectoids
{
    internal class IngestionOutcomeDoer_Immunity : IngestionOutcomeDoer
    {
        public float percent = 0.05f;



        public override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            if (pawn.health != null && pawn.health.hediffSet.HasImmunizableNotImmuneHediff() && pawn.health.hediffSet.HasHediff(HediffDefOf.WoundInfection) && pawn.health.immunity != null && pawn.health.immunity.GetImmunity(HediffDefOf.WoundInfection) != 1)
            {
                ImmunityRecord cPawn = pawn.health.immunity.GetImmunityRecord(HediffDefOf.WoundInfection);
                cPawn.immunity += (percent / 100) * ingested.stackCount;
            }
        }
    }
}