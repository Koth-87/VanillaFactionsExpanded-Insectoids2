using RimWorld;
using System.Collections.Generic;
using Verse;

namespace VFEInsectoids
{
    public class CompProperties_TargetableHive : CompProperties_Targetable
    {
        public ThingDef hive;
        public CompProperties_TargetableHive()
        {
            this.compClass = typeof(CompTargetable_Hive);
        }
    }

    public class CompTargetable_Hive : CompTargetable
    {
        public override bool PlayerChoosesTarget => true;

        public CompProperties_TargetableHive Props => base.props as CompProperties_TargetableHive;

        public override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPawns = false,
                canTargetBuildings = true,
                canTargetItems = false,
            };
        }

        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            yield return targetChosenByPlayer;
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (target.Thing?.def != Props.hive)
            {
                if (showMessages)
                {
                    Messages.Message("VFEI_PherocoreCanBeUsedOnHive".Translate(parent.LabelCap, Props.hive.label), MessageTypeDefOf.RejectInput);
                }
                return false;
            }
            else
            {
                var comp = target.Thing.TryGetComp<CompHive>();
                if (comp.InsectCapacity >= CompHive.MaxInsectCapacity)
                {
                    if (showMessages)
                    {
                        Messages.Message("VFEI_HiveCannotBeUpgradedMore".Translate(Props.hive.label), MessageTypeDefOf.RejectInput);
                    }
                    return false;
                }
            }
            return base.ValidateTarget(target.Thing, showMessages);
        }
    }
}
