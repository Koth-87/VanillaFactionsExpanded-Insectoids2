using System;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    public class IncidentWorker_InfestedChunkCrash : IncidentWorker_InfestedCrashBase
    {
        public override ThingDef MainPartDef => VFEI_DefOf.VFEI2_InfestedShipChunk;

        public override int MainPartPoints => 250;
    }
}
