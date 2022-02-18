using Assets.Hexs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Debugger
{
    public class HexGridTester : MonoBehaviour
    {
        public Vector2Int size;
        public float radius;
        public Vector2Int address;
        public int borderIndex;
        public int cornerIndex;
        public bool borderAdresses;
        public int ringLenght;

        [TextArea(3, 10)]
        public string info;

        public string serializated;

        internal HexScreenHelper screen;

        public Vector2 a;
        public Vector2 b;

        //private void OnDrawGizmos()
        //{
        //    this.screen = new HexScreenHelper(this.size, this.radius);
        //    Gizmos.color = Color.black;

        //    for (int x = 0; x < this.size.x; x++)
        //        for (int y = 0; y < this.size.y; y++)
        //            this.DrawHex(new HexAddress(x, y));

        //    HexAddress ad = new HexAddress(address.x, address.y);
        //    this.info = ad.ToString();
        //    Gizmos.color = Color.blue;
        //    this.DrawHex(ad);

        //    if (this.borderIndex >= 0 && borderIndex < 6)
        //    {
        //        HexBorderAddress borderAddress = new HexBorderAddress(ad, (HexBorder)this.borderIndex);
        //        this.info += System.Environment.NewLine + "BorderIndex: " + borderAddress.ToString();

        //        HexCornerAddress[] corners = borderAddress.Corners().ToArray();
        //        this.info += " Corners: " + string.Join(", ", ((HexBorder)this.borderIndex).Corners().Select(i => i.ToString())) + " - " + corners[0].ToString() + ", " + corners[1].ToString();

        //        if (this.borderAdresses)
        //        {
        //            Gizmos.color = Color.green;
        //            foreach (HexAddress h in borderAddress.Addresses())
        //                this.DrawHex(h);
        //        }

        //        Gizmos.color = Color.blue;
        //        this.DrawHex(ad);

        //        Gizmos.color = Color.red;
        //        this.DrawBorder(borderAddress);
        //    }

        //    if (this.cornerIndex >= 0 && this.cornerIndex < 6)
        //    {
        //        HexCornerAddress cornerAddress = new HexCornerAddress(ad, (HexCorner)this.cornerIndex);
        //        this.info = this.info + System.Environment.NewLine + "CornerIndex: " + cornerAddress;

        //        Gizmos.color = Color.red;
        //        this.DrawCorner(cornerAddress);

        //        Gizmos.color = Color.green;
        //        foreach (HexBorderAddress border in cornerAddress.Borders())
        //            this.DrawBorder(border);

        //        if (this.borderAdresses)
        //        {
        //            Gizmos.color = Color.green;
        //            foreach (HexAddress h in cornerAddress.Addresses())
        //                this.DrawHex(h);
        //        }

        //        Gizmos.color = Color.blue;
        //        this.DrawHex(ad);
        //    }

        //    if (serializated != null)
        //    {
        //        List<HexBorderAddress> b = HexExtension.DeSerialize(serializated);
        //        Gizmos.color = Color.blue;
        //        foreach (HexBorderAddress bi in b)
        //            this.DrawBorder(bi);
        //    }

        //    if (this.ringLenght > 0)
        //    {
        //        Gizmos.color = Color.blue;
        //        foreach (HexAddress n in ad.RingNeighbors(this.ringLenght))
        //            this.DrawHex(n);
        //    }

        //    this.info += System.Environment.NewLine +
        //        Vector2.Angle(this.a, this.b);
        //}

        private void DrawHex(HexAddress hex)
        {
            Vector3[] points = screen.AllCornerPositions(hex).Select(i => this.ToVector3(i)).ToArray();
            for (int i = 1; i <= 6; i++)
                Gizmos.DrawLine(points[i - 1], points[i % 6 ]);
        }

        private void DrawBorder(HexBorderAddress border)
        {
            Vector3[] borderLinePos = border.Corners().Select(i => i.Position(this.screen)).Select(i => this.ToVector3(i)).ToArray();
            Gizmos.DrawLine(borderLinePos[0], borderLinePos[1]);
        }

        private void DrawCorner(HexCornerAddress corner)
        {
            Gizmos.DrawSphere(this.ToVector3(corner.Position(this.screen)), 0.1f);
        }

        public Vector3 ToVector3(Vector2 vector) => new Vector3(vector.x, 0, vector.y);
    }

    [System.Serializable]
    public class Hexinterest
    {
        public Vector2Int address;
    }
}
