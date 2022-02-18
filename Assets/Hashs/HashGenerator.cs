using UnityEngine;

namespace Assets.Hashs
{
    public static class HashGenerator
    {
        const float UIntMaxValue = uint.MaxValue;
        const uint PRIME_NUMBER = 198491317;
        //const uint PRIME_NUMBER2 = 6542989;

        public static uint HashUnsignedInt(uint position, uint seed)
        {
            const uint BIT_NOISE1 = 0xB5297A4D;
            const uint BIT_NOISE2 = 0x68E31DA4;
            const uint BIT_NOISE3 = 0x1B56C4E9;

            uint mangled = position;
            mangled *= BIT_NOISE1;
            mangled += seed;
            mangled ^= (mangled >> 8);
            mangled += BIT_NOISE2;
            mangled ^= (mangled << 8);
            mangled *= BIT_NOISE3;
            mangled ^= (mangled >> 8);
            return mangled;
        }

        public static uint HashUnsignedInt(uint positionX, uint positionY, uint seed) => HashUnsignedInt(unchecked(positionX + PRIME_NUMBER * positionY), seed);

        public static float Hash01(int position, int seed) => (float)HashGenerator.HashUnsignedInt(unchecked((uint)position), unchecked((uint)seed)) / HashGenerator.UIntMaxValue;
        public static float Hash01(int positionX, int positionY, int seed) => (float)HashGenerator.HashUnsignedInt(unchecked((uint)positionX), unchecked((uint)positionY), unchecked((uint)seed)) / HashGenerator.UIntMaxValue;

        public static float Perlin01(float position, int seed)
        {
            int lower = Mathf.FloorToInt(position);
            float hashLower = HashGenerator.Hash01(lower, seed);
            float hashUpper = HashGenerator.Hash01(lower + 1, seed);
            return Mathf.Lerp(hashLower, hashUpper, position - lower);
        }

        public static float Perlin01(Vector2 position, int seed)
        {
            int minX = Mathf.FloorToInt(position.x);
            int minY = Mathf.FloorToInt(position.y);

            float hashMinMin = HashGenerator.Hash01(minX, minY, seed);
            float hashMaxMin = HashGenerator.Hash01(minX + 1, minY, seed);
            float hashMinMax = HashGenerator.Hash01(minX, minY + 1, seed);
            float hashMaxMax = HashGenerator.Hash01(minX + 1, minY + 1, seed);

            float decimals = position.x - (float)minX;
            float lerpMin = Mathf.Lerp(hashMinMin, hashMaxMin, decimals);
            float lerpMax = Mathf.Lerp(hashMinMax, hashMaxMax, decimals);
            decimals = position.y - (float)minY;
            return Mathf.Lerp(lerpMin, lerpMax, decimals);
        }
    }
}
