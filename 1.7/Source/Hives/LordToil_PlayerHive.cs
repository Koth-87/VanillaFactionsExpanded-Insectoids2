using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace VFEInsectoids
{
    public class LordToil_PlayerHive : LordToil
    {
        public Thing hive;

        public override IntVec3 FlagLoc => hive.Position;

        public override bool AllowSatisfyLongNeeds => true;

        public LordToil_PlayerHive(Thing hive)
        {
            this.hive = hive;
        }

        public override void UpdateAllDuties()
        {
            for (int i = 0; i < lord.ownedPawns.Count; i++)
            {
                Pawn pawn = lord.ownedPawns[i];
                if (pawn?.mindState != null && pawn.IsColonyInsect(out var hediff))
                {
                    if (hediff.InsectType == InsectType.Hunter)
                    {
                        pawn.mindState.duty = new PawnDuty(VFEI_DefOf.VFEI_Hunter, hive);
                    }
                    else if (hediff.InsectType == InsectType.Defender)
                    {
                        pawn.mindState.duty = new PawnDuty(VFEI_DefOf.VFEI_Defender, hive, 30);
                    }
                    else
                    {
                        pawn.mindState.duty = new PawnDuty(VFEI_DefOf.VFEI_Worker, hive);
                    }
                }
            }
        }
    }
}
