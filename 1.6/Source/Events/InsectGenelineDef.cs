using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    public class InsectGenelineDef : Def
    {
        public List<PawnGenOption> insects;
        public PawnKindDef boss;
        public ThingDef hive;
        public float spawnWeight;
        public string waveIconPath;

        [Unsaved(false)]
        public Texture2D waveIcon = BaseContent.BadTex;

        public override void PostLoad()
        {
            base.PostLoad();
            if (!waveIconPath.NullOrEmpty())
            {
                LongEventHandler.ExecuteWhenFinished(delegate
                {
                    waveIcon = ContentFinder<Texture2D>.Get(waveIconPath);
                });
            }
        }
    }
}
