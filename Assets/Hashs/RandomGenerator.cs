namespace Assets.Hashs
{
    public class RandomGenerator
    {
        public int seed;
        public int position;

        public RandomGenerator(int seed) { this.seed = seed; }

        public float Random01() => HashGenerator.Hash01(this.position++, this.seed);
        public float RandomMinMax(float min, float max) => min + HashGenerator.Hash01(this.position++, this.seed) * (max - min);
        public int RandomMinMax(int minInclusive, int maxExclusive)
        {
            int ret = minInclusive + (int)(HashGenerator.Hash01(this.position++, this.seed) * (maxExclusive - minInclusive));
            while (ret >= maxExclusive)
                ret = minInclusive + (int)(HashGenerator.Hash01(this.position++, this.seed) * (maxExclusive - minInclusive));
            return ret;
        }
        public int Random0Max(int maxExclusive)
        {
            int ret = (int)(HashGenerator.Hash01(this.position++, this.seed) * maxExclusive);
            while (ret >= maxExclusive)
                ret = (int)(HashGenerator.Hash01(this.position++, this.seed) * maxExclusive);
            return ret;
        }

        public T PopRandom<T>(System.Collections.Generic.IList<T> items)
        {
            int index = this.Random0Max(items.Count);
            T ret = items[index];
            items.RemoveAt(index);
            return ret;
        }

        public T GetRandom<T>(System.Collections.Generic.IList<T> items) => items[this.Random0Max(items.Count)];
    }
}
