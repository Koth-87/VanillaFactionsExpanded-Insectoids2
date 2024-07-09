using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace VFEInsectoids
{
    public class Alert_InsectWaveIncoming : Alert_Critical
    {
        public Alert_InsectWaveIncoming()
        {
            defaultPriority = AlertPriority.Critical;
        }

        public override string GetLabel()
        {
            return "VFEI_AlertInsectWaveIncoming".Translate(InsectBossKind().LabelCap);
        }

        public override TaggedString GetExplanation()
        {
            return "VFEI_AlertInsectWaveIncomingDesc".Translate(InsectBossKind().label);
        }

        public PawnKindDef InsectBossKind()
        {
            var thumper = GameComponent_Insectoids.Instance.thumperActivated;
            return thumper.TryGetComp<CompThumper>().Props.wave.waves[0].insects[0].kindDef;
        }

        public override AlertReport GetReport()
        {
            if (GameComponent_Insectoids.Instance.thumperActivated != null)
            {
                return AlertReport.CulpritsAre(new List<Thing>
                { 
                    GameComponent_Insectoids.Instance.thumperActivated 
                });
            }
            return AlertReport.Inactive;
        }
    }
}
