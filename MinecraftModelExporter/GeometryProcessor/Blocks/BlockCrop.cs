using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class BlockCrop : Block
    {
        float onePix = 1f / 16f;
        float topOnePix = 15f / 16f;
        float fourPix = 4f / 16f;
        float backFourPix = 12f / 16f;

        public BlockCrop(byte id)
        {
            ID = id;
            UseMetadata = true;
        }

        public virtual string GetTextureForStage(byte stage)
        {
            return "stone";
        }

        public override bool IsTransparent()
        {
            return true;
        }

        public override bool IsFullyCustomModel()
        {
            return true;
        }

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            List<CustomBlockData> data = new List<CustomBlockData>();

            string tex = GetTextureForStage(metadata);

            data.Add(new CustomBlockData()
            {
                Normal = new Vector3(1, 0, 0),
                TriFlip = true,
                DoubleSided = true,

                Texture = tex,

                Vertex1 = new Vector3(backFourPix, -onePix, 0),
                Vertex2 = new Vector3(backFourPix, topOnePix, 0),
                Vertex3 = new Vector3(backFourPix, topOnePix, 1f),
                Vertex4 = new Vector3(backFourPix, -onePix, 1f)
            }.CreateUVsRotated(90));

            data.Add(new CustomBlockData()
            {
                Normal = new Vector3(-1, 0, 0),
                TriFlip = true,
                DoubleSided = true,

                Texture = tex,

                Vertex1 = new Vector3(fourPix, -onePix, 0),
                Vertex2 = new Vector3(fourPix, topOnePix, 0),
                Vertex3 = new Vector3(fourPix, topOnePix, 1f),
                Vertex4 = new Vector3(fourPix, -onePix, 1f)
            }.CreateUVsRotated(90));

            data.Add(new CustomBlockData()
            {
                Normal = new Vector3(0, 0, -1),
                TriFlip = true,
                DoubleSided = true,

                Texture = tex,

                Vertex1 = new Vector3(0, -onePix, fourPix),
                Vertex2 = new Vector3(0, topOnePix, fourPix),
                Vertex3 = new Vector3(1, topOnePix, fourPix),
                Vertex4 = new Vector3(1, -onePix, fourPix)
            }.CreateUVsRotated(90));

            data.Add(new CustomBlockData()
            {
                Normal = new Vector3(0, 0, 1),
                TriFlip = true,
                DoubleSided = true,

                Texture = tex,

                Vertex1 = new Vector3(0, -onePix, backFourPix),
                Vertex2 = new Vector3(0, topOnePix, backFourPix),
                Vertex3 = new Vector3(1, topOnePix, backFourPix),
                Vertex4 = new Vector3(1, -onePix, backFourPix)
            }.CreateUVsRotated(90));

            return data;
        }
    }
}
