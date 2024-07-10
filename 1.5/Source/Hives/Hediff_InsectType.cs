using Verse;

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
        public override bool ShouldRemove => pawn.MapHeld is null || CompHive is null || CompHive.parent.Destroyed 
            || CompHive.parent.MapHeld != pawn.MapHeld;

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
