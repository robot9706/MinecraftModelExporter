using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Fence : Block
    {
        public Fence(byte id)
        {
            ID = id;
            Name = "Fence";
            UsesOneTexture = true;
            UseMetadata = false;
        }

        public override bool IsTransparent()
        {
            return true;
        }

        public override bool IsFullSide(MinecraftModelExporter.GeometryProcessor.BlockSide side)
        {
            return false;
        }

        public override bool IsFullyCustomModel()
        {
            return true;
        }

        float conw = 0.125f;
        float conl = 0.375f;
        float conh = 0.1875f;

        float constart = 0.4375f;
        float conend = 0.5625f;

        float onepx = 0.0625f;
        float uponepx = 1f - 0.0625f;

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            List<CustomBlockData> d = new List<CustomBlockData>();

            float tsize = 0.25f;
            float tstart = 0.375f;
            float tend = 1f - tstart;

            //Post
            {
                //Top
                if (!Ypos.IsSolid && Ypos.ID != ID)
                {
                    d.Add(new CustomBlockData()
                    {
                        Texture = "planks_oak",

                        Vertex1 = new Vector3(tstart, 1, tstart),
                        Vertex2 = new Vector3(tend, 1, tstart),
                        Vertex3 = new Vector3(tend, 1, tend),
                        Vertex4 = new Vector3(tstart, 1, tend),

                        Normal = new Vector3(0, 1, 0)
                    }.CreateUVs(tstart, tend, tend, tstart));
                }

                //Bottom
                if (!Yneg.IsSolid && Yneg.ID != ID)
                {
                    d.Add(new CustomBlockData()
                    {
                        Texture = "planks_oak",

                        Vertex1 = new Vector3(tstart, 0, tstart),
                        Vertex2 = new Vector3(tend, 0, tstart),
                        Vertex3 = new Vector3(tend, 0, tend),
                        Vertex4 = new Vector3(tstart, 0, tend),

                        Normal = new Vector3(0, -1, 0)
                    }.CreateUVs(tstart, tend, tend, tstart));
                }

                //Sides
                d.Add(new CustomBlockData()
                {
                    Normal = new Vector3(1, 0, 0),

                    Texture = "planks_oak",

                    Vertex2 = new Vector3(tend, 0, tstart),
                    Vertex1 = new Vector3(tend, 1, tstart),
                    Vertex4 = new Vector3(tend, 1, tend),
                    Vertex3 = new Vector3(tend, 0, tend),
                }.CreateUVs(0, tstart, 1, tend));
                d.Add(new CustomBlockData()
                {
                    Normal = new Vector3(-1, 0, 0),

                    Texture = "planks_oak",

                    Vertex2 = new Vector3(tstart, 0, tstart),
                    Vertex1 = new Vector3(tstart, 1, tstart),
                    Vertex4 = new Vector3(tstart, 1, tend),
                    Vertex3 = new Vector3(tstart, 0, tend),
                }.CreateUVs(0, tstart, 1, tend));

                d.Add(new CustomBlockData()
                {
                    Normal = new Vector3(0, 0, -1),

                    Texture = "planks_oak",

                    Vertex2 = new Vector3(tstart, 0, tstart),
                    Vertex1 = new Vector3(tstart, 1, tstart),
                    Vertex4 = new Vector3(tend, 1, tstart),
                    Vertex3 = new Vector3(tend, 0, tstart),
                }.CreateUVs(0, tstart, 1, tend));
                d.Add(new CustomBlockData()
                {
                    Normal = new Vector3(0, 0, 1),

                    Texture = "planks_oak",

                    Vertex2 = new Vector3(tstart, 0, tend),
                    Vertex1 = new Vector3(tstart, 1, tend),
                    Vertex4 = new Vector3(tend, 1, tend),
                    Vertex3 = new Vector3(tend, 0, tend),
                }.CreateUVs(0, tstart, 1, tend));
            }

            //Connections
            {
                if (Xneg.IsSolid || Xneg.ID == ID)
                {
                    BuildConnectionSide(d, 0);
                }
                if (Xpos.IsSolid || Xpos.ID == ID)
                {
                    BuildConnectionSide(d, 180);
                }

                if (Zpos.IsSolid || Zpos.ID == ID)
                {
                    BuildConnectionSide(d, 270);
                }
                if (Zneg.IsSolid || Zneg.ID == ID)
                {
                    BuildConnectionSide(d, 90);
                }
            }

            return d;
        }

        private void BuildConnectionSide(List<CustomBlockData> data, float a)
        {
            BuildConnectorPart(data, uponepx, a);
            BuildConnectorPart(data, uponepx - onepx * 6, a);
        }

        private void BuildConnectorPart(List<CustomBlockData> d, float baseY, float rotateY)
        {
            List<CustomBlockData> data = new List<CustomBlockData>();

            //Top
            data.Add(new CustomBlockData()
            {
                Texture = "planks_oak",

                Normal = new Vector3(0, 1, 0),

                Vertex1 = new Vector3(0, baseY, constart),
                Vertex2 = new Vector3(conl, baseY, constart),
                Vertex3 = new Vector3(conl, baseY, conend),
                Vertex4 = new Vector3(0, baseY, conend),
            }.CreateUVs(constart, conend, conend, constart));

            //Bottom
            data.Add(new CustomBlockData()
            {
                Texture = "planks_oak",

                Normal = new Vector3(0, -1, 0),

                Vertex1 = new Vector3(0, baseY - conh, constart),
                Vertex2 = new Vector3(conl, baseY - conh, constart),
                Vertex3 = new Vector3(conl, baseY - conh, conend),
                Vertex4 = new Vector3(0, baseY - conh, conend),
            }.CreateUVs(constart, conend, conend, constart));

            data.Add(new CustomBlockData()
            {
                Texture = "planks_oak",

                Normal = new Vector3(0, 0, 1),

                Vertex1 = new Vector3(0, baseY - conh, conend),
                Vertex2 = new Vector3(conl, baseY - conh, conend),
                Vertex3 = new Vector3(conl, baseY, conend),
                Vertex4 = new Vector3(0, baseY, conend)
            }.CreateUVs(0, 0, conl, conh));
            data.Add(new CustomBlockData()
            {
                Texture = "planks_oak",

                Normal = new Vector3(0, 0, -1),

                Vertex1 = new Vector3(0, baseY - conh, constart),
                Vertex2 = new Vector3(conl, baseY - conh, constart),
                Vertex3 = new Vector3(conl, baseY, constart),
                Vertex4 = new Vector3(0, baseY, constart)
            }.CreateUVs(0, 0, conl, conh));

            Vector3 rotBase = new Vector3(0.5f, 0, 0.5f);
            for (int x = 0; x < data.Count; x++)
            {
                CustomBlockData dat = data[x];

                dat.Vertex1 = Rot(dat.Vertex1, rotBase, rotateY);
                dat.Vertex2 = Rot(dat.Vertex2, rotBase, rotateY);
                dat.Vertex3 = Rot(dat.Vertex3, rotBase, rotateY);
                dat.Vertex4 = Rot(dat.Vertex4, rotBase, rotateY);

                if (dat.Normal.Y == 0)
                {
                    if ((rotateY == 180 || rotateY == 270) && dat.Normal.Z != 0)
                    {
                        dat.TriFlip = true;
                    }

                    dat.Normal = Rot(dat.Normal, new Vector3(0, 0, 0), rotateY);
                }
                else
                {
                    if (rotateY == 180 || rotateY == 270)
                    {
                        dat = dat.FlipUVsY();
                    }
                }

                d.Add(dat);
            }
        }

        private Vector3 Rot(Vector3 a, Vector3 c, float y)
        {
            Vector2 p = new Vector2(a.X, a.Z);
            Vector2 n = Vector2.RotateAround(p, new Vector2(c.X, c.Z), y);

            return new Vector3(n.X, a.Y, n.Y);
        }
    }
}
