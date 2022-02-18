using Assets.Hashs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Tests
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    internal class Hexagon3DRendererBehaviour : MonoBehaviour
    {
        public float h1;
        public float h2;
        public float h3;
        public float h4;
        public float h5;
        public float h6;
        public float radius;
        public Vector2Int uvHexAmmounts;
        public Vector2 uvMargins;
        public int indexX = 0;
        public int indexZ = 0;

        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        public Material material;

        private void Start()
        {
            this.meshRenderer = gameObject.GetComponent<MeshRenderer>();
            this.meshFilter = gameObject.GetComponent<MeshFilter>();

            int[] arr = new int[10];
            for (int i = 0; i < 10000; i++)
                arr[(int)(HashGenerator.Hash01(i, 10)* 10)]++;
            foreach (int i in arr)
                Debug.Log(i);
        }

        private void Update()
        {
            float width = Mathf.Sqrt(3f) * this.radius;
            float height = 2f * this.radius;

            float baseX = indexX * width + (indexZ % 2 == 0 ? 0 : width / 2);
            float baseZ = indexZ * height * 3f / 4f;

            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[]
            {
                new Vector3(baseX + width / 2, (this.h1 + this.h2 + this.h3 + this.h4 + this.h5 + this.h6) /6f, baseZ + height / 2f),
                new Vector3(baseX + width / 2, this.h1, baseZ),
                new Vector3(baseX + width, this.h2, baseZ + height / 4f),
                new Vector3(baseX + width, this.h3, baseZ + height * 3f / 4f),
                new Vector3(baseX + width / 2, this.h4, baseZ + height),
                new Vector3(baseX, this.h5, baseZ + height * 3f / 4f),
                new Vector3(baseX, this.h6, baseZ + height / 4f)
            };
            mesh.vertices = vertices;

            int[] tris = new int[]
            {
                0, 2, 1,
                0, 3, 2,
                0, 4, 3,
                0, 5, 4,
                0, 6, 5,
                0, 1, 6
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[]
            {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up
            };
            mesh.normals = normals;

            float uvWidth = 1f - this.uvMargins.x * 2;
            float uvHeight = 1f - this.uvMargins.y * 2;
            float uvItemWidth;
            float uvItemHeight;

            if (this.uvHexAmmounts.y == 1)
            {
                uvItemHeight = uvHeight;
                uvItemWidth = uvWidth / this.uvHexAmmounts.x;
            }
            else
            {
                uvItemHeight = uvHeight / (this.uvHexAmmounts.y * 3f / 4f + 1f / 4f);
                uvItemWidth = uvWidth / (this.uvHexAmmounts.x + 1f / 2f);
            }

            float uvBaseX = this.uvMargins.x + (indexX % this.uvHexAmmounts.x) * uvItemWidth + (indexZ % 2 == 0 ? 0 : uvItemWidth / 2);
            float uvBaseZ = this.uvMargins.y + indexZ * uvItemHeight * 3f / 4f;


            Vector2[] uv = new Vector2[]
            {
                new Vector2(uvBaseX + uvItemWidth / 2, uvBaseZ + uvItemHeight / 2f),
                new Vector2(uvBaseX + uvItemWidth / 2, uvBaseZ),
                new Vector2(uvBaseX + uvItemWidth, uvBaseZ + uvItemHeight / 4f),
                new Vector2(uvBaseX + uvItemWidth, uvBaseZ + uvItemHeight * 3f / 4f),
                new Vector2(uvBaseX + uvItemWidth / 2, uvBaseZ + uvItemHeight),
                new Vector2(uvBaseX, uvBaseZ + uvItemHeight * 3f / 4f),
                new Vector2(uvBaseX, uvBaseZ + uvItemHeight / 4f)
            };
            mesh.uv = uv;

            this.meshFilter.mesh = mesh;
            this.meshRenderer.material = this.material;
        }
    }
}
