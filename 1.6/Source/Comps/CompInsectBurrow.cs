
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace VFEInsectoids
{

    public class WeightedAnimals
    {

        public PawnKindDef pawn;
        public float weight;

    }


    public class CompInsectBurrow : ThingComp
    {



        public CompProperties_InsectBurrow Props => (CompProperties_InsectBurrow)props;

        public int nextSpawnTick;
        public int tickCounter;
        public int collapseCounter;

        public override void CompTick()
        {
            tickCounter--;
            if (tickCounter <= 0 && collapseCounter>0)
            {
                SpawnInsectoids();
                collapseCounter--;
                
                nextSpawnTick = (int)(Props.spawningDelayMultiplier * nextSpawnTick);
                tickCounter = nextSpawnTick;
                if (collapseCounter <= 0)
                {
                    this.parent.Kill(null);
                }
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            collapseCounter = Props.maxWavesToSpawn;
            nextSpawnTick = Props.spawningStartingTimer;
            tickCounter = nextSpawnTick;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.nextSpawnTick, "nextSpawnTick");
            Scribe_Values.Look<int>(ref this.collapseCounter, "collapseCounter");
            Scribe_Values.Look<int>(ref this.tickCounter, "tickCounter");
        }

        public void SpawnInsectoids()
        {
            CellRect cellRect = GenAdj.OccupiedRect(this.parent.Position, Rot4.North, this.parent.def.Size);
            List<PawnFlyer> list = new List<PawnFlyer>();
            List<IntVec3> list2 = new List<IntVec3>();

          

            WeightedAnimals insectoidList;
            Props.insectoidsToSpawn.TryRandomElementByWeight((WeightedAnimals x) => x.weight, out insectoidList);
            PawnKindDef chosenPawn = insectoidList.pawn;
         
            Pawn p = PawnGenerator.GeneratePawn(chosenPawn, this.parent.Faction);
            p.ageTracker.AgeBiologicalTicks = 3600000;
            IntVec3 randomCell = cellRect.RandomCell;
            GenSpawn.Spawn(p, randomCell, this.parent.Map);
            if (CellFinder.TryFindRandomCellNear(this.parent.Position, this.parent.Map, 3, (IntVec3 c) => !c.Fogged(this.parent.Map) && c.Walkable(this.parent.Map) && !c.Impassable(this.parent.Map), out IntVec3 result))
            {
                p.rotationTracker.FaceCell(result);
                list.Add(PawnFlyer.MakeFlyer(VFEI_DefOf.VFEI2_PawnFlyer_Stun, p, result, null, null, flyWithCarriedThing: false, randomCell.ToVector3()));
                list2.Add(randomCell);
            }

            if (list2.Count != 0)
            {
                SpawnRequest spawnRequest = new SpawnRequest(list.Cast<Thing>().ToList(), list2, 1, 1f);
                spawnRequest.initialDelay = 400;
                this.parent.Map.deferredSpawner.AddRequest(spawnRequest);
                VFEI_DefOf.Hive_Spawn.PlayOneShot(this.parent);
                List<Pawn> pawns = new List<Pawn> { p };

                if (this.parent.Map.IsInfested())
                {
                    WildAnimalSpawner_SpawnRandomWildAnimalAt_Patch.TryAddLordJob(p, null);
                }
                else
                {

                    Lord lord = null;
                    if (this.parent.Map.mapPawns.SpawnedPawnsInFaction(Faction.OfInsects).Any((Pawn x) => x != p))
                    {
                        lord = ((Pawn)GenClosest.ClosestThing_Global(this.parent.Position, this.parent.Map.mapPawns.SpawnedPawnsInFaction(Faction.OfInsects), 99999f, (Thing x) => x != p && ((Pawn)p).GetLord() != null)).GetLord();
                    }
                    if (lord == null || !lord.CanAddPawn(p))
                    {
                        lord = LordMaker.MakeNewLord(Faction.OfInsects, new LordJob_AssaultColony(Faction.OfInsects, canKidnap: true, canTimeoutOrFlee: false), parent.Map);
                    }
                    if (lord != null && lord.LordJob.CanAutoAddPawns)
                    {
                        lord.AddPawn(p);
                    }

                }
            }

        }

        public override string CompInspectStringExtra()
        {
            return "VFEI_NextInsectBurrowing".Translate(tickCounter.ToStringTicksToPeriod()) +"\n"+
                "VFEI_BurrowWaves".Translate(Props.maxWavesToSpawn- collapseCounter,Props.maxWavesToSpawn);



        }


       

    }
}