using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VFEInsectoids
{
    public class InsectWave
    {
        public List<PawnKindDefCount> insects;

        public string GetPawnList()
        {
            var entries = new List<string>();
            foreach (var entry in insects)
            {
                entries.Add(GenLabel.BestKindLabel(entry.kindDef, Gender.None).CapitalizeFirst()
                    + " x" + entry.count);
            }
            return entries.ToLineList("  - ");
        }
    }

    public class InsectWaveDef : Def
    {
        public List<InsectWave> waves;
        public int minWaveRepeatIndex;
    }

    public class CompProperties_Thumper : CompProperties_UseEffect
    {
        public GraphicData topGraphic;
        public InsectWaveDef wave;
        public CompProperties_Thumper()
        {
            compClass = typeof(CompThumper);
        }
    }

    [HotSwappable]
    public class CompThumper : CompUseEffect
    {
        public bool activated;
        public int thumpDuration;
        private Graphic _topGraphic;
        public float topGraphicOffset;
        public bool infestationSpawned;
        public bool isLowering;
        public SoundDef soundThumping;
        public Graphic TopGraphic => _topGraphic ??= Props.topGraphic.Graphic;

        public CompProperties_Thumper Props => (CompProperties_Thumper)props;

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            activated = true;
            GameComponent_Insectoids.Instance.thumperActivated = parent;
            thumpDuration = (int)(new FloatRange(30f, 90f).RandomInRange * 60f);
            var wave = GameComponent_Insectoids.Instance.GetNextInsectWave(Props.wave);
            Find.LetterStack.ReceiveLetter("VFEI_LetterLabelInsectWaveSummoned".Translate(
                GenLabel.BestKindLabel(wave.insects.First().kindDef, Gender.None).CapitalizeFirst()), 
                "VFEI_LetterInsectWaveSummoned".Translate(Faction.OfInsects.NameColored), 
                LetterDefOf.NeutralEvent, parent);
        }

        public override TaggedString ConfirmMessage(Pawn p)
        {
            var wave = GameComponent_Insectoids.Instance.GetNextInsectWave(Props.wave);
            return "VFEI_InsectSummonWarning".Translate(parent.Label, wave.GetPawnList().Named("PAWNS"));
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (GameComponent_Insectoids.Instance.thumperActivated == this.parent)
            {
                GameComponent_Insectoids.Instance.thumperActivated = null;
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            TopGraphic.Draw(parent.DrawPos + new Vector3(0, 0.01f, topGraphicOffset), parent.Rotation, parent);
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            if (activated)
            {
                return "VFEI_AlreadyActivated".Translate();
            }
            if (GameComponent_Insectoids.Instance.thumperActivated is Thing thing && thing.Destroyed is false)
            {
                return "VFEI_OtherThumperIsActive".Translate();
            }
            var lastBossArrival = GameComponent_Insectoids.Instance.lastInsectoidBossArrival;
            if (lastBossArrival != 0 && Find.TickManager.TicksGame <= lastBossArrival + GenDate.TicksPerDay * 2)
            {
                return "VFEI_OnCooldown".Translate(((lastBossArrival + GenDate.TicksPerDay * 2) - Find.TickManager.TicksGame).ToStringTicksToPeriod());
            }
            return base.CanBeUsedBy(p);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (activated)
            {
                var speedMult = isLowering ? 0.1f : 0.02f;
                var speed = 0.1f * speedMult;
                thumpDuration--;
                if (thumpDuration <= 0)
                {
                    MoveDown(speed);
                    if (infestationSpawned is false)
                    {
                        var wave = GameComponent_Insectoids.Instance.GetNextInsectWave(Props.wave);
                        SpawnInfestation(wave);
                        infestationSpawned = true;
                    }
                }
                else
                {
                    if (isLowering)
                    {
                        MoveDown(speed);
                    }
                    else
                    {
                        MoveUp(speed);
                    }
                }
            }
        }

        public void SpawnInfestation(InsectWave wave)
        {
            GameComponent_Insectoids.Instance.lastInsectoidBossArrival = Find.TickManager.TicksGame;
            GameComponent_Insectoids.Instance.lastWavesIndices[Props.wave] = Props.wave.waves.IndexOf(wave);
            var hives = new List<LargeTunnelHiveSpawner>();
            var hiveCount = wave.insects.First().count;
            IncidentWorker_LargeInfestation.SpawnTunnels(hiveCount, parent.Map, parent.Position, hives);
            var dict = new Dictionary<PawnKindDef, List<List<PawnKindDef>>>();
            foreach (var pawnCount in wave.insects)
            {
                var list = new List<PawnKindDef>();
                for (int i = 0; i < pawnCount.count; i++)
                {
                    list.Add(pawnCount.kindDef);
                }
                dict[pawnCount.kindDef] = list.Split(hives.Count);
            }

            for (var i = 0; i < hives.Count; i++)
            {
                var hive = hives[i];
                hive.spawnHive = false;
                hive.otherHives = hives.Where(x => x != hive).ToList();
                hive.pawnsToSpawn = new List<PawnKindDef>();
                foreach (var pawnCount in wave.insects)
                {
                    var list = dict[pawnCount.kindDef];
                    if (i < list.Count)
                    {
                        hive.pawnsToSpawn.AddRange(list[i]);
                    }
                }
            }

            var insectLeaderKind = wave.insects.First().kindDef;
            Find.LetterStack.ReceiveLetter("VFEI_LetterLabelInsectWaveArrived".Translate(
                GenLabel.BestKindLabel(insectLeaderKind, Gender.None).CapitalizeFirst()),
                "VFEI_LetterInsectWaveArrived".Translate(Faction.OfInsects.NameColored, wave.GetPawnList()),
                VFEI_DefOf.VFEI_InsectWaveArrived, parent);
        }


        private void MoveUp(float speed)
        {
            topGraphicOffset = Mathf.Min(topGraphicOffset + speed, 0.05f);
            if (topGraphicOffset >= 0.05f)
            {
                isLowering = true;
            }
        }

        private void MoveDown(float speed)
        {
            var oldValue = topGraphicOffset;
            topGraphicOffset = Mathf.Max(topGraphicOffset - speed, 0);
            if (topGraphicOffset <= 0 && oldValue > 0)
            {
                isLowering = false;
                FleckMaker.Static(parent.DrawPos, parent.Map, FleckDefOf.PsycastAreaEffect, 5f);
                if (soundThumping is null)
                {
                    soundThumping = new List<SoundDef> {VFEI_DefOf.VFEI2_InsectThumperOne,
                        VFEI_DefOf.VFEI2_InsectThumperTwo, VFEI_DefOf.VFEI2_InsectThumperThree,
                        VFEI_DefOf.VFEI2_InsectThumperFour}.RandomElement();
                }
                soundThumping.PlayOneShot(parent);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref activated, "activated");
            Scribe_Values.Look(ref thumpDuration, "thumpDuration");
            Scribe_Values.Look(ref topGraphicOffset, "topGraphicOffset");
            Scribe_Values.Look(ref isLowering, "isLowering");
            Scribe_Values.Look(ref infestationSpawned, "infestationSpawned");
            Scribe_Defs.Look(ref soundThumping, "soundThumping");
        }
    }
}
