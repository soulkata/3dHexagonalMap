using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Hashs
{
    public interface IPerlin2D
    {
        float Value(Vector2 location);
    }

    public class Perlin2D : IPerlin2D
    {
        public readonly int seed;

        public Perlin2D(int seed)
        {
            this.seed = seed;
        }

        public float Value(Vector2 location) => HashGenerator.Perlin01(location, this.seed);
    }
}
