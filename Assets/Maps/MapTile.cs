namespace Assets.Maps
{
    //public class MapTile
    //{
    //    public MapRegion region;
    //    //public float height;
    //    public TileRenderingMode renderingMode;
    //    public bool water = false;
    //    public bool river = false;
    //    public bool mountain = false;

    //    public float Height
    //    {
    //        get
    //        {
    //            if (this.water) return 0.0f;
    //            if (this.river) return 0.1f;
    //            if (this.mountain) return 1.0f;
    //            return 0.3f;
    //        }
    //    }
    //}

    public enum TileRenderingMode
    {
        Simple,
        Platform,
        Impassable
    }
}
