using System.Collections.Generic;

namespace Assets.Maps.Generations
{
    public class MapGenRiver
    {
        public readonly MapGenConnectionCorner wideToNarrow;
        public readonly MapGenRiver parent;
        public readonly List<MapGenRiver> children = new List<MapGenRiver>();

        public MapGenRiver(MapGenConnectionCorner wideToNarrow)
        {
            this.wideToNarrow = wideToNarrow;
        }

        public MapGenRiver(MapGenRiver parent, MapGenConnectionCorner wideToNarrow)
        {
            this.parent = parent;
            this.wideToNarrow = wideToNarrow;
            this.parent.children.Add(this);
        }
    } 
}
