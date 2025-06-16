using Verse;

namespace VFEInsectoids
{
    public class IncidentWorker_InfestedModuleCrash : IncidentWorker_InfestedCrashBase
    {
        public override ThingDef MainPartDef => VFEI_DefOf.VFEI2_InfestedShipModule;

        public override int MainPartPoints => 1600;
    }
}
