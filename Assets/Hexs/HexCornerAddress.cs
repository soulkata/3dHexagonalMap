using System.Collections.Generic;
using UnityEngine;

namespace Assets.Hexs
{
    public struct HexCornerAddress : System.IEquatable<HexCornerAddress>
    {
        private readonly HexAddress address;
        private readonly bool upper;

        public HexCornerAddress(HexAddress address, HexCorner corner)
        {
            switch (corner)
            {
                case HexCorner.UpperCenter:
                    this.address = address;
                    this.upper = true;
                    break;
                case HexCorner.UpperRight:
                    this.address = address.GetNeighborAddress(HexBorder.UpperRight);
                    this.upper = false;
                    break;
                case HexCorner.LowerRight:
                    this.address = address.GetNeighborAddress(HexBorder.LowerRight);
                    this.upper = true;
                    break;
                case HexCorner.LowerCenter:
                    this.address = address;
                    this.upper = false;
                    break;
                case HexCorner.LowerLeft:
                    this.address = address.GetNeighborAddress(HexBorder.LowerLeft);
                    this.upper = true;
                    break;
                case HexCorner.UpperLeft:
                    this.address= address.GetNeighborAddress(HexBorder.UpperLeft);
                    this.upper = false;
                    break;
                default: throw new System.ArgumentOutOfRangeException(nameof(corner));
            }
        }

        public IEnumerable<HexAddress> Addresses()
        {
            if (this.upper)
            {
                yield return address;
                yield return address.GetNeighborAddress(HexBorder.UpperRight);
                yield return address.GetNeighborAddress(HexBorder.UpperLeft);
            }
            else
            {
                yield return address;
                yield return address.GetNeighborAddress(HexBorder.LowerLeft);
                yield return address.GetNeighborAddress(HexBorder.LowerRight);
            }
        }

        public IEnumerable<HexBorderAddress> Borders()
        {
            if (this.upper)
            {
                yield return new HexBorderAddress(address, HexBorder.UpperRight);
                yield return new HexBorderAddress(address, HexBorder.UpperLeft);
                yield return new HexBorderAddress(address.GetNeighborAddress(HexBorder.UpperLeft), HexBorder.MiddleRight);
            }
            else
            {
                yield return new HexBorderAddress(address, HexBorder.LowerLeft);
                yield return new HexBorderAddress(address, HexBorder.LowerRight);
                yield return new HexBorderAddress(address.GetNeighborAddress(HexBorder.LowerLeft), HexBorder.MiddleRight);
            }
        }

        bool System.IEquatable<HexCornerAddress>.Equals(HexCornerAddress other) => this == other;
        public override bool Equals(object obj) => (obj is HexCornerAddress typed) && this == typed;
        public override int GetHashCode() => this.address.GetHashCode() ^ this.upper.GetHashCode();
        public override string ToString() => $"{(this.upper ? "Upper" : "Lower")} corner of {this.address}";
        public static bool operator ==(HexCornerAddress x, HexCornerAddress y) => (x.upper == y.upper) && (x.address == y.address);
        public static bool operator !=(HexCornerAddress x, HexCornerAddress y) => (x.upper != y.upper) || (x.address != y.address);
        public Vector2 Position(HexScreenHelper screen) => screen.CornerPosition(this.address, this.upper ? HexCorner.UpperCenter : HexCorner.LowerCenter);
    }
}
