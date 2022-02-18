using System.Collections.Generic;
using UnityEngine;

namespace Assets.Hexs
{
    public class HexScreenHelper
    {
        public readonly Vector2Int hexAmmount;
        public readonly float radius;
        public readonly float hexSizeX;
        public readonly float hexSizeY;
        public readonly float hexSizeX05;
        public readonly float hexSizeY025;
        public readonly float hexSizeY05;
        public readonly float hexSizeY075;
        public const float div13 = 1f / 3f;
        public const float div23 = 2f / 3f;

        public HexScreenHelper(Vector2Int hexAmmount, float radius)
        {
            this.hexAmmount = hexAmmount;
            this.radius = radius;

            this.hexSizeX = Mathf.Sqrt(3f) * radius;
            this.hexSizeY = 2f * radius;

            this.hexSizeX05 = this.hexSizeX * 0.5f;
            this.hexSizeY025 = this.hexSizeY * 0.25f;
            this.hexSizeY05 = this.hexSizeY * 0.5f;
            this.hexSizeY075 = this.hexSizeY * 0.75f;
        }

        public Vector2 ScreenSize()
        {
            if (this.hexAmmount.y == 1)
                return new Vector2(this.hexSizeX * this.hexAmmount.x, this.hexSizeY);
            else
                return new Vector2(this.hexSizeX * this.hexAmmount.x + this.hexSizeX05, this.hexAmmount.y * this.hexSizeY075 + this.hexSizeY025);
        }

        protected float BaseX(HexAddress hexAddress) => hexAddress.offsetX * this.hexSizeX + (hexAddress.offsetY % 2 == 0 ? 0 : this.hexSizeX05);
        protected float BaseY(HexAddress hexAddress) => hexAddress.offsetY * this.hexSizeY075;
        protected Vector2 BaseValues(HexAddress hexAddress) => new Vector2(this.BaseX(hexAddress), this.BaseY(hexAddress));

        public Vector2 CenterPosition(HexAddress hexAddress) => new Vector2(this.BaseX(hexAddress) + this.hexSizeX05, this.BaseY(hexAddress) + this.hexSizeY05);

        public Vector2 CornerPosition(HexAddress hexAddress, HexCorner cornerindex)
        {
            switch (cornerindex)
            {
                case HexCorner.UpperCenter: return new Vector2(this.BaseX(hexAddress) + this.hexSizeX05, this.BaseY(hexAddress) + this.hexSizeY);
                case HexCorner.UpperRight: return new Vector2(this.BaseX(hexAddress) + this.hexSizeX, this.BaseY(hexAddress) + this.hexSizeY075);
                case HexCorner.LowerRight: return new Vector2(this.BaseX(hexAddress) + this.hexSizeX, this.BaseY(hexAddress) + this.hexSizeY025 );
                case HexCorner.LowerCenter: return new Vector2(this.BaseX(hexAddress) + this.hexSizeX05, this.BaseY(hexAddress));
                case HexCorner.LowerLeft: return new Vector2(this.BaseX(hexAddress), this.BaseY(hexAddress) + this.hexSizeY025);
                case HexCorner.UpperLeft: return new Vector2(this.BaseX(hexAddress), this.BaseY(hexAddress) + this.hexSizeY075);
                default: throw new System.Exception();
            }
        }

        public IEnumerable<Vector2> AllCornerPositions(HexAddress hexAddress)
        {
            float baseX = this.BaseX(hexAddress);
            float baseY = this.BaseY(hexAddress);
            yield return new Vector2(baseX + this.hexSizeX05, baseY);
            yield return new Vector2(baseX + this.hexSizeX, baseY + this.hexSizeY025);
            yield return new Vector2(baseX + this.hexSizeX, baseY + this.hexSizeY075);
            yield return new Vector2(baseX + this.hexSizeX05, baseY + this.hexSizeY);
            yield return new Vector2(baseX, baseY + this.hexSizeY075);
            yield return new Vector2(baseX, baseY + this.hexSizeY025);
        }

        public Rect ScreenRect(HexAddress hex1, HexAddress hex2)
        {
            if (hex1.offsetY == hex2.offsetY)
            {
                int minX = Mathf.Min(hex1.offsetX, hex2.offsetX);
                int maxX = Mathf.Max(hex1.offsetX, hex2.offsetX);
                int curY = hex1.offsetY;
                return Rect.MinMaxRect(minX * this.hexSizeX + (curY % 2 == 0 ? 0 : this.hexSizeX05),
                    curY * this.hexSizeY075,
                    (maxX + 1) * this.hexSizeX + (curY % 2 == 0 ? 0 : this.hexSizeX05),
                    curY * this.hexSizeY075 + this.hexSizeY);
            }
            else
            {
                int minX = Mathf.Min(hex1.offsetX, hex2.offsetX);
                int maxX = Mathf.Max(hex1.offsetX, hex2.offsetX);
                int minY = Mathf.Min(hex1.offsetY, hex2.offsetY);
                int maxY = Mathf.Max(hex1.offsetY, hex2.offsetY);
                return Rect.MinMaxRect(minX * this.hexSizeX,
                    minY * this.hexSizeY075,
                    (maxX + 1) * this.hexSizeX + this.hexSizeX05,
                    maxY * this.hexSizeY075 + this.hexSizeY);
            }
        }

        public HexAddressFraction HexOfPosition(Vector3 position) => this.HexOfPosition(position.x, position.z);
        public HexAddressFraction HexOfPosition(Vector2 position) => this.HexOfPosition(position.x, position.y);
        public HexAddressFraction HexOfPosition(float x, float y)
        {
            x -= this.hexSizeX05;
            y -= this.hexSizeY05;
            return new HexAddressFraction((Mathf.Sqrt(3f) / 3f * x - HexScreenHelper.div13 * y) / this.radius, (HexScreenHelper.div23 * y) / this.radius);
        }
    }
}
