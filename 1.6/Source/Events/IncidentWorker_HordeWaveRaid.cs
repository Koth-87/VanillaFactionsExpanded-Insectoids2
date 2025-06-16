using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace VFEInsectoids
{
    public class IncidentWorker_HordeWaveRaid : IncidentWorker_PawnsArrive
    {
        public bool TryGenerateRaidInfo(IncidentParms parms, out List<Pawn> pawns, WaveActivity wave, bool debugTest = false)
        {
            pawns = null;
            parms.faction = Faction.OfInsects;
            parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
            PawnGroupKindDef groupKind = PawnGroupKindDefOf.Combat;
            if (!debugTest)
            {
                parms.raidStrategy.Worker.TryGenerateThreats(parms);
            }
            if (!debugTest && !parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
            {
                return false;
            }
            pawns = new List<Pawn>();
            foreach (var kvp in wave.insects)
            {
                for (var i  = 0; i < kvp.Value; i++)
                {
                    pawns.Add(PawnGenerator.GeneratePawn(kvp.Key, Faction.OfInsects));
                }
            }
            parms.raidArrivalMode.Worker.Arrive(pawns, parms);
            parms.pawnCount = pawns.Count;
            return true;
        }

        protected string GetLetterLabel(IncidentParms parms)
        {
            return parms.raidStrategy.letterLabelEnemy + ": " + parms.faction.Name;
        }

        protected string GetLetterText(IncidentParms parms, List<Pawn> pawns)
        {
            string text = string.Format(parms.raidArrivalMode.textEnemy, parms.faction.def.pawnsPlural, parms.faction.Name.ApplyTag(parms.faction)).CapitalizeFirst();
            text += "\n\n";
            text += parms.raidStrategy.arrivalTextEnemy;
            Pawn pawn = pawns.Find((Pawn x) => x.Faction.leader == x);
            if (pawn != null)
            {
                text += "\n\n";
                text += "EnemyRaidLeaderPresent".Translate(pawn.Faction.def.pawnsPlural, pawn.LabelShort, pawn.Named("LEADER")).Resolve();
            }
            if (parms.raidAgeRestriction != null && !parms.raidAgeRestriction.arrivalTextExtra.NullOrEmpty())
            {
                text += "\n\n";
                text += parms.raidAgeRestriction.arrivalTextExtra.Formatted(parms.faction.def.pawnsPlural.Named("PAWNSPLURAL")).Resolve();
            }
            return text;
        }

        public override bool TryExecuteWorker(IncidentParms parms)
        {
            var wave = GameComponent_Insectoids.Instance.hordeModeManager.CurrentActivity;
            TryGenerateRaidInfo(parms, out var pawns, wave);
            TaggedString letterLabel = GetLetterLabel(parms);
            TaggedString letterText = GetLetterText(parms, pawns);
            SendStandardLetter(letterLabel, letterText, def.letterDef, parms, pawns);
            Map map = (Map)parms.target;
            Lord lord = LordMaker.MakeNewLord(parms.faction, parms.raidStrategy.Worker.MakeLordJob(parms, map, pawns, pawns.GetHashCode()), map, pawns);
            lord.inSignalLeave = parms.inSignalEnd;
            wave.lord = lord;
            QuestUtility.AddQuestTag(lord, parms.questTag);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.EquippingWeapons, OpportunityType.Critical);
            GameComponent_Insectoids.Instance.hordeModeManager.currentActivities.Add(wave);
            return true;
        }
    }
}