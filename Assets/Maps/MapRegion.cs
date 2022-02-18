using Assets.Hexs;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Maps
{
    public class MapRegion
    {
        public HexAddress centerLocation;
        public MapRegionBiome biome;
        public List<MapRegionNeighbor> neighbors = new List<MapRegionNeighbor>();
    }

    public enum MapRegionBiome
    {
        Water,
        Grassland
    }

    public class MapRegionNeighbor
    {
        public readonly MapRegion otherRegion;
        public readonly MapRegion currentRegion;

        public MapRegionNeighbor(MapRegion currentRegion, MapRegion otherRegion)
        {
            this.currentRegion = currentRegion;
            this.otherRegion = otherRegion;
        }

        public MapRegionNeighborConnection conenction;        
    }

    public class MapRegionNeighborConnection
    {
        public MapRegionNeighbor[] regions;
        public bool hasRiver = false;
        public bool hasMountain = false;
        public HashSet<HexBorderAddress> borders = new HashSet<HexBorderAddress>();
        public HexCornerAddress[] corners;

        public bool LandConnection => this.regions[0].currentRegion.biome != MapRegionBiome.Water && this.regions[1].currentRegion.biome != MapRegionBiome.Water;
        public bool WaterConnection => this.regions[0].currentRegion.biome == MapRegionBiome.Water && this.regions[1].currentRegion.biome == MapRegionBiome.Water;
        public bool CoastConnection => this.regions[0].currentRegion.biome == MapRegionBiome.Water ? this.regions[1].currentRegion.biome != MapRegionBiome.Water : this.regions[1].currentRegion.biome == MapRegionBiome.Water;
    }
}
