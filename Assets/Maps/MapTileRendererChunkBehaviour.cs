using Assets.Hashs;
using Assets.Hexs;
using Assets.Hexs.Hashs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Maps
{
    public class MapTileRendererChunkBehaviour : MonoBehaviour
    {
        public MeshFilter meshFilter;
        public MeshCollider meshCollider;
        public MeshRenderer meshRenderer;

        internal MapBehaviour map;
        internal Vector2Int chunkIndex;

        //internal void ReloadChunk()
        //{
        //    HexAddress startLoc = new HexAddress(this.chunkIndex.x * this.map.parameterGameplay.chunkSize.x,
        //        this.chunkIndex.y * this.map.parameterGameplay.chunkSize.y);
        //    HexAddress endLoc = new HexAddress(
        //        Mathf.Min(map.finalSize.x - 1, (this.chunkIndex.x + 1) * this.map.parameterGameplay.chunkSize.x - 1),
        //        Mathf.Min(map.finalSize.y - 1, (this.chunkIndex.y + 1) * this.map.parameterGameplay.chunkSize.y - 1));

        //    ReloadOperationHolder reloarOperation = new ReloadOperationHolder(this.hexScreenHelper, this.perlin, this.map.parameterGameplay.standableLerpLocations, this.map.parameterGameplay.standableLerpHeight, this.map.tiles, this.map.parameterGameplay.heightScale);
        //    reloarOperation.Reload(startLoc, endLoc);

        //    Rect uvBounds = this.hexScreenHelper.ScreenRect(startLoc, endLoc);
        //    Vector2[] uvs = new Vector2[reloarOperation.vertices.Count];
        //    Vector3[] normals = new Vector3[reloarOperation.vertices.Count];
        //    for (int i = reloarOperation.vertices.Count - 1; i >= 0; i--)
        //    {
        //        uvs[i] = this.ToUV(reloarOperation.vertices[i], uvBounds);
        //        normals[i] = Vector3.up;
        //    }

        //    Mesh mesh = new Mesh();
        //    mesh.vertices = reloarOperation.vertices.ToArray();
        //    mesh.triangles = reloarOperation.triangles.ToArray();
        //    mesh.normals = normals.ToArray();
        //    mesh.uv = uvs.ToArray();

        //    this.meshFilter.sharedMesh = mesh;
        //    this.meshCollider.sharedMesh = mesh;


        //    this.meshRenderer.sharedMaterial = Material.Instantiate(this.map.landMaterialPrefab);
        //    this.meshRenderer.sharedMaterial.SetTexture("_BiomeMetaTexture", text);
        //}
        
        //class ReloadOperationHolder
        //{
        //    public HexScreenHelper hexScreenHelper;
        //    public IHexPerlin2D perlin;
        //    public Vector2 standableLerpLocations;
        //    public Vector2 standableLerpHeight;
        //    public float heighScale;
        //    public Dictionary<Vector3, int> vertexIndexCache = new Dictionary<Vector3, int>();
        //    public List<Vector3> vertices = new List<Vector3>();
        //    public List<int> triangles = new List<int>();

        //    public ReloadOperationHolder(HexScreenHelper hexScreenHelper, IHexPerlin2D perlin, Vector2 standableLerpLocations, Vector2 standableLerpHeight, float heighScale)
        //    {
        //        this.hexScreenHelper = hexScreenHelper;
        //        this.perlin = perlin;
        //        this.standableLerpLocations = standableLerpLocations;
        //        this.standableLerpHeight = standableLerpHeight;
        //        this.heighScale = heighScale;
        //    }

        //    public void Reload(HexAddress startLoc, HexAddress endLoc)
        //    {
        //        for (int x = startLoc.offsetX; x <= endLoc.offsetX; x++)
        //            for (int y = startLoc.offsetY; y <= endLoc.offsetY; y++)
        //            {
        //                HexAddress hex = new HexAddress(x, y);

        //                Vector2 topCenterPosition = this.hexScreenHelper.CenterPosition(hex);

        //                float h = this.perlin.HexagonValue(hex);
        //                if (h == 0)
        //                    continue;

        //                Vector3 centerPosition = new Vector3(topCenterPosition.x, h * this.heighScale, topCenterPosition.y);

        //                int centerIndex = this.AddVertexIndex(centerPosition);

        //                Vector3[] cornerPositions = new Vector3[6];
        //                int[] cornerVertexindexes = new int[6];

        //                for (int i = 0; i < 6; i++)
        //                {
        //                    HexCornerAddress corner = new HexCornerAddress(hex, (HexCorner)i);
        //                    Vector2 cornerPos = corner.Position(this.hexScreenHelper);

        //                    cornerPositions[i] = new Vector3(cornerPos.x, this.perlin.CornerValue(corner) * this.heighScale, cornerPos.y);
        //                    cornerVertexindexes[i] = this.FindVertexIndex(cornerPositions[i]);
        //                }

        //                for (int i = 0; i < 6; i++)
        //                    this.FillSide(centerPosition, centerIndex, cornerPositions[i], cornerVertexindexes[i], cornerPositions[(i + 1) % 6], cornerVertexindexes[(i + 1) % 6], TileRenderingMode.Platform);
        //            }
        //    }

        //    public int AddVertexIndex(Vector3 position)
        //    {
        //        int centerIndex = this.vertices.Count;
        //        this.vertexIndexCache.Add(position, centerIndex);
        //        this.vertices.Add(position);
        //        return centerIndex;
        //    }

        //    public int FindVertexIndex(Vector3 position)
        //    {
        //        if (!this.vertexIndexCache.TryGetValue(position, out int ret))
        //        {
        //            ret = this.vertices.Count;
        //            this.vertexIndexCache.Add(position, ret);
        //            this.vertices.Add(position);
        //        }

        //        return ret;
        //    }

        //    const float div13 = 1f / 3f;
        //    const float div23 = 2f / 3f;

        //    private void FillSide(Vector3 centerPosition, int centerVertexIndex, Vector3 lowerPosition, int lowerVertexIndex, Vector3 upperPosition, int upperVertexIndex, TileRenderingMode renderingMode)
        //    {
        //        if (renderingMode == TileRenderingMode.Simple)
        //        {
        //            this.triangles.Add(centerVertexIndex);
        //            this.triangles.Add(lowerVertexIndex);
        //            this.triangles.Add(upperVertexIndex);
        //            return;
        //        }

        //        Vector2 lerpPositions = renderingMode == TileRenderingMode.Platform ? this.standableLerpLocations : new Vector2(div13, div23);
        //        int[,] lineIndexes = new int[4, 3];

        //        Vector3 upper1 = Vector3.Lerp(upperPosition, lowerPosition, div13);
        //        // upper1 = new Vector3(upper1.x, this.perlin.Value(new Vector2(upper1.x, upper1.z)) * this.heighScale, upper1.z);

        //        Vector3 lower1 = Vector3.Lerp(upperPosition, lowerPosition, div23);
        //        // lower1 = new Vector3(lower1.x, this.perlin.Value(new Vector2(lower1.x, lower1.z)) * this.heighScale, lower1.z);

        //        this.FillLineIndexes(lineIndexes, 0, centerPosition, upperPosition, upperVertexIndex, lerpPositions, renderingMode);
        //        this.FillLineIndexes(lineIndexes, 1, centerPosition, upper1, this.FindVertexIndex(upper1), lerpPositions, renderingMode);
        //        this.FillLineIndexes(lineIndexes, 2, centerPosition, lower1, this.FindVertexIndex(lower1), lerpPositions, renderingMode);
        //        this.FillLineIndexes(lineIndexes, 3, centerPosition, lowerPosition, lowerVertexIndex, lerpPositions, renderingMode);

        //        this.DrawLineA(centerVertexIndex, lineIndexes, 0);
        //        this.DrawLineA(centerVertexIndex, lineIndexes, 1);
        //        this.DrawLineB(centerVertexIndex, lineIndexes, 2);
        //    }

        //    private void FillLineIndexes(int[,] lineIndexes, int index, Vector3 centerPosition, Vector3 cornerPosition, int cornerIndex, Vector2 lerpPositions, TileRenderingMode renderingMode)
        //    {
        //        Vector3 intInner;
        //        Vector3 intOutter;

        //        switch (renderingMode)
        //        {
        //            case TileRenderingMode.Platform:
        //                intInner = new Vector3(
        //                    Mathf.Lerp(centerPosition.x, cornerPosition.x, lerpPositions.x),
        //                    Mathf.Lerp(centerPosition.y, cornerPosition.y, this.standableLerpHeight.x),
        //                    Mathf.Lerp(centerPosition.z, cornerPosition.z, lerpPositions.x));
        //                intOutter = new Vector3(
        //                    Mathf.Lerp(centerPosition.x, cornerPosition.x, lerpPositions.y),
        //                    Mathf.Lerp(centerPosition.y, cornerPosition.y, this.standableLerpHeight.y),
        //                    Mathf.Lerp(centerPosition.z, cornerPosition.z, lerpPositions.y));
        //                break;
        //            case TileRenderingMode.Impassable:
        //                {
        //                    intInner = new Vector3(
        //                        Mathf.Lerp(centerPosition.x, cornerPosition.x, lerpPositions.x),
        //                        Mathf.Lerp(centerPosition.y, cornerPosition.y, HexScreenHelper.div13),
        //                        Mathf.Lerp(centerPosition.z, cornerPosition.z, lerpPositions.x));
        //                    intOutter = new Vector3(
        //                        Mathf.Lerp(centerPosition.x, cornerPosition.x, lerpPositions.y),
        //                        Mathf.Lerp(centerPosition.y, cornerPosition.y, HexScreenHelper.div23),
        //                        Mathf.Lerp(centerPosition.z, cornerPosition.z, lerpPositions.y));
        //                }
        //                break;
        //            default: throw new System.Exception();
        //        }

                

        //        if (index == 0 || index == 3)
        //        {
        //            lineIndexes[index, 0] = this.FindVertexIndex(intInner);
        //            lineIndexes[index, 1] = this.FindVertexIndex(intOutter);
        //            lineIndexes[index, 2] = cornerIndex;
        //        }
        //        else
        //        {
        //            lineIndexes[index, 0] = this.AddVertexIndex(intInner);
        //            lineIndexes[index, 1] = this.AddVertexIndex(intOutter);
        //            lineIndexes[index, 2] = cornerIndex;
        //        }
        //    }

        //    private void DrawLineA(int centerIndex, int[,] lineIndexes, int lowRow)
        //    {
        //        int higRow = lowRow + 1;

        //        this.triangles.Add(centerIndex);
        //        this.triangles.Add(lineIndexes[higRow, 0]);
        //        this.triangles.Add(lineIndexes[lowRow, 0]);

        //        this.triangles.Add(lineIndexes[lowRow, 0]);
        //        this.triangles.Add(lineIndexes[higRow, 0]);
        //        this.triangles.Add(lineIndexes[higRow, 1]);

        //        this.triangles.Add(lineIndexes[lowRow, 0]);
        //        this.triangles.Add(lineIndexes[higRow, 1]);
        //        this.triangles.Add(lineIndexes[lowRow, 1]);

        //        this.triangles.Add(lineIndexes[lowRow, 1]);
        //        this.triangles.Add(lineIndexes[higRow, 1]);
        //        this.triangles.Add(lineIndexes[higRow, 2]);

        //        this.triangles.Add(lineIndexes[lowRow, 1]);
        //        this.triangles.Add(lineIndexes[higRow, 2]);
        //        this.triangles.Add(lineIndexes[lowRow, 2]);
        //    }

        //    private void DrawLineB(int centerIndex, int[,] lineIndexes, int lowRow)
        //    {
        //        int higRow = lowRow + 1;

        //        this.triangles.Add(centerIndex);
        //        this.triangles.Add(lineIndexes[higRow, 0]);
        //        this.triangles.Add(lineIndexes[lowRow, 0]);

        //        this.triangles.Add(lineIndexes[lowRow, 0]);
        //        this.triangles.Add(lineIndexes[higRow, 0]);
        //        this.triangles.Add(lineIndexes[lowRow, 1]);

        //        this.triangles.Add(lineIndexes[higRow, 0]);
        //        this.triangles.Add(lineIndexes[higRow, 1]);
        //        this.triangles.Add(lineIndexes[lowRow, 1]);

        //        this.triangles.Add(lineIndexes[lowRow, 1]);
        //        this.triangles.Add(lineIndexes[higRow, 1]);
        //        this.triangles.Add(lineIndexes[lowRow, 2]);

        //        this.triangles.Add(lineIndexes[higRow, 1]);
        //        this.triangles.Add(lineIndexes[higRow, 2]);
        //        this.triangles.Add(lineIndexes[lowRow, 2]);
        //    }
        //}

        //class ReloadOperationHolder
        //{
        //    public HexScreenHelper hexScreenHelper;
        //    public IPerlin2D perlin;
        //    public Vector2 standableLerpLocations;
        //    public Vector2 standableLerpHeight;
        //    public HexAddressRegionArray<MapTile> tiles;
        //    public float heighScale;
        //    public Dictionary<Vector3, int> vertexIndexCache = new Dictionary<Vector3, int>();
        //    public List<Vector3> vertices = new List<Vector3>();
        //    public List<int> triangles = new List<int>();

        //    public ReloadOperationHolder(HexScreenHelper hexScreenHelper, IPerlin2D perlin, Vector2 standableLerpLocations, Vector2 standableLerpHeight, HexAddressRegionArray<MapTile> tiles, float heighScale)
        //    {
        //        this.hexScreenHelper = hexScreenHelper;
        //        this.perlin = perlin;
        //        this.standableLerpLocations = standableLerpLocations;
        //        this.standableLerpHeight = standableLerpHeight;
        //        this.tiles = tiles;
        //        this.heighScale = heighScale;
        //    }

        //    public void Reload(HexAddress startLoc, HexAddress endLoc)
        //    {
        //        for (int x = startLoc.offsetX; x <= endLoc.offsetX; x++)
        //            for (int y = startLoc.offsetY; y <= endLoc.offsetY; y++)
        //            {
        //                HexAddress hex = new HexAddress(x, y);

        //                Vector2 topCenterPosition = this.hexScreenHelper.CenterPosition(hex);
        //                MapTile tile = this.tiles[hex];
        //                //float height = tile.height;
        //                Vector3 centerPosition = new Vector3(topCenterPosition.x, this.perlin.Value(topCenterPosition) * this.heighScale, topCenterPosition.y);

        //                int centerIndex = this.AddVertexIndex(centerPosition);

        //                Vector2[] topCornerPositions = this.hexScreenHelper.AllCornerPositions(hex).ToArray();
        //                Vector3[] cornerPositions = new Vector3[6];
        //                int[] cornerVertexindexes = new int[6];

        //                for (int i = 0; i < 6; i++)
        //                {
        //                    //height = this.perlin.Value(topCornerPositions[i]) * this.heighScale;
        //                    cornerPositions[i] = new Vector3(topCornerPositions[i].x, this.perlin.Value(topCornerPositions[i]) * this.heighScale, topCornerPositions[i].y);
        //                    cornerVertexindexes[i] = this.FindVertexIndex(cornerPositions[i]);
        //                }

        //                for (int i = 0; i < 6; i++)
        //                    this.FillSide(centerPosition, centerIndex, cornerPositions[(i + 1) % 6], cornerVertexindexes[(i + 1) % 6], cornerPositions[i], cornerVertexindexes[i], tile.renderingMode);
        //            }
        //    }

        //    public int AddVertexIndex(Vector3 position)
        //    {
        //        int centerIndex = this.vertices.Count;
        //        this.vertexIndexCache.Add(position, centerIndex);
        //        this.vertices.Add(position);
        //        return centerIndex;
        //    }

        //    public int FindVertexIndex(Vector3 position)
        //    {
        //        if (!this.vertexIndexCache.TryGetValue(position, out int ret))
        //        {
        //            ret = this.vertices.Count;
        //            this.vertexIndexCache.Add(position, ret);
        //            this.vertices.Add(position);
        //        }

        //        return ret;
        //    }

        //    const float div13 = 1f / 3f;
        //    const float div23 = 2f / 3f;

        //    private void FillSide(Vector3 centerPosition, int centerVertexIndex, Vector3 lowerPosition, int lowerVertexIndex, Vector3 upperPosition, int upperVertexIndex, TileRenderingMode renderingMode)
        //    {
        //        if (renderingMode == TileRenderingMode.Simple)
        //        {
        //            this.triangles.Add(centerVertexIndex);
        //            this.triangles.Add(lowerVertexIndex);
        //            this.triangles.Add(upperVertexIndex);
        //            return;
        //        }

        //        Vector2 lerpPositions = renderingMode == TileRenderingMode.Platform ? this.standableLerpLocations : new Vector2(div13, div23);
        //        int[,] lineIndexes = new int[4, 3];

        //        Vector3 upper1 = Vector3.Lerp(upperPosition, lowerPosition, div13);
        //        // upper1 = new Vector3(upper1.x, this.perlin.Value(new Vector2(upper1.x, upper1.z)) * this.heighScale, upper1.z);

        //        Vector3 lower1 = Vector3.Lerp(upperPosition, lowerPosition, div23);
        //        // lower1 = new Vector3(lower1.x, this.perlin.Value(new Vector2(lower1.x, lower1.z)) * this.heighScale, lower1.z);

        //        this.FillLineIndexes(lineIndexes, 0, centerPosition, upperPosition, upperVertexIndex, lerpPositions, renderingMode);
        //        this.FillLineIndexes(lineIndexes, 1, centerPosition, upper1, this.FindVertexIndex(upper1), lerpPositions, renderingMode);
        //        this.FillLineIndexes(lineIndexes, 2, centerPosition, lower1, this.FindVertexIndex(lower1), lerpPositions, renderingMode);
        //        this.FillLineIndexes(lineIndexes, 3, centerPosition, lowerPosition, lowerVertexIndex, lerpPositions, renderingMode);

        //        this.DrawLineA(centerVertexIndex, lineIndexes, 0);
        //        this.DrawLineA(centerVertexIndex, lineIndexes, 1);
        //        this.DrawLineB(centerVertexIndex, lineIndexes, 2);
        //    }

        //    private void FillLineIndexes(int[,] lineIndexes, int index, Vector3 centerPosition, Vector3 cornerPosition, int cornerIndex, Vector2 lerpPositions, TileRenderingMode renderingMode)
        //    {
        //        Vector3 intInner;
        //        Vector3 intOutter;

        //        switch (renderingMode)
        //        {
        //            case TileRenderingMode.Platform:
        //                intInner = new Vector3(
        //                    Mathf.Lerp(centerPosition.x, cornerPosition.x, lerpPositions.x),
        //                    Mathf.Lerp(centerPosition.y, cornerPosition.y, this.standableLerpHeight.x),
        //                    Mathf.Lerp(centerPosition.z, cornerPosition.z, lerpPositions.x));
        //                intOutter = new Vector3(
        //                    Mathf.Lerp(centerPosition.x, cornerPosition.x, lerpPositions.y),
        //                    Mathf.Lerp(centerPosition.y, cornerPosition.y, this.standableLerpHeight.y),
        //                    Mathf.Lerp(centerPosition.z, cornerPosition.z, lerpPositions.y));
        //                break;
        //            case TileRenderingMode.Impassable:
        //                {
        //                    Vector2 aux = new Vector2(
        //                        Mathf.Lerp(centerPosition.x, cornerPosition.x, lerpPositions.x),
        //                        Mathf.Lerp(centerPosition.z, cornerPosition.z, lerpPositions.x));
        //                    intInner = new Vector3(
        //                        aux.x,
        //                        this.perlin.Value(aux) * this.heighScale,
        //                        aux.y);
        //                    aux = new Vector2(
        //                        Mathf.Lerp(centerPosition.x, cornerPosition.x, lerpPositions.y),
        //                        Mathf.Lerp(centerPosition.z, cornerPosition.z, lerpPositions.y));
        //                    intOutter = new Vector3(
        //                        aux.x,
        //                        this.perlin.Value(aux) * this.heighScale,
        //                        aux.y);
        //                }
        //                break;
        //            default: throw new System.Exception();
        //        }



        //        if (index == 0 || index == 3)
        //        {
        //            lineIndexes[index, 0] = this.FindVertexIndex(intInner);
        //            lineIndexes[index, 1] = this.FindVertexIndex(intOutter);
        //            lineIndexes[index, 2] = cornerIndex;
        //        }
        //        else
        //        {
        //            lineIndexes[index, 0] = this.AddVertexIndex(intInner);
        //            lineIndexes[index, 1] = this.AddVertexIndex(intOutter);
        //            lineIndexes[index, 2] = cornerIndex;
        //        }
        //    }

        //    private void DrawLineA(int centerIndex, int[,] lineIndexes, int lowRow)
        //    {
        //        int higRow = lowRow + 1;

        //        this.triangles.Add(centerIndex);
        //        this.triangles.Add(lineIndexes[higRow, 0]);
        //        this.triangles.Add(lineIndexes[lowRow, 0]);

        //        this.triangles.Add(lineIndexes[lowRow, 0]);
        //        this.triangles.Add(lineIndexes[higRow, 0]);
        //        this.triangles.Add(lineIndexes[higRow, 1]);

        //        this.triangles.Add(lineIndexes[lowRow, 0]);
        //        this.triangles.Add(lineIndexes[higRow, 1]);
        //        this.triangles.Add(lineIndexes[lowRow, 1]);

        //        this.triangles.Add(lineIndexes[lowRow, 1]);
        //        this.triangles.Add(lineIndexes[higRow, 1]);
        //        this.triangles.Add(lineIndexes[higRow, 2]);

        //        this.triangles.Add(lineIndexes[lowRow, 1]);
        //        this.triangles.Add(lineIndexes[higRow, 2]);
        //        this.triangles.Add(lineIndexes[lowRow, 2]);
        //    }

        //    private void DrawLineB(int centerIndex, int[,] lineIndexes, int lowRow)
        //    {
        //        int higRow = lowRow + 1;

        //        this.triangles.Add(centerIndex);
        //        this.triangles.Add(lineIndexes[higRow, 0]);
        //        this.triangles.Add(lineIndexes[lowRow, 0]);

        //        this.triangles.Add(lineIndexes[lowRow, 0]);
        //        this.triangles.Add(lineIndexes[higRow, 0]);
        //        this.triangles.Add(lineIndexes[lowRow, 1]);

        //        this.triangles.Add(lineIndexes[higRow, 0]);
        //        this.triangles.Add(lineIndexes[higRow, 1]);
        //        this.triangles.Add(lineIndexes[lowRow, 1]);

        //        this.triangles.Add(lineIndexes[lowRow, 1]);
        //        this.triangles.Add(lineIndexes[higRow, 1]);
        //        this.triangles.Add(lineIndexes[lowRow, 2]);

        //        this.triangles.Add(lineIndexes[higRow, 1]);
        //        this.triangles.Add(lineIndexes[higRow, 2]);
        //        this.triangles.Add(lineIndexes[lowRow, 2]);
        //    }
        //}

        //private Vector2 ToUV(Vector3 screenLocation, Rect bounds) => new Vector2((screenLocation.x - bounds.x) / bounds.width, (screenLocation.z - bounds.y) / bounds.height);
    }
}
