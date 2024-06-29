using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VFEInsectoids
{
    public class QuestPart_CreateInsectSettlement : QuestPart
    {
        public string inSignal;

        public MapParent parent;

        public override void Notify_QuestSignalReceived(Signal signal)
        {
            if (!(signal.tag == inSignal))
            {
                return;
            }
            Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
            settlement.SetFaction(Faction.OfInsects);
            settlement.Tile = parent.Tile;
            settlement.Name = SettlementNameGenerator.GenerateSettlementName(settlement);
            Find.WorldObjects.Add(settlement);
        }

        public override void Notify_PreCleanup()
        {
            base.Notify_PreCleanup();
            if (quest.State == QuestState.EndedOfferExpired)
            {
                QuestUtility.SendQuestTargetSignals(parent.questTags, "InfestationHasSpread", parent.Named("SUBJECT"));
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref inSignal, "inSignal");
            Scribe_References.Look(ref parent, "parent");
        }
    }
}
