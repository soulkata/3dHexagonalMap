using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Hexs
{
    public class HexAddressRegionDictionary<T> : HexAddressRegion, IEnumerable<KeyValuePair<HexAddress, T>>
        where T : class
    {
        private readonly Dictionary<int, T> data;

        public HexAddressRegionDictionary(Vector2Int size)
            : base(size)
        {
            this.data = new Dictionary<int, T>();
        }

        IEnumerator<KeyValuePair<HexAddress, T>> IEnumerable<KeyValuePair<HexAddress, T>>.GetEnumerator() => this.data.Select(i => new KeyValuePair<HexAddress, T>(this.HexAddress(i.Key), i.Value)).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.data.Select(i => new KeyValuePair<HexAddress, T>(this.HexAddress(i.Key), i.Value)).GetEnumerator();

        public T this[HexAddress hexAddress]
        {
            get
            {
                if (this.data.TryGetValue(this.IntAddress(hexAddress), out T value))
                    return value;
                return null;
            }
            set
            {
                int intAddress = this.IntAddress(hexAddress);
                if (value == null)
                    this.data.Remove(this.IntAddress(hexAddress));
                else
                    this.data[this.IntAddress(hexAddress)] = value;
            }
        }

        public bool TryGetValue(HexAddress address, out T value) => this.data.TryGetValue(this.IntAddress(address), out value);

        public void Add(HexAddress address, T value) => this.data.Add(this.IntAddress(address), value);

        public bool Remove(HexAddress address) => this.data.Remove(this.IntAddress(address));

        public IEnumerable<T> Values => this.data.Values;

        public void Clear() => this.data.Clear();
    }
}
