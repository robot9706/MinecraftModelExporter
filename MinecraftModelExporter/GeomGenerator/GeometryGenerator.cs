using MinecraftModelExporter.GeometryProcessor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeomGenerator
{
    class GeometryGenerator
    {
        /// <summary>
        /// Can build side: Input1: Block at side, Input2: Base block
        /// </summary>
        public List<CustomBlockData> GenerateModel(List<BoundingBox> boxes, BlockSource source, Point3 blockPos, string textureName, string textureNameY, Func<BlockData, BlockData, bool> CanBuildSideMethod)
        {
            BlockData currentBlock = source.GetData(blockPos);

            Vector3[] norms = new Vector3[] { new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, -1, 0), new Vector3(0, 0, 1), new Vector3(0, 0, -1) };

            Dictionary<Vector3, Dictionary<float, List<Face>>> faces = new Dictionary<Vector3, Dictionary<float, List<Face>>>();
            //Vector3 key: normal
            //float key: value of normal component

            //Foreach bounding box
            foreach (BoundingBox bb in boxes)
            {
                //Foreach normal (Minecraft normals)
                foreach (Vector3 normal in norms)
                {
                    //Get the face for the normal
                    Face face = GetFace(bb, normal);

                    //Store it using the important Normal component
                    float val = face.GetNormalValue();

                    if (float.IsNaN(val))
                        continue;

                    Dictionary<float, List<Face>> dict;
                    if (faces.ContainsKey(normal))
                    {
                        dict = faces[normal];
                    }
                    else
                    {
                        dict = new Dictionary<float, List<Face>>();
                        faces.Add(normal, dict);
                    }

                    if (dict.ContainsKey(val))
                        dict[val].Add(face);
                    else
                        dict.Add(val, new List<Face>() { face });
                }
            }

            List<CustomBlockData> datas = new List<CustomBlockData>();

            //Foreach normal
            foreach (KeyValuePair<Vector3, Dictionary<float, List<Face>>> pair in faces)
            { 
                //Foreach normal component
                foreach (KeyValuePair<float, List<Face>> pair2 in pair.Value)
                { 
                    //Check if we should build the side
                    if(pair2.Key == 0 && (pair.Key.X == -1 || pair.Key.Y == -1 || pair.Key.Z == -1)) //- sides
                    {
                        Point3 dir3 = pair.Key.ToPoint3();
                        Point3 atSide = blockPos + dir3;

                        BlockData atSideBlock = source.GetData(atSide);
                        if (!CanBuildSideMethod(atSideBlock, currentBlock))
                        {
                            continue;
                        }
                    }

                    if (pair2.Key == 1 && (pair.Key.X == 1 || pair.Key.Y == 1 || pair.Key.Z == 1)) //+ sides
                    {
                        Point3 dir3 = pair.Key.ToPoint3();
                        Point3 atSide = blockPos + dir3;

                        BlockData atSideBlock = source.GetData(atSide);
                        if (!CanBuildSideMethod(atSideBlock, currentBlock))
                        {
                            continue;
                        }
                    }

                    //pair2.Value contains the faces of the current Normal and Normal component value
                    //Let's calculate what we can see:

                    GpcPolygon polygon = new GpcPolygon();
                    foreach(Face face in pair2.Value)
                    {
                        GpcPolygon addPoly = new GpcPolygon();
                        addPoly.AddContour(new GpcVertexList(face.ConvertToPointList()), false);

                        polygon = polygon.Clip(GpcOperation.Union, addPoly);
                    }

                    //Remove the invisible parts
                    Vector3 inverseNormal = -pair.Key;
                    if (faces.ContainsKey(inverseNormal))
                    {
                        if (faces[inverseNormal].ContainsKey(pair2.Key)) //There are faces, which make this face invisible
                        {
                            GpcPolygon invisible = new GpcPolygon();
                            foreach (Face face2 in faces[inverseNormal][pair2.Key])
                            {
                                GpcPolygon addPoly = new GpcPolygon();
                                addPoly.AddContour(new GpcVertexList(face2.ConvertToPointList()), false);

                                invisible = invisible.Clip(GpcOperation.Union, addPoly);
                            }

                            polygon = polygon.Clip(GpcOperation.Difference, invisible); //Remove the invisible parts
                        }
                    }

                    //Create the face
                    if (polygon.NofContours == 0)
                        continue;

                    List<PolygonPoint> points = new List<PolygonPoint>();
                    foreach (GpcVertexList polys in polygon.Contour)
                    {
                        foreach (GpcVertex vert in polys.Vertex)
                        {
                            points.Add(new PolygonPoint(vert.X, vert.Y));
                        }
                    }
                    Polygon triangulatorPoly = new Polygon(points);

                    //MAGIC :D
                    Triangulator.Triangulate(triangulatorPoly);

                    foreach (DelaunayTriangle tri in triangulatorPoly.Triangles)
                    {
                        CustomBlockData bd = new CustomBlockData();

                        bd.IsOneTriangle = true;
                        if (pair.Key.Y != 0)
                            bd.Texture = textureNameY;
                        else
                            bd.Texture = textureName;

                        bd.Vertex1 = ConvertToVertexPosition(tri.Points[0], pair.Key, pair2.Key);
                        bd.Vertex2 = ConvertToVertexPosition(tri.Points[1], pair.Key, pair2.Key);
                        bd.Vertex3 = ConvertToVertexPosition(tri.Points[2], pair.Key, pair2.Key);

                        bd.UV1 = new Vector2(tri.Points[0].Xf, tri.Points[0].Yf);
                        bd.UV2 = new Vector2(tri.Points[1].Xf, tri.Points[1].Yf);
                        bd.UV3 = new Vector2(tri.Points[2].Xf, tri.Points[2].Yf);

                        bd.Normal = pair.Key;

                        datas.Add(bd);
                    }
                }
            }

            return datas;
        }

        private Face GetFace(BoundingBox bb, Vector3 normal)
        {
            if (normal.Y == 0 && normal.Z == 0)
            {
                if (normal.X < 0)
                {
                    return new Face(new Vector3(bb.Min.X, bb.Min.Y, bb.Min.Z), new Vector3(bb.Min.X, bb.Max.Y, bb.Max.Z), normal);
                }
                if (normal.X > 0)
                {
                    return new Face(new Vector3(bb.Max.X, bb.Min.Y, bb.Min.Z), new Vector3(bb.Max.X, bb.Max.Y, bb.Max.Z), normal);
                }
            }
            else if (normal.X == 0 && normal.Z == 0)
            {
                if (normal.Y < 0)
                {
                    return new Face(new Vector3(bb.Min.X, bb.Min.Y, bb.Min.Z), new Vector3(bb.Max.X, bb.Min.Y, bb.Max.Z), normal);
                }
                if (normal.Y > 0)
                {
                    return new Face(new Vector3(bb.Min.X, bb.Max.Y, bb.Min.Z), new Vector3(bb.Max.X, bb.Max.Y, bb.Max.Z), normal);
                }
            }
            else if (normal.X == 0 && normal.Y == 0)
            {
                if (normal.Z < 0)
                {
                    return new Face(new Vector3(bb.Min.X, bb.Min.Y, bb.Min.Z), new Vector3(bb.Max.X, bb.Max.Y, bb.Min.Z), normal);
                }
                if (normal.Z > 0)
                {
                    return new Face(new Vector3(bb.Min.X, bb.Min.Y, bb.Max.Z), new Vector3(bb.Max.X, bb.Max.Y, bb.Max.Z), normal);
                }
            }

            return new Face(new Vector3(0, 0, 0), new Vector3(0, 0, 0), normal);
        }

        public static PointF GetXYByNormal(Vector3 normal, Vector3 position)
        {
            if (normal.X != 0 && normal.Y == 0 && normal.Z == 0)
            {
                return new PointF(position.Z, position.Y);
            }
            if (normal.X == 0 && normal.Y != 0 && normal.Z == 0)
            {
                return new PointF(position.X, position.Z);
            }
            if (normal.X == 0 && normal.Y == 0 && normal.Z != 0)
            {
                return new PointF(position.X, position.Y);
            }

            return new PointF(0, 0);
        }

        private Vector3 ConvertToVertexPosition(TriangulationPoint point, Vector3 normal, float normalComponent)
        {
            if (normal.X != 0 && normal.Y == 0 && normal.Z == 0)
            {
                return new Vector3(normalComponent, point.Yf, point.Xf);
            }
            if (normal.X == 0 && normal.Y != 0 && normal.Z == 0)
            {
                return new Vector3(point.Xf, normalComponent, point.Yf);
            }
            if (normal.X == 0 && normal.Y == 0 && normal.Z != 0)
            {
                return new Vector3(point.Xf, point.Yf, normalComponent);
            }

            return new Vector3(0, 0, 0);
        }
    }
}
