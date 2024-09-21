using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    public class HordeModeManager : IExposable
    {
        private List<WaveActivity> waveActivities;

        public HordeModeManager()
        {
            if (waveActivities is null)
            {
                InitializeWaveActivities();
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref waveActivities, "waveActivities", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (waveActivities is null)
                {
                    InitializeWaveActivities();
                }
            }
        }

        public void InitializeWaveActivities()
        {
            Log.Message("InitializeWaveActivities: " + waveActivities);
            waveActivities = DefDatabase<WaveActivityDef>.AllDefs
                .OrderBy(x => x.order)
                .Select(def => new WaveActivity(def))
                .ToList();
        }

        public void Tick()
        {
            if (Find.AnyPlayerHomeMap != null)
            {
                CurrentActivity.ticksStarting--;
                if (CurrentActivity.ticksStarting <= 0)
                {
                    StartNextWave();
                    MoveToNextActivity();
                }
            }
        }

        public void Update()
        {

        }

        private WaveActivity CurrentActivity => waveActivities[0];

        public void StartNextWave()
        {
            var map = Find.AnyPlayerHomeMap;
            if (map != null)
            {
                GenerateRaid(map);
                CompleteWave(map);
            }
        }

        private void MoveToNextActivity()
        {
            var currentActivity = CurrentActivity;
            waveActivities.RemoveAt(0);

            // Get all the activities ordered by the 'order' field
            var orderedActivities = DefDatabase<WaveActivityDef>.AllDefs.OrderBy(x => x.order).ToList();

            // Find the current activity in the ordered list
            var currentIndex = orderedActivities.IndexOf(currentActivity.def);

            if (currentIndex >= 0)
            {
                if (currentIndex + 1 < orderedActivities.Count)
                {
                    // Scale up to the next wave
                    var nextActivity = orderedActivities[currentIndex + 1];
                    waveActivities.Add(new WaveActivity(nextActivity));
                }
                else if (currentIndex - 1 >= 0)
                {
                    // Scale down to the previous wave
                    var previousActivity = orderedActivities[currentIndex - 1];
                    waveActivities.Add(new WaveActivity(previousActivity));
                }
            }
        }

        private float CalculateRaidSize()
        {
            return StorytellerUtility.DefaultThreatPointsNow(Find.World) * CurrentActivity.def.raidSizeMultiplier;
        }

        private void GenerateRaid(Map map)
        {
            var raidSize = CalculateRaidSize();
            IncidentParms parms = new IncidentParms
            {
                target = map,
                faction = Faction.OfInsects,
                points = raidSize
            };
            IncidentDefOf.RaidEnemy.Worker.TryExecute(parms);
        }

        private void CompleteWave(Map map)
        {
            if (DefDatabase<ResearchProjectDef>.AllDefsListForReading
                .Where(x => x.CanStartNow && !x.IsFinished).TryRandomElement(out var research))
            {
                Find.ResearchManager.FinishProject(research, false);
                Messages.Message("VFEI_ResearchUnlockedForCompletingWave".Translate(research.LabelCap),
                    MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                List<Thing> things = new List<Thing>();
                ThingDef thingDef = DefDatabase<ThingDef>.AllDefs.Where(x => x.BaseMarketValue > 0).RandomElement();
                float marketValue = thingDef.BaseMarketValue;
                if (marketValue < 200f)
                {
                    int itemCount = Mathf.CeilToInt(200f / marketValue);
                    Thing thing = ThingMaker.MakeThing(thingDef, GenStuff.RandomStuffFor(thingDef));
                    thing.stackCount = itemCount;
                    things.Add(thing);
                }
                else
                {
                    Thing thing = ThingMaker.MakeThing(thingDef);
                    things.Add(thing);
                }
                IntVec3 dropCell = DropCellFinder.RandomDropSpot(map);
                DropPodUtility.DropThingsNear(dropCell, map, things);
                Messages.Message("VFEI_CargoPodsSentForCompletingWave".Translate(research.LabelCap), 
                    new TargetInfo(dropCell, map), MessageTypeDefOf.PositiveEvent);
            }
        }


    }
}