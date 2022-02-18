using Assets.Hexs;
using Assets.Maps;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Debugger
{
    public class MapDebuggerBehaviour : MonoBehaviour
    {
        public bool reload;
        public MapBehaviour map;

        private void Update()
        {
            if (this.reload)
            {
                this.reload = false;
                this.map.ReloadMapRegions();
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                this.hitInfo = $"Hit: Pos: {hit.point}{Environment.NewLine}{this.map.hexScreenHelper.HexOfPosition(hit.point)}{Environment.NewLine}{this.map.hexScreenHelper.HexOfPosition(hit.point).Round()}";
            }
            else
                this.hitInfo = "";
        }

        [TextArea(3, 10)]
        public string hitInfo;

        //private void OnDrawGizmosSelected()
        //{
        //    if (this.map.regions == null)
        //        return;

        //    foreach (MapRegion region in this.map.regions)
        //    {
        //        Gizmos.color = Color.white;
        //        this.DrawHex(region.centerLocation);

        //        foreach (MapRegionNeighbor c in region.neighbors)
        //        {
        //            Gizmos.color = Color.black;
        //            foreach (HexBorderAddress b in c.conenction.borders)
        //                this.DrawBorder(b);
        //        }
        //    }
        //}

        private void DrawHex(HexAddress hex)
        {
            Vector3[] points = this.map.hexScreenHelper.AllCornerPositions(hex).Select(i => this.ToVector3(i)).ToArray();
            for (int i = 1; i <= 6; i++)
                Gizmos.DrawLine(points[i - 1], points[i % 6]);
        }

        private void DrawBorder(HexBorderAddress border)
        {
            Vector3[] borderLinePos = border.Corners().Select(i => i.Position(this.map.hexScreenHelper)).Select(i => this.ToVector3(i)).ToArray();
            Gizmos.DrawLine(borderLinePos[0], borderLinePos[1]);
        }

        private void DrawCorner(HexCornerAddress corner)
        {
            Gizmos.DrawSphere(this.ToVector3(corner.Position(this.map.hexScreenHelper)), 0.1f);
        }

        public Vector3 ToVector3(Vector2 vector) => new Vector3(vector.x, 20, vector.y);
    }
}
