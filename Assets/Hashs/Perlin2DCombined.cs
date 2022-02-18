using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Hashs
{
    public class Perlin2DCombined : IPerlin2D
    {
        public readonly KeyValuePair<IPerlin2D, float>[] items;
        public readonly float totalAmplitude;

        public Perlin2DCombined(IEnumerable<KeyValuePair<IPerlin2D, float>> baseItems)
        {
            this.items = baseItems.ToArray();
            this.totalAmplitude = this.items.Sum(i => i.Value);
        }

        public float Value(Vector2 location)
        {
            float ret = 0;
            foreach (KeyValuePair<IPerlin2D, float> item in this.items)
                ret += item.Key.Value(location) * item.Value;
            return ret / this.totalAmplitude;
        }
    }
}
