using System.Collections.Generic;
using System.Linq;

namespace Assets.Hexs
{
    public struct HexBorderAddress : System.IEquatable<HexBorderAddress>
    {
        private readonly HexAddress address;
        private readonly HexBorder border;

        public HexBorderAddress(HexAddress address, HexBorder border)
        {
            switch (border)
            {
                case HexBorder.MiddleLeft:
                case HexBorder.LowerLeft:
                case HexBorder.LowerRight:
                    this.address = address;
                    this.border = border;
                    break;
                case HexBorder.MiddleRight:
                case HexBorder.UpperRight:
                case HexBorder.UpperLeft:
                    this.address = address.GetNeighborAddress(border);
                    this.border = border.Flip();
                    break;
                default: throw new System.ArgumentOutOfRangeException(nameof(border));
            }
        }

        public IEnumerable<HexAddress> Addresses()
        {
            yield return address;
            yield return this.address.GetNeighborAddress(this.border);
        }

        public IEnumerable<HexCornerAddress> Corners() => this.border.Corners().SelectConstant(this.address, (i, a) => new HexCornerAddress(a, i));
        bool System.IEquatable<HexBorderAddress>.Equals(HexBorderAddress other) => this == other;
        public override bool Equals(object obj) => (obj is HexBorderAddress typed) && this == typed;
        public override int GetHashCode() => this.address.GetHashCode() ^ this.border.GetHashCode();
        public override string ToString() => $"Border {this.border} of {this.address}";
        public static bool operator ==(HexBorderAddress x, HexBorderAddress y) => (x.border == y.border) && (x.address == y.address);
        public static bool operator !=(HexBorderAddress x, HexBorderAddress y) => (x.border != y.border) || (x.address != y.address);
        
        public string Serialize() => this.address.offsetX + "," + this.address.offsetY + "," + (int)this.border;
        public static HexBorderAddress DeSerialize(string msg)
        {
            string[] p = msg.Split(',');
            return new HexBorderAddress(new HexAddress(int.Parse(p[0]), int.Parse(p[1])), (HexBorder)int.Parse(p[2]));
        }
    }
}
