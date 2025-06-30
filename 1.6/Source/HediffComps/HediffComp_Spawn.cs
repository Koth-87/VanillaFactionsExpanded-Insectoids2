using RimWorld;
using Verse;
using Verse.Sound;
using System.Collections.Generic;

namespace VFEInsectoids
{
    public class HediffComp_Spawn : HediffComp
    {
      


        public HediffCompProperties_Spawn Props
        {
            get
            {
                return (HediffCompProperties_Spawn)this.props;
            }
        }

    


        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {         
            Map map = this.parent.pawn.Corpse.Map;
            IntVec3 pos = this.parent.pawn.Corpse.PositionHeld;
            Name name = this.parent.pawn.Name;
            if (map != null && this.parent.Severity > 0.99)
            {
                Hatch(map,pos,name);
                for (int i = 0; i < 20; i++)
                {
                    IntVec3 c;
                    CellFinder.TryFindRandomReachableCellNearPosition(this.parent.pawn.Corpse.Position, this.parent.pawn.Corpse.Position, map, 2, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out c);
                    FilthMaker.TryMakeFilth(c, this.parent.pawn.Corpse.Map, ThingDefOf.Filth_Blood);
                }

                VFEI_DefOf.Hive_Spawn.PlayOneShot(new TargetInfo(this.parent.pawn.Corpse.Position, map, false));
            }
        }

        public void Hatch(Map map, IntVec3 pos,Name name)
        {
      
            PawnGenerationRequest request = new PawnGenerationRequest(Props.spawn, Faction.OfPlayerSilentFail, fixedBiologicalAge: 0);
            Pawn newinsectoid = PawnGenerator.GeneratePawn(request);
            GenSpawn.Spawn(newinsectoid, CellFinder.RandomClosewalkCellNear(pos, map, 3, null), map, WipeMode.Vanish);
            newinsectoid.Name = name;
            newinsectoid.SetFaction(Faction.OfPlayer);
        }


        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            base.CompGetGizmos();
            if (!DebugSettings.ShowDevGizmos)
            {
                yield break;
            }

            yield return new Command_Action
            {
                defaultLabel = "DEV: Advance severity",
                defaultDesc = "haha insectoid goes brrrr",
                action = delegate
                {
                    this.parent.Severity += 0.1f;
                }

            };



        }
    }
}


