using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFEInsectoids
{
    public class QuestNode_GetSitePart : QuestNode
    {
        public SlateRef<SitePartDef> ratingOne, ratingTwo, ratingThree;
        [NoTranslate]
        public SlateRef<string> storeAs;

        [NoTranslate]
        public SlateRef<string> storeFactionAs;

        public override bool TestRunInt(Slate slate)
        {
            return TrySetVars(slate);
        }

        public override void RunInt()
        {
            if (!TrySetVars(QuestGen.slate))
            {
                Log.Error("Could not resolve site parts.");
            }
        }

        private bool TrySetVars(Slate slate)
        {
            var rating = slate.Get<int>("challengeRating");
            var siteParts = new List<SitePartDef>();
            switch (rating)
            {
                case 1: siteParts.Add(ratingOne.GetValue(slate)); break;
                case 2: siteParts.Add(ratingTwo.GetValue(slate)); break;
                case 3: siteParts.Add(ratingThree.GetValue(slate)); break;
            }
            slate.Set(storeAs.GetValue(slate), siteParts);
            slate.Set("sitePartCount", siteParts.Count);
            if (QuestGen.Working)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                for (int j = 0; j < siteParts.Count; j++)
                {
                    dictionary[siteParts[j].defName + "_exists"] = "True";
                }
                QuestGen.AddQuestDescriptionConstants(dictionary);
            }
            if (!storeFactionAs.GetValue(slate).NullOrEmpty())
            {
                slate.Set(storeFactionAs.GetValue(slate), Faction.OfInsects);
            }
            return true;
        }
    }
}
