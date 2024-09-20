using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace VFEInsectoids
{
    public class WaveActivityDef : Def
    {
        public float raidSizeMultiplier;
        public float timeUntilWaveArrives;
        public bool includeBoss;
        public float order;
    }

    public class WaveActivity : IExposable
    {
        public WaveActivityDef waveActivity;
        public int ticksLaunch;
        public InsectGenelineDef geneline;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref waveActivity, "waveActivity");
            Scribe_Defs.Look(ref geneline, "geneline");
            Scribe_Values.Look(ref ticksLaunch, "ticksLaunch");
        }
    }

    public class GameComponent_Insectoids : GameComponent
    {
        public Dictionary<Settlement, InsectTerritory> insectTiles = new();

        public float insectTerritoryScale = 1f;

        public static GameComponent_Insectoids Instance;

        public int lastInsectoidBossArrival;
        public Thing thumperActivated;
        private int nextEmpressEvilFiringTick;
        public int NextEmpressEvilFiringTick
        {
            get
            {
                if (nextEmpressEvilFiringTick <= 0)
                {
                    SetNextEmpressEvilFiringTick();
                }
                return nextEmpressEvilFiringTick;
            }
        }

        public void SetNextEmpressEvilFiringTick()
        {
            nextEmpressEvilFiringTick = Find.TickManager.TicksGame + new IntRange(2200000, 3600000).RandomInRange;
        }

        public Dictionary<InsectWaveDef, int> lastWavesIndices = new Dictionary<InsectWaveDef, int>();

        public GameComponent_Insectoids()
        {
            Instance = this;
        }

        public GameComponent_Insectoids(Game game)
        {
            Instance = this;
        }

        public int GetNextWaveIndex(InsectWaveDef def)
        {
            if (!lastWavesIndices.TryGetValue(def, out int waveIndex))
            {
                waveIndex = 0;
            }
            else
            {
                waveIndex += 1;
            }
            if (waveIndex > def.waves.Count - 1)
            {
                if (waveIndex >= def.minWaveRepeatIndex)
                {
                    waveIndex = def.minWaveRepeatIndex;
                }
                else
                {
                    waveIndex = 0;
                }
            }
            return waveIndex;
        }

        public InsectWave GetInsectWave(InsectWaveDef def, int waveIndex)
        {
            return def.waves[waveIndex];
        }

        public InsectWave GetNextInsectWave(InsectWaveDef def)
        {
            return def.waves[GetNextWaveIndex(def)];
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
            Scribe_Values.Look(ref lastInsectoidBossArrival, "lastInsectoidBossArrival");
            Scribe_Collections.Look(ref lastWavesIndices, "lastWavesIndices", LookMode.Def, LookMode.Value);
            Scribe_References.Look(ref thumperActivated, "thumperActivated");
            Scribe_Values.Look(ref nextEmpressEvilFiringTick, "nextEmpressEvilFiringTick");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                insectTiles ??= new Dictionary<Settlement, InsectTerritory>();
                lastWavesIndices ??= new Dictionary<InsectWaveDef, int>();
            }
        }

        private List<Settlement> settlementsTmp;
        private List<InsectTerritory> insectTerritoriesTmp;
    }
}
