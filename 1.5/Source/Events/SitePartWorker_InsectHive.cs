using RimWorld;

namespace VFEInsectoids
{
    public class SitePartWorker_InsectHive : SitePartWorker
    {
        public override bool FactionCanOwn(Faction faction)
        {
            return faction == Faction.OfInsects;
        }
    }
}
