using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Rails : Block
    {
        public Rails(byte id)
        {
            ID = id;
            UseMetadata = true;
            UsesOneTexture = false;
            Name = "Rail";
        }

        public override bool IsFullyCustomModel()
        {
            return true;
        }

        public override bool IsTransparent()
        {
            return true;
        }

        public override string GetTextureForSide(BlockSide side, byte metadata)
        {
            bool[] bits = BitHelper.GetBits(metadata);
            bool on = bits[0];

            if (ID == 27)
                return "rail_golden" + (on ? "_powered" : "");
            if(ID == 28)
                return "rail_detector" + (on ? "_powered" : "");
            if (ID == 157)
                return "rail_activator" + (on ? "_powered" : "");

            if (metadata == 6 || metadata == 7 || metadata == 8 || metadata == 9)
                return "rail_normal_turned";

            return "rail_normal";
        }

        public override string[] GetTextures(byte metadata)
        {
            string tx = GetTextureForSide(BlockSide.Ypos, metadata);

            return new string[] { tx, tx, tx, tx, tx, tx };
        }

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            List<CustomBlockData> l = new List<CustomBlockData>();

            float d = 0.01f;
            if (metadata == 0)
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, d, 0),
                    Vertex2 = new Vector3(1, d, 0),
                    Vertex3 = new Vector3(1, d, 1),
                    Vertex4 = new Vector3(0, d, 1),

                    Texture = GetTextureForSide(BlockSide.Ypos, metadata),

                    Normal = new Vector3(0, 1, 0)
                }.CreateUVs());
            }
            if (metadata == 1)
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, d, 0),
                    Vertex2 = new Vector3(1, d, 0),
                    Vertex3 = new Vector3(1, d, 1),
                    Vertex4 = new Vector3(0, d, 1),

                    Texture = GetTextureForSide(BlockSide.Ypos, metadata),

                    Normal = new Vector3(0, 1, 0)
                }.CreateUVsRotated90());
            }
            if (metadata == 2)
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, d, 0),
                    Vertex2 = new Vector3(1, d + 1, 0),
                    Vertex3 = new Vector3(1, d + 1, 1),
                    Vertex4 = new Vector3(0, d, 1),

                    Texture = GetTextureForSide(BlockSide.Ypos, metadata),

                    Normal = new Vector3(0, 1, 0)
                }.CreateUVsRotated90());
            }
            if (metadata == 3)
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, d + 1, 0),
                    Vertex2 = new Vector3(1, d, 0),
                    Vertex3 = new Vector3(1, d, 1),
                    Vertex4 = new Vector3(0, d + 1, 1),

                    Texture = GetTextureForSide(BlockSide.Ypos, metadata),

                    Normal = new Vector3(0, 1, 0)
                }.CreateUVsRotated90());
            }
            if (metadata == 4)
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, d+1, 0),
                    Vertex2 = new Vector3(1, d+1, 0),
                    Vertex3 = new Vector3(1, d, 1),
                    Vertex4 = new Vector3(0, d, 1),

                    Texture = GetTextureForSide(BlockSide.Ypos, metadata),

                    Normal = new Vector3(0, 1, 0)
                }.CreateUVs());
            }
            if (metadata == 5)
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, d, 0),
                    Vertex2 = new Vector3(1, d, 0),
                    Vertex3 = new Vector3(1, d + 1, 1),
                    Vertex4 = new Vector3(0, d + 1, 1),

                    Texture = GetTextureForSide(BlockSide.Ypos, metadata),

                    Normal = new Vector3(0, 1, 0)
                }.CreateUVs());
            }

            if (metadata == 6)
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, d, 0),
                    Vertex2 = new Vector3(1, d, 0),
                    Vertex3 = new Vector3(1, d, 1),
                    Vertex4 = new Vector3(0, d, 1),

                    Texture = GetTextureForSide(BlockSide.Ypos, metadata),

                    Normal = new Vector3(0, 1, 0)
                }.CreateUVsXYFliped());
            }
            if (metadata == 7)
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, d, 0),
                    Vertex2 = new Vector3(1, d, 0),
                    Vertex3 = new Vector3(1, d, 1),
                    Vertex4 = new Vector3(0, d, 1),

                    Texture = GetTextureForSide(BlockSide.Ypos, metadata),

                    Normal = new Vector3(0, 1, 0)
                }.CreateUVsYFliped());
            }
            if (metadata == 8)
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, d, 0),
                    Vertex2 = new Vector3(1, d, 0),
                    Vertex3 = new Vector3(1, d, 1),
                    Vertex4 = new Vector3(0, d, 1),

                    Texture = GetTextureForSide(BlockSide.Ypos, metadata),

                    Normal = new Vector3(0, 1, 0)
                }.CreateUVsCrossFlip());
            }
            if (metadata == 9)
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, d, 0),
                    Vertex2 = new Vector3(1, d, 0),
                    Vertex3 = new Vector3(1, d, 1),
                    Vertex4 = new Vector3(0, d, 1),

                    Texture = GetTextureForSide(BlockSide.Ypos, metadata),

                    Normal = new Vector3(0, 1, 0)
                }.CreateUVs());
            }

            return l;
        }
    }
}
