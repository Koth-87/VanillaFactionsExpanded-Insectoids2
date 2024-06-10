using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace VFEInsectoids
{
    public class IncidentWorker_InfestedChunkCrash : IncidentWorker_InfestedCrashBase
    {
        public override ThingDef MainPartDef => VFEI_DefOf.VFEI2_InfestedShipChunk;

        public override int MainPartPoints => 250;
    }
}
