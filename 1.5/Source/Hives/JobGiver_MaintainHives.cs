using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    public class JobGiver_MaintainHives : JobGiver_AIFightEnemies
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            var allMaintenables = new List<Thing>();
            foreach (var maintainableDef in Utils.allMaintainableDefs)
            {
                foreach (var maintaineable in pawn.Map.listerThings.ThingsOfDef(maintainableDef)
                    .Where(x => x.Faction == pawn.Faction && pawn.CanReserve(x) 
                    && pawn.CanReach(x, PathEndMode.Touch, Danger.Deadly)))
                {
                    CompMaintainable compMaintainable = maintaineable.TryGetComp<CompMaintainable>();
                    if (compMaintainable != null && compMaintainable.CurStage != MaintainableStage.Healthy)
                    {
                        allMaintenables.Add(maintaineable);
                    }
                }
            }
            var maintainaeable = allMaintenables.OrderBy(x => x.Position.DistanceTo(pawn.Position)).FirstOrDefault();
            if (maintainaeable != null)
            {
                return JobMaker.MakeJob(JobDefOf.Maintain, maintainaeable);
            }
            return null;
        }
    }

}
