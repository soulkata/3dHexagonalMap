using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Hexs
{
    public class HexAddressRegion
    {
        public readonly Vector2Int size;

        public HexAddressRegion(Vector2Int size)
        {
            this.size = size;
        }

        public bool TryGetNeighbor(HexAddress hex, HexBorder direction, int distance, out HexAddress neighborHex)
        {
            neighborHex = hex.GetNeighborAddress(direction, distance);
            return neighborHex.Contained(this.size);
        }

        public bool TryGetNeighbor(HexAddress hex, HexBorder direction, out HexAddress neighborHex)
        {
            neighborHex = hex.GetNeighborAddress(direction);
            return neighborHex.Contained(this.size);
        }

        protected int IntAddress(HexAddress hexAddress) => hexAddress.offsetX + this.size.x * hexAddress.offsetY;
        protected HexAddress HexAddress(int indAddress) => new HexAddress(indAddress % this.size.x, indAddress / this.size.x);

        public IEnumerable<HexAddress> Keys
        {
            get
            {
                for (int x = 0; x < this.size.x; x++)
                    for (int y = 0; y < this.size.y; y++)
                        yield return new HexAddress(x, y);
            }
        }

        public IEnumerable<HexAddress> RingNeighbors(HexAddress center, int range) => center.RingNeighbors(range).Where(i => i.Contained(this.size));

        public IEnumerable<HexAddress> LineAddresses(HexAddress a, HexAddress b) => HexExtension.LineAddresses(a, b).Where(i => i.Contained(this.size));

        //public bool TryMouseHover(Tilemap tileMap, out HexAddress location)
        //{
        //    Vector3Int position = tileMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //    if (position.x >= 0 && position.x < this.size.x &&
        //        position.y >= 0 && position.y < this.size.y)
        //    {
        //        location = new HexAddress(position.x, position.y);
        //        return true;
        //    }

        //    location = default;
        //    return false;
        //}
    }
}
