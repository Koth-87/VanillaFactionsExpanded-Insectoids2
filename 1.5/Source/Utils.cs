using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    [StaticConstructorOnStartup]
    public static class Utils
    {
        private static readonly Texture2D DefenderIcon = ContentFinder<Texture2D>.Get("UI/InsectoidType_Defender");
        private static readonly Texture2D HunterIcon = ContentFinder<Texture2D>.Get("UI/InsectoidType_Hunter");
        private static readonly Texture2D WorkerIcon = ContentFinder<Texture2D>.Get("UI/InsectoidType_Worker");

        static Utils()
        {
            VFEI_DefOf.VFEI_RoamingInsectoids.mtbDaysByBiome = new List<MTBByBiome>();
            foreach (var def in DefDatabase<BiomeDef>.AllDefs)
            {
                VFEI_DefOf.VFEI_RoamingInsectoids.mtbDaysByBiome.Add(new MTBByBiome
                {
                    biome = def,
                    mtbDays = 1f
                });
            }
        }

        public static Texture2D GetInsectTypeTexture(this InsectType insectType)
        {
            switch (insectType)
            {
                case InsectType.Hunter: return HunterIcon;
                case InsectType.Defender: return DefenderIcon;
                case InsectType.Worker: return WorkerIcon;
                default: return null;
            }
        }
        public static HediffDef GetInsectTypeHediff(this InsectType insectType)
        {
            switch (insectType)
            {
                case InsectType.Hunter: return VFEI_DefOf.VFEI_HunterInsectType;
                case InsectType.Defender: return VFEI_DefOf.VFEI_DefenderInsectType;
                case InsectType.Worker: return VFEI_DefOf.VFEI_WorkerInsectType;
                default: return null;
            }
        }

        public static bool IsColonyInsect(this Pawn pawn)
        {
            return pawn.IsColonyInsect(out _);
        }

        public static bool IsColonyInsect(this Pawn pawn, out Hediff_InsectType hediff)
        {
            if (pawn.RaceProps.Insect)
            {
                hediff = pawn.health.hediffSet.GetFirstHediff<Hediff_InsectType>();
                return hediff != null;
            }
            hediff = null;
            return false;
        }

        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static List<List<T>> Split<T>(this List<T> list, int parts)
        {
            int i = 0;
            var splits = from item in list
                         group item by i++ % parts into part
                         select part.ToList();
            return splits.ToList();
        }

        public static bool IsInfestedTile(this int tile)
        {
            if (tile <= 0) return false;
            foreach (var insectData in GameComponent_Insectoids.Instance.insectTiles)
            {
                if (insectData.Value.tiles.Contains(tile))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsInfested(this Map map)
        {
            return map.Tile.IsInfestedTile();
        }
    }
}
