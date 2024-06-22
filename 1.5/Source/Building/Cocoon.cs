using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace VFEInsectoids
{
    public class CocoonSpecific : ThingWithComps
    {

    }

    public class Cocoon : ThingWithComps
    {
        private bool once = true;
        private int timeBeforeInsect;
        private int timeBeforeInsectString;
        public WeightedInsectoids[] array = new WeightedInsectoids[5];

        public struct WeightedInsectoids
        {

            public PawnKindDef pawn;
            public float weight;
           
        }

        public Cocoon()
        {
            
            WeightedInsectoids insectoid = new WeightedInsectoids
            {
                pawn = VFEI_DefOf.Megascarab,
                weight = 0.5f
            };
            array[0] = insectoid;
            insectoid = new WeightedInsectoids
            {
                pawn = VFEI_DefOf.Spelopede,
                weight = 0.25f
            };
            array[1] = insectoid;
            insectoid = new WeightedInsectoids
            {
                pawn = VFEI_DefOf.Megaspider,
                weight = 0.15f
            };
            array[2] = insectoid;
            insectoid = new WeightedInsectoids
            {
                pawn = VFEI_DefOf.VFEI2_Megapede,
                weight = 0.05f
            };
            array[3] = insectoid;
            insectoid = new WeightedInsectoids
            {
                pawn = VFEI_DefOf.VFEI2_Queen,
                weight = 0.03f
            };
            array[4] = insectoid;

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.timeBeforeInsect, "timeBeforeInsect");
            Scribe_Values.Look<int>(ref this.timeBeforeInsectString, "timeBeforeInsectString");
            Scribe_Values.Look<bool>(ref this.once, "onceCocoonDev");
        }

        public override string GetInspectString()
        {
            return "VFEI_CocoonInsectSpawnIn".Translate(timeBeforeInsectString.ToStringTicksToPeriod());
        }

        public override void Tick()
        {
            base.Tick();
            if (once) {
                int timeToGo = new IntRange(10000, 30000).RandomInRange;  
                this.timeBeforeInsect = Find.TickManager.TicksGame + timeToGo; 
                timeBeforeInsectString = timeToGo; 
                once = false;           
            }
            if (Find.TickManager.TicksGame == this.timeBeforeInsect)
            {
                CellFinder.TryFindRandomReachableCellNearPosition(this.Position,this.Position, this.Map, 4, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out IntVec3 c);
                FilthMaker.TryMakeFilth(c, this.Map, ThingDefOf.Filth_Slime);
                VFEI_DefOf.Hive_Spawn.PlayOneShot(new TargetInfo(this.Position, this.Map));

               

                
                    Pawn p = PawnGenerator.GeneratePawn(array.RandomElementByWeight(x => x.weight).pawn, this.Faction);
                    p.ageTracker.AgeBiologicalTicks = 30000;
                    GenSpawn.Spawn(p, this.Position, this.Map);
                    List<Pawn> pawns = new List<Pawn> { p };
                    if (this.Map.ParentFaction == this.Faction)
                    {
                        LordMaker.MakeNewLord(this.Faction, new LordJob_DefendBase(this.Faction, this.Map.Center), this.Map, pawns);
                    }
                    else
                    {
                        pawns.ForEach(p1 => p1.mindState.spawnedByInfestationThingComp = true);
                        SpawnedPawnParams spp = new SpawnedPawnParams
                        {
                            aggressive = false,
                            defSpot = Position,
                            defendRadius = 5
                        };
                        LordMaker.MakeNewLord(this.Faction, new LordJob_DefendAndExpandHive(spp), this.Map, pawns);
                    }
                
                this.Destroy();
            }
            timeBeforeInsectString--;
        }
    }
}