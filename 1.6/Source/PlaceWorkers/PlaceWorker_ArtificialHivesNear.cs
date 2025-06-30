using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;
namespace VFEInsectoids
{
    public class PlaceWorker_ArtificialHivesNear : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            CompHive.DrawArtificialHiveOverlay(center, def, Find.CurrentMap, VFEInsectoidsSettings.minHiveStabilityDistance);
        }

        public override void PostPlace(Map map, BuildableDef def, IntVec3 loc, Rot4 rot)
        {
            base.PostPlace(map, def, loc, rot);
            if (CompHive.GetAllNearbyArtificialHives(loc, map).Any(x => x.Position == loc && DebugSettings.godMode is false || x.Position != loc && DebugSettings.godMode))
            {
                Messages.Message("VFEI_HiveCloseProximityWarning".Translate(), new TargetInfo(loc, map), MessageTypeDefOf.CautionInput);
            }
        }
    }
}
