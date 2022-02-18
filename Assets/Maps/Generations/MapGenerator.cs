using Assets.Hashs;
using Assets.Hexs;
using Assets.Hexs.Hashs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Maps.Generations
{
    public class MapGenerator
    {
        public readonly MapBehaviour map;
        public readonly MapParameter parameter;
        public readonly HexScreenHelper hexScreen;
        public readonly Vector2Int finalSize;
        public readonly MapGenRegion[,] regions;
        public readonly HexAddressRegionArray<MapGenTile> tiles;
        public readonly List<MapGenConnection> connections = new List<MapGenConnection>();
        public readonly List<MapGenConnection> landConnections = new List<MapGenConnection>();
        public readonly List<MapGenConnection> coastConnections = new List<MapGenConnection>();
        public readonly List<MapGenConnection> landAvailableConnections = new List<MapGenConnection>();
        public readonly List<MapGenMountain> mountains = new List<MapGenMountain>();
        public readonly List<MapGenRiver> riverEstuaries = new List<MapGenRiver>();
        public readonly List<MapGenRiver> rivers = new List<MapGenRiver>();
        public readonly List<MapGenRiver> riversThatCanbeExtended = new List<MapGenRiver>();
        public readonly List<KeyValuePair<float, MapGenRegion>> moistureFrontier = new List<KeyValuePair<float, MapGenRegion>>();

        public MapGenerator(MapBehaviour mapBehaviour)
        {
            this.map = mapBehaviour;
            this.parameter = mapBehaviour.parameterGameplay;
            this.hexScreen = mapBehaviour.hexScreenHelper;
            this.finalSize = new Vector2Int(this.parameter.regionCount.x * this.parameter.regionSize.x, this.parameter.regionCount.y * this.parameter.regionSize.y);
            this.tiles = new HexAddressRegionArray<MapGenTile>(this.finalSize);
            this.regions = new MapGenRegion[this.parameter.regionCount.x, this.parameter.regionCount.y];
        }

        public void Generate()
        {
            this.FillRegions();
            this.FillTiles();
            this.InitializeRegionConnections();
            this.AddMountains();
            this.AddRivers();
            this.CarveMountains();
            this.CarveRivers();
            this.PaintMoisture();
            this.GenerateMoistureTiles();
            this.GenerateChunks();
        }

        private void FillRegions()
        {
            IPerlin2D elevationNoisePerlin = this.LandSeparationNoise();
            IPerlin2D cliffPerlin = this.CliffPerlin();

            int xBorder = (this.parameter.regionSize.x - this.parameter.regionCenterSize.x) / 2;
            int yBorder = (this.parameter.regionSize.y - this.parameter.regionCenterSize.y) / 2;

            RandomGenerator random = new RandomGenerator(this.map.seed + nameof(FillRegions).GetHashCode());
            for (int regionX = 0; regionX < this.parameter.regionCount.x; regionX++)
                for (int regionY = 0; regionY < this.parameter.regionCount.y; regionY++)
                {
                    List<int> availableXPositions = new List<int>();
                    for (int availXIndex = 0; availXIndex < this.parameter.regionCenterSize.x; availXIndex++)
                    {
                        int index = regionX * this.parameter.regionSize.x + xBorder + availXIndex;

                        if (regionY > 0)
                            if (Mathf.Abs(index - this.regions[regionX, regionY - 1].centerLocation.offsetX) < this.parameter.offsetFromPreviousCoordinate.x)
                                continue;

                        availableXPositions.Add(index);
                    }

                    List<int> availableYPositions = new List<int>();
                    for (int availYIndex = 0; availYIndex < this.parameter.regionCenterSize.y; availYIndex++)
                    {
                        int index = regionY * this.parameter.regionSize.y + yBorder + availYIndex;

                        if (regionX > 0)
                            if (Mathf.Abs(index - this.regions[regionX - 1, regionY].centerLocation.offsetY) < this.parameter.offsetFromPreviousCoordinate.y)
                                continue;

                        availableYPositions.Add(index);
                    }

                    MapGenRegion region = new MapGenRegion();
                    region.centerLocation = new HexAddress(availableXPositions[random.Random0Max(availableXPositions.Count)], availableYPositions[random.Random0Max(availableYPositions.Count)]);
                    region.cliff = cliffPerlin.Value(this.hexScreen.CenterPosition(region.centerLocation)) > 0.55f;
                    this.regions[regionX, regionY] = region;
                    this.tiles[region.centerLocation] = new MapGenTile() { region = region };

                    if ((regionX == 0) ||
                        (regionX == this.parameter.regionCount.x - 1) ||
                        (regionY == 0) ||
                        (regionY == this.parameter.regionCount.y - 1))
                        region.biome = MapRegionBiome.Water;
                    else
                    {
                        if (elevationNoisePerlin.Value(this.hexScreen.CenterPosition(region.centerLocation)) < this.parameter.landDistributionWaterLevel)
                            region.biome = MapRegionBiome.Water;
                        else
                            region.biome = MapRegionBiome.Grassland;
                    }
                }

            this.regions[1, 1].biome = MapRegionBiome.Water;
            this.regions[1, this.parameter.regionCount.y - 2].biome = MapRegionBiome.Water;
            this.regions[this.parameter.regionCount.x - 2, 1].biome = MapRegionBiome.Water;
            this.regions[this.parameter.regionCount.x - 2, this.parameter.regionCount.y - 2].biome = MapRegionBiome.Water;

            foreach (MapGenRegion r in this.regions)
                if (r.biome == MapRegionBiome.Water)
                    this.SetMoisture(r, this.parameter.waterMoisture);
        }

        private void FillTiles()
        {
            foreach (HexAddress hex in this.tiles.Keys)
            {
                int regCenterIndexX = hex.offsetX / this.parameter.regionSize.x;
                int regCenterIndexY = hex.offsetY / this.parameter.regionSize.y;

                MapGenTile tile = this.tiles[hex];
                if (tile == null)
                {
                    tile = new MapGenTile();
                    this.tiles[hex] = tile;
                }

                if (tile.region == null)
                {
                    Vector2 v = this.hexScreen.CenterPosition(hex);
                    float minDistance = float.MaxValue;
                    List<MapGenRegion> regions = new List<MapGenRegion>();
                    for (int regIndexX = Mathf.Max(0, regCenterIndexX - 1); regIndexX <= Mathf.Min(regCenterIndexX + 1, this.parameter.regionCount.x - 1); regIndexX++)
                        for (int regIndexY = Mathf.Max(0, regCenterIndexY - 1); regIndexY <= Mathf.Min(regCenterIndexY + 1, this.parameter.regionCount.y - 1); regIndexY++)
                        {
                            float d = Vector2.Distance(v, this.hexScreen.CenterPosition(this.regions[regIndexX, regIndexY].centerLocation));
                            if (d + 0.001f < minDistance)
                            {
                                minDistance = d;
                                regions.Clear();
                                regions.Add(this.regions[regIndexX, regIndexY]);
                            }
                            else if (d - 0.001f > minDistance)
                                continue;
                            else
                                regions.Add(this.regions[regIndexX, regIndexY]);
                        }

                    tile.region = regions[0];
                }
            }
        }

        private void InitializeRegionConnections()
        {
            RandomGenerator random = new RandomGenerator(nameof(InitializeRegionConnections).GetHashCode() + this.map.seed);

            foreach (HexAddress hex in this.tiles.Keys)
            {
                MapGenTile hexTile = this.tiles[hex];
                foreach (Hexs.HexBorder border in HexExtension.fullBorders)
                {
                    HexAddress neighbor = hex.GetNeighborAddress(border);
                    if (!neighbor.Contained(this.finalSize))
                        continue;

                    MapGenTile neighborTile = this.tiles[neighbor];
                    if (neighborTile.region == hexTile.region)
                        continue;

                    MapGenConnection connection = hexTile.region.connections.WhereConstant(neighborTile.region, (i, c) => i.Contains(c)).FirstOrDefault();
                    if (connection == null)
                    {
                        connection = new MapGenConnection(hexTile.region, neighborTile.region);
                        this.connections.Add(connection);

                        if ((hexTile.region.biome != MapRegionBiome.Water) &&
                            (neighborTile.region.biome != MapRegionBiome.Water))
                            this.landConnections.Add(connection);
                        else if ((hexTile.region.biome != MapRegionBiome.Water) ||
                            (neighborTile.region.biome != MapRegionBiome.Water))
                            this.coastConnections.Add(connection);

                    }

                    connection.borders.Add(new HexBorderAddress(hex, border));
                }
            }
            this.landAvailableConnections.AddRange(this.landConnections);

            foreach (MapGenConnection connection in this.connections)
            {
                connection.borders.Corners(out HexCornerAddress cornerA, out HexCornerAddress cornerB);
                HexAddress addressA = cornerA.Addresses().Where(i => i.Contained(this.finalSize)).First();
                HexAddress addressB = cornerB.Addresses().Where(i => i.Contained(this.finalSize)).First();

                connection.cornerA = new MapGenConnectionCorner(connection, cornerA, addressA, addressB, this.hexScreen);
                connection.cornerB = new MapGenConnectionCorner(connection, cornerB, addressB, addressA, this.hexScreen);

                connection.cornerA.reverse = connection.cornerB;
                connection.cornerB.reverse = connection.cornerA;
            }
        }

        #region [  Mountains  ]

        private void AddMountains()
        {
            RandomGenerator random = new RandomGenerator(this.map.seed + nameof(AddMountains).GetHashCode());

            while ((this.landAvailableConnections.Count > 0) &&
                ((float)this.mountains.Count / (float)this.landAvailableConnections.Count < this.parameter.percentualOfConnectionsAsMoutains))
            {
                MapGenConnection connection = random.PopRandom(this.landAvailableConnections);

                MapGenMountain mountain = new MapGenMountain();
                mountain.cornerA = connection.cornerA;
                mountain.cornerB = connection.cornerB;
                mountain.connections.Add(connection);
                this.landAvailableConnections.Remove(connection);
                if (!this.ValidSpotForMountainCorner(mountain.cornerA.hexagon, out mountain.cornerARestricted))
                    continue;
                if (!this.ValidSpotForMountainCorner(mountain.cornerB.hexagon, out mountain.cornerBRestricted))
                    continue;

                if (mountain.cornerARestricted && mountain.cornerBRestricted)
                    continue;

                this.mountains.Add(mountain);
                connection.mountain = mountain;

                int expansions = (int)this.parameter.mountainExtensionLenght.Evaluate(random.Random01());
                while (expansions > 0)
                {
                    expansions--;

                    List<MountainPossibleContinuation> nextExpansionsCandidate = new List<MountainPossibleContinuation>();

                    if (!mountain.cornerARestricted)
                    {
                        Vector2 direction = (this.hexScreen.CenterPosition(mountain.cornerB.hexagon) - this.hexScreen.CenterPosition(mountain.cornerA.hexagon)).normalized;
                        foreach (MapGenConnectionCorner corner in this.OtherConnections(mountain.cornerA).Select(i => i.reverse))
                        {
                            if (this.ValidSpotForMountainCorner(corner.hexagon, out bool restricted, mountain))
                            {
                                if (restricted && mountain.cornerBRestricted)
                                    continue;

                                nextExpansionsCandidate.Add(new MountainPossibleContinuation() { corner = corner, restrict = restricted, sideA = true, angleDif = Vector2.Angle(direction, corner.direction) });
                            }
                        }
                    }

                    if (!mountain.cornerBRestricted)
                    {
                        Vector2 direction = (this.hexScreen.CenterPosition(mountain.cornerA.hexagon) - this.hexScreen.CenterPosition(mountain.cornerB.hexagon)).normalized;
                        foreach (MapGenConnectionCorner corner in this.OtherConnections(mountain.cornerB).Select(i => i.reverse))
                        {
                            if (this.ValidSpotForMountainCorner(corner.hexagon, out bool restricted, mountain))
                            {
                                if (restricted && mountain.cornerARestricted)
                                    continue;

                                nextExpansionsCandidate.Add(new MountainPossibleContinuation() { corner = corner, restrict = restricted, sideA = false, angleDif = Vector2.Angle(direction, corner.direction) });
                            }
                        }
                    }

                    if (nextExpansionsCandidate.Count == 0)
                        break;

                    nextExpansionsCandidate.Sort((x, y) => y.angleDif.CompareTo(x.angleDif));

                    MountainPossibleContinuation nextExpansion = nextExpansionsCandidate[Mathf.Clamp((int)((float)nextExpansionsCandidate.Count * this.parameter.mountainStraitness.Evaluate(random.Random01())), 0, nextExpansionsCandidate.Count - 1)];

                    if (nextExpansion.sideA)
                    {
                        mountain.connections.Insert(0, nextExpansion.corner.connection);
                        mountain.cornerA = nextExpansion.corner;
                        mountain.cornerARestricted = nextExpansion.restrict;
                    }
                    else
                    {
                        mountain.connections.Add(nextExpansion.corner.connection);
                        mountain.cornerB = nextExpansion.corner;
                        mountain.cornerBRestricted = nextExpansion.restrict;
                    }
                    nextExpansion.corner.connection.mountain = mountain;
                    this.landAvailableConnections.Remove(nextExpansion.corner.connection);
                }
            }
        }

        class MountainPossibleContinuation
        {
            public bool sideA;
            public MapGenConnectionCorner corner;
            public bool restrict;
            public float angleDif;
        }

        private bool ValidSpotForMountainCorner(HexAddress hex, out bool restricted, MapGenMountain ignoreMountain = null)
        {
            restricted = false;
            for (int ringSize = 1; ringSize <= this.parameter.mountainConnectionDistance; ringSize++)
            {
                foreach (HexAddress ringHex in this.tiles.RingNeighbors(hex, ringSize))
                {
                    MapGenTile ringTile = this.tiles[ringHex];
                    if (ringTile.region.biome == MapRegionBiome.Water)
                        restricted = true;

                    IEnumerable<MapGenConnection> connections = this.ConnectionsOfTile(ringHex, ringTile);
                    if (ignoreMountain != null)
                        connections = connections.WhereConstant(ignoreMountain, (i, m) => i.mountain != m);

                    if (connections.Where(i => i.mountain != null || i.river != null).Any())
                    {
                        restricted = true;
                        return false;
                    }
                }
            }

            return true;
        }

        private void CarveMountains()
        {
            RandomGenerator random = new RandomGenerator(nameof(CarveMountains).GetHashCode() + this.map.seed);
            foreach (MapGenMountain mountain in this.mountains)
            {
                HexCornerAddress fromCorner = mountain.cornerA.corner;
                HexAddress fromAddress = mountain.cornerA.hexagon;
                float fromWide = 1;
                for (int connectionIndex = 0; connectionIndex < mountain.connections.Count; connectionIndex++)
                {
                    MapGenConnection connection = mountain.connections[connectionIndex];

                    HexCornerAddress toCorner;
                    HexAddress toAddress;
                    float toWide;
                    if (connectionIndex == mountain.connections.Count - 1)
                    {
                        toCorner = mountain.cornerB.corner;
                        toAddress = mountain.cornerB.hexagon;
                        toWide = 1;
                    }
                    else
                    {
                        if (connection.cornerA.corner == fromCorner)
                        {
                            toCorner = connection.cornerB.corner;
                            toAddress = connection.cornerB.hexagon;
                        }
                        else
                        {
                            toCorner = connection.cornerA.corner;
                            toAddress = connection.cornerA.hexagon;
                        }

                        toWide = this.parameter.mountainConnectionHexagon.Evaluate(random.Random01());
                    }

                    // float midWide = this.parameter.mountainConnectionHexagon.Evaluate(random.Random01());

                    foreach (KeyValuePair<HexAddress, float> it in HexExtension.LineAddresses(fromAddress, fromWide, toAddress, toWide))
                    {
                        if (!it.Key.Contained(this.finalSize))
                            continue;

                        //float v;
                        //if (it.Value < 0.5f)
                        //    v = Mathf.Lerp(fromWide, midWide, it.Value / 2f);
                        //else
                        //    v = Mathf.Lerp(midWide, toWide, 0.5f + it.Value / 2f);

                        if (it.Value < 2)
                            this.tiles[it.Key].mountain = true;
                        else if (it.Value < 3)
                        {
                            this.tiles[it.Key].mountain = true;

                            HexAddress a = it.Key.GetNeighborAddress(HexBorder.MiddleLeft);
                            if (a.Contained(this.finalSize))
                                this.tiles[a].mountain = true;

                            a = it.Key.GetNeighborAddress(HexBorder.LowerLeft);
                            if (a.Contained(this.finalSize))
                                this.tiles[a].mountain = true;
                        } else if (it.Value < 4)
                        {
                            this.tiles[it.Key].mountain = true;
                            foreach (HexAddress a in this.tiles.RingNeighbors(it.Key, 1))
                                this.tiles[a].mountain = true;
                        } else if (it.Value < 5)
                        {
                            this.tiles[it.Key].mountain = true;
                            foreach (HexAddress ar in this.tiles.RingNeighbors(it.Key, 1))
                                this.tiles[ar].mountain = true;

                            HexAddress a = it.Key.GetNeighborAddress(HexBorder.MiddleLeft);
                            if (a.Contained(this.finalSize))
                                this.tiles[a].mountain = true;
                            foreach (HexAddress ar in this.tiles.RingNeighbors(a, 1))
                                this.tiles[ar].mountain = true;

                            a = it.Key.GetNeighborAddress(HexBorder.LowerLeft);
                            if (a.Contained(this.finalSize))
                                this.tiles[a].mountain = true;
                            foreach (HexAddress ar in this.tiles.RingNeighbors(a, 1))
                                this.tiles[ar].mountain = true;
                        } else
                        {
                            this.tiles[it.Key].mountain = true;
                            foreach (HexAddress a in this.tiles.RingNeighbors(it.Key, 1))
                                this.tiles[a].mountain = true;
                            foreach (HexAddress a in this.tiles.RingNeighbors(it.Key, 2))
                                this.tiles[a].mountain = true;
                        }
                    }

                    fromCorner = toCorner;
                    fromAddress = toAddress;
                    fromWide = toWide;
                }
            }
        }

        #endregion

        #region [  Rivers  ]

        private void AddRivers()
        {
            this.AddRiverEstuaries();
            this.AppendRiverExtensions();
        }

        private void AddRiverEstuaries()
        {
            List<MapGenConnectionCorner> availableCoastPlaces = new List<MapGenConnectionCorner>();
            foreach (MapGenConnection cnn in this.coastConnections)
            {
                availableCoastPlaces.Add(cnn.cornerA);
                availableCoastPlaces.Add(cnn.cornerB);
            }

            RandomGenerator random = new RandomGenerator(nameof(AddRiverEstuaries).GetHashCode() + this.map.seed);
            while ((availableCoastPlaces.Count > 0) &&
                ((float)this.riverEstuaries.Count / (float)availableCoastPlaces.Count < this.parameter.percentualOfConnectionsAsNewRivers))
            {
                MapGenConnectionCorner coastCorner = random.PopRandom(availableCoastPlaces);
                this.AppendRiverEstuary(coastCorner, random);
            }
        }

        private void AppendRiverEstuary(MapGenConnectionCorner coastCorner, RandomGenerator random)
        {
            foreach (MapGenConnectionCorner riverCandidate in this.OtherConnections(coastCorner))
            {
                if (riverCandidate.connection.regionA.biome == MapRegionBiome.Water ||
                    riverCandidate.connection.regionB.biome == MapRegionBiome.Water)
                    continue;

                if ((riverCandidate.connection.river != null) || (riverCandidate.connection.mountain != null))
                    continue;

                if (!this.ValidSpotForRiverCorner(riverCandidate.hexagon, false))
                    continue;

                // Look of the other corner of this connection, to find if isn't another river of coast there
                MapGenConnectionCorner cornerFarFromCoast = riverCandidate.reverse;

                if (!this.ValidSpotForRiverCorner(cornerFarFromCoast.hexagon, false))
                    continue;

                MapGenRiver river = new MapGenRiver(riverCandidate);
                riverCandidate.connection.river = river;
                this.rivers.Add(river);
                this.riverEstuaries.Add(river);
                this.riversThatCanbeExtended.Add(river);
                this.landAvailableConnections.Remove(riverCandidate.connection);

                this.SetMoisture(riverCandidate.connection.regionA, this.parameter.riverMoisture);
                this.SetMoisture(riverCandidate.connection.regionB, this.parameter.riverMoisture);

                Vector2 origin = this.hexScreen.CenterPosition(river.wideToNarrow.hexagon);
                int lenght = (int)this.parameter.riverExtensionLenght.Evaluate(random.Random01());
                while (lenght > 0)
                {
                    lenght--;
                    if (!this.AppendRiverExtension(origin, ref river))
                    {
                        this.riversThatCanbeExtended.Remove(river);
                        break;
                    }
                }

                return;
            }
        }

        private void AppendRiverExtensions()
        {
            RandomGenerator random = new RandomGenerator(nameof(AppendRiverExtensions).GetHashCode() + this.map.seed);
            while ((this.riversThatCanbeExtended.Count > 0) &&
                (this.landAvailableConnections.Count > 0) &&
                ((float)this.rivers.Count / (float)this.landAvailableConnections.Count < this.parameter.percentualOfConnectionsAsRiversExtensions))
            {
                MapGenRiver river = random.GetRandom(this.riversThatCanbeExtended);
                Vector2 origin = this.hexScreen.CenterPosition(river.wideToNarrow.hexagon);

                if (this.AppendRiverExtension(origin, ref river))
                {
                    int lenght = (int)this.parameter.riverExtensionLenght.Evaluate(random.Random01());
                    while (lenght > 0)
                    {
                        lenght--;
                        if (!this.AppendRiverExtension(origin, ref river))
                        {
                            this.riversThatCanbeExtended.Remove(river);
                            break;
                        }
                    }
                }
                else
                    this.riversThatCanbeExtended.Remove(river);
            }
        }

        private bool AppendRiverExtension(Vector2 origin, ref MapGenRiver river)
        {
            MapGenConnectionCorner narrowCorner = river.wideToNarrow.reverse;

            List<MapGenConnectionCorner> narrowConnections = this.OtherConnections(narrowCorner).ToList();

            Vector2 fromRiverDirection = (this.hexScreen.CenterPosition(river.wideToNarrow.hexagon) - origin).normalized;
            while (narrowConnections.Count > 0)
            {
                float minAngle = 400;
                MapGenConnectionCorner connectionFromNarrow = null;
                for (int i = 0; i < narrowConnections.Count; i++)
                {
                    float angle = Vector2.Angle(fromRiverDirection, narrowConnections[i].direction);
                    if (angle < minAngle)
                    {
                        minAngle = angle;
                        connectionFromNarrow = narrowConnections[i];
                    }
                }

                narrowConnections.Remove(connectionFromNarrow);

                if (connectionFromNarrow.connection.river != null ||
                    connectionFromNarrow.connection.mountain != null ||
                    connectionFromNarrow.connection.regionA.biome == MapRegionBiome.Water ||
                    connectionFromNarrow.connection.regionB.biome == MapRegionBiome.Water)
                    continue;

                if (!this.ValidSpotForRiverCorner(connectionFromNarrow.reverse.hexagon, true, river))
                    continue;

                river = new MapGenRiver(river, connectionFromNarrow);
                connectionFromNarrow.connection.river = river;
                this.rivers.Add(river);
                this.riverEstuaries.Add(river);
                this.riversThatCanbeExtended.Add(river);
                this.landAvailableConnections.Remove(connectionFromNarrow.connection);
                this.SetMoisture(connectionFromNarrow.connection.regionA, this.parameter.riverMoisture);
                this.SetMoisture(connectionFromNarrow.connection.regionB, this.parameter.riverMoisture);
                return true;
            }

            return false;
        }

        private bool ValidSpotForRiverCorner(HexAddress hex, bool checkWater, MapGenRiver ignoreRiver = null)
        {
            for (int ringSize = 1; ringSize <= this.parameter.riverConnectionDistance; ringSize++)
            {
                foreach (HexAddress ringHex in this.tiles.RingNeighbors(hex, ringSize))
                {
                    MapGenTile ringTile = this.tiles[ringHex];
                    if (checkWater &&
                        (ringTile.region.biome == MapRegionBiome.Water))
                        return false;

                    IEnumerable<MapGenConnection> connections = this.ConnectionsOfTile(ringHex, ringTile);
                    if (ignoreRiver != null)
                    {
                        if (ignoreRiver.parent != null)
                            connections = connections.WhereConstant(ignoreRiver, (i, m) => i.river != m && i.river != m.parent);
                        else
                            connections = connections.WhereConstant(ignoreRiver, (i, m) => i.river != m);
                    }

                    if (connections.Where(i => i.mountain != null || i.river != null).Any())
                        return false;
                }
            }

            return true;
        }

        private void CarveRivers()
        {
            RandomGenerator random = new RandomGenerator(nameof(CarveRivers).GetHashCode() + this.map.seed);
            foreach (MapGenRiver river in this.riverEstuaries)
                this.CarveRiverBranch(river, random);
        }

        private float CarveRiverBranch(MapGenRiver river, RandomGenerator random)
        {
            float volume;
            if (river.children.Count > 0)
            {
                List<float> vols = new List<float>();
                foreach (MapGenRiver riverExtension in river.children)
                    vols.Add(this.CarveRiverBranch(riverExtension, random));

                float max = vols.Max();
                volume = max;
                foreach (float f in vols)
                {
                    if (f == max)
                        continue;
                    volume += f / 4;
                }
            }
            else
                volume = this.parameter.riverInitialVolume.Evaluate(random.Random01());

            MapGenConnectionCorner waterFlow = river.wideToNarrow.reverse;
            foreach (HexAddress riverCenterAddress in this.tiles.LineAddresses(waterFlow.hexagon, river.wideToNarrow.hexagon))
            {
                int hex = (int)this.parameter.riverHexagonByVolume.Evaluate(volume);
                if (hex < 2)
                {
                    MapGenTile tile = this.tiles[riverCenterAddress];
                    tile.river = true;
                }
                else if (hex < 3)
                {
                    MapGenTile tile = this.tiles[riverCenterAddress];
                    tile.river = true;

                    HexAddress riverCorner1 = riverCenterAddress.GetNeighborAddress(HexBorder.MiddleLeft);
                    if (riverCorner1.Contained(this.finalSize))
                    {
                        tile = this.tiles[riverCorner1];
                        tile.river = true;
                    }

                    HexAddress riverCorner2 = riverCenterAddress.GetNeighborAddress(HexBorder.LowerLeft);
                    if (riverCorner2.Contained(this.finalSize))
                    {
                        tile = this.tiles[riverCorner2];
                        tile.river = true;
                    }
                }
                else
                {
                    MapGenTile tile = this.tiles[riverCenterAddress];
                    tile.river = true;

                    foreach (HexAddress riverCornerAddress in this.tiles.RingNeighbors(riverCenterAddress, 1))
                    {
                        tile = this.tiles[riverCornerAddress];
                        tile.river = true;
                    }
                }

                volume += this.parameter.riverVolumeGainPerTile.Evaluate(random.Random01());
            }

            return volume;
        }

        #endregion

        private IEnumerable<MapGenConnection> ConnectionsOfTile(HexAddress hex) => this.ConnectionsOfTile(hex, this.tiles[hex]);
        private IEnumerable<MapGenConnection> ConnectionsOfTile(HexAddress hex, MapGenTile tile)
        {
            foreach (MapGenConnection connection in tile.region.connections)
            {
                if (connection.borders.WhereConstant(hex, (i, h) => i.Addresses().Contains(h)).Any())
                    yield return connection;
            }
        }
        private List<MapGenConnectionCorner> OtherConnections(MapGenConnectionCorner corner)
        {
            List<MapGenConnectionCorner> ret = new List<MapGenConnectionCorner>();
            foreach (HexAddress address in corner.corner.Addresses().Where(i => i.Contained(this.finalSize)))
            {
                foreach (MapGenConnection cnn in this.tiles[address].region.connections)
                {
                    if (cnn == corner.connection)
                        continue;

                    if (ret.Select(i => i.connection).Contains(cnn))
                        continue;

                    if (cnn.cornerA.corner == corner.corner)
                        ret.Add(cnn.cornerA);
                    else if (cnn.cornerB.corner == corner.corner)
                        ret.Add(cnn.cornerB);
                }
            }

            return ret;
        }

        public virtual IPerlin2D LandSeparationNoise()
        {
            IPerlin2D elevationNoisePerlin = new Perlin2DCombined(new[]
                {
                    new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.map.seed + 1111), this.parameter.noiseBaseLenght), 0.5f),
                    new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.map.seed + 2222), this.parameter.noiseBaseLenght / 2), 0.25f),
                    new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.map.seed + 3333), this.parameter.noiseBaseLenght / 4), 0.125f)
                });
            return new Perlin2DEnsureWaterOnEdge(elevationNoisePerlin, this.map);
        }

        public virtual IPerlin2D CliffPerlin()
        {
            return new Perlin2DCombined(new[]
                {
                    new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.map.seed + 1111 + nameof(CliffPerlin).GetHashCode()), 30), 0.5f),
                    new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.map.seed + 2222 + nameof(CliffPerlin).GetHashCode()), 15), 0.25f),
                    new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.map.seed + 3333 + nameof(CliffPerlin).GetHashCode()), 7), 0.125f)
                });
        }

        public virtual IPerlin2D MoisurePerlin()
        {
            return new Perlin2DCombined(new[]
                {
                    new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.map.seed + 1111 + nameof(MoisurePerlin).GetHashCode()), 30), 0.5f),
                    new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.map.seed + 2222 + nameof(MoisurePerlin).GetHashCode()), 15), 0.25f),
                    new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.map.seed + 3333 + nameof(MoisurePerlin).GetHashCode()), 7), 0.125f)
                });
        }

        private void SetMoisture(MapGenRegion region, float moisture)
        {
            if (region.moisture >= moisture)
                return;

            if (moisture < 0)
                return;

            region.moisture = moisture;
            for (int i = 0; i < this.moistureFrontier.Count; i++)
            {
                if (this.moistureFrontier[i].Value == region)
                {
                    if (moisture > this.moistureFrontier[i].Key)
                        this.moistureFrontier[i] = new KeyValuePair<float, MapGenRegion>(moisture, region);
                    return;
                }
            }

            this.moistureFrontier.Add(new KeyValuePair<float, MapGenRegion>(moisture, region));
        }

        private void PaintMoisture()
        {
            float previousFrontier = Mathf.Max(this.parameter.riverMoisture, this.parameter.waterMoisture);
            while (this.moistureFrontier.Count > 0)
            {
                int greaterIndex = -1;
                float greaterMoisture = 0;
                for (int nextIndex = 0; nextIndex < this.moistureFrontier.Count; nextIndex++)
                {
                    float moisture = this.moistureFrontier[nextIndex].Key;
                    if (moisture == previousFrontier)
                    {
                        greaterMoisture = moisture;
                        greaterIndex = nextIndex;
                        break;
                    }

                    if (moisture > greaterMoisture)
                    {
                        greaterMoisture = moisture;
                        greaterIndex = nextIndex;
                    }

                    nextIndex++;
                }
                previousFrontier = greaterMoisture;
                MapGenRegion greaterRegion = this.moistureFrontier[greaterIndex].Value;
                this.moistureFrontier.RemoveAt(greaterIndex);

                foreach (MapGenConnection connection in greaterRegion.connections)
                {
                    if (connection.regionA != greaterRegion)
                    {
                        this.SetMoisture(connection.regionA, greaterMoisture - (connection.mountain != null ? this.parameter.moistureDecayMountain : this.parameter.moistureDecay));
                        continue;
                    }
                    if (connection.regionB != greaterRegion)
                    {
                        this.SetMoisture(connection.regionB, greaterMoisture - (connection.mountain != null ? this.parameter.moistureDecayMountain : this.parameter.moistureDecay));
                        continue;
                    }
                }
            }
        }

        private IPerlin2D ElevationPerlin()
        {
            float l = 4f; // this.parameterGameplay.noiseBaseLenght;
            return new Perlin2DCombined(new[]
                {
                    new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.map.seed + 1111), l), 0.5f),
                    new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.map.seed + 2222), l / 2f), 0.25f),
                    new System.Collections.Generic.KeyValuePair<IPerlin2D, float>(new Perlin2DFrequency(new Perlin2D(this.map.seed + 3333), l / 4f), 0.125f)
                });
        }

        const float waterLvl = 2.2f / 20f;
        const float riverLvl = 2f / 20f;
        const float beachLvl = 3f / 20f;
        const float grassLvl = 5f / 20f;
        const float mountainRangeLvl = 1f - grassLvl;

        public class Perlin2DTileHeight : IHexPerlin2D
        {
            internal HexAddressRegionArray<MapGenTile> tiles;
            internal HexScreenHelper hexScreenHelper;
            internal float heightScale;
            internal Vector2Int finalSize;
            internal IPerlin2D perlin;
            internal float[] mountainHeightWeight;

            public Perlin2DTileHeight(HexAddressRegionArray<MapGenTile> tiles, HexScreenHelper hexScreenHelper, Vector2Int finalSize, float heightScale, IPerlin2D perlin, float[] mountainHeightWeight)
            {
                this.tiles = tiles;
                this.hexScreenHelper = hexScreenHelper;
                this.heightScale = heightScale;
                this.finalSize = finalSize;
                this.perlin = perlin;
                this.mountainHeightWeight = mountainHeightWeight;
            }

            public float CornerValue(HexCornerAddress corner)
            {
                List<float> values = new List<float>();
                int waterCount = 0;
                foreach (HexAddress hex in corner.Addresses().Where(i => i.Contained(this.finalSize)))
                {
                    MapGenTile tile = this.tiles[hex];
                    if (tile.region.biome == MapRegionBiome.Water || tile.river)
                        waterCount++;
                    values.Add(this.HexagonValue(hex));
                }
                if (waterCount > 1)
                    return values.Min();

                if (values.Count == 3)
                {
                    values.Sort();
                    values.Add(values[1]);
                    values.Add(values[1]);
                }

                return values.Average();
            }

            public float HexagonValue(HexAddress address)
            {
                MapGenTile tile = this.tiles[address];
                if (tile.region.biome == MapRegionBiome.Water)
                {
                    bool anyBeach = false;
                    bool anyCliff = false;

                    foreach (HexAddress ring in this.tiles.RingNeighbors(address, 1))
                    {
                        MapGenTile ringTile = this.tiles[ring];

                        if (ringTile.region.biome == MapRegionBiome.Water)
                            continue;

                        anyCliff |= ringTile.region.cliff;
                        anyBeach |= !ringTile.region.cliff;
                    }

                    if (anyBeach)
                        return this.AddPerlin(address, riverLvl, 0.2f);

                    if (anyCliff)
                        return this.AddPerlin(address, riverLvl * 0.9f, 0.2f);

                    foreach (HexAddress ring in this.tiles.RingNeighbors(address, 2))
                    {
                        MapGenTile ringTile = this.tiles[ring];

                        if (ringTile.region.biome == MapRegionBiome.Water)
                            continue;

                        anyCliff = true;
                        break;
                    }

                    if (anyCliff)
                        return this.AddPerlin(address, riverLvl * 0.75f, 0.2f);

                    foreach (HexAddress ring in this.tiles.RingNeighbors(address, 3))
                    {
                        MapGenTile ringTile = this.tiles[ring];

                        if (ringTile.region.biome == MapRegionBiome.Water)
                            continue;

                        anyCliff = true;
                        break;
                    }

                    if (anyCliff)
                        return this.AddPerlin(address, riverLvl * 0.5f, 0.2f);

                    foreach (HexAddress ring in this.tiles.RingNeighbors(address, 4))
                    {
                        MapGenTile ringTile = this.tiles[ring];

                        if (ringTile.region.biome == MapRegionBiome.Water)
                            continue;

                        anyCliff = true;
                        break;
                    }

                    if (anyCliff)
                        return this.AddPerlin(address, riverLvl * 0.25f, 0.2f);

                    return 0;
                }

                if (tile.river)
                    return this.AddPerlin(address, riverLvl, 0.2f);

                if (tile.mountain)
                    return MountainHeight(address);


                return this.BiomeHeight(address, tile);
            }

            private float BiomeHeight(HexAddress address, MapGenTile tile)
            {
                if (tile.region.cliff)
                    return this.AddPerlin(address, grassLvl, 0.8f);

                bool ringCliff = false;
                foreach (HexAddress ring in this.tiles.RingNeighbors(address, 1))
                {
                    MapGenTile ringTile = this.tiles[ring];
                    ringCliff |= ringTile.region.cliff;

                    if (ringTile.river || ringTile.region.biome == MapRegionBiome.Water)
                        return this.AddPerlin(address, beachLvl, 0.2f);
                }

                if (ringCliff)
                    return this.AddPerlin(address, grassLvl, 0.8f);

                foreach (HexAddress ring in this.tiles.RingNeighbors(address, 2))
                {
                    MapGenTile ringTile = this.tiles[ring];
                    ringCliff |= ringTile.region.cliff;

                    if (ringTile.river || ringTile.region.biome == MapRegionBiome.Water)
                        return this.AddPerlin(address, (beachLvl + beachLvl + grassLvl) / 3f, 0.3f);
                }

                if (ringCliff)
                    return this.AddPerlin(address, grassLvl, 0.8f);

                foreach (HexAddress ring in this.tiles.RingNeighbors(address, 3))
                {
                    MapGenTile ringTile = this.tiles[ring];
                    ringCliff |= ringTile.region.cliff;

                    if (ringTile.river || ringTile.region.biome == MapRegionBiome.Water)
                        return this.AddPerlin(address, (beachLvl + grassLvl + grassLvl) / 3f, 0.3f);
                }

                return this.AddPerlin(address, grassLvl, 0.8f);
            }

            private float MountainHeight(HexAddress address) => this.AddPerlin(address, grassLvl + this.MountainGaussian(address) * MountainLvl(address) * mountainRangeLvl, 0.8f);

            private float MountainGaussian(HexAddress address)
            {
                float initial = mountainHeightWeight[0];
                float r1 = mountainHeightWeight[1];
                float r2 = mountainHeightWeight[2];
                float r3 = mountainHeightWeight[3];
                float max = initial + r1 * 6 + r2 * 12 + r3 * 18;

                float mountainCount = initial;
                foreach (HexAddress a in this.tiles.RingNeighbors(address, 1))
                    if (this.tiles[a].mountain)
                        mountainCount += r1;
                foreach (HexAddress a in this.tiles.RingNeighbors(address, 2))
                    if (this.tiles[a].mountain)
                        mountainCount += r2;
                foreach (HexAddress a in this.tiles.RingNeighbors(address, 3))
                    if (this.tiles[a].mountain)
                        mountainCount += r3;

                mountainCount /= max;
                return mountainCount;
            }

            public float MountainLvl(HexAddress adress)
            {
                bool allMountain = true;

                int mountainCount = 0;
                foreach (MapGenTile tile in this.tiles.RingNeighbors(adress, 1).Select(i => this.tiles[i]))
                {
                    if (!tile.mountain)
                        allMountain = false;
                    else
                        mountainCount++;
                }

                if (allMountain)
                {
                    foreach (MapGenTile tile in this.tiles.RingNeighbors(adress, 2).Select(i => this.tiles[i]))
                    {
                        if (!tile.mountain)
                            allMountain = false;
                    }

                    if (!allMountain)
                        return 0.5f;
                    else
                    {
                        foreach (MapGenTile tile in this.tiles.RingNeighbors(adress, 3).Select(i => this.tiles[i]))
                        {
                            if (!tile.mountain)
                                allMountain = false;
                        }

                        if (!allMountain)
                            return 0.75f;
                        else
                            return 1;
                    }
                }
                else
                    return 0.25f;
            }

            private float AddPerlin(HexAddress address, float maxValue, float perlinAMM = 0.6f)
            {
                float div = 1 + perlinAMM;
                float baseValue = maxValue / div;

                return baseValue * (1 + perlinAMM * this.perlin.Value(this.hexScreenHelper.CenterPosition(address)));
            }
        }

        private void GenerateChunks()
        {
            IHexPerlin2D perlin = new Perlin2DTileHeight(this.tiles, this.hexScreen, this.finalSize, this.parameter.heightScale, this.ElevationPerlin(), this.parameter.mountainHeightWeight);
            foreach (Transform child in this.map.tileChunkParent)
                GameObject.Destroy(child.gameObject);
            this.map.chunks = new MapTileRendererChunkBehaviour[this.finalSize.x / this.parameter.chunkSize.x, this.finalSize.y / this.parameter.chunkSize.y];
            for (int chunkX = 0; chunkX < this.finalSize.x / this.parameter.chunkSize.x; chunkX++)
                for (int chunkY = 0; chunkY < this.finalSize.y / this.parameter.chunkSize.y; chunkY++)
                {
                    MapTileRendererChunkBehaviour chunk = GameObject.Instantiate<MapTileRendererChunkBehaviour>(this.map.tileChunkPrefab);
                    chunk.transform.SetParent(this.map.tileChunkParent, false);
                    chunk.map = this.map;
                    chunk.chunkIndex = new Vector2Int(chunkX, chunkY);
                    this.GenerateChunk(chunk, perlin);
                    this.map.chunks[chunkX, chunkY] = chunk;
                }
        }

        private void GenerateChunk(MapTileRendererChunkBehaviour chunk, IHexPerlin2D perlin)
        {
            HexAddress startLoc = new HexAddress(
                chunk.chunkIndex.x * this.parameter.chunkSize.x,
                chunk.chunkIndex.y * this.parameter.chunkSize.y);
            HexAddress endLoc = new HexAddress(
                Mathf.Min(this.finalSize.x - 1, (chunk.chunkIndex.x + 1) * this.parameter.chunkSize.x - 1),
                Mathf.Min(this.finalSize.y - 1, (chunk.chunkIndex.y + 1) * this.parameter.chunkSize.y - 1));

            GenerateChunkHolder reloarOperation = new GenerateChunkHolder(this.hexScreen, perlin, this.parameter.standableLerpLocations, this.parameter.standableLerpHeight, this.map.renderingMode, this.map.parameterGameplay.heightScale);
            reloarOperation.Reload(startLoc, endLoc);

            Rect uvBounds = this.hexScreen.ScreenRect(startLoc, endLoc);
            Vector2[] uvs = new Vector2[reloarOperation.vertices.Count];
            Vector3[] normals = new Vector3[reloarOperation.vertices.Count];
            for (int i = reloarOperation.vertices.Count - 1; i >= 0; i--)
            {
                uvs[i] = MapGenerator.ToUV(reloarOperation.vertices[i], uvBounds);
                normals[i] = Vector3.up;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = reloarOperation.vertices.ToArray();
            mesh.triangles = reloarOperation.triangles.ToArray();
            mesh.normals = normals.ToArray();
            mesh.uv = uvs.ToArray();

            chunk.meshFilter.sharedMesh = mesh;
            chunk.meshCollider.sharedMesh = mesh;

            MapTileRegionCameraBehaviour renderParent = MapTileRegionCameraBehaviour.Instantiate(this.map.cameraForTileMap);
            renderParent.transform.SetParent(this.map.tileChunkParent);
            renderParent.finalSize = this.finalSize;
            renderParent.chunkIndex = chunk.chunkIndex;
            renderParent.chunkSize = this.parameter.chunkSize;
            renderParent.hexPixelSize = new Vector2Int(24, 24);
            renderParent.pixelPerUnit = 100;

            renderParent.Initialize();
            renderParent.StartReload();

            //this.map.StartCoroutine(this.DisableCamera(renderParent));
            //Hex2DTextureWriter moistureTexture = new Hex2DTextureWriter(startLoc, endLoc, new Vector2Int(24, 28), 6);
            //for (int x = startLoc.offsetX; x <= endLoc.offsetX; x++)
            //    for (int y = startLoc.offsetY; y <= endLoc.offsetY; y++)
            //    {
            //        HexAddress hex = new HexAddress(x, y);
            //        moistureTexture.DrawSprite(hex, this.map.whiteHexagonSprite, Color.red * this.tiles[hex].region.moisture);
            //    }

            chunk.meshRenderer.sharedMaterial = Material.Instantiate(this.map.landMaterialPrefab);
            chunk.meshRenderer.sharedMaterial.SetTexture("_BiomeMetaTexture", renderParent.texture);
        }

        class GenerateChunkHolder
        {
            public HexScreenHelper hexScreenHelper;
            public IHexPerlin2D perlin;
            public Vector2 standableLerpLocations;
            public Vector2 standableLerpHeight;
            public float heighScale;
            public Dictionary<Vector3, int> vertexIndexCache = new Dictionary<Vector3, int>();
            public List<Vector3> vertices = new List<Vector3>();
            public List<int> triangles = new List<int>();
            public TileRenderingMode renderingMode;

            public GenerateChunkHolder(HexScreenHelper hexScreenHelper, IHexPerlin2D perlin, Vector2 standableLerpLocations, Vector2 standableLerpHeight, TileRenderingMode renderingMode, float heighScale)
            {
                this.hexScreenHelper = hexScreenHelper;
                this.perlin = perlin;
                this.standableLerpLocations = standableLerpLocations;
                this.standableLerpHeight = standableLerpHeight;
                this.heighScale = heighScale;
                this.renderingMode = renderingMode;
            }

            public void Reload(HexAddress startLoc, HexAddress endLoc)
            {
                for (int x = startLoc.offsetX; x <= endLoc.offsetX; x++)
                    for (int y = startLoc.offsetY; y <= endLoc.offsetY; y++)
                    {
                        HexAddress hex = new HexAddress(x, y);

                        Vector2 topCenterPosition = this.hexScreenHelper.CenterPosition(hex);

                        float h = this.perlin.HexagonValue(hex);
                        if (h == 0)
                            continue;

                        Vector3 centerPosition = new Vector3(topCenterPosition.x, h * this.heighScale, topCenterPosition.y);

                        int centerIndex = this.AddVertexIndex(centerPosition);

                        Vector3[] cornerPositions = new Vector3[6];
                        int[] cornerVertexindexes = new int[6];

                        for (int i = 0; i < 6; i++)
                        {
                            HexCornerAddress corner = new HexCornerAddress(hex, (HexCorner)i);
                            Vector2 cornerPos = corner.Position(this.hexScreenHelper);

                            cornerPositions[i] = new Vector3(cornerPos.x, this.perlin.CornerValue(corner) * this.heighScale, cornerPos.y);
                            cornerVertexindexes[i] = this.FindVertexIndex(cornerPositions[i]);
                        }

                        for (int i = 0; i < 6; i++)
                            this.FillSide(centerPosition, centerIndex, cornerPositions[i], cornerVertexindexes[i], cornerPositions[(i + 1) % 6], cornerVertexindexes[(i + 1) % 6], this.renderingMode);
                    }
            }

            public int AddVertexIndex(Vector3 position)
            {
                int centerIndex = this.vertices.Count;
                this.vertexIndexCache.Add(position, centerIndex);
                this.vertices.Add(position);
                return centerIndex;
            }

            public int FindVertexIndex(Vector3 position)
            {
                if (!this.vertexIndexCache.TryGetValue(position, out int ret))
                {
                    ret = this.vertices.Count;
                    this.vertexIndexCache.Add(position, ret);
                    this.vertices.Add(position);
                }

                return ret;
            }

            const float div13 = 1f / 3f;
            const float div23 = 2f / 3f;

            private void FillSide(Vector3 centerPosition, int centerVertexIndex, Vector3 lowerPosition, int lowerVertexIndex, Vector3 upperPosition, int upperVertexIndex, TileRenderingMode renderingMode)
            {
                if (renderingMode == TileRenderingMode.Simple)
                {
                    this.triangles.Add(centerVertexIndex);
                    this.triangles.Add(lowerVertexIndex);
                    this.triangles.Add(upperVertexIndex);
                    return;
                }

                Vector2 lerpPositions = renderingMode == TileRenderingMode.Platform ? this.standableLerpLocations : new Vector2(div13, div23);
                int[,] lineIndexes = new int[4, 3];

                Vector3 upper1 = Vector3.Lerp(upperPosition, lowerPosition, div13);
                // upper1 = new Vector3(upper1.x, this.perlin.Value(new Vector2(upper1.x, upper1.z)) * this.heighScale, upper1.z);

                Vector3 lower1 = Vector3.Lerp(upperPosition, lowerPosition, div23);
                // lower1 = new Vector3(lower1.x, this.perlin.Value(new Vector2(lower1.x, lower1.z)) * this.heighScale, lower1.z);

                this.FillLineIndexes(lineIndexes, 0, centerPosition, upperPosition, upperVertexIndex, lerpPositions, renderingMode);
                this.FillLineIndexes(lineIndexes, 1, centerPosition, upper1, this.FindVertexIndex(upper1), lerpPositions, renderingMode);
                this.FillLineIndexes(lineIndexes, 2, centerPosition, lower1, this.FindVertexIndex(lower1), lerpPositions, renderingMode);
                this.FillLineIndexes(lineIndexes, 3, centerPosition, lowerPosition, lowerVertexIndex, lerpPositions, renderingMode);

                this.DrawLineA(centerVertexIndex, lineIndexes, 0);
                this.DrawLineA(centerVertexIndex, lineIndexes, 1);
                this.DrawLineB(centerVertexIndex, lineIndexes, 2);
            }

            private void FillLineIndexes(int[,] lineIndexes, int index, Vector3 centerPosition, Vector3 cornerPosition, int cornerIndex, Vector2 lerpPositions, TileRenderingMode renderingMode)
            {
                Vector3 intInner;
                Vector3 intOutter;

                switch (renderingMode)
                {
                    case TileRenderingMode.Platform:
                        intInner = new Vector3(
                            Mathf.Lerp(centerPosition.x, cornerPosition.x, lerpPositions.x),
                            Mathf.Lerp(centerPosition.y, cornerPosition.y, this.standableLerpHeight.x),
                            Mathf.Lerp(centerPosition.z, cornerPosition.z, lerpPositions.x));
                        intOutter = new Vector3(
                            Mathf.Lerp(centerPosition.x, cornerPosition.x, lerpPositions.y),
                            Mathf.Lerp(centerPosition.y, cornerPosition.y, this.standableLerpHeight.y),
                            Mathf.Lerp(centerPosition.z, cornerPosition.z, lerpPositions.y));
                        break;
                    case TileRenderingMode.Impassable:
                        {
                            intInner = new Vector3(
                                Mathf.Lerp(centerPosition.x, cornerPosition.x, lerpPositions.x),
                                Mathf.Lerp(centerPosition.y, cornerPosition.y, HexScreenHelper.div13),
                                Mathf.Lerp(centerPosition.z, cornerPosition.z, lerpPositions.x));
                            intOutter = new Vector3(
                                Mathf.Lerp(centerPosition.x, cornerPosition.x, lerpPositions.y),
                                Mathf.Lerp(centerPosition.y, cornerPosition.y, HexScreenHelper.div23),
                                Mathf.Lerp(centerPosition.z, cornerPosition.z, lerpPositions.y));
                        }
                        break;
                    default: throw new System.Exception();
                }



                if (index == 0 || index == 3)
                {
                    lineIndexes[index, 0] = this.FindVertexIndex(intInner);
                    lineIndexes[index, 1] = this.FindVertexIndex(intOutter);
                    lineIndexes[index, 2] = cornerIndex;
                }
                else
                {
                    lineIndexes[index, 0] = this.AddVertexIndex(intInner);
                    lineIndexes[index, 1] = this.AddVertexIndex(intOutter);
                    lineIndexes[index, 2] = cornerIndex;
                }
            }

            private void DrawLineA(int centerIndex, int[,] lineIndexes, int lowRow)
            {
                int higRow = lowRow + 1;

                this.triangles.Add(centerIndex);
                this.triangles.Add(lineIndexes[higRow, 0]);
                this.triangles.Add(lineIndexes[lowRow, 0]);

                this.triangles.Add(lineIndexes[lowRow, 0]);
                this.triangles.Add(lineIndexes[higRow, 0]);
                this.triangles.Add(lineIndexes[higRow, 1]);

                this.triangles.Add(lineIndexes[lowRow, 0]);
                this.triangles.Add(lineIndexes[higRow, 1]);
                this.triangles.Add(lineIndexes[lowRow, 1]);

                this.triangles.Add(lineIndexes[lowRow, 1]);
                this.triangles.Add(lineIndexes[higRow, 1]);
                this.triangles.Add(lineIndexes[higRow, 2]);

                this.triangles.Add(lineIndexes[lowRow, 1]);
                this.triangles.Add(lineIndexes[higRow, 2]);
                this.triangles.Add(lineIndexes[lowRow, 2]);
            }

            private void DrawLineB(int centerIndex, int[,] lineIndexes, int lowRow)
            {
                int higRow = lowRow + 1;

                this.triangles.Add(centerIndex);
                this.triangles.Add(lineIndexes[higRow, 0]);
                this.triangles.Add(lineIndexes[lowRow, 0]);

                this.triangles.Add(lineIndexes[lowRow, 0]);
                this.triangles.Add(lineIndexes[higRow, 0]);
                this.triangles.Add(lineIndexes[lowRow, 1]);

                this.triangles.Add(lineIndexes[higRow, 0]);
                this.triangles.Add(lineIndexes[higRow, 1]);
                this.triangles.Add(lineIndexes[lowRow, 1]);

                this.triangles.Add(lineIndexes[lowRow, 1]);
                this.triangles.Add(lineIndexes[higRow, 1]);
                this.triangles.Add(lineIndexes[lowRow, 2]);

                this.triangles.Add(lineIndexes[higRow, 1]);
                this.triangles.Add(lineIndexes[higRow, 2]);
                this.triangles.Add(lineIndexes[lowRow, 2]);
            }
        }

        private static Vector2 ToUV(Vector3 screenLocation, Rect bounds) => new Vector2((screenLocation.x - bounds.x) / bounds.width, (screenLocation.z - bounds.y) / bounds.height);

        private void GenerateMoistureTiles()
        {
            IPerlin2D moisturePerlin = this.MoisurePerlin();
            Dictionary<int, Tile> tileCache = new Dictionary<int, Tile>();
            this.map.moistureTilemap.ClearAllTiles();
            for (int x = 0; x < this.finalSize.x; x++)
                for (int y = 0; y < this.finalSize.y; y++)
                {
                    HexAddress hex = new HexAddress(x, y);
                    float moisture = this.tiles[hex].region.moisture + (moisturePerlin.Value(this.hexScreen.CenterPosition(hex)) - 0.5f) * 0.4f;
                    int moistureIndex = Mathf.Clamp(Mathf.RoundToInt(moisture * 20f), 0, 20);

                    if (!tileCache.TryGetValue(moistureIndex, out Tile tile))
                    {
                        tile = GameObject.Instantiate(this.map.whiteHexagonTile);
                        tile.color = new Color((float)moistureIndex / 20f, 0, 0, 1);
                        tileCache.Add(moistureIndex, tile);
                    }

                    this.map.moistureTilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
        }
    }
}
