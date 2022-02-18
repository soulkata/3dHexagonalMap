using System.Collections.Generic;
using UnityEngine;

namespace Assets.Hexs
{
    public struct HexAddress : System.IEquatable<HexAddress>
    {
        public readonly int offsetX;
        public readonly int offsetY;

        public HexAddress(int x, int y)
        {
            this.offsetX = x;
            this.offsetY = y;
        }

        public static HexAddress FromOffset(int x, int y) => new HexAddress(x, y);
        public static HexAddress FromOffset(Vector2Int position) => new HexAddress(position.x, position.y);
        public static HexAddress FromAxial(int q, int r) => new HexAddress(q + (r - (r & 1)) / 2, r);

        public Vector3Int TileAddress(int z) => new Vector3Int(this.offsetX, this.offsetY, this.offsetY + z);

        public int CubeQ => this.offsetX - (this.offsetY - (this.offsetY & 1)) / 2;
        public int CubeR => this.offsetY;
        public int CubeS => -this.CubeQ - this.CubeR;

        public bool Contained(RectInt size) =>
            this.offsetX >= size.x && this.offsetX <= size.xMax &&
            this.offsetY >= size.y && this.offsetY <= size.yMax;

        public bool Contained(Vector2Int size) =>
            this.offsetX >= 0 && this.offsetX < size.x &&
            this.offsetY >= 0 && this.offsetY < size.y;

        public HexAddress GetNeighborAddress(HexBorder direction, int distance = 1)
        {
            switch (direction)
            {
                case HexBorder.MiddleLeft: return new HexAddress(this.offsetX - distance, this.offsetY);
                case HexBorder.LowerLeft: return new HexAddress(this.offsetY % 2 == 0 ? this.offsetX - (distance + 1) / 2 : this.offsetX - distance / 2, this.offsetY - distance);
                case HexBorder.LowerRight: return new HexAddress(this.offsetY % 2 == 0 ? this.offsetX + distance / 2 : this.offsetX + (distance + 1) / 2, this.offsetY - distance);
                case HexBorder.MiddleRight: return new HexAddress(this.offsetX + distance, this.offsetY);
                case HexBorder.UpperRight: return new HexAddress(this.offsetY % 2 == 0 ? this.offsetX + distance / 2 : this.offsetX + (distance + 1) / 2, this.offsetY + distance);
                case HexBorder.UpperLeft: return new HexAddress(this.offsetY % 2 == 0 ? this.offsetX - (distance + 1) / 2 : this.offsetX - distance / 2, this.offsetY + distance);
                default: throw new System.Exception();
            }
        }

        public HexAddress GetNeighborAddress(HexBorder direction)
        {
            switch (direction)
            {
                case HexBorder.MiddleLeft: return new HexAddress(this.offsetX - 1, this.offsetY);
                case HexBorder.LowerLeft: return new HexAddress(this.offsetY % 2 == 0 ? this.offsetX - 1 : this.offsetX, this.offsetY - 1);
                case HexBorder.LowerRight: return new HexAddress(this.offsetY % 2 == 0 ? this.offsetX : this.offsetX + 1, this.offsetY - 1);
                case HexBorder.MiddleRight: return new HexAddress(this.offsetX + 1, this.offsetY);
                case HexBorder.UpperRight: return new HexAddress(this.offsetY % 2 == 0 ? this.offsetX : this.offsetX + 1, this.offsetY + 1);
                case HexBorder.UpperLeft: return new HexAddress(this.offsetY % 2 == 0 ? this.offsetX - 1 : this.offsetX, this.offsetY + 1);
                default: throw new System.Exception();
            }
        }

