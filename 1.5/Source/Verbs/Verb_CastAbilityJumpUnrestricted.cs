
using RimWorld;
using RimWorld.Utility;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    public class Verb_CastAbilityJumpUnrestricted : Verb_CastAbility
    {
        private float cachedEffectiveRange = -1f;

        public override bool MultiSelect => true;

        public virtual ThingDef JumpFlyerDef => ThingDefOf.PawnFlyer;

        public override float EffectiveRange
        {
            get
            {
                if (cachedEffectiveRange < 0f)
                {
                    if (EquipmentSource != null)
                    {
                        cachedEffectiveRange = EquipmentSource.GetStatValue(StatDefOf.JumpRange);
                    }
                    else
                    {
                        cachedEffectiveRange = verbProps.range;
                    }
                }
                return cachedEffectiveRange;
            }
        }

        public override bool TryCastShot()
        {
            if (base.TryCastShot())
            {
                return JumpUtility.DoJump(CasterPawn, currentTarget, ReloadableCompSource, verbProps, ability, CurrentTarget, JumpFlyerDef);
            }
            return false;
        }

        public override void OnGUI(LocalTargetInfo target)
        {
            if (JumpUtility.ValidJumpTarget(caster.Map, target.Cell))
            {
                base.OnGUI(target);
            }
            else
            {
                GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
            }
        }

        public override void OrderForceTarget(LocalTargetInfo target)
        {
            JumpUtility.OrderJump(CasterPawn, target, this, EffectiveRange);
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (caster == null)
            {
                return false;
            }
            if (!JumpUtility.ValidJumpTarget(caster.Map, target.Cell))
            {
                return false;
            }
            if (!ReloadableUtility.CanUseConsideringQueuedJobs(CasterPawn, EquipmentSource))
            {
                return false;
            }
            return true;
        }

        public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
        {
            return CheckCanHitTargetFrom(CasterPawn, root, targ, EffectiveRange);
        }

        public override void DrawHighlight(LocalTargetInfo target)
        {
            if (target.IsValid && JumpUtility.ValidJumpTarget(caster.Map, target.Cell))
            {
                GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.MetaOverlays);
            }
            //GenDraw.DrawRadiusRing(caster.Position, EffectiveRange, Color.white, (IntVec3 c) => GenSight.LineOfSight(caster.Position, c, caster.Map) && JumpUtility.ValidJumpTarget(caster.Map, c));
        }

        public static bool CheckCanHitTargetFrom(Pawn pawn, IntVec3 root, LocalTargetInfo targ, float range)
        {
            float num = range * range;
            IntVec3 cell = targ.Cell;
            if ((float)pawn.Position.DistanceToSquared(cell) <= num)
            {
                return true;
            }
            return false;
        }
    }
}