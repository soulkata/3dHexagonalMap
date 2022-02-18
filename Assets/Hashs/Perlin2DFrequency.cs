using UnityEngine;

namespace Assets.Hashs
{
    public class Perlin2DFrequency : IPerlin2D
    {
        public readonly IPerlin2D source;
        public readonly float waveLenght;

        public Perlin2DFrequency(IPerlin2D source, float waveLenght)
        {
            this.source = source;
            this.waveLenght = waveLenght;
        }

        public float Value(Vector2 location) => this.source.Value(location / this.waveLenght);
    }
}
