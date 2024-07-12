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

    public class LordJob_PlayerHive : LordJob
    {
        public override bool CanBlockHostileVisitors => false;

        public override bool AddFleeToil => false;

        public override bool KeepExistingWhileHasAnyBuilding => true;

        public Thing hive;

        public LordJob_PlayerHive()
        {

        }

        public LordJob_PlayerHive(Thing hive)
        {
            this.hive = hive;
        }

        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            stateGraph.AddToil(new LordToil_PlayerHive(hive));
            return stateGraph;
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref hive, "hive");
        }
    }
}
