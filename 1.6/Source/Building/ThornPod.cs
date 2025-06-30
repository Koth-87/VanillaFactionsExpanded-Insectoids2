using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace VFEInsectoids
{
    public class ThornPod : Building
    {
        public CompExplosive compExplosive;
        public int lastFiredThornsTick;
        public bool CanFire => lastFiredThornsTick <= 0 || Find.TickManager.TicksGame - lastFiredThornsTick >= GenDate.TicksPerDay * 7;
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            compExplosive = GetComp<CompExplosive>();
        }

        private Graphic _emptyGraphic;
        public override Graphic Graphic
        {
            get
            {
                if (CanFire)
                {
                    return base.Graphic;
                }
                else
                {
                    if (_emptyGraphic is null)
                    {
                        var data = new GraphicData();
                        data.CopyFrom(def.graphicData);
                        data.texPath += "_Empty";
                        _emptyGraphic = data.GraphicColoredFor(this);
                    }
                    return _emptyGraphic;
                }
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (CanFire && this.IsHashIntervalTick(60))
            {
                if (GenRadial.RadialDistinctThingsAround(Position, this.Map, 4.9f, true).OfType<Pawn>()
                    .Where(x => x.HostileTo(Faction)).Any())
                {
                    compExplosive.AddThingsIgnoredByExplosion(new List<Thing>
                    {
                        this
                    });
                    compExplosive.Detonate(Map);
                    lastFiredThornsTick = Find.TickManager.TicksGame;
                }
            }
        }

        public override string GetInspectString()
        {
            var sb = new StringBuilder(base.GetInspectString());
            if (CanFire is false)
            {
                var duration = (lastFiredThornsTick + (GenDate.TicksPerDay * 7)) - Find.TickManager.TicksGame;
                sb.AppendLine("VFEI_TimeUntilThornsRegrow".Translate(duration.ToStringTicksToPeriod()));
            }
            return sb.ToString().TrimEndNewlines();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref lastFiredThornsTick, "lastFiredThornsTick");
        }
    }
}