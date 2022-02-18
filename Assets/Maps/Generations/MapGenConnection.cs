using Assets.Hexs;
using System.Collections.Generic;

namespace Assets.Maps.Generations
{
    public class MapGenConnection
    {
        public readonly MapGenRegion regionA;
        public readonly MapGenRegion regionB;
        public MapGenRiver river;
        public MapGenMountain mountain;
        public readonly HashSet<HexBorderAddress> borders = new HashSet<HexBorderAddress>();
        public MapGenConnectionCorner cornerA;
        public MapGenConnectionCorner cornerB;

        public MapGenConnection(MapGenRegion regionA, MapGenRegion regionB)
        {
            this.regionA = regionA;
            this.regionB = regionB;
            regionA.connections.Add(this);
            regionB.connections.Add(this);
        }

        public bool Contains(MapGenRegion region) => this.regionA == region || this.regionB == region;
    }
}
