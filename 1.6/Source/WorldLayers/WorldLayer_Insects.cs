using RimWorld;
using RimWorld.Planet;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    public class WorldLayer_Insects : WorldLayer
    {
        private static readonly Color DefaultTileColor = Color.white;

        private List<Vector3> verts = new List<Vector3>();

        private Dictionary<int, List<LayerSubMesh>> subMeshesByRegion = new Dictionary<int, List<LayerSubMesh>>();

        private Queue<int> regionsToRegenerate = new Queue<int>();

        private Material overlayMat;

        private List<PlanetTile> tmpNeighbors = new List<PlanetTile>();

        private HashSet<Vector3> tmpBordersUninsectVerts = new HashSet<Vector3>();

        private List<Vector3> tmpVerts = new List<Vector3>();

        private static List<PlanetTile> tmpChangedNeighbours = new List<PlanetTile>();

        private Material OverlayMat
        {
            get
            {
                if (overlayMat == null)
                {
                    overlayMat = MaterialPool.MatFrom("UI/InfestationOverlay", ShaderDatabase.WorldOverlayTransparentLitPollution, 3510);
                }
                return overlayMat;
            }
        }

        private int GetRegionIdForTile(int tileId)
        {
            return Mathf.FloorToInt((float)tileId / 500f);
        }

        public List<LayerSubMesh> GetSubMeshesForRegion(int regionId)
        {
            if (!subMeshesByRegion.ContainsKey(regionId))
            {
                subMeshesByRegion[regionId] = new List<LayerSubMesh>();
            }
            return subMeshesByRegion[regionId];
        }

        public LayerSubMesh GetSubMeshForMaterialAndRegion(Material material, int regionId)
        {
            List<LayerSubMesh> subMeshesForRegion = GetSubMeshesForRegion(regionId);
            for (int i = 0; i < subMeshesForRegion.Count; i++)
            {
                if (subMeshesForRegion[i].material == material)
                {
                    return subMeshesForRegion[i];
                }
            }
            Mesh mesh = new Mesh();
            if (UnityData.isEditor)
            {
                mesh.name = "WorldLayerSubMesh_" + GetType().Name + "_" + Find.World.info.seedString;
            }
            LayerSubMesh layerSubMesh = new LayerSubMesh(mesh, material);
            subMeshesForRegion.Add(layerSubMesh);
            subMeshes.Add(layerSubMesh);
            return layerSubMesh;
        }

        private void RegnerateRegion(int regionId)
        {
            List<LayerSubMesh> subMeshesForRegion = GetSubMeshesForRegion(regionId);
            for (int i = 0; i < subMeshesForRegion.Count; i++)
            {
                subMeshesForRegion[i].Clear(MeshParts.All);
            }
            int num = regionId * 500;
            int num2 = num + 500;
            for (int j = num; j < num2 && Find.World.grid.InBounds(j); j++)
            {
                TryAddMeshForTile(j);
            }
            for (int k = 0; k < subMeshesForRegion.Count; k++)
            {
                if (subMeshesForRegion[k].verts.Count > 0)
                {
                    subMeshesForRegion[k].FinalizeMesh(MeshParts.All);
                }
            }
        }

        public override IEnumerable Regenerate()
        {
            foreach (object item in base.Regenerate())
            {
                yield return item;
            }
            int num = 500;
            Mathf.CeilToInt((float)Find.WorldGrid.TilesCount / (float)num);
            WorldGrid worldGrid = Find.WorldGrid;
            int tilesCount = worldGrid.TilesCount;
            int insectMeshesPrinted = 0;
            verts.Clear();
            subMeshesByRegion.Clear();
            regionsToRegenerate.Clear();
            for (int i = 0; i < tilesCount; i++)
            {
                if (TryAddMeshForTile(i))
                {
                    insectMeshesPrinted++;
                    if (insectMeshesPrinted % 1000 == 0)
                    {
                        yield return null;
                    }
                }
            }
            FinalizeMesh(MeshParts.All);
        }

        private bool TryAddMeshForTile(int tileId)
        {
            var hives = Find.WorldObjects.Settlements.Where(x => x.Faction == Faction.OfInsects);
            foreach (var hive in hives)
            {
                if (GameComponent_Insectoids.Instance.insectTiles.TryGetValue(hive, out var insectTerritory) && insectTerritory.tiles.Contains(tileId))
                {
                    Material mat = OverlayMat;
                    if (mat == null)
                    {
                        return false;
                    }
                    int regionIdForTile = GetRegionIdForTile(tileId);
                    LayerSubMesh subMeshForMaterialAndRegion = GetSubMeshForMaterialAndRegion(mat, regionIdForTile);
                    Find.WorldGrid.GetTileVertices(tileId, verts);
                    Find.WorldGrid.GetTileNeighbors(tileId, tmpNeighbors);
                    int count = subMeshForMaterialAndRegion.verts.Count;
                    tmpBordersUninsectVerts.Clear();
                    tmpVerts.Clear();
                    for (int i = 0; i < tmpNeighbors.Count; i++)
                    {
                        if (Find.World.grid[tmpNeighbors[i]].PollutionLevel() >= PollutionLevel.Moderate)
                        {
                            continue;
                        }
                        Vector3 center = Find.WorldGrid.GetTileCenter(tmpNeighbors[i]);
                        tmpVerts.AddRange(verts);
                        tmpVerts.SortBy((Vector3 v) => Vector2.Distance(center, v));
                        for (int j = 0; j < 2; j++)
                        {
                            if (!tmpBordersUninsectVerts.Contains(tmpVerts[j]))
                            {
                                tmpBordersUninsectVerts.Add(tmpVerts[j]);
                            }
                        }
                    }
                    int k = 0;
                    for (int count2 = verts.Count; k < count2; k++)
                    {
                        Vector3 vector = verts[k] + verts[k].normalized * 0.012f;
                        subMeshForMaterialAndRegion.verts.Add(vector);
                        subMeshForMaterialAndRegion.uvs.Add(vector * 0.1f);
                        Color color = DefaultTileColor;// (tmpBordersUninsectVerts.Contains(verts[k]) ? BordersUninsectTileColor : DefaultTileColor);
                        subMeshForMaterialAndRegion.colors.Add(color);
                        if (k < count2 - 2)
                        {
                            subMeshForMaterialAndRegion.tris.Add(count + k + 2);
                            subMeshForMaterialAndRegion.tris.Add(count + k + 1);
                            subMeshForMaterialAndRegion.tris.Add(count);
                        }
                    }
                    tmpBordersUninsectVerts.Clear();
                    tmpVerts.Clear();
                    return true;
                }
            }
            return false;
        }

        public void Notify_TilePollutionChanged(int tileId)
        {
            int regionIdForTile = GetRegionIdForTile(tileId);
            if (!regionsToRegenerate.Contains(regionIdForTile))
            {
                regionsToRegenerate.Enqueue(regionIdForTile);
            }
            Find.WorldGrid.GetTileNeighbors(tileId, tmpChangedNeighbours);
            for (int i = 0; i < tmpChangedNeighbours.Count; i++)
            {
                int regionIdForTile2 = GetRegionIdForTile(tmpChangedNeighbours[i]);
                if (!regionsToRegenerate.Contains(regionIdForTile2))
                {
                    regionsToRegenerate.Enqueue(regionIdForTile2);
                }
            }
            tmpChangedNeighbours.Clear();
        }

        public override void Render()
        {
            if (regionsToRegenerate.Count > 0)
            {
                int regionId = regionsToRegenerate.Dequeue();
                RegnerateRegion(regionId);
            }
            base.Render();
        }
    }
}
