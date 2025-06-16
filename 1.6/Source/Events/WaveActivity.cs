using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace VFEInsectoids
{
    [HotSwappable]
    public class WaveActivity : IExposable
    {
        public WaveActivityDef def;
        public int ticksStarting;
        public InsectGenelineDef geneline;
        public Dictionary<PawnKindDef, int> insects;
        public Lord lord;
        public WaveActivity()
        {

        }

        public WaveActivity(WaveActivityDef waveActivity)
        {
            this.def = waveActivity;
            SetTicksStarting();
            this.geneline = DefDatabase<InsectGenelineDef>.GetRandom();
        }

        public void SetTicksStarting()
        {
            this.ticksStarting = (int)((VFEInsectoidsSettings.baseTimeBetweenHanHordeWaves * def.timeUntilWaveArrives)
                * GenDate.TicksPerDay);
        }

        public void FormRaidComposition()
        {
            var raidSize = Mathf.Max(StorytellerUtility.DefaultThreatPointsNow(Find.World) * def.raidSizeMultiplier,
                geneline.insects.Min(x => x.kind.combatPower));
            if (def.includeBoss)
            {
                raidSize += geneline.boss.combatPower;
            }
            insects = GetRaidList(raidSize);
        }

        public string GetRaidInfo()
        {
            var raidInfo = new StringBuilder();
            foreach (var insect in insects.Keys.OrderBy(x => x.combatPower))
            {
                raidInfo.AppendLine(insects[insect] + " " + insect.GetLabelPlural(insects[insect]));
            }
            return raidInfo.ToString();
        }

        private Dictionary<PawnKindDef, int> GetRaidList(float raidSize)
        {
            var dict = new Dictionary<PawnKindDef, int>();
            if (def.includeBoss)
            {
                dict[geneline.boss] = 1;
                raidSize -= geneline.boss.combatPower;
            }
            var insects = new List<PawnKindDef>();
            GenerateInsects(geneline, insects, raidSize);
            foreach (var insect in insects)
            {
                if (!dict.TryGetValue(insect, out var count))
                {
                    dict[insect] = 1;
                }
                else
                {
                    dict[insect] = count + 1;
                }
            }

            return dict;
        }

        public static void GenerateInsects(InsectGenelineDef genelineDef, 
            List<PawnKindDef> __result, float points)
        {
            while (true)
            {
                var insect = GenerateInsect(points, genelineDef.insects);
                if (insect != null)
                {
                    points -= insect.combatPower;
                    __result.Add(insect);
                }
                else
                {
                    break;
                }
            }
        }

        private static PawnKindDef GenerateInsect(float curPoints, IEnumerable<PawnGenOption> source)
        {
            var chosenKind = RandomPawnKindDef(curPoints, source);
            if (chosenKind != null)
            {
                return chosenKind;
            }
            return null;
        }

        private static PawnKindDef RandomPawnKindDef(float curPoints, IEnumerable<PawnGenOption> source)
        {
            source = source.Where((PawnGenOption x) => curPoints >= x.kind.combatPower);
            if (source.TryRandomElementByWeight(x => x.selectionWeight, out var result))
            {
                return result.kind;
            }
            return null;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Defs.Look(ref geneline, "geneline");
            Scribe_Values.Look(ref ticksStarting, "ticksStarting");
            Scribe_References.Look(ref lord, "lord");
            Scribe_Collections.Look(ref insects, "insects", LookMode.Def, LookMode.Value);
        }
    }
}
