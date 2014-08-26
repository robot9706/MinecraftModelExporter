using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor
{
    class CustomBlockData
    {
        public bool TriFlip = false;

        public Vector3 Vertex1;
        public Vector3 Vertex2;
        public Vector3 Vertex3;
        public Vector3 Vertex4;

        public Vector2 UV1;
        public Vector2 UV2;
        public Vector2 UV3;
        public Vector2 UV4;

        public Vector3 Normal;

        public string Texture;
        public BlockData Source;

        public CustomBlockData CreateUVs()
        {
            UV1 = new Vector2(0, 0);
            UV2 = new Vector2(1, 0);
            UV3 = new Vector2(1, 1);
            UV4 = new Vector2(0, 1);

            return this;
        }

        public CustomBlockData CreateUVs(float xMin, float yMin, float xMax, float yMax)
        {
            UV1 = new Vector2(xMin, yMin);
            UV2 = new Vector2(xMax, yMin);
            UV3 = new Vector2(xMax, yMax);
            UV4 = new Vector2(xMin, yMax);

            return this;
        }

        public CustomBlockData CreateUVsCrossFlip()
        {
            UV1 = new Vector2(1, 0);
            UV2 = new Vector2(0, 0);
            UV3 = new Vector2(0, 1);
            UV4 = new Vector2(1, 1);

            return this;
        }

        public CustomBlockData CreateUVsYFliped()
        {
            UV3 = new Vector2(0, 0);
            UV4 = new Vector2(1, 0);
            UV1 = new Vector2(1, 1);
            UV2 = new Vector2(0, 1);

            return this;
        }

        public CustomBlockData CreateUVsXYFliped()
        {
            UV3 = new Vector2(1, 0);
            UV2 = new Vector2(0, 0);
            UV1 = new Vector2(0, 1);
            UV4 = new Vector2(1, 1);

            return this;
        }

        public CustomBlockData CreateUVsRotated90()
        {
            UV3 = new Vector2(0, 0);
            UV2 = new Vector2(1, 0);
            UV1 = new Vector2(1, 1);
            UV4 = new Vector2(0, 1);

            return this;
        }

        public CustomBlockData CreateUVsRotated90(float xMin, float yMin, float xMax, float yMax)
        {
            UV3 = new Vector2(xMin, yMin);
            UV2 = new Vector2(xMax, yMin);
            UV1 = new Vector2(xMax, yMax);
            UV4 = new Vector2(xMin, yMax);

            return this;
        }
    }
}
