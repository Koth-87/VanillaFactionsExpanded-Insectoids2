using RimWorld;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    public class PawnRenderNode_Unarmored : PawnRenderNode_AnimalPart
    {
        public PawnRenderNode_Unarmored(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
            : base(pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            if (pawn.health.hediffSet.GetFirstHediffOfDef(VFEI_DefOf.VFEI2_ArmorDegradation) != null)
            {
                Graphic graphic = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.Graphic;
                return GraphicDatabase.Get<Graphic_Multi>(graphic.path + "_Unarmored", ShaderDatabase.Cutout, graphic.drawSize, graphic.color);
            }

            return base.GraphicFor(pawn);
        }
    }
}
