using Verse;
using RimWorld;


namespace VFEInsectoids
{
    public class CompCreateBurrow : CompAbilityEffect
    {
        public new CompProperties_CreateBurrow Props
        {
            get
            {
                return (CompProperties_CreateBurrow)this.props;
            }
        }

        public override bool GizmoDisabled(out string reason)
        {
            if (parent.pawn.Faction.IsPlayer)
            {
                reason = "VFEI_NotForPlayers".Translate();
                return true;
            }
            return base.GizmoDisabled(out reason);
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            IntVec3 loc = target.Cell;
            Map map = this.parent.pawn.Map;

            

            if (loc.InBounds(map) && loc.GetEdifice(map) == null)
            {

                var tunnelHiveSpawner = (BurrowSpawner)ThingMaker.MakeThing(Props.tunnel);
                tunnelHiveSpawner.thingsToSpawn.Add(Props.building);
                tunnelHiveSpawner.faction = this.parent.pawn.Faction.def;
                tunnelHiveSpawner.size = Props.size;
                GenSpawn.Spawn(tunnelHiveSpawner, loc, map, WipeMode.FullRefund);
               
            }
           
            base.Apply(target, dest);

        }

        public override bool AICanTargetNow(LocalTargetInfo target)
        {
            return true;
        }
    }
}
