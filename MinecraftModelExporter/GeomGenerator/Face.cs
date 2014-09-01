using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeomGenerator
{
    struct Face
    {
        public Vector3 A;
        public Vector3 B;

        public Vector3 Normal;

        public string TextureTag;

        public Face(Vector3 a, Vector3 b, Vector3 normal)
        {
            A = a;
            B = b;
            Normal = normal;
            TextureTag = "";
        }

        public float GetNormalValue()
        {
            if (Normal.Y == 0 && Normal.Z == 0)
                return A.X;
            if (Normal.X == 0 && Normal.Z == 0)
                return A.Y;
            if (Normal.X == 0 && Normal.Y == 0)
                return A.Z;

            return float.NaN;
        }

        public PointF[] ConvertToPointList()
        {
            PointF[] array = new PointF[4];

            PointF fa = GeometryGenerator.GetXYByNormal(Normal, A);
            PointF fb = GeometryGenerator.GetXYByNormal(Normal, B);

            array[0] = fa;
            array[1] = new PointF(fb.X, fa.Y);
            array[2] = fb;
            array[3] = new PointF(fa.X, fb.Y);

            return array;
        }
    }
}
