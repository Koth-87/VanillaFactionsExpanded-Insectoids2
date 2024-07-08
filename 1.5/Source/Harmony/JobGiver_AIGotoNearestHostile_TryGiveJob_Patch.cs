
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace VFEInsectoids
{
    [HarmonyPatch(typeof(JobGiver_AIGotoNearestHostile), "TryGiveJob")]
    public static class JobGiver_AIGotoNearestHostile_TryGiveJob_Patch
    {
        public static void Postfix(JobGiver_AIGotoNearestHostile __instance, ref Job __result, Pawn pawn)
        {
            if (__result is null && pawn.abilities != null)
            {
                foreach (var ability in pawn.abilities.abilities)
                {
                    var verb = ability.verb as Verb_CastAbilityJumpUnrestricted;
                    if (verb != null && ability.CanCast)
                    {
                        float num = float.MaxValue;
                        (Thing thing, IntVec3 cell) thingWithTarget = default;
                        List<IAttackTarget> potentialTargetsFor = pawn.Map.attackTargetsCache.GetPotentialTargetsFor(pawn);
                        for (int i = 0; i < potentialTargetsFor.Count; i++)
                        {
                            IAttackTarget attackTarget = potentialTargetsFor[i];
                            if (!attackTarget.ThreatDisabled(pawn) &&
                                AttackTargetFinder.IsAutoTargetable(attackTarget) &&
                                (!__instance.humanlikesOnly || !(attackTarget is Pawn pawn2)
                                || pawn2.RaceProps.Humanlike) && (!(attackTarget.Thing is Pawn pawn3)
                                || pawn3.IsCombatant()))
                            {
                                Thing thing2 = (Thing)attackTarget;
                                int num2 = thing2.Position.DistanceToSquared(pawn.Position);
                                if ((float)num2 < num && TryGetTargetCell(pawn, thing2, verb, out var result))
                                {
                                    thingWithTarget = result;
                                    num = num2;
                                }
                            }
                        }

                        if (thingWithTarget != default)
                        {
                            Job job = ability.GetJob(thingWithTarget.cell, thingWithTarget.cell);
                            __result = job;
                        }
                    }
                }
            }
        }

        public static bool TryGetTargetCell(Pawn pawn, Thing thing2, Verb verb, out (Thing thing, IntVec3 cell) targetWithCell)
        {
            targetWithCell = default;
            if (verb.OutOfRange(pawn.Position, thing2.Position, thing2.OccupiedRect()) is false)
            {
                var targets = GenAdj.CellsAdjacent8Way(thing2).Where(x => x != thing2.Position 
                    && x.GetRoom(pawn.Map) == thing2.GetRoom()).Distinct().OrderBy(x => x.DistanceTo(pawn.Position));
                foreach (var target in targets)
                {
                    if (Verb_CastAbilityJumpUnrestricted.CheckCanHitTargetFrom(pawn, pawn.Position, target, verb.EffectiveRange)
                        && verb.OutOfRange(pawn.Position, target, CellRect.SingleCell(target)) is false
                        && target.WalkableBy(pawn.Map, pawn) && verb.ValidateTarget(target, false))
                    {
                        targetWithCell = (thing2, target);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}