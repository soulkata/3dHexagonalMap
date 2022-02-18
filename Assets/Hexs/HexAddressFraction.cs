using System.Collections.Generic;
using UnityEngine;

namespace Assets.Hexs
{
    public struct HexAddressFraction
    {
        public readonly float cubeQ;
        public readonly float cubeR;
        public float CubeS => -this.cubeQ - this.cubeR;

        public HexAddressFraction(float q, float r)
        {
            this.cubeQ = q;
            this.cubeR = r;
        }

        public HexAddress Round()
        {
            int q = Mathf.RoundToInt(this.cubeQ);
            int r = Mathf.RoundToInt(this.cubeR);
            int s = Mathf.RoundToInt(this.CubeS);

            float q_diff = Mathf.Abs(q - this.cubeQ);
            float r_diff = Mathf.Abs(r - this.cubeR);
            float s_diff = Mathf.Abs(s - this.CubeS);

            if (q_diff > r_diff && q_diff > s_diff)
                q = -r - s;
            else if (r_diff > s_diff)
                r = -q - s;
            //else
            //    s = -q - r;

            return HexAddress.FromAxial(q, r);
        }

        public override string ToString() => $"Hex Rac ({this.cubeQ}, {this.cubeR}, {this.CubeS})";
    }
}
