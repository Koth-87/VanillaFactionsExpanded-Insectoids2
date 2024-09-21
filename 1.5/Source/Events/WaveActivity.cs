using RimWorld;
using Verse;

namespace VFEInsectoids
{
    public class WaveActivity : IExposable
    {
        public WaveActivityDef def;
        public int ticksStarting;
        public InsectGenelineDef geneline;

        public WaveActivity()
        {

        }

        public WaveActivity(WaveActivityDef waveActivity)
        {
            this.def = waveActivity;
            this.ticksStarting = 
                (int)((VFEInsectoidsSettings.baseTimeBetweenHanHordeWaves * waveActivity.timeUntilWaveArrives) * GenDate.TicksPerDay);
            this.geneline = DefDatabase<InsectGenelineDef>.GetRandom();
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "waveActivity");
            Scribe_Defs.Look(ref geneline, "geneline");
            Scribe_Values.Look(ref ticksStarting, "ticksStarting");
        }
    }
}
