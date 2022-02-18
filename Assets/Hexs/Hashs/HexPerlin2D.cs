namespace Assets.Hexs.Hashs
{
    public interface IHexPerlin2D
    {
        float HexagonValue(HexAddress address);
        float CornerValue(HexCornerAddress corner);
    }    
}
