using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Hexs
{
    public static class HexExtension
    {
        public static readonly HexBorder[] halfBorders = new[] { HexBorder.MiddleLeft, HexBorder.LowerLeft, HexBorder.LowerRight };
        public static readonly HexBorder[] fullBorders = new[] { HexBorder.MiddleLeft, HexBorder.LowerLeft, HexBorder.LowerRight, HexBorder.MiddleRight, HexBorder.UpperRight, HexBorder.UpperLeft };
        public static readonly HexCorner[] fullCorners = new[] { HexCorner.UpperCenter, HexCorner.UpperRight, HexCorner.LowerRight, HexCorner.LowerCenter, HexCorner.LowerLeft, HexCorner.UpperLeft };

        public static HexBorder Flip(this HexBorder border) => (HexBorder)(((int)border + 3) % 6);

        public static HexCorner[] Corners(this HexBorder border)
        {
            switch (border)
            {
                case HexBorder.MiddleLeft: return new [] { HexCorner.LowerLeft, HexCorner.UpperLeft };
                case HexBorder.LowerLeft: return new[] { HexCorner.LowerCenter, HexCorner.LowerLeft };
                case HexBorder.LowerRight: return new[] { HexCorner.LowerRight, HexCorner.LowerCenter };
                case HexBorder.MiddleRight: return new[] { HexCorner.UpperRight, HexCorner.LowerRight };
                case HexBorder.UpperRight: return new[] { HexCorner.UpperCenter, HexCorner.UpperRight };
                case HexBorder.UpperLeft: return new[] { HexCorner.UpperLeft, HexCorner.UpperCenter };
                default: throw new System.Exception();
            }
        }

        public static int Distance(HexAddress a, HexAddress b) => (Mathf.Abs(a.CubeQ - b.CubeQ) + Mathf.Abs(a.CubeQ + a.CubeR - b.CubeQ - b.CubeR) + Mathf.Abs(a.CubeR - b.CubeR)) / 2;

        public static IEnumerable<HexAddress> LineAddresses(HexAddress a, HexAddress b)
        {
            float steps = HexExtension.Distance(a, b);
            Vector2 aLoc = HexExtension.HexToLocalLogic(a);
            Vector2 bLoc = HexExtension.HexToLocalLogic(b);

            for (float i = 0; i <= steps; i++)
            {
                Vector2 lerp = Vector2.Lerp(aLoc, bLoc, i / steps);
                yield return HexExtension.LocalLogicToHex(lerp);
            }
        }

        public static IEnumerable<KeyValuePair<HexAddress, float>> LineAddresses(HexAddress a, float va, HexAddress b, float vb)
        {
            float steps = HexExtension.Distance(a, b);
            Vector2 aLoc = HexExtension.HexToLocalLogic(a);
            Vector2 bLoc = HexExtension.HexToLocalLogic(b);

            for (float i = 0; i <= steps; i++)
            {
                Vector2 lerp = Vector2.Lerp(aLoc, bLoc, i / steps);
                yield return new KeyValuePair<HexAddress, float>(HexExtension.LocalLogicToHex(lerp), Mathf.Lerp(va, vb, i / steps));
            }
        }

        private static Vector2 HexToLocalLogic(HexAddress hex)
        {
            float x = Mathf.Sqrt(3f) * (hex.offsetX + 0.5f * (hex.offsetY & 1));
            float y = 1.5f * hex.offsetY;
            return new Vector2(x, y);
        }

        private static HexAddress LocalLogicToHex(Vector2 point)
        {
            float q = Mathf.Sqrt(3f) / 3f * point.x - HexScreenHelper.div13 * point.y;
            float r = HexScreenHelper.div23 * point.y;
            return new HexAddressFraction(q, r).Round();
        }

        public static void Corners(this IEnumerable<HexBorderAddress> borderLine, out HexCornerAddress cornerA, out HexCornerAddress cornerB)
        {
            Dictionary<HexCornerAddress, List<HexCornerAddress>> items = new Dictionary<HexCornerAddress, List<HexCornerAddress>>();
            foreach (HexBorderAddress border in borderLine)
            {
                foreach (HexCornerAddress corner in border.Corners())
                {
                    if (!items.TryGetValue(corner, out List<HexCornerAddress> cornerAtPos))
                    {
                        cornerAtPos = new List<HexCornerAddress>();
                        items.Add(corner, cornerAtPos);
                        cornerAtPos.Add(corner);
                    }
                    else
                    {
                        cornerAtPos.Add(corner);
                        if (cornerAtPos.Count > 2)
                            throw new System.Exception();
                    }
                }
            }

            IEnumerator<List<HexCornerAddress>> enumerator = items.Values.Where(i => i.Count == 1).GetEnumerator();
            if (!enumerator.MoveNext())
                throw new System.Exception(borderLine.Serialize());
            cornerA = enumerator.Current[0];
            if (!enumerator.MoveNext())
                throw new System.Exception(borderLine.Serialize());
            cornerB = enumerator.Current[0];
            if (enumerator.MoveNext())
                throw new System.Exception(borderLine.Serialize());
        }

        public static IEnumerable<HexBorderAddress> SortFromCorner(this IEnumerable<HexBorderAddress> borderLine, HexCornerAddress fromCorner)
        {
            List<HexBorderAddress> remaining = new List<HexBorderAddress>(borderLine);
            while (remaining.Count > 0)
            {
                HexBorderAddress border = remaining.WhereConstant(fromCorner, (r, c) => r.Corners().Contains(c)).First();
                yield return border;
                remaining.Remove(border);
                fromCorner = border.Corners().WhereConstant(fromCorner, (c1, c2) => c1 != c2).First();
            }
        }

        public static string Serialize(this IEnumerable<HexBorderAddress> borderLine) => string.Join("_", borderLine.Select(i => i.Serialize()));
        public static List<HexBorderAddress> DeSerialize(string serialized)
        {
            List<HexBorderAddress> ret = new List<HexBorderAddress>();
            foreach (string s in serialized.Split("_"))
                ret.Add(HexBorderAddress.DeSerialize(s));
            return ret;
        }
    }
}
