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

    public class CompProperties_Hive : CompProperties_SpawnerPawn
    {
        public List<PawnKindWithType> insectTypes;
        public int insectoidRespawnTime;
        public CompProperties_Hive()
        {
            this.compClass = typeof(CompHive);
        }
    }

    [HotSwappable]
    public class CompHive : CompSpawnerPawn
    {
        public CompProperties_Hive Props => base.props as CompProperties_Hive;

        public Lord lord;

        public Color insectColor;

        public List<Pawn> insects = new List<Pawn>();

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

        public List<PawnKindDef> AllAvailableInsects => Props.insectTypes.Select(x => x.insect).ToList();

        public override void Initialize(CompProperties props)
        {
            this.props = props;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            pawnsLeftToSpawn = 0;
            if (chosenKind is null)
            {
                chosenKind = AllAvailableInsects.First();
                insectColor = VFEI_DefOf.Structure_BrownSubtle.color;
                if (CanSpawn())
                {
                    SetNextRespawnTick();
                }
            }
        }

        private void SetNextRespawnTick()
        {
            nextPawnSpawnTick = Find.TickManager.TicksGame + Props.insectoidRespawnTime;
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
                        defaultLabel = "DEV: Spawn " + chosenKind.label,
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
            foreach (var insect in insects.ToList())
            {
                RemoveInsect(insect);
            }
            if (lord != null)
            {
                lord.RemoveAllBuildings();
                lord.RemoveAllPawns();
                previousMap.lordManager.RemoveLord(lord);
            }
        }

        public override void CompTick()
        {
            if (CanSpawn())
            {
                if (nextPawnSpawnTick == -1)
                {
                    SetNextRespawnTick();
                }
                if (Find.TickManager.TicksGame >= nextPawnSpawnTick)
                {
                    DoSpawn();
                }
            }
            else
            {
                nextPawnSpawnTick = -1;
            }
        }

        private void DoSpawn()
        {
            TrySpawnPawn(parent.Position);
            if (insects.Count >= InsectCapacity)
            {
                nextPawnSpawnTick = -1;
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

        public void TrySpawnPawn(IntVec3 position)
        {
            PawnGenerationRequest request = new PawnGenerationRequest(chosenKind, parent.Faction);
            int index = chosenKind.lifeStages.Count - 1;
            request.FixedBiologicalAge = chosenKind.race.race.lifeStageAges[index].minAge;
            var pawn = PawnGenerator.GeneratePawn(request);
            AddInsect(pawn);
            GenSpawn.Spawn(pawn, position, parent.Map);
            if (Props.spawnSound != null)
            {
                Props.spawnSound.PlayOneShot(pawn);
            }
        }

        public void ChangePawnKind(PawnKindDef def)
        {
            chosenKind = def;
            foreach (var insect in insects.ToList())
            {
                SpawnCocoon(insect);
            }
        }

        public void SpawnCocoon(Pawn insect)
        {
            var pos = insect.Position;
            var cocoon = GenSpawn.Spawn(VFEI_DefOf.VFEI2_InsectoidCocoonHive, pos, parent.Map) 
                as CocoonHive;
            insect.DeSpawn();
            cocoon.innerContainer.TryAdd(insect);
            cocoon.hive = this.parent;
            cocoon.spawnInTick = 6000;
        }

        public void AddInsect(Pawn insect)
        {
            if (insect.IsColonyInsect(out var hediff))
            {
                insect.health.RemoveHediff(hediff);
            }
            var hediffDef = Props.insectTypes.First(x => x.insect == chosenKind).insectType.GetInsectTypeHediff();
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
            if (CanSpawn() && nextPawnSpawnTick != -1)
            {
                var period = nextPawnSpawnTick - Find.TickManager.TicksGame;
                return "VFEI_InsectoidWillBeSpawnedIn".Translate(chosenKind.LabelCap, period.ToStringTicksToPeriod());
            }
            return null;
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref insectColor, "insectColor");
            Scribe_Collections.Look(ref insects, "insects", LookMode.Reference);
            Scribe_References.Look(ref lord, "lord");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                insects ??= new List<Pawn>();
            }
        }
    }
}