        public HexBorder GetNeighborDirection(HexAddress neighbor)
        {
            switch (this.offsetY - neighbor.offsetY)
            {
                case 0:
                    switch (this.offsetX - neighbor.offsetX)
                    {
                        case -1: return HexBorder.MiddleRight;
                        case 1: return HexBorder.MiddleLeft;
                        default: throw new System.Exception($"The hex {this} isnt neigbor of hex {neighbor}");
                    }
                case -1:
                    if (this.offsetY % 2 == 0)
                    {
                        switch (this.offsetX - neighbor.offsetX)
                        {
                            case 1: return HexBorder.UpperLeft;
                            case 0: return HexBorder.UpperRight;
                            default: throw new System.Exception($"The hex {this} isnt neigbor of hex {neighbor}");
                        }
                    }
                    else
                    {
                        switch (this.offsetX - neighbor.offsetX)
                        {
                            case -1: return HexBorder.UpperRight;
                            case 0: return HexBorder.UpperLeft;
                            default: throw new System.Exception($"The hex {this} isnt neigbor of hex {neighbor}");
                        }
                    }
                case 1:
                    if (this.offsetY % 2 == 0)
                    {
                        switch (this.offsetX - neighbor.offsetX)
                        {
                            case 1: return HexBorder.LowerLeft;
                            case 0: return HexBorder.LowerRight;
                            default: throw new System.Exception($"The hex {this} isnt neigbor of hex {neighbor}");
                        }
                    }
                    else
                    {
                        switch (this.offsetX - neighbor.offsetX)
                        {
                            case -1: return HexBorder.LowerRight;
                            case 0: return HexBorder.LowerLeft;
                            default: throw new System.Exception($"The hex {this} isnt neigbor of hex {neighbor}");
                        }
                    }
                default: throw new System.Exception($"The hex {this} isnt neigbor of hex {neighbor}");
            }
        }

        //public IEnumerable<HexAddress> GetHexSharingCorner(int cornerIndex)
        //{
        //    switch (cornerIndex)
        //    {
        //        case 0:
        //            yield return this;
        //            yield return HexAddress.FromAxial(this.CubeQ, this.CubeR - 1);
        //            yield return HexAddress.FromAxial(this.CubeQ + 1, this.CubeR - 1);
        //            break;
        //        case 1:
        //            yield return this;
        //            yield return HexAddress.FromAxial(this.CubeQ + 1, this.CubeR - 1);
        //            yield return HexAddress.FromAxial(this.CubeQ + 1, this.CubeR);
        //            break;
        //        case 2:
        //            yield return this;
        //            yield return HexAddress.FromAxial(this.CubeQ + 1, this.CubeR);
        //            yield return HexAddress.FromAxial(this.CubeQ, this.CubeR + 1);
        //            break;
        //        case 3:
        //            yield return this;
        //            yield return HexAddress.FromAxial(this.CubeQ, this.CubeR + 1);
        //            yield return HexAddress.FromAxial(this.CubeQ - 1, this.CubeR + 1);
        //            break;
        //        case 4:
        //            yield return this;
        //            yield return HexAddress.FromAxial(this.CubeQ - 1, this.CubeR + 1);
        //            yield return HexAddress.FromAxial(this.CubeQ - 1, this.CubeR);
        //            break;
        //        case 5:
        //            yield return this;
        //            yield return HexAddress.FromAxial(this.CubeQ - 1, this.CubeR);
        //            yield return HexAddress.FromAxial(this.CubeQ, this.CubeR - 1);
        //            break;
        //        default: throw new System.Exception();
        //    }
        //}

        public override bool Equals(object obj) => (obj is HexAddress typed) && this == typed;
        public override int GetHashCode() => this.offsetX ^ this.offsetY;
        public override string ToString() => $"Hex ({this.offsetX}, {this.offsetY})";
        bool System.IEquatable<HexAddress>.Equals(HexAddress other) => this.offsetX == other.offsetX && this.offsetY == other.offsetY;
        public static bool operator ==(HexAddress x, HexAddress y) => x.offsetX == y.offsetX && x.offsetY == y.offsetY;
        public static bool operator !=(HexAddress x, HexAddress y) => x.offsetX != y.offsetX || x.offsetY != y.offsetY;

        public IEnumerable<HexAddress> RingNeighbors(int range)
        {
            for (int direction = 0; direction < 6; direction++)
            {
                int dirStep = (direction + 2) % 6;
                HexAddress address = this.GetNeighborAddress((HexBorder)direction, range);
                yield return address;

                for (int rangeSteps = 1; rangeSteps < range; rangeSteps++)
                {
                    address = address.GetNeighborAddress((HexBorder)dirStep);
                    yield return address;
                }
            }
        }
    }
}
