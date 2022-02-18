using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Hexs
{
    public class HexAddressRegionArray<T> : HexAddressRegion, IEnumerable<KeyValuePair<HexAddress, T>>
    {
        private readonly T[,] data;

        public HexAddressRegionArray(Vector2Int size)
            : base(size)
        {
            this.data = new T[size.x, size.y];
        }

        public T this[HexAddress hexAddress]
        {
            get
            {
                if (!hexAddress.Contained(this.size))
                    throw new System.IndexOutOfRangeException($"The {hexAddress} is not in the range ({data.GetLength(0)}, {data.GetLength(1)})");
                return this.data[hexAddress.offsetX, hexAddress.offsetY];
            }
            set
            {
                if (!hexAddress.Contained(this.size))
                    throw new System.IndexOutOfRangeException($"The {hexAddress} is not in the range ({data.GetLength(0)}, {data.GetLength(1)})");
                this.data[hexAddress.offsetX, hexAddress.offsetY] = value;
            }
        }

        IEnumerator<KeyValuePair<HexAddress, T>> IEnumerable<KeyValuePair<HexAddress, T>>.GetEnumerator()
        {
            for (int x = 0; x < this.size.x; x++)
                for (int y = 0; y < this.size.y; y++)
                    yield return new KeyValuePair<HexAddress, T>(new HexAddress(x, y), this.data[x, y]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int x = 0; x < this.size.x; x++)
                for (int y = 0; y < this.size.y; y++)
                    yield return new KeyValuePair<HexAddress, T>(new HexAddress(x, y), this.data[x, y]);
        }

        public void Clear(T value)
        {
            for (int x = 0; x < this.size.x; x++)
                for (int y = 0; y < this.size.y; y++)
                    this.data[x, y] = value;
        }
    }
}
