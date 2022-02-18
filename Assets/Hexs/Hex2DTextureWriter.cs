using UnityEngine;

namespace Assets.Hexs
{
    public class Hex2DTextureWriter : Hex2DMath
    {
        public readonly Texture2D texture;

        public Hex2DTextureWriter(HexAddress startLocation, HexAddress endLocation, Vector2Int hexagonSize)
            : base(startLocation, endLocation, hexagonSize)
        {
            this.texture = new Texture2D(this.totalSize.width, this.totalSize.height);
        }        

        protected int BaseX(HexAddress hexAddress) => hexAddress.offsetX * this.hexSizeX + (hexAddress.offsetY % 2 == 0 ? 0 : this.hexSizeX05);
        protected int BaseY(HexAddress hexAddress) => hexAddress.offsetY * this.hexSizeY075;
        public Vector2Int CenterPosition(HexAddress hexAddress) => new Vector2Int(this.BaseX(hexAddress) + this.hexSizeX05, this.BaseY(hexAddress) + this.hexSizeY05);

        public void DrawSprite(HexAddress hexAddress, Sprite sprite, Color color)
        {
            Vector2Int centerPosition = this.CenterPosition(hexAddress);

            Vector2Int pivotPixel = new Vector2Int((int)sprite.pivot.x, (int)sprite.pivot.y);
            for (int x = 0; x < sprite.rect.width; x++)
                for (int y = 0; y < sprite.rect.height; y++)
                {
                    Color sourceColor = sprite.texture.GetPixel(x + (int)sprite.rect.x, (int)sprite.rect.y + y);
                    if (sourceColor.a == 0)
                        continue;

                    sourceColor *= color;
                    Vector2Int targetPos = new Vector2Int(centerPosition.x - pivotPixel.x + x, centerPosition.y - pivotPixel.y + y);

                    if (!this.totalSize.Contains(targetPos))
                        continue;

                    this.texture.SetPixel(targetPos.x - this.totalSize.x, targetPos.y - this.totalSize.y, sourceColor);
                }
            this.texture.Apply();
        }
    }
}
