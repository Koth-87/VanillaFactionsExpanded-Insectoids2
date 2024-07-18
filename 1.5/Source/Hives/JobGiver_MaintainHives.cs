using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    public class JobGiver_MaintainHives : JobGiver_AIFightEnemies
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            foreach (var hive in pawn.Map.listerThings.AllThings.Where(x => (x is Hive || x.def == VFEI_DefOf.VFEI2_JellyFarm)
                && x.Faction == pawn.Faction && pawn.CanReserve(x) && pawn.CanReach(x, PathEndMode.Touch, Danger.Deadly))
                .OrderBy(x => x.Position.DistanceTo(pawn.Position)))
            {
                CompMaintainable compMaintainable = hive.TryGetComp<CompMaintainable>();
                if (compMaintainable != null && compMaintainable.CurStage != MaintainableStage.Healthy)
                {
                    return JobMaker.MakeJob(JobDefOf.Maintain, hive);
                }
            }
            return null;
        }
    }

}
