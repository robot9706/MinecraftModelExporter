using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Bed : Block
    {
        public Bed(byte id)
        {
            ID = id;
            UseMetadata = true;
            Name = "Bed";
        }

        public override bool IsTransparent()
        {
            return true;
        }

        public override bool IsFullyCustomModel()
        {
            return true;
        }

        public override string[] GetTextures(byte metadata)
        {
            return new string[]{
                "bed_head_top",
                "bed_head_side",
                "bed_head_end",
                "bed_feet_end",
                "bed_feet_side",
                "bed_feet_top"
            };
        }

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            List<CustomBlockData> d = new List<CustomBlockData>();

            bool head = ((metadata & 8) != 0);
            int direction = metadata & 3;

            float height = 0.5625f;

            Rotate rot = Rotate.None;
            Vector3 bedDir = new Vector3(0, 0, 0);
            switch (direction)
            { 
                case 0:
                    rot = Rotate.Deg270;
                    bedDir.Z = 1;
                    break;
                case 1:
                    rot = Rotate.Deg180;
                    bedDir.X = -1;
                    break;
                case 2:
                    rot = Rotate.Deg90;
                    bedDir.Z = -1;
                    break;
                case 3:
                    bedDir.X = 1;
                    break;
            }

            //top
            d.Add(new CustomBlockData()
            {
                Vertex1 = new Vector3(0, height, 0),
                Vertex2 = new Vector3(1, height, 0),
                Vertex3 = new Vector3(1, height, 1),
                Vertex4 = new Vector3(0, height, 1),

                Normal = new Vector3(0, 1, 0),

                Texture = head ? "bed_head_top" : "bed_feet_top"
            }.CreateUVsRotated(rot));

            //bottom
            float h = 0.1875f;

            d.Add(new CustomBlockData()
            {
                Vertex1 = new Vector3(0, h, 0),
                Vertex2 = new Vector3(1, h, 0),
                Vertex3 = new Vector3(1, h, 1),
                Vertex4 = new Vector3(0, h, 1),

                Normal = new Vector3(0, -1, 0),

                Texture = "planks_oak"
            }.CreateUVsRotated(rot));

            //head
            if (head)
            {
                //end
                Vector3[] endVerts = BuildBasedOnNormal(bedDir, height);

                d.Add(new CustomBlockData()
                {
                    Vertex1 = endVerts[3],
                    Vertex2 = endVerts[2],
                    Vertex3 = endVerts[1],
                    Vertex4 = endVerts[0],

                    Normal = bedDir,

                    Texture = "bed_head_end",
                }.CreateUVs(0, 0, height, 1).RotateUVs(90));

                //sides
                Vector3 sideNormalA;
                Vector3 sideNormalB;

                if (bedDir.X != 0)
                {
                    sideNormalA = new Vector3(0, 0, 1);
                    sideNormalB = new Vector3(0, 0, -1);
                }
                else
                {
                    sideNormalA = new Vector3(1, 0, 0);
                    sideNormalB = new Vector3(-1, 0, 0);
                }

                Vector3[] sideVertsA = BuildBasedOnNormal(sideNormalA, height);
                CustomBlockData sideA = new CustomBlockData()
                {
                    Vertex1 = sideVertsA[3],
                    Vertex2 = sideVertsA[2],
                    Vertex3 = sideVertsA[1],
                    Vertex4 = sideVertsA[0],

                    Normal = sideNormalA,

                    Texture = "bed_head_side",
                }.CreateUVs(0, 0, height, 1).RotateUVs(90);

                Vector3[] sideVertsB = BuildBasedOnNormal(sideNormalB, height);
                CustomBlockData sideB = new CustomBlockData()
                {
                    Vertex1 = sideVertsB[3],
                    Vertex2 = sideVertsB[2],
                    Vertex3 = sideVertsB[1],
                    Vertex4 = sideVertsB[0],

                    Normal = sideNormalB,

                    Texture = "bed_head_side",
                }.CreateUVs(0, 0, height, 1).RotateUVs(90);

                if (direction == 1 || direction == 2)
                {
                    sideA = sideA.FlipUVsX();
                    sideB = sideB.FlipUVsX();
                }

                d.Add(sideA);
                d.Add(sideB);
            }
            //feet
            else
            {
                //end
                Vector3[] endVerts = BuildBasedOnNormal(-bedDir, height);

                d.Add(new CustomBlockData()
                {
                    Vertex1 = endVerts[3],
                    Vertex2 = endVerts[2],
                    Vertex3 = endVerts[1],
                    Vertex4 = endVerts[0],

                    Normal = -bedDir,

                    Texture = "bed_feet_end",
                }.CreateUVs(0, 0, height, 1).RotateUVs(90));

                //sides
                Vector3 sideNormalA;
                Vector3 sideNormalB;

                if (bedDir.X != 0)
                {
                    sideNormalA = new Vector3(0, 0, 1);
                    sideNormalB = new Vector3(0, 0, -1);
                }
                else
                {
                    sideNormalA = new Vector3(1, 0, 0);
                    sideNormalB = new Vector3(-1, 0, 0);
                }

                Vector3[] sideVertsA = BuildBasedOnNormal(sideNormalA, height);
                CustomBlockData sideA = new CustomBlockData()
                {
                    Vertex1 = sideVertsA[3],
                    Vertex2 = sideVertsA[2],
                    Vertex3 = sideVertsA[1],
                    Vertex4 = sideVertsA[0],

                    Normal = sideNormalA,

                    Texture = "bed_feet_side",
                }.CreateUVs(0, 0, height, 1).RotateUVs(90);

                Vector3[] sideVertsB = BuildBasedOnNormal(sideNormalB, height);
                CustomBlockData sideB = new CustomBlockData()
                {
                    Vertex1 = sideVertsB[3],
                    Vertex2 = sideVertsB[2],
                    Vertex3 = sideVertsB[1],
                    Vertex4 = sideVertsB[0],

                    Normal = sideNormalB,

                    Texture = "bed_feet_side",
                }.CreateUVs(0, 0, height, 1).RotateUVs(90);

                if (direction == 1 || direction == 2)
                {
                    sideA = sideA.FlipUVsX();
                    sideB = sideB.FlipUVsX();
                }

                d.Add(sideA);
                d.Add(sideB);
            }

            return d;
        }

        private Vector3[] BuildBasedOnNormal(Vector3 normal, float y)
        {
            Vector3[] rt = new Vector3[4];

            if (normal.X != 0)
            {
                float x = normal.X;

                if (x == -1)
                    x = 0;

                rt[0] = new Vector3(x, 0, 0);
                rt[1] = new Vector3(x, y, 0);
                rt[2] = new Vector3(x, y, 1);
                rt[3] = new Vector3(x, 0, 1);
            }
            else
            {
                float z = normal.Z;

                if (z == -1)
                    z = 0;

                rt[0] = new Vector3(0, 0, z);
                rt[1] = new Vector3(0, y, z);
                rt[2] = new Vector3(1, y, z);
                rt[3] = new Vector3(1, 0, z);
            }

            return rt;
        }
    }
}
