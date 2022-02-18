using UnityEngine;

namespace Assets.Hexs
{
    public class Hex2DMath
    {
        public readonly HexAddress startLocation;
        public readonly HexAddress endLocation;
        public readonly int hexSizeX;
        public readonly int hexSizeX05;
        public readonly int hexSizeY;
        public readonly int hexSizeY05;
        public readonly int hexSizeY075;
        public readonly RectInt totalSize;

        public Hex2DMath(HexAddress startLocation, HexAddress endLocation, Vector2Int hexagonSize)
        {
            this.startLocation = startLocation;
            this.endLocation = endLocation;
            this.hexSizeX = hexagonSize.x;
            this.hexSizeX05 = hexagonSize.x / 2;
            this.hexSizeY = hexagonSize.y;
            this.hexSizeY05 = hexagonSize.y / 2;
            this.hexSizeY075 = hexagonSize.y * 3 / 4;
            this.totalSize = this.ScreenRect(startLocation, endLocation);
        }

        public RectInt ScreenRect(HexAddress hex1, HexAddress hex2)
        {
            int pixMinX;
            int pixMaxX;
            int pixMinY;
            int pixMaxY;

            if (hex1.offsetY == hex2.offsetY)
            {
                int minX = Mathf.Min(hex1.offsetX, hex2.offsetX);
                int maxX = Mathf.Max(hex1.offsetX, hex2.offsetX);
                int curY = hex1.offsetY;

                pixMinX = minX * this.hexSizeX + (curY % 2 == 0 ? 0 : this.hexSizeX05);
                pixMaxX = (maxX + 1) * this.hexSizeX + (curY % 2 == 0 ? 0 : this.hexSizeX05);
                pixMinY = curY * this.hexSizeY075;
                pixMaxY = curY * this.hexSizeY075 + this.hexSizeY;
            }
            else
            {
                int minX = Mathf.Min(hex1.offsetX, hex2.offsetX);
                int maxX = Mathf.Max(hex1.offsetX, hex2.offsetX);
                int minY = Mathf.Min(hex1.offsetY, hex2.offsetY);
                int maxY = Mathf.Max(hex1.offsetY, hex2.offsetY);

                pixMinX = minX * this.hexSizeX;
                pixMaxX = (maxX + 1) * this.hexSizeX + this.hexSizeX05;
                pixMinY = minY * this.hexSizeY075;
                pixMaxY = maxY * this.hexSizeY075 + this.hexSizeY;
            }

            return new RectInt(pixMinX,
                pixMinY,
                pixMaxX - pixMinX + 1,
                pixMaxY - pixMinY + 1);
        }
    }
}
