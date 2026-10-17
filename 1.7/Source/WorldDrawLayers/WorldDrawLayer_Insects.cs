using RimWorld;
using RimWorld.Planet;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VFEInsectoids
{
    public class WorldDrawLayer_Insects : WorldDrawLayer
    {
        private static readonly Color DefaultTileColor = Color.white;

        private readonly List<Vector3> verts = [];

        private readonly Dictionary<int, List<LayerSubMesh>> subMeshesByRegion = new();

        private readonly Queue<int> regionsToRegenerate = new();

        private Material overlayMat;

        private readonly List<PlanetTile> tmpNeighbors = [];

        private readonly HashSet<Vector3> tmpBordersUninsectVerts = [];

        private readonly List<Vector3> tmpVerts = [];

        private static readonly List<PlanetTile> tmpChangedNeighbours = [];

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

        private PlanetTile GetRegionIdForTile(PlanetTile tile)
        {
            return Mathf.FloorToInt(tile.tileId / 500f);
        }

        public List<LayerSubMesh> GetSubMeshesForRegion(int regionId)
        {
            if (!subMeshesByRegion.TryGetValue(regionId, out var meshes))
            {
                subMeshesByRegion[regionId] = meshes = [];
            }
            return meshes;
        }

        public LayerSubMesh GetSubMeshForMaterialAndRegion(Material material, int regionId)
        {
            var subMeshesForRegion = GetSubMeshesForRegion(regionId);
            for (var i = 0; i < subMeshesForRegion.Count; i++)
            {
                if (subMeshesForRegion[i].material == material)
                {
                    return subMeshesForRegion[i];
                }
            }
            var mesh = new Mesh();
            if (UnityData.isEditor)
            {
                mesh.name = "WorldLayerSubMesh_" + GetType().Name + "_" + Find.World.info.seedString;
            }
            var layerSubMesh = new LayerSubMesh(mesh, material);
            subMeshesForRegion.Add(layerSubMesh);
            subMeshes.Add(layerSubMesh);
            return layerSubMesh;
        }

        private void RegenerateRegion(int regionId)
        {
            var subMeshesForRegion = GetSubMeshesForRegion(regionId);
            for (var i = 0; i < subMeshesForRegion.Count; i++)
                subMeshesForRegion[i].Clear(MeshParts.All);

            var start = regionId * 500;
            var max = start + 500;
            for (var j = start; j < max; j++)
            {
                var planetTile = new PlanetTile(j, planetLayer);
                if (!Find.World.grid.InBounds(planetTile))
                    break;

                TryAddMeshForTile(j);
            }

            for (var k = 0; k < subMeshesForRegion.Count; k++)
            {
                if (subMeshesForRegion[k].verts.Count > 0)
                    subMeshesForRegion[k].FinalizeMesh(MeshParts.All);
            }
        }

        public override IEnumerable Regenerate()
        {
            foreach (var item in base.Regenerate())
            {
                yield return item;
            }

            var insectMeshesPrinted = 0;
            verts.Clear();
            subMeshesByRegion.Clear();
            regionsToRegenerate.Clear();

            for (var i = 0; i < planetLayer.TilesCount; i++)
            {
                if (TryAddMeshForTile(planetLayer.PlanetTileForID(i)))
                {
                    insectMeshesPrinted++;
                    if (insectMeshesPrinted % 1000 == 0)
                        yield return null;
                }
            }

            FinalizeMesh(MeshParts.All);
        }

        private bool TryAddMeshForTile(PlanetTile tile)
        {
            var hives = Find.WorldObjects.Settlements.Where(x => x.Faction == Faction.OfInsects);
            foreach (var hive in hives)
            {
                if (GameComponent_Insectoids.Instance.insectTiles.TryGetValue(hive, out var insectTerritory) && insectTerritory.tiles.Contains(tile))
                {
                    var mat = OverlayMat;
                    if (mat == null)
                    {
                        return false;
                    }
                    int regionIdForTile = GetRegionIdForTile(tile);
                    var subMeshForMaterialAndRegion = GetSubMeshForMaterialAndRegion(mat, regionIdForTile);
                    Find.WorldGrid.GetTileVertices(tile, verts);
                    Find.WorldGrid.GetTileNeighbors(tile, tmpNeighbors);
                    var count = subMeshForMaterialAndRegion.verts.Count;
                    tmpBordersUninsectVerts.Clear();
                    tmpVerts.Clear();
                    for (var i = 0; i < tmpNeighbors.Count; i++)
                    {
                        if (Find.World.grid[tmpNeighbors[i]].PollutionLevel() >= PollutionLevel.Moderate)
                        {
                            continue;
                        }
                        var center = Find.WorldGrid.GetTileCenter(tmpNeighbors[i]);
                        tmpVerts.AddRange(verts);
                        tmpVerts.SortBy((Vector3 v) => Vector2.Distance(center, v));
                        for (var j = 0; j < 2; j++)
                        {
                            if (!tmpBordersUninsectVerts.Contains(tmpVerts[j]))
                            {
                                tmpBordersUninsectVerts.Add(tmpVerts[j]);
                            }
                        }
                    }
                    var k = 0;
                    for (var count2 = verts.Count; k < count2; k++)
                    {
                        var vector = verts[k] + verts[k].normalized * 0.012f;
                        subMeshForMaterialAndRegion.verts.Add(vector);
                        subMeshForMaterialAndRegion.uvs.Add(vector * 0.1f);
                        var color = DefaultTileColor;// (tmpBordersUninsectVerts.Contains(verts[k]) ? BordersUninsectTileColor : DefaultTileColor);
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

        public void Notify_TilePollutionChanged(PlanetTile tile)
        {
            var regionIdForTile = GetRegionIdForTile(tile);
            if (!regionsToRegenerate.Contains(regionIdForTile))
            {
                regionsToRegenerate.Enqueue(regionIdForTile);
            }
            Find.WorldGrid.GetTileNeighbors(tile, tmpChangedNeighbours);
            for (var i = 0; i < tmpChangedNeighbours.Count; i++)
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
                var regionId = regionsToRegenerate.Dequeue();
                RegenerateRegion(regionId);
            }

            base.Render();
        }
    }
}
