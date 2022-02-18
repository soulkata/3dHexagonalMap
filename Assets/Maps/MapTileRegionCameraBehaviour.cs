using Assets.Hexs;
using System.Collections;
using UnityEngine;

namespace Assets.Maps
{
    public class MapTileRegionCameraBehaviour : MonoBehaviour
    {
        public Camera randerCamera;

        internal RenderTexture texture;
        internal Vector2Int finalSize;
        internal Vector2Int chunkIndex;
        internal Vector2Int chunkSize;
        internal Vector2Int hexPixelSize;
        internal float pixelPerUnit = 100;

        internal void Initialize()
        {
            HexAddress startLoc = new HexAddress(this.chunkIndex.x * this.chunkSize.x,
                this.chunkIndex.y * this.chunkSize.y);
            HexAddress endLoc = new HexAddress(
                Mathf.Min(this.finalSize.x - 1, (this.chunkIndex.x + 1) * this.chunkSize.x - 1),
                Mathf.Min(this.finalSize.y - 1, (this.chunkIndex.y + 1) * this.chunkSize.y - 1));

            Hex2DMath hex2dMath = new Hex2DMath(startLoc, endLoc, hexPixelSize);
            this.texture = new RenderTexture(hex2dMath.totalSize.width, hex2dMath.totalSize.height, 0);
            this.randerCamera.targetTexture = this.texture;
            this.randerCamera.orthographicSize = ((float)hex2dMath.totalSize.height / 2f) / this.pixelPerUnit;
            this.transform.localPosition = new Vector3(hex2dMath.totalSize.center.x / this.pixelPerUnit, this.transform.localPosition.y, hex2dMath.totalSize.center.y / this.pixelPerUnit);
        }

        public void StartReload()
        {
            if (this.reloadVersion == byte.MaxValue)
                this.reloadVersion = byte.MinValue;
            else
                this.reloadVersion++;
            this.gameObject.SetActive(true);
            this.StartCoroutine(this.DoReload());
        }

        private byte reloadVersion;

        private IEnumerator DoReload()
        {
            byte version = this.reloadVersion;
            yield return null;
            if (this.reloadVersion != version)
                yield break;
            yield return null;
            if (this.reloadVersion != version)
                yield break;
            this.gameObject.SetActive(false);
        }
    }
}
