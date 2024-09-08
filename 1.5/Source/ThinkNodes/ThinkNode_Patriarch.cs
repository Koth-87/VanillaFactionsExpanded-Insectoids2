
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    public class ThinkNode_Patriarch : ThinkNode_Conditional
    {
        public override bool Satisfied(Pawn pawn)
        {
            if (pawn.kindDef == VFEI_DefOf.VFEI2_Patriarch)
            {
                return true;
            }
            return false;
        }
    }
}
