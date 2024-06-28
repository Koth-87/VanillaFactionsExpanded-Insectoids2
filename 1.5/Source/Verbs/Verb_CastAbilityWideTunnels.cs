
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;


namespace VFEInsectoids
{
    public class Verb_CastAbilityWideTunnels : Verb_CastAbility
    {

     

        public override bool TryCastShot()
        {
            if (base.TryCastShot())
            {
                return DoJump(CasterPawn, currentTarget, ReloadableCompSource, verbProps);
            }
            return false;

        }

        public static bool DoJump(Pawn pawn, LocalTargetInfo currentTarget, CompApparelReloadable comp, VerbProperties verbProps, Ability triggeringAbility = null, LocalTargetInfo target = default(LocalTargetInfo), ThingDef pawnFlyerOverride = null)
        {

            IntVec3 position = pawn.Position;
            IntVec3 cell = currentTarget.Cell;
            Map map = pawn.Map;
            bool flag = Find.Selector.IsSelected(pawn);
            Tunneler pawnFlyer = (Tunneler)PawnFlyer.MakeFlyer(VFEI_DefOf.VFEI2_Tunneler, pawn, cell, verbProps.flightEffecterDef, verbProps.soundLanding, verbProps.flyWithCarriedThing, null, triggeringAbility, target);

            if (pawnFlyer != null)
            {
                FleckMaker.ThrowDustPuff(position.ToVector3Shifted() + Gen.RandomHorizontalVector(0.5f), map, 2f);
                GenSpawn.Spawn(pawnFlyer, cell, map);
                if (flag)
                {
                    Find.Selector.Select(pawn, playSound: false, forceDesignatorDeselect: false);
                }
                return true;
            }
            return false;
        }

      





    }
}
