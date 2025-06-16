using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace VFEInsectoids
{
    public class QuestNode_CreateInsectSettlement : QuestNode
    {
        [NoTranslate]
        public SlateRef<string> inSignal;

        public override bool TestRunInt(Slate slate)
        {
            return true;
        }

        public override void RunInt()
        {
            Slate slate = QuestGen.slate;
            var questPart = new QuestPart_CreateInsectSettlement();
            questPart.inSignal = QuestGenUtility.HardcodedSignalWithQuestID(inSignal.GetValue(slate)) ?? QuestGen.slate.Get<string>("inSignal");
            questPart.parent = slate.Get<Site>("site");
            QuestGen.quest.AddPart(questPart);
        }
    }
}
