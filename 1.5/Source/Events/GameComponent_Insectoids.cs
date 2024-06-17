using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace VFEInsectoids
{
    public class GameComponent_Insectoids : GameComponent
    {
        public Dictionary<Settlement, InsectTerritory> insectTiles = new();

        public float insectTerritoryScale = 1f;

        public static GameComponent_Insectoids Instance;

        public static Faction HiveFaction => Find.FactionManager.FirstFactionOfDef(VFEI_DefOf.VFEI2_Hive);

        public GameComponent_Insectoids()
        {
            Instance = this;
        }

        public GameComponent_Insectoids(Game game)
        {
            Instance = this;
        }

        private List<int> tmpTiles = new List<int>();

        public void AddInsectHive(Settlement hive)
        {
            var insectTerritory = insectTiles[hive] = new InsectTerritory();
            var hiveTile = hive.Tile;
            tmpTiles.Clear();
            var radius = 30 * insectTerritoryScale;
            var tilesCountMax = 50 * insectTerritoryScale;
            Find.WorldFloodFiller.FloodFill(hiveTile, (int curTile) => 
            Find.WorldGrid.ApproxDistanceInTiles(hiveTile, curTile) 
            <= radius && !Find.World.Impassable(curTile), delegate (int curTile, int dist)
            {
                if (dist <= 1)
                {
                    tmpTiles.Add(curTile);
                }
                else if (Rand.Chance((2f * insectTerritoryScale) / (float)dist))
                {
                    var neighbors = new List<int>();
                    Find.WorldGrid.GetTileNeighbors(curTile, neighbors);
                    if (neighbors.Any(x => tmpTiles.Contains(x)))
                    {
                        tmpTiles.Add(curTile);
                    }
                }
                if (tmpTiles.Count >= tilesCountMax)
                {
                    return true;
                }
                return false;
            });
            for (int j = 0; j < tmpTiles.Count; j++)
            {
                insectTerritory.tiles.Add(tmpTiles[j]);
            }
            tmpTiles.Clear();
        }

        public float InfestationMtbDays(int tile)
        {
            foreach (var insectData in insectTiles)
            {
                if (insectData.Value.tiles.Contains(tile))
                {
                    var dist = Find.WorldGrid.ApproxDistanceInTiles(tile, insectData.Key.Tile);
                    return dist * 5f;
                }
            }
            return -1f;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref insectTiles, "insectTiles", LookMode.Reference, LookMode.Deep, ref settlementsTmp, ref insectTerritoriesTmp);
            Scribe_Values.Look(ref insectTerritoryScale, "insectTerritoryScale", 1f);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                insectTiles ??= new Dictionary<Settlement, InsectTerritory>();
            }
        }

        private List<Settlement> settlementsTmp;
        private List<InsectTerritory> insectTerritoriesTmp;
    }
}
