using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Ladder : Block
    {
        public Ladder(byte id)
        {
            ID = id;
            UseMetadata = true;
            Name = "Ladder";
        }

        public override bool IsTransparent()
        {
            return true;
        }

        public override bool IsFullyCustomModel()
        {
            return true;
        }

        public override string GetTextureForSide(BlockSide side, byte metadata)
        {
            return "ladder";
        }

        public override string[] GetTextures(byte metadata)
        {
            return new string[] { "ladder", "ladder", "ladder", "ladder", "ladder", "ladder" };
        }

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            List<CustomBlockData> dat = new List<CustomBlockData>();
            
            float d = 0.01f;
            if (metadata == 2)
            {
                dat.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, 1, 1-d),
                    Vertex2 = new Vector3(0, 0, 1-d),
                    Vertex3 = new Vector3(1, 0, 1-d),
                    Vertex4 = new Vector3(1, 1, 1-d),

                    Normal = new Vector3(0, 0, -1),

                    Texture = "ladder"
                }.CreateUVsRotated90());
            }
            if (metadata == 3)
            {
                dat.Add(new CustomBlockData()
                {
                    Vertex1  =new Vector3(0,1,d),
                    Vertex2 = new Vector3(0,0,d),
                    Vertex3 = new Vector3(1,0,d),
                    Vertex4 = new Vector3(1,1,d),

                    Normal = new Vector3(0,0,1),

                    Texture = "ladder"
                }.CreateUVsRotated90());
            }
            if (metadata == 4)
            {
                dat.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(1-d, 1, 0),
                    Vertex2 = new Vector3(1-d, 0, 0),
                    Vertex3 = new Vector3(1-d, 0, 1),
                    Vertex4 = new Vector3(1-d, 1, 1),

                    Normal = new Vector3(-1, 0, 0),

                    Texture = "ladder"
                }.CreateUVsRotated90());
            }
            if (metadata == 5)
            {
                dat.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(d, 1, 0),
                    Vertex2 = new Vector3(d, 0, 0),
                    Vertex3 = new Vector3(d, 0, 1),
                    Vertex4 = new Vector3(d, 1, 1),

                    Normal = new Vector3(1, 0, 0),

                    Texture = "ladder"
                }.CreateUVsRotated90());
            }

            return dat;
        }
    }
}
