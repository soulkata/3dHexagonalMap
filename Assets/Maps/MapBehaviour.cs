using Assets.Hashs;
using Assets.Hexs;
using Assets.Hexs.Hashs;
using Assets.Maps.Generations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Maps
{
    public class MapBehaviour : MonoBehaviour
    {
        public int seed;
        public TileRenderingMode renderingMode;
        public MapParameter parameterGameplay;
        public Transform tileChunkParent;
        public MapTileRendererChunkBehaviour tileChunkPrefab;
        public Material landMaterialPrefab;
        public Tile whiteHexagonTile;
        public Tilemap moistureTilemap;
        public MapTileRegionCameraBehaviour cameraForTileMap;

        internal Vector2Int finalSize;
        //internal HexAddressRegionArray<MapTile> tiles;
        internal HexScreenHelper hexScreenHelper;
        internal MapTileRendererChunkBehaviour[,] chunks;

        public void ReloadMapRegions()
        {
            this.moistureTilemap.ClearAllTiles();

            this.finalSize = new Vector2Int(this.parameterGameplay.regionCount.x * this.parameterGameplay.regionSize.x, this.parameterGameplay.regionCount.y * this.parameterGameplay.regionSize.y);
            this.hexScreenHelper = new HexScreenHelper(this.finalSize, this.parameterGameplay.radius);
            Generations.MapGenerator generator = new MapGenerator(this);
            generator.Generate();

            //this.tiles = new HexAddressRegionArray<MapTile>(this.finalSize);
            //foreach (KeyValuePair<HexAddress, MapGenTile> tile in generator.tiles)
            //    this.tiles[tile.Key] = new MapTile() { mountain = tile.Value.mountain, river = tile.Value.river, water = tile.Value.region.biome == MapRegionBiome.Water, renderingMode = TileRenderingMode.Platform };
        }

        //public void ReloadMapRegions()
        //{
        //    this.finalSize = new Vector2Int(this.parameterGameplay.regionCount.x * this.parameterGameplay.regionSize.x, this.parameterGameplay.regionCount.y * this.parameterGameplay.regionSize.y);
        //    this.hexScreenHelper = new HexScreenHelper(this.finalSize, this.parameterGameplay.radius);

        //    IPerlin2D elevationNoisePerlin = new Perlin2DCombined(new[]
        //        {
        //            new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.seed + 1111), this.parameterGameplay.noiseBaseLenght), 0.5f),
        //            new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.seed + 2222), this.parameterGameplay.noiseBaseLenght / 2), 0.25f),
        //            new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.seed + 3333), this.parameterGameplay.noiseBaseLenght / 4), 0.125f)
        //        });
        //    elevationNoisePerlin = new Generations.Perlin2DEnsureWaterOnEdge(elevationNoisePerlin, this);

        //    this.FillRegions(elevationNoisePerlin);
        //    this.FillTiles();
        //    this.InitializeRegionConnections(out List<MapRegionNeighborConnection> landConnections, out List<MapRegionNeighborConnection> coastConnections);

        //    RandomGenerator random = new RandomGenerator(028432 + this.seed);

        //    List<MapRegionNeighborConnection> allMountains = new List<MapRegionNeighborConnection>();
        //    this.AppendAllMountainExtensions(allMountains, landConnections, random);
        //    this.CarveMountains(allMountains, random);

        //    List<RiverTree> riverEstuaries = new List<RiverTree>();
        //    List<RiverTree> riverAllPlaces = new List<RiverTree>();
        //    List<RiverTree> riverAvailableExtensionPlace = new List<RiverTree>();

        //    this.AppendAllCoastRiverConnections(landConnections, coastConnections, random, riverEstuaries, riverAllPlaces, riverAvailableExtensionPlace);
        //    this.AppendAllRiverExtensions(landConnections, random, riverEstuaries, riverAvailableExtensionPlace);
        //    this.CarveRivers(riverEstuaries, random);

        //    this.GenerateChunks();
        //}

             

        

        //public void ReloadMap()
        //{
        //    this.finalSize = new Vector2Int(this.parameterGameplay.regionCount.x * this.parameterGameplay.regionSize.x, this.parameterGameplay.regionCount.y * this.parameterGameplay.regionSize.y);
        //    this.tiles = new HexAddressRegionArray<MapTile>(this.finalSize);

        //    this.hexScreenHelper = new HexScreenHelper(this.finalSize, this.parameterGameplay.radius);

        //    //IPerlin2D perlin = new Perlin2DFrequency(new Perlin2D(this.seed + 1111), 40);
        //    IPerlin2D perlin = new Perlin2DCombined(new[]
        //        {
        //            new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.seed + 1111), this.parameterGameplay.noiseBaseLenght), 0.5f),
        //            new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.seed + 2222), this.parameterGameplay.noiseBaseLenght / 2), 0.25f),
        //            new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.seed + 3333), this.parameterGameplay.noiseBaseLenght / 4), 0.125f)
        //        });
        //    perlin = new Perlin2DPow(perlin);
        //    perlin = new Perlin2DEnsureWaterOnEdge(perlin, this);

        //    foreach (HexAddress hex in this.tiles.Keys)
        //    {
        //        MapTile tile = new MapTile();
        //        List<float> heights = hexScreenHelper.AllCornerPositions(hex).SelectConstant(perlin, (i, p) => p.Value(i)).ToList();
        //        tile.height = perlin.Value(hexScreenHelper.CenterPosition(hex));
        //        heights.Add(tile.height);
        //        tile.height *= this.parameterGameplay.heightScale;
        //        float stdDev = heights.StdDev(); // heights.Max() - heights.Min();
        //        //if (stdDev < 0.0001)
        //        //    tile.renderingMode = TileRenderingMode.Simple;
        //        //else if (stdDev < 0.01)
        //        //    tile.renderingMode = TileRenderingMode.Platform;
        //        //else
        //        //    tile.renderingMode = TileRenderingMode.Impassable;
        //        tile.renderingMode = TileRenderingMode.Impassable;
        //        this.tiles[hex] = tile;
        //    }

        //    foreach (Transform child in this.tileChunkParent)
        //        GameObject.Destroy(child.gameObject);
        //    this.chunks = new MapTileRendererChunkBehaviour[this.finalSize.x / this.parameterGameplay.chunkSize.x, this.finalSize.y / this.parameterGameplay.chunkSize.y];
        //    for (int chunkX = 0; chunkX < this.finalSize.x / this.parameterGameplay.chunkSize.x; chunkX++)
        //        for (int chunkY = 0; chunkY < this.finalSize.y / this.parameterGameplay.chunkSize.y; chunkY++)
        //        {
        //            MapTileRendererChunkBehaviour chunk = GameObject.Instantiate<MapTileRendererChunkBehaviour>(this.tileChunkPrefab);
        //            chunk.transform.SetParent(this.tileChunkParent, false);
        //            chunk.map = this;
        //            chunk.hexScreenHelper = hexScreenHelper;
        //            chunk.perlin = perlin;
        //            chunk.chunkIndex = new Vector2Int(chunkX, chunkY);
        //            chunk.ReloadChunk();
        //            this.chunks[chunkX, chunkY] = chunk;
        //        }
        //}

        //public class Perlin2DPow : Perlin2DFunc
        //{
        //    public float power;

        //    public Perlin2DPow(IPerlin2D source, float power = 2)
        //        : base(source) 
        //    {
        //        this.power = power;
        //    }

        //    public override float Selector(Vector2 position, float value) => Mathf.Pow(value, this.power);
        //}        
    }
}
