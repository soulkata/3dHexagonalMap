using Assets.Hexs;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Maps.Generations
{
    public class MapGenMountain
    {
        public MapGenConnectionCorner cornerA;
        public MapGenConnectionCorner cornerB;
        public bool cornerARestricted;
        public bool cornerBRestricted;
        public List<MapGenConnection> connections = new List<MapGenConnection>();

        public Vector2 ABDirection(HexScreenHelper screenHelper) => (screenHelper.CenterPosition(this.cornerB.hexagon) - screenHelper.CenterPosition(this.cornerA.hexagon)).normalized;
        public Vector2 BADirection(HexScreenHelper screenHelper) => (screenHelper.CenterPosition(this.cornerA.hexagon) - screenHelper.CenterPosition(this.cornerB.hexagon)).normalized;
    }
}
