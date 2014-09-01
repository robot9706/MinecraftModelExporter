using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor
{
    enum Rotate
    { 
        None = 0,
        Deg90 = 1,
        Deg180 = 2,
        Deg270 = 3
    }

    class CustomBlockData
    {
        public bool TriFlip = false;
        public bool KeepNormal = false;

        public bool DoubleSided = false;

        public Vector3 Vertex1;
        public Vector3 Vertex2;
        public Vector3 Vertex3;
        public Vector3 Vertex4;

        public Vector2 UV1;
        public Vector2 UV2;
        public Vector2 UV3;
        public Vector2 UV4;

        public Vector3 Normal;

        public bool IsOneTriangle = false;

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

        public CustomBlockData CreateUVsRotated(Rotate rotate)
        {
            switch (rotate)
            { 
                case Rotate.None:
                    return CreateUVs();
                case Rotate.Deg90:
                    return CreateUVsRotated(90);
                case Rotate.Deg180:
                    return CreateUVsRotated(180);
                case Rotate.Deg270:
                    return CreateUVsRotated(270);
            }

            return this.CreateUVs();
        }

        public CustomBlockData CreateUVsRotated(float angles)
        {
            CreateUVs();

            Vector2 center = new Vector2(0.5f, 0.5f);

            UV1 = Vector2.RotateAround(UV1, center, angles);
            UV2 = Vector2.RotateAround(UV2, center, angles);
            UV3 = Vector2.RotateAround(UV3, center, angles);
            UV4 = Vector2.RotateAround(UV4, center, angles);

            return this;
        }

        public CustomBlockData RotateUVs(float angles)
        {
            Vector2 center = new Vector2(0.5f, 0.5f);

            UV1 = Vector2.RotateAround(UV1, center, angles);
            UV2 = Vector2.RotateAround(UV2, center, angles);
            UV3 = Vector2.RotateAround(UV3, center, angles);
            UV4 = Vector2.RotateAround(UV4, center, angles);

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

        public CustomBlockData CreateUVsXFlipped()
        {
            UV2 = new Vector2(0, 0);
            UV1 = new Vector2(1, 0);
            UV4 = new Vector2(1, 1);
            UV3 = new Vector2(0, 1);

            return this;
        }

        public CustomBlockData CreateUVsYFlipped()
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

        public CustomBlockData CalculateNormal()
        {
            if (TriFlip && !KeepNormal)
            {
                Vector3 normalA = Vector3.Cross(Vertex2 - Vertex3, Vertex1 - Vertex3);
                Vector3 normalB = Vector3.Cross(Vertex4 - Vertex3, Vertex1 - Vertex3);

                Normal = (normalA + normalB) / 2;
            }
            else
            {
                Vector3 normalA = Vector3.Cross(Vertex2 - Vertex1, Vertex3 - Vertex1);
                Vector3 normalB = Vector3.Cross(Vertex4 - Vertex3, Vertex1 - Vertex3);

                Normal = (normalA + normalB) / 2;
            }

            return this;
        }

        public CustomBlockData FlipUVsX()
        {
            Vector2[] uvs = new Vector2[] { UV1, UV2, UV3, UV4 };

            float yMin = float.MaxValue;
            float yMax = 0;

            for (int x = 0; x < uvs.Length; x++)
            {
                if (uvs[x].Y > yMax)
                    yMax = uvs[x].Y;
                else if (uvs[x].Y < yMin)
                    yMin = uvs[x].Y;
            }

            float xMinA = float.MaxValue;
            float xMaxA = 0;
            float xMinB = float.MaxValue;
            float xMaxB = 0;
            for (int x = 0; x < uvs.Length; x++)
            {
                if (uvs[x].Y == yMin)
                {
                    if (uvs[x].X > xMaxA)
                        xMaxA = uvs[x].X;
                    if (uvs[x].X < xMinA)
                        xMinA = uvs[x].X;
                }
                else if (uvs[x].Y == yMax)
                {
                    if (uvs[x].X > xMaxB)
                        xMaxB = uvs[x].X;
                    if (uvs[x].X < xMinB)
                        xMinB = uvs[x].X;
                }
            }

            for (int x = 0; x < uvs.Length; x++)
            {
                if (uvs[x].Y == yMin)
                {
                    if (uvs[x].X == xMinA)
                        uvs[x].X = xMaxA;
                    else if (uvs[x].X == xMaxA)
                        uvs[x].X = xMinA;
                }
                else if (uvs[x].Y == yMax)
                {
                    if (uvs[x].X == xMinB)
                        uvs[x].X = xMaxB;
                    else if (uvs[x].X == xMaxB)
                        uvs[x].X = xMinB;
                }
            }

            return this;
        }

        public CustomBlockData FlipUVsY()
        {
            Vector2[] uvs = new Vector2[] { UV1, UV2, UV3, UV4 };

            float xMin = float.MaxValue;
            float xMax = 0;

            for (int x = 0; x < uvs.Length; x++)
            {
                if (uvs[x].X > xMax)
                    xMax = uvs[x].X;
                else if (uvs[x].X < xMin)
                    xMin = uvs[x].X;
            }

            float yMinA = float.MaxValue;
            float yMaxA = 0;
            float yMinB = float.MaxValue;
            float yMaxB = 0;
            for (int x = 0; x < uvs.Length; x++)
            {
                if (uvs[x].X == xMin)
                {
                    if (uvs[x].Y > yMaxA)
                        yMaxA = uvs[x].Y;
                    if (uvs[x].Y < yMinA)
                        yMinA = uvs[x].Y;
                }
                else if (uvs[x].X == xMax)
                {
                    if (uvs[x].Y > yMaxB)
                        yMaxB = uvs[x].Y;
                    if (uvs[x].Y < yMinB)
                        yMinB = uvs[x].Y;
                }
            }

            for (int x = 0; x < uvs.Length; x++)
            {
                if (uvs[x].X == xMin)
                {
                    if (uvs[x].Y == yMinA)
                        uvs[x].Y = yMaxA;
                    else if (uvs[x].Y == yMaxA)
                        uvs[x].Y = yMinA;
                }
                else if (uvs[x].X == xMax)
                {
                    if (uvs[x].Y == yMinB)
                        uvs[x].Y = yMaxB;
                    else if (uvs[x].Y == yMaxB)
                        uvs[x].Y = yMinB;
                }
            }

            return this;
        }
    }
}
