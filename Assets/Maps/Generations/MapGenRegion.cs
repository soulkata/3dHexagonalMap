using Assets.Hexs;
using System.Collections.Generic;

namespace Assets.Maps.Generations
{
    public class MapGenRegion
    {
        public bool cliff;
        public HexAddress centerLocation;
        public MapRegionBiome biome;
        public float moisture;
        public List<MapGenConnection> connections = new List<MapGenConnection>();
    }
}
