
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
                                if ((float)num2 < num)
                                {
                                    var targets = GenSight.PointsOnLineOfSight(pawn.Position, thing2.Position)
                                        .Where(x => x.GetRoom(pawn.Map) == thing2.GetRoom() 
                                        && x.Roofed(pawn.Map) is false).Reverse().Skip(1);
                                    foreach (var target in targets)
                                    {
                                        if (Verb_CastAbilityJumpUnrestricted.CheckCanHitTargetFrom(pawn, pawn.Position, target, verb.EffectiveRange) 
                                            && verb.OutOfRange(pawn.Position, target, CellRect.SingleCell(target)) is false
                                            && target.WalkableBy(pawn.Map, pawn))
                                        {
                                            thingWithTarget = (thing2, target);
                                            break;
                                        }
                                    }
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
    }
}