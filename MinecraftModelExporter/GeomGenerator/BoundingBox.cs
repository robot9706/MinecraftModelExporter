using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeomGenerator
{
    class BoundingBox
    {
        public Vector3 Min;
        public Vector3 Max;

        public string TextureTag;

        public BoundingBox(Vector3 min, Vector3 max)
        {
            Min = new Vector3(MinFloat(min.X, max.X), MinFloat(min.Y, max.Y), MinFloat(min.Z, max.Z));
            Max = new Vector3(MaxFloat(min.X, max.X), MaxFloat(min.Y, max.Y), MaxFloat(min.Z, max.Z));
        }

        public BoundingBox(Vector3 min, Vector3 max, string textureTag)
        {
            Min = new Vector3(MinFloat(min.X, max.X), MinFloat(min.Y, max.Y), MinFloat(min.Z, max.Z));
            Max = new Vector3(MaxFloat(min.X, max.X), MaxFloat(min.Y, max.Y), MaxFloat(min.Z, max.Z));
            TextureTag = textureTag;
        }

        private float MinFloat(float a, float b)
        {
            if (a < b)
                return a;

            return b;
        }

        private float MaxFloat(float a, float b)
        {
            if (a > b)
                return a;

            return b;
        }

        public Vector3[] GetCorners()
        {
            return new Vector3[] {
                new Vector3(this.Min.X, this.Max.Y, this.Max.Z), 
                new Vector3(this.Max.X, this.Max.Y, this.Max.Z),
                new Vector3(this.Max.X, this.Min.Y, this.Max.Z), 
                new Vector3(this.Min.X, this.Min.Y, this.Max.Z), 
                new Vector3(this.Min.X, this.Max.Y, this.Min.Z),
                new Vector3(this.Max.X, this.Max.Y, this.Min.Z),
                new Vector3(this.Max.X, this.Min.Y, this.Min.Z),
                new Vector3(this.Min.X, this.Min.Y, this.Min.Z)
            };
        }
    }
}
