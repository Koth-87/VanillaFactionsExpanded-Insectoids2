using LudeonTK;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{

    [HotSwappable]
    [StaticConstructorOnStartup]
    public class HordeModeManager : IExposable
    {
        [DebugAction("General", "Start wave", allowedGameStates = AllowedGameStates.PlayingOnMap, actionType = DebugActionType.Action)]
        public static void StartWave_Debug()
        {
            var map = Find.RandomPlayerHomeMap;
            if (map != null)
            {
                GameComponent_Insectoids.Instance.hordeModeManager.StartWave(map);
                GameComponent_Insectoids.Instance.hordeModeManager.AddNextWave();
            }
        }

        [DebugAction("General", "Next wave", allowedGameStates = AllowedGameStates.PlayingOnMap, actionType = DebugActionType.Action)]
        public static void CompleteWave_Debug()
        {
            GameComponent_Insectoids.Instance.hordeModeManager.waveActivities.RemoveAt(0);
            GameComponent_Insectoids.Instance.hordeModeManager.AddNextWave();
        }

        private static readonly Texture2D SwarmActivityCounter = ContentFinder<Texture2D>.Get("UI/WaveSurvivalUI/SwarmActivityCounter");
        private static readonly Texture2D NextWaveIndicator = ContentFinder<Texture2D>.Get("UI/WaveSurvivalUI/NextWaveIndicator");
        private static readonly Texture2D InsectWaveBG = ContentFinder<Texture2D>.Get("UI/WaveSurvivalUI/InsectWaveBG");

        public List<WaveActivity> waveActivities = new List<WaveActivity>();
        public List<WaveActivity> currentActivities = new List<WaveActivity>();

        public HordeModeManager()
        {
            if (waveActivities is null)
            {
                InitializeWaveActivities();
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref scaleUp, "scaleUp", true);
            Scribe_Collections.Look(ref waveActivities, "waveActivities", LookMode.Deep);
            Scribe_Collections.Look(ref currentActivities, "currentActivities", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (waveActivities is null)
                {
                    InitializeWaveActivities();
                }
                currentActivities ??= new List<WaveActivity>();
            }
        }

        public void InitializeWaveActivities()
        {
            var allWaveDefsInOrder = DefDatabase<WaveActivityDef>.AllDefs.OrderBy(x => x.intensity).ToList();
            var reversedWaveDefs = DefDatabase<WaveActivityDef>.AllDefs.OrderByDescending(x => x.intensity).ToList();
            waveActivities = new List<WaveActivity>();
            bool ascending = true;
            WaveActivityDef lastAddedDef = null;
            while (waveActivities.Count < 8)
            {
                var defs = ascending ? allWaveDefsInOrder : reversedWaveDefs;

                foreach (var def in defs)
                {
                    if (waveActivities.Count >= 8)
                        break;

                    if (def != lastAddedDef)
                    {
                        waveActivities.Add(new WaveActivity(def));
                        lastAddedDef = def;
                    }
                }
                ascending = !ascending;
            }
        }

        public void Tick()
        {
            if (Find.AnyPlayerHomeMap != null)
            {
                CurrentActivity.ticksStarting--;
                if (CurrentActivity.ticksStarting <= 0)
                {
                    var map = Find.RandomPlayerHomeMap;
                    if (map != null)
                    {
                        this.StartWave(map);
                        AddNextWave();
                    }
                }
            }

            for (var i = currentActivities.Count - 1; i >= 0; i--)
            {
                var activity = currentActivities[i];
                if (activity.lord is null || activity.lord.AnyActivePawn is false)
                {
                    CompleteWave(activity.lord.Map ?? Find.RandomPlayerHomeMap);
                    currentActivities.RemoveAt(i);
                }
            }
        }

        private Vector2 waveIconSize = new Vector2(30, 30);
        private Vector2 waveIntensitySize = new Vector2(30, 30);

        public void DoGUI()
        {
            if (Find.CurrentMap is Map map && map.IsPlayerHome)
            {
                DrawWaveOverlay();
            }
        }

        private void DrawWaveOverlay()
        {
            if (waveActivities.NullOrEmpty())
            {
                InitializeWaveActivities();
            }

            float screenWidth = UI.screenWidth;
            float iconWidthWithSpacing = waveIconSize.x + 5f;
            int totalIcons = waveActivities.Count;
            float totalIconsWidth = totalIcons * iconWidthWithSpacing;
            var maxIntensity = waveActivities.Max(x => x.def.intensity);
            var intensitySpacing = 8;
            var intensityIconWithSpacing = waveIntensitySize.y + intensitySpacing + 5;

            var iconPos = new Vector2(screenWidth - totalIconsWidth, intensityIconWithSpacing);
            var waveTicks = 0;

            for (int i = 0; i < waveActivities.Count; i++)
            {
                var curActivity = waveActivities[i];
                var intensityPos = new Vector2(iconPos.x, iconPos.y - (6 + intensitySpacing));
                for (var j = 0; j < curActivity.def.intensity; j++)
                {
                    var intensityRect = new Rect(intensityPos.x, intensityPos.y, waveIntensitySize.x, waveIntensitySize.y);
                    GUI.DrawTexture(intensityRect, SwarmActivityCounter);
                    intensityPos.y -= intensitySpacing;
                }

                Rect iconRect = new Rect(iconPos.x, iconPos.y, waveIconSize.x, waveIconSize.y);
                GUI.DrawTexture(iconRect, InsectWaveBG);
                GUI.DrawTexture(iconRect, curActivity.geneline.waveIcon);
                iconPos.x += iconWidthWithSpacing;
                waveTicks += curActivity.ticksStarting;
                if (i == 0)
                {
                    var arrowSize = 30;
                    Rect arrowRect = new Rect(iconRect.x + (iconRect.width - arrowSize) / 2f, iconRect.yMax, arrowSize, arrowSize); 
                    GUI.DrawTexture(arrowRect, NextWaveIndicator);
                }

                if (Mouse.IsOver(iconRect))
                {
                    TooltipHandler.TipRegion(iconRect, "VFEI_WaveActivityOverlay".Translate(curActivity.def.label,
                        curActivity.def.raidSizeMultiplier.ToStringPercent(), waveTicks.ToStringTicksToPeriod(), curActivity.def.label)); ;
                }
            }

            Text.Anchor = TextAnchor.UpperRight;

            Rect timerRect = new Rect(screenWidth - (350 + 15), iconPos.y + waveIconSize.y, 350, 50);
            Text.Font = GameFont.Medium;
            Text.CurFontStyle.fontSize += 40;
            Widgets.Label(timerRect, $"{TicksToString(CurrentActivity.ticksStarting)}");
            Text.CurFontStyle.fontSize -= 40;
            Text.Font = GameFont.Small;
            var enemiesComingRect = new Rect(timerRect.x, timerRect.yMax, timerRect.width, 24);
            if (CurrentActivity.insects is null)
            {
                CurrentActivity.FormRaidComposition();
            }

            Widgets.Label(enemiesComingRect, "VFEI_EnemiesComing".Translate());
            var raidInfo = CurrentActivity.GetRaidInfo();
            var height = Text.CalcHeight(raidInfo, enemiesComingRect.width);
            var raidInfoRect = new Rect(enemiesComingRect.x, enemiesComingRect.yMax, enemiesComingRect.width, height);
            Widgets.Label(raidInfoRect, raidInfo);
            Text.Anchor = TextAnchor.UpperLeft;
        }

        public static string TicksToString(int ticks)
        {
            int totalSeconds = ticks / 60;
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            if (hours > 0)
            {
                return $"{hours}:{minutes:D2}:{seconds:D2}";
            }
            else
            {
                return $"{minutes:D2}:{seconds:D2}";
            }
        }

        public WaveActivity CurrentActivity => waveActivities[0];

        public void StartWave(Map map)
        {
            IncidentParms parms = new IncidentParms
            {
                target = map,
                faction = Faction.OfInsects,
            };
            VFEI_DefOf.VFEI_HordeWaveRaid.Worker.TryExecute(parms);
            waveActivities.RemoveAt(0);
        }

        private bool scaleUp = true;

        public void AddNextWave()
        {
            var lastActivity = waveActivities.Last();
            var curIntensity = lastActivity.def.intensity;
            var orderedActivities = DefDatabase<WaveActivityDef>.AllDefs.Where(x => x != lastActivity.def)
                .OrderBy(x => x.intensity).ToList();
            var def = FindNextWave(orderedActivities, curIntensity);
            waveActivities.Add(new WaveActivity(def));
            CurrentActivity.FormRaidComposition();
        }

        private WaveActivityDef FindNextWave(List<WaveActivityDef> orderedActivities, int curIntensity)
        {
            while (true)
            {
                if (scaleUp)
                {
                    var def = orderedActivities.FirstOrDefault(x => curIntensity < x.intensity);
                    if (def is null)
                    {
                        scaleUp = false;
                    }
                    else
                    {
                        return def;
                    }
                }
                else
                {
                    var def = orderedActivities.LastOrDefault(x => curIntensity > x.intensity);
                    if (def is null)
                    {
                        scaleUp = true;
                    }
                    else
                    {
                        return def;
                    }
                }
            }
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
                ThingDef thingDef = DefDatabase<ThingDef>.AllDefs.Where(x => x.BaseMarketValue > 0 
                && x.category == ThingCategory.Item && DebugThingPlaceHelper.IsDebugSpawnable(x) 
                && x.race is null).RandomElement();
                float marketValue = thingDef.BaseMarketValue;
                if (marketValue < 200f)
                {
                    int itemCount = Mathf.CeilToInt(200f / marketValue);
                    int stackLimit = thingDef.stackLimit;
                    while (itemCount > 0)
                    {
                        Thing thing = ThingMaker.MakeThing(thingDef, GenStuff.RandomStuffFor(thingDef));
                        thing.stackCount = Mathf.Min(itemCount, stackLimit);
                        things.Add(thing);
                        itemCount -= thing.stackCount;
                    }
                }
                else
                {
                    Thing thing = ThingMaker.MakeThing(thingDef, GenStuff.RandomStuffFor(thingDef));
                    things.Add(thing);
                }
                IntVec3 dropCell = DropCellFinder.RandomDropSpot(map);
                DropPodUtility.DropThingsNear(dropCell, map, things);
                Messages.Message("VFEI_CargoPodsSentForCompletingWave".Translate(), 
                    new TargetInfo(dropCell, map), MessageTypeDefOf.PositiveEvent);
            }
        }
    }
}