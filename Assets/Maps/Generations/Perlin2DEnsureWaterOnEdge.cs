using Assets.Hashs;
using UnityEngine;

namespace Assets.Maps.Generations
{
    public class Perlin2DEnsureWaterOnEdge : Perlin2DFunc
    {
        public readonly Vector2 finalSize;
        public readonly float borderOfTheWorldMinWater;
        public readonly float borderOfTheWorldMaxWater;
        public readonly float cornerOfTheWorldMinWater;
        public readonly float cornerOfTheWorldMaxWater;

        public Perlin2DEnsureWaterOnEdge(IPerlin2D source, MapBehaviour map)
            : base(source)
        {
            this.finalSize = map.hexScreenHelper.ScreenSize();
            this.borderOfTheWorldMinWater = Mathf.Min(map.parameterGameplay.borderOfTheWorldMinWater, map.parameterGameplay.borderOfTheWorldMaxWater) * map.parameterGameplay.radius * 2;
            this.borderOfTheWorldMaxWater = Mathf.Max(map.parameterGameplay.borderOfTheWorldMinWater, map.parameterGameplay.borderOfTheWorldMaxWater) * map.parameterGameplay.radius * 2;
            this.cornerOfTheWorldMinWater = Mathf.Min(map.parameterGameplay.cornerOfTheWorldMinWater, map.parameterGameplay.cornerOfTheWorldMaxWater) * map.parameterGameplay.radius * 2;
            this.cornerOfTheWorldMaxWater = Mathf.Max(map.parameterGameplay.cornerOfTheWorldMinWater, map.parameterGameplay.cornerOfTheWorldMaxWater) * map.parameterGameplay.radius * 2;
        }

        public override float Selector(Vector2 position, float value)
        {
            return value * Mathf.Min(this.Process(position.x, this.borderOfTheWorldMinWater, this.borderOfTheWorldMaxWater),
                this.Process(position.y, this.borderOfTheWorldMinWater, this.borderOfTheWorldMaxWater),
                this.Process(this.finalSize.x - position.x, this.borderOfTheWorldMinWater, this.borderOfTheWorldMaxWater),
                this.Process(this.finalSize.y - position.y, this.borderOfTheWorldMinWater, this.borderOfTheWorldMaxWater),
                this.Process(Vector2.Distance(Vector2.zero, position), this.cornerOfTheWorldMinWater, this.cornerOfTheWorldMaxWater),
                this.Process(Vector2.Distance(new Vector2(0, this.finalSize.y), position), this.cornerOfTheWorldMinWater, this.cornerOfTheWorldMaxWater),
                this.Process(Vector2.Distance(new Vector2(this.finalSize.x, 0), position), this.cornerOfTheWorldMinWater, this.cornerOfTheWorldMaxWater),
                this.Process(Vector2.Distance(new Vector2(this.finalSize.x, this.finalSize.y), position), this.cornerOfTheWorldMinWater, this.cornerOfTheWorldMaxWater));
        }

        private float Process(float actualDistance, float min, float max)
        {
            if (actualDistance >= max)
                return 1;

            if (actualDistance <= min)
                return 0;

            return (actualDistance - min) / (max - min);
        }
    }
}
