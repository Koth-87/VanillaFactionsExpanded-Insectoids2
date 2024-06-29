using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
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
            MakeSettlement();
        }

        private Settlement MakeSettlement()
        {
            Settlement settlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
            settlement.SetFaction(Faction.OfInsects);
            settlement.Tile = parent.Tile;
            settlement.Name = SettlementNameGenerator.GenerateSettlementName(settlement);
            Find.WorldObjects.Add(settlement);
            return settlement;
        }

        public override void Notify_PreCleanup()
        {
            base.Notify_PreCleanup();
            if (quest.State == QuestState.EndedOfferExpired)
            {
                var settlement = MakeSettlement();
                Find.LetterStack.ReceiveLetter("VFEI_InfestationHasSpread".Translate(quest.name),
                    "VFEI_InfestationHasSpreadDesc".Translate(quest.name), LetterDefOf.NegativeEvent, settlement, settlement.Faction, quest); ;
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
