using RimWorld;
using System.Collections.Generic;
using Verse;

namespace VFEInsectoids
{
    public class StorytellerCompProperties_ExclusiveIncidents : StorytellerCompProperties
    {
        public List<IncidentDef> incidents;

        public StorytellerCompProperties_ExclusiveIncidents()
        {
            compClass = typeof(StorytellerComp_ExclusiveIncidents);
        }
    }

    public class StorytellerComp_ExclusiveIncidents : StorytellerComp
    {
        public StorytellerCompProperties_ExclusiveIncidents Props => (StorytellerCompProperties_ExclusiveIncidents)props;

        public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
        {
            if (Find.TickManager.TicksGame >= GameComponent_Insectoids.Instance.NextEmpressEvilFiringTick)
            {
                foreach (var incident in Props.incidents.InRandomOrder())
                {
                    IncidentParms parms = GenerateParms(incident.category, target);
                    if (incident.Worker.CanFireNow(parms))
                    {
                        GameComponent_Insectoids.Instance.SetNextEmpressEvilFiringTick();
                        yield return new FiringIncident(incident, this, parms);
                        yield break;
                    }
                }
            }
        }
    }
}
