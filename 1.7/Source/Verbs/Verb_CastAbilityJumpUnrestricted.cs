
using RimWorld;
using RimWorld.Utility;
using UnityEngine;
using Verse;
using Verse.AI;

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
                return JumpUtility.DoJump(CasterPawn, currentTarget, ReloadableCompSource, verbProps, 
                    ability, CurrentTarget, JumpFlyerDef);
            }
            return false;
        }

        public override void OnGUI(LocalTargetInfo target)
        {
            if (JumpUtility.ValidJumpTarget(caster, caster.Map, target.Cell))
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
            OrderJump(CasterPawn, target, this, EffectiveRange);
        }

        public static void OrderJump(Pawn pawn, LocalTargetInfo target, Verb verb, float range)
        {
            Map map = pawn.Map;
            Job job = JobMaker.MakeJob(JobDefOf.CastJump, target.Cell);
            job.verbToUse = verb;
            if (pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc))
            {
                FleckMaker.Static(target.Cell, map, FleckDefOf.FeedbackGoto);
            }
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (caster == null)
            {
                return false;
            }
            if (!JumpUtility.ValidJumpTarget(caster, caster.Map, target.Cell))
            {
                return false;
            }
            if (target.Cell.Roofed(caster.Map))
            {
                return false;
            }
            if (OutOfRange(caster.Position, target.Cell, CellRect.SingleCell(target.Cell)))
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
            if (target.IsValid && ValidateTarget(target.Cell, false))
            {
                GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.MetaOverlays);
            }
            GenDraw.DrawRadiusRing(caster.Position, EffectiveRange, Color.white, (IntVec3 c) => ValidateTarget(c, false));
        }

        public static bool CheckCanHitTargetFrom(Pawn pawn, IntVec3 root, LocalTargetInfo targ, float range)
        {
            float num = range * range;
            IntVec3 cell = targ.Cell;
            if (cell.WalkableBy(pawn.Map, pawn) is false)
            {
                return false;
            }
            if ((float)pawn.Position.DistanceToSquared(cell) <= num)
            {
                return true;
            }
            return false;
        }
    }
}