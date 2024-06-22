using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace VFEInsectoids
{

    public enum InsectType { Worker, Defender, Hunter};
    public class PawnKindWithType
    {
        public PawnKindDef insect;
        public InsectType insectType;
    }

    public class CompProperties_Hive : CompProperties
    {
        public List<PawnKindWithType> insectTypes;
        public int insectoidRespawnTime;
        public SoundDef spawnSound;
        public CompProperties_Hive()
        {
            this.compClass = typeof(CompHive);
        }
    }

    [HotSwappable]
    public class CompHive : ThingComp
    {
        public CompProperties_Hive Props => base.props as CompProperties_Hive;

        public List<Pawn> insects = new List<Pawn>();

        public Lord lord;

        public PawnKindDef currentPawnKindToSpawn;

        public Color insectColor;

        private int? nextRespawnTick;

        public int InsectCapacity
        {
            get
            {
                var baseValue = 3;
                if (VFEI_DefOf.VFEI2_StandardHivetech.IsFinished)
                {
                    baseValue += 1;
                }
                if (VFEI_DefOf.VFEI2_ExoticHivetech.IsFinished)
                {
                    baseValue += 2;
                }
                return baseValue;
            }
        }

        public List<PawnKindDef> AllAvailableInsects
        {
            get
            {
                var list = new List<PawnKindDef>();
                return Props.insectTypes.Select(x => x.insect).ToList();
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (currentPawnKindToSpawn is null)
            {
                currentPawnKindToSpawn = AllAvailableInsects.First();
                insectColor = VFEI_DefOf.Structure_BrownSubtle.color;
                if (CanSpawn())
                {
                    SetNextRespawnTick();
                }
            }
        }

        private void SetNextRespawnTick()
        {
            nextRespawnTick = Find.TickManager.TicksGame + Props.insectoidRespawnTime;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent.Faction == Faction.OfPlayer)
            {
                yield return new Gizmo_Hive
                {
                    compHive = this
                };
                if (DebugSettings.ShowDevGizmos && CanSpawn())
                {
                    yield return new Command_Action
                    {
                        defaultLabel = "DEV: Spawn " + currentPawnKindToSpawn.label,
                        action = delegate
                        {
                            DoSpawn();
                        }
                    };
                }
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            foreach (var insect in insects)
            {
                RemoveInsect(insect);
            }
            lord?.RemoveAllBuildings();
            lord?.RemoveAllPawns();
        }

        public override void CompTick()
        {
            base.CompTick();
            if (CanSpawn())
            {
                if (nextRespawnTick is null)
                {
                    SetNextRespawnTick();
                }
                if (Find.TickManager.TicksGame >= nextRespawnTick)
                {
                    DoSpawn();
                }
            }
        }

        private void DoSpawn()
        {
            TrySpawnPawn();
            if (insects.Count >= InsectCapacity)
            {
                nextRespawnTick = null;
                Messages.Message("VFEI_SpawningStoppedMaxCapacity".Translate(), parent, MessageTypeDefOf.NeutralEvent);
            }
            else
            {
                SetNextRespawnTick();
            }
        }

        private bool CanSpawn()
        {
            return insects.Count < InsectCapacity;
        }

        private void TrySpawnPawn()
        {
            PawnGenerationRequest request = new PawnGenerationRequest(currentPawnKindToSpawn, parent.Faction);
            int index = currentPawnKindToSpawn.lifeStages.Count - 1;
            request.FixedBiologicalAge = currentPawnKindToSpawn.race.race.lifeStageAges[index].minAge;
            var pawn = PawnGenerator.GeneratePawn(request);
            AddInsect(pawn);
            GenSpawn.Spawn(pawn, parent.Position, parent.Map);
            if (Props.spawnSound != null)
            {
                Props.spawnSound.PlayOneShot(parent);
            }
        }

        public void ChangePawnKind(PawnKindDef def)
        {
            currentPawnKindToSpawn = def;
            foreach (var insect in insects)
            {
                SpawnCocoon(insect);
            }
        }

        public void SpawnCocoon(Pawn insect)
        {

        }

        public void AddInsect(Pawn insect)
        {
            if (insect.IsColonyInsect(out var hediff))
            {
                insect.health.RemoveHediff(hediff);
            }
            var hediffDef = Props.insectTypes.First(x => x.insect == currentPawnKindToSpawn).insectType.GetInsectTypeHediff();
            hediff = insect.health.AddHediff(hediffDef) as Hediff_InsectType;
            hediff.hive = this.parent;
            insects.Add(insect);
            var otherLord = insect.GetLord();
            if (otherLord != null)
            {
                otherLord.RemovePawn(insect);
            }
            if (lord is not null)
            {
                lord.AddPawn(insect);
            }
            else
            {
                lord = LordMaker.MakeNewLord(parent.Faction, new LordJob_PlayerHive(parent), parent.Map);
                lord.AddBuilding(parent as Building);
                lord.AddPawn(insect);
            }
        }

        public void RemoveInsect(Pawn insect)
        {
            if (insect.IsColonyInsect(out var hediff))
            {
                insect.health.RemoveHediff(hediff);
            }
            insects.Remove(insect);
            var lordJob = insect.GetLord()?.LordJob as LordJob_PlayerHive;
            if (lordJob != null)
            {
                lordJob.lord.RemovePawn(insect);
            }
        }

        public override string CompInspectStringExtra()
        {
            if (CanSpawn() && nextRespawnTick is not null)
            {
                var period = nextRespawnTick.Value - Find.TickManager.TicksGame;
                return "VFEI_InsectoidWillBeSpawnedIn".Translate(currentPawnKindToSpawn.LabelCap, period.ToStringTicksToPeriod());
            }
            return base.CompInspectStringExtra();
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref insects, "insects", LookMode.Reference);
            Scribe_Values.Look(ref insectColor, "insectColor");
            Scribe_Values.Look(ref nextRespawnTick, "nextRespawnTick");
            Scribe_References.Look(ref lord, "lord");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                insects ??= new List<Pawn>();
            }
        }
    }
}
