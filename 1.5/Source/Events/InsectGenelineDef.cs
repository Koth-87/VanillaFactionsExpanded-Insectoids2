using RimWorld;
using System.Collections.Generic;
using Verse;

namespace VFEInsectoids
{
    public class InsectGenelineDef : Def
    {
        public List<PawnGenOption> insects;
        public PawnKindDef boss;
        public ThingDef hive;
        public float spawnWeight;
    }
}
