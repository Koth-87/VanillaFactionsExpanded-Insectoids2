using RimWorld;
using Verse;
using Verse.AI.Group;

namespace VFEInsectoids
{

    public class Hediff_InsectWorker : Hediff_InsectType
    {
        public override InsectType InsectType => InsectType.Worker;
    }
    public class Hediff_InsectHunter : Hediff_InsectType
    {
        public override InsectType InsectType => InsectType.Hunter;
    }
    public class Hediff_InsectDefender : Hediff_InsectType
    {
        public override InsectType InsectType => InsectType.Defender;
    }

    public abstract class Hediff_InsectType : HediffWithComps
    {
        public Thing hive;
        private CompHive _compHive;
        public CompHive CompHive =>_compHive ??= hive?.TryGetComp<CompHive>();
        public abstract InsectType InsectType { get; }
        public override bool ShouldRemove => CompHive is null || CompHive.parent.Destroyed 
            || CompHive.parent.MapHeld != pawn.MapHeld && pawn.Spawned && 
            pawn.GetLord().LordJob is not LordJob_FormAndSendCaravan;

        public override void PostRemoved()
        {
            base.PostRemoved();
            CompHive?.RemoveInsect(pawn);
        }

        public override void Notify_PawnKilled()
        {
            base.Notify_PawnKilled();
            CompHive?.RemoveInsect(pawn);
        }

        public override void Notify_Spawned()
        {
            base.Notify_Spawned();
            UpdateArea();
        }

        public override void Tick()
        {
            base.Tick();
            if (pawn.IsHashIntervalTick(60))
            {
                var lord = pawn.GetLord();
                if (lord is null)
                {
                    CompHive.lord.AddPawn(pawn);
                }
                else if (hive is Pawn pawnHive && pawnHive.GetLord() is Lord lord2
                    && lord2.LordJob is LordJob_FormAndSendCaravan lordCaravan)
                {
                    lord.RemovePawn(pawn);
                    lord2.AddPawn(pawn);
                    if (pawn.jobs.curDriver?.asleep == true)
                    {
                        pawn.jobs.StopAll();
                    }
                }
            }
        }

        public void UpdateArea()
        {
            var area = pawn.playerSettings.AreaRestrictionInPawnCurrentMap;
            var hiveArea = pawn.Map.areaManager.Get<Area_Hive>();
            if (area != null && hiveArea == area && hiveArea.TrueCount <= 0)
            {
                pawn.playerSettings.AreaRestrictionInPawnCurrentMap = null;
            }
            else if (hiveArea != null && hiveArea != area && hiveArea.TrueCount > 0)
            {
                pawn.playerSettings.AreaRestrictionInPawnCurrentMap = hiveArea;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref hive, "hive");
        }
    }
}
