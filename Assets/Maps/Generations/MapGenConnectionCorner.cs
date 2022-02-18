using Assets.Hexs;
using UnityEngine;

namespace Assets.Maps.Generations
{
    public class MapGenConnectionCorner
    {
        public readonly MapGenConnection connection;
        public readonly HexCornerAddress corner;
        public readonly HexAddress hexagon;
        public readonly Vector2 direction;
        public MapGenConnectionCorner reverse;

        public MapGenConnectionCorner(MapGenConnection connection, HexCornerAddress cornerAddress, HexAddress hexAddress, HexAddress otherorner, HexScreenHelper helper)
        {
            this.connection = connection;
            this.corner = cornerAddress;
            this.hexagon = hexAddress;
            this.direction = (helper.CenterPosition(otherorner) - helper.CenterPosition(hexAddress)).normalized;
        }
    }
}
