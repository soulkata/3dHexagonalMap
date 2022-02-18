using Assets.Hashs;

namespace Assets.Hexs.Hashs
{
    public class HexPerlin2DFromPerlin2D : IHexPerlin2D
    {
        public readonly HexScreenHelper screenHelper;
        public readonly IPerlin2D perlin;

        public HexPerlin2DFromPerlin2D(HexScreenHelper screenHelper, IPerlin2D perlin)
        {
            this.screenHelper = screenHelper;
            this.perlin = perlin;
        }

        public float CornerValue(HexCornerAddress corner) => this.perlin.Value(corner.Position(this.screenHelper));
        public float HexagonValue(HexAddress address) => this.perlin.Value(this.screenHelper.CenterPosition(address));
    }
}
