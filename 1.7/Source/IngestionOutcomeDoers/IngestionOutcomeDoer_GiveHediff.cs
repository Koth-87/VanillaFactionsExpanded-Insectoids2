
using System.Collections.Generic;
using RimWorld;
using Verse;
namespace VFEInsectoids
{
    public class IngestionOutcomeDoer_GiveHediff : IngestionOutcomeDoer
    {
        public HediffDef hediffDef;

        public float severity = -1f;

      

        public override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            Hediff hediff = HediffMaker.MakeHediff(hediffDef, pawn);
            float effect = ((!(severity > 0f)) ? hediffDef.initialSeverity : severity);
            hediff.Severity = effect;
            pawn.health.AddHediff(hediff);
        }

        public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
        {
            if (!parentDef.IsDrug || !(chance >= 1f))
            {
                yield break;
            }
            foreach (StatDrawEntry item in hediffDef.SpecialDisplayStats(StatRequest.ForEmpty()))
            {
                yield return item;
            }
        }
    }
}