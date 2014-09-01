using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Farmland : Block
    {
        private float _height = 15f / 16f;

        public Farmland(byte id)
        {
            ID = id;
            Name = "Farmland";
            UseMetadata = true;
        }

        public override string GetTextureForSide(BlockSide side, byte metadata)
        {
            if (side == BlockSide.Ypos)
                return (metadata == 7 ? "farmland_wet" : "farmland_dry");

            return "dirt";
        }

        public override bool IsFullSide(MinecraftModelExporter.GeometryProcessor.BlockSide side)
        {
            return (side == BlockSide.Yneg);
        }

        public override List<CustomBlockData> GenerateSide(BlockSide side, byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg)
        {
            List<CustomBlockData> bd = new List<CustomBlockData>();

            switch (side)
            {
                case BlockSide.Xneg:
                    bd.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(0, 0, 0),
                        Vertex2 = new Vector3(0, _height, 0),
                        Vertex3 = new Vector3(0, _height, 1),
                        Vertex4 = new Vector3(0, 0, 1),

                        Normal = new Vector3(-1, 0, 0),
                        Texture = "dirt",

                        TriFlip = true
                    }.CreateUVs(0, 0, 1, _height));
                    break;
                case BlockSide.Xpos:
                    bd.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(1, 0, 0),
                        Vertex2 = new Vector3(1, _height, 0),
                        Vertex3 = new Vector3(1, _height, 1),
                        Vertex4 = new Vector3(1, 0, 1),

                        Normal = new Vector3(1, 0, 0),
                        Texture = "dirt",

                        TriFlip = true
                    }.CreateUVs(0, 0, 1, _height));
                    break;

                case BlockSide.Zneg:
                    bd.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(0, 0, 0),
                        Vertex2 = new Vector3(0, _height, 0),
                        Vertex3 = new Vector3(1, _height, 0),
                        Vertex4 = new Vector3(1, 0, 0),

                        Normal = new Vector3(0, 0, -1),
                        Texture = "dirt",

                        TriFlip = true
                    }.CreateUVs(0, 0, 1, _height));
                    break;
                case BlockSide.Zpos:
                    bd.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(0, 0, 1),
                        Vertex2 = new Vector3(0, _height, 1),
                        Vertex3 = new Vector3(1, _height, 1),
                        Vertex4 = new Vector3(1, 0, 1),

                        Normal = new Vector3(0, 0, 1),
                        Texture = "dirt",

                        TriFlip = true
                    }.CreateUVs(0, 0, 1, _height));
                    break;

                case BlockSide.Ypos:
                    bd.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(0, _height, 0),
                        Vertex2 = new Vector3(1, _height, 0),
                        Vertex3 = new Vector3(1, _height, 1),
                        Vertex4 = new Vector3(0, _height, 1),

                        Normal = new Vector3(0, 1, 0),
                        Texture = GetTextureForSide(BlockSide.Ypos, metadata),
                    }.CreateUVs(0, 0, 1, _height));
                    break;
            }

            return bd;
        }
    }
}
