using Verse;

namespace VFEInsectoids
{
    public class IncidentWorker_InfestedPartCrash : IncidentWorker_InfestedCrashBase
    {
        public override ThingDef MainPartDef => VFEI_DefOf.VFEI2_InfestedShipPart;

        public override int MainPartPoints => 800;
    }
}
