using RimWorld;
using Verse;
using System.Collections.Generic;

namespace VFEInsectoids
{
    internal class IngestionOutcomeDoer_IncreaseSpawnHediffs : IngestionOutcomeDoer
    {

        List<HediffDef> hediffs => new List<HediffDef> { VFEI_DefOf.VFEI2_EmpressSpawn, VFEI_DefOf.VFEI2_GigamiteSpawn,
        VFEI_DefOf.VFEI2_TitantickSpawn,VFEI_DefOf.VFEI2_TeramantisSpawn,VFEI_DefOf.VFEI2_SilverfishSpawn};

        public float increase;

        public override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            if (pawn.health != null) {

                foreach (HediffDef hediffdef in hediffs)
                {
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(hediffdef);
                    if (hediff != null)
                    {
                        hediff.Severity += (increase * ingestedCount);
                    }

                }

            }
               
        }
    }
}