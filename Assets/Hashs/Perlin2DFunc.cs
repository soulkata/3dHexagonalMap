using UnityEngine;

namespace Assets.Hashs
{
    public abstract class Perlin2DFunc : IPerlin2D
    {
        public readonly IPerlin2D source;
        public abstract float Selector(Vector2 position, float value);

        public Perlin2DFunc(IPerlin2D source)
        {
            this.source = source;
        }

        public float Value(Vector2 location) => this.Selector(location, this.source.Value(location));
    }
}
