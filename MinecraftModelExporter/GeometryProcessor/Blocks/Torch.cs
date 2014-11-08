using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Torch : Block
    {
        public Torch(byte id)
        {
            ID = id;
            Name = "Torch";
            UseMetadata = true;
            UsesOneTexture = true;
        }

        public override bool IsTransparent()
        {
            return true;
        }

        public override bool IsFullyCustomModel()
        {
            return true;
        }

        public override bool IsFullSide(BlockSide side)
        {
            return false;
        }

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            List<CustomBlockData> d = new List<CustomBlockData>();

            bool standing = (metadata == 5);

            float h = 0.625f;
            float ts = 1.0f - h;
            float s = 0.4375f;
            float s2 = 1f - s;

            float xDif = 0f;
            float zDif = 0f;

            if (!standing)
            {
                switch (metadata)
                {
                    case 1:
                        xDif = -0.5f;
                        break;
                    case 2:
                        xDif = 0.5f;
                        break;
                    case 3:
                        zDif = -0.5f;
                        break;
                    case 4:
                        zDif = 0.5f;
                        break;
                }

                //Bottom
                d.Add(new CustomBlockData()
                {
                    Texture = "torch_on",

                    Vertex1 = new Vector3(s + xDif, 0, s + zDif),
                    Vertex2 = new Vector3(s2 + xDif, 0, s + zDif),
                    Vertex3 = new Vector3(s2 + xDif, 0, s2 + zDif),
                    Vertex4 = new Vector3(s + xDif, 0, s2 + zDif),

                    Normal = new Vector3(0, -1, 0)
                }.CreateUVs(0.4375f, 0f, 0.5625f, 0.125f));
            }

            //Sides
            d.Add(new CustomBlockData()
            {
                Texture = "torch_on",

                Vertex2 = new Vector3(s2 + xDif, 0, s + zDif),
                Vertex1 = new Vector3(s2 + (xDif / 2), h, s + (zDif / 2)),
                Vertex4 = new Vector3(s2 + (xDif / 2), h, s2 + (zDif / 2)),
                Vertex3 = new Vector3(s2 + xDif, 0, s2 + zDif),

                Normal = new Vector3(1, 0, 0)
            }.CreateUVs(ts,s,1,s2).RotateUVs(270));
            d.Add(new CustomBlockData()
            {
                Texture = "torch_on",

                Vertex2 = new Vector3(s + xDif, 0, s + zDif),
                Vertex1 = new Vector3(s + (xDif / 2), h, s + (zDif / 2)),
                Vertex4 = new Vector3(s + (xDif / 2), h, s2 + (zDif / 2)),
                Vertex3 = new Vector3(s + xDif, 0, s2 + zDif),

                Normal = new Vector3(-1, 0, 0)
            }.CreateUVs(ts, s, 1, s2).RotateUVs(270));

            d.Add(new CustomBlockData()
            {
                Texture = "torch_on",

                Vertex2 = new Vector3(s + xDif, 0, s + zDif),
                Vertex1 = new Vector3(s + (xDif / 2), h, s + (zDif / 2)),
                Vertex4 = new Vector3(s2 + (xDif / 2), h, s + (zDif / 2)),
                Vertex3 = new Vector3(s2 + xDif, 0, s + zDif),

                Normal = new Vector3(0, 0, -1)
            }.CreateUVs(ts, s, 1, s2).RotateUVs(270));
            d.Add(new CustomBlockData()
            {
                Texture = "torch_on",

                Vertex2 = new Vector3(s + xDif, 0, s2 + zDif),
                Vertex1 = new Vector3(s + (xDif / 2), h, s2 + (zDif / 2)),
                Vertex4 = new Vector3(s2 + (xDif / 2), h, s2 + (zDif / 2)),
                Vertex3 = new Vector3(s2 + xDif, 0, s2 + zDif),

                Normal = new Vector3(0, 0, 1)
            }.CreateUVs(ts, s, 1, s2).RotateUVs(270));

            //Top
            d.Add(new CustomBlockData()
            {
                Texture = "torch_on",

                Vertex1 = new Vector3(s + (xDif / 2), h, s + (zDif / 2)),
                Vertex2 = new Vector3(s2 + (xDif / 2), h, s + (zDif / 2)),
                Vertex3 = new Vector3(s2 + (xDif / 2), h, s2 + (zDif / 2)),
                Vertex4 = new Vector3(s + (xDif / 2), h, s2 + (zDif / 2)),

                Normal = new Vector3(0,1,0)
            }.CreateUVs(0.4375f, 0.5f, 0.5625f, 0.625f));

            return d;
        }
    }
}
