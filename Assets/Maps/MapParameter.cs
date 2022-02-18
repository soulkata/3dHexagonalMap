using UnityEngine;

namespace Assets.Maps
{
    [CreateAssetMenu(fileName = "MapParameter", menuName = "ScriptableObjects/Map Parameters")]
    public class MapParameter : ScriptableObject
    {
        /// <summary>
        /// Number of region, in each axis
        /// </summary>
        public Vector2Int regionCount;

        /// <summary>
        /// The average size of the region, in each axis
        /// </summary>
        public Vector2Int regionSize;

        /// <summary>
        /// The average size of the region where the central point can be
        /// </summary>
        public Vector2Int regionCenterSize;

        /// <summary>
        /// To avoid bad randomization issues, offset to avoid that same value on each axig that looks like squares when done
        /// </summary>
        public Vector2Int offsetFromPreviousCoordinate;
        /// <summary>
        /// The chunk size, in each axis...
        /// </summary>
        public Vector2Int chunkSize;

        /// <summary>
        /// The tile size, greater this numer, wider the hex will be on x and z axis
        /// </summary>
        public float radius;

        /// <summary>
        /// The average height scale of the map, raising this value changes the y coordinate of the hexes
        /// </summary>
        public float heightScale;

        public float waterMoisture;

        public float percentualOfConnectionsAsNewRivers;
        public float percentualOfConnectionsAsRiversExtensions;
        public float percentualOfConnectionsAsMoutains;
        public int riverConnectionDistance;

        public int mountainConnectionDistance;
        public AnimationCurve mountainStraitness;
        public AnimationCurve mountainExtensionLenght;
        public AnimationCurve mountainConnectionHexagon;
        public float[] mountainHeightWeight;

        public Vector2 standableLerpLocations;
        public Vector2 standableLerpHeight;

        public float noiseBaseLenght;
        public float landDistributionWaterLevel;
        public float borderOfTheWorldMinWater;
        public float borderOfTheWorldMaxWater;
        public float cornerOfTheWorldMinWater;
        public float cornerOfTheWorldMaxWater;

        public AnimationCurve riverInitialVolume;
        public AnimationCurve riverVolumeGainPerTile;
        public AnimationCurve riverHexagonByVolume;
        public AnimationCurve riverExtensionLenght;
        public float riverMoisture;

        public float moistureDecay;
        public float moistureDecayMountain;
    }
}
