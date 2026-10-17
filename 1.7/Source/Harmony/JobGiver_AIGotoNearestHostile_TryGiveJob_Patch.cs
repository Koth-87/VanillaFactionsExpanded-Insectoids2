
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
                                AttackTargetFinder.IsAutoTargetable(attackTarget) 
                                && (!(attackTarget.Thing is Pawn pawn3) || pawn3.IsCombatant()))
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

        public static bool TryGetTargetCell(Pawn pawn, Thing thing, Verb verb, 
            out (Thing thing, IntVec3 cell) targetWithCell)
        {
            targetWithCell = default;
            if (pawn.Position.Roofed(pawn.Map) is false && verb.OutOfRange(pawn.Position, thing.Position, thing.OccupiedRect()) is false 
                && pawn.Position.DistanceTo(thing.Position) > 2)
            {
                var targets = GenAdj.CellsAdjacent8Way(thing).Where(x => x != thing.Position 
                    && x.GetRoom(pawn.Map) == thing.GetRoom()).Distinct().OrderBy(x => x.DistanceTo(pawn.Position));
                foreach (var target in targets)
                {
                    if (Verb_CastAbilityJumpUnrestricted.CheckCanHitTargetFrom(pawn, pawn.Position, target, verb.EffectiveRange)
                        && verb.OutOfRange(pawn.Position, target, CellRect.SingleCell(target)) is false
                        && target.WalkableBy(pawn.Map, pawn) && target.GetFirstPawn(pawn.Map) is null 
                        && verb.ValidateTarget(target, false))
                    {
                        targetWithCell = (thing, target);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}