using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace VFEInsectoids
{
    public class IncidentWorker_Ambush_RoamingInsectoids : IncidentWorker_Ambush
    {
        public override bool CanFireNowSub(IncidentParms parms)
        {
            if (parms.target.Tile.IsInfestedTile())
            {
                Log.Message("CanFireNowSub: 1");
                if (!base.CanFireNowSub(parms))
                {
                    Log.Message("CanFireNowSub: false");
                    return false;
                }
                Log.Message("CanFireNowSub: true");
                return true;
            }
            Log.Message("CanFireNowSub: false 2");
            return false;
        }

        public override bool TryExecuteWorker(IncidentParms parms)
        {
            Log.Message("TryExecuteWorker: 1");
            return base.TryExecuteWorker(parms);
        }

        public override List<Pawn> GeneratePawns(IncidentParms parms)
        {
            parms.faction = Faction.OfInsects;
            PawnGroupMakerParms defaultPawnGroupMakerParms = IncidentParmsUtility.GetDefaultPawnGroupMakerParms(PawnGroupKindDefOf.Combat, parms);
            defaultPawnGroupMakerParms.generateFightersOnly = true;
            defaultPawnGroupMakerParms.dontUseSingleUseRocketLaunchers = true;
            return PawnGroupMakerUtility.GeneratePawns(defaultPawnGroupMakerParms).ToList();
        }

        public override LordJob CreateLordJob(List<Pawn> generatedPawns, IncidentParms parms)
        {
            return new LordJob_AssaultColony(parms.faction, canKidnap: true, canTimeoutOrFlee: false);
        }

        public override string GetLetterText(Pawn anyPawn, IncidentParms parms)
        {
            Caravan caravan = parms.target as Caravan;
            return def.letterText.Formatted((caravan != null) ? caravan.Name : "yourCaravan".TranslateSimple(), parms.faction.def.pawnsPlural, parms.faction.NameColored).Resolve().CapitalizeFirst();
        }
    }
}
