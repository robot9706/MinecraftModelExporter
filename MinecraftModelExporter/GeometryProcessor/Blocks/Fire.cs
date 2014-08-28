using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Fire : Block
    {
        public Fire(byte id)
        {
            ID = id;
            Name = "Fire";
            UseMetadata = true;
        }

        public override string[] GetTextures(byte metadata)
        {
            return new string[]{
                "fire_layer_0","fire_layer_0","fire_layer_0","fire_layer_0","fire_layer_0","fire_layer_0"
            };
        }

        public override string GetTextureForSide(BlockSide side, byte metadata)
        {
            return "fire_layer_0";
        }

        public override bool IsTransparent()
        {
            return true;
        }

        public override bool IsFullyCustomModel()
        {
            return true;
        }

        public override bool IsFullSide(MinecraftModelExporter.GeometryProcessor.BlockSide side)
        {
            return false;
        }

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            List<CustomBlockData> data = new List<CustomBlockData>();

            string tex = "fire_layer_0";

            //Side X
            data.Add(new CustomBlockData()
            {
                Normal = new Vector3(1,0,0),
                Texture = tex,

                Vertex1 = new Vector3(1,0,0),
                Vertex2 = new Vector3(1,0,1),
                Vertex3 = new Vector3(1,1,1),
                Vertex4 = new Vector3(1,1,0),
            }.CreateUVs());

            data.Add(new CustomBlockData()
            {
                Normal = new Vector3(-1, 0, 0),
                Texture = tex,

                Vertex1 = new Vector3(0, 0, 0),
                Vertex2 = new Vector3(0, 0, 1),
                Vertex3 = new Vector3(0, 1, 1),
                Vertex4 = new Vector3(0, 1, 0),
            }.CreateUVs());

            //Side Z
            data.Add(new CustomBlockData()
            {
                Normal = new Vector3(0, 0, 1),
                Texture = tex,

                Vertex1 = new Vector3(0, 0, 1),
                Vertex2 = new Vector3(1, 0, 1),
                Vertex3 = new Vector3(1, 1, 1),
                Vertex4 = new Vector3(0, 1, 1),
            }.CreateUVs());

            data.Add(new CustomBlockData()
            {
                Normal = new Vector3(0, 0, -1),
                Texture = tex,

                Vertex1 = new Vector3(0, 0, 0),
                Vertex2 = new Vector3(1, 0, 0),
                Vertex3 = new Vector3(1, 1, 0),
                Vertex4 = new Vector3(0, 1, 0),
            }.CreateUVs());

            //X shape: X
            float shiftl = 0.25f;
            float shifth = 0.75f;

            data.Add(new CustomBlockData()
            {
                Texture = tex,

                Vertex2 = new Vector3(shiftl, 0, 0),
                Vertex1 = new Vector3(shiftl, 0, 1),
                Vertex4 = new Vector3(shifth, 1, 1),
                Vertex3 = new Vector3(shifth, 1, 0),

                TriFlip = true
            }.CreateUVs().CalculateNormal());

            data.Add(new CustomBlockData()
            {
                Texture = tex,

                Vertex1 = new Vector3(shifth, 0, 0),
                Vertex2 = new Vector3(shifth, 0, 1),
                Vertex3 = new Vector3(shiftl, 1, 1),
                Vertex4 = new Vector3(shiftl, 1, 0),

                TriFlip = true,
                KeepNormal = true
            }.CreateUVs().CalculateNormal());

            //X shape: Z
            data.Add(new CustomBlockData()
            {
                Texture = tex,

                Vertex1 = new Vector3(0, 0, shiftl),
                Vertex2 = new Vector3(1, 0, shiftl),
                Vertex3 = new Vector3(1, 1, shifth),
                Vertex4 = new Vector3(0, 1, shifth),

                TriFlip = true,
                KeepNormal = false
            }.CreateUVs().CalculateNormal());

            data.Add(new CustomBlockData()
            {
                Texture = tex,

                Vertex1 = new Vector3(0, 0, shifth),
                Vertex2 = new Vector3(1, 0, shifth),
                Vertex3 = new Vector3(1, 1, shiftl),
                Vertex4 = new Vector3(0, 1, shiftl),

                TriFlip = true,
                KeepNormal = true
            }.CreateUVs().CalculateNormal());

            return data;
        }
    }
}
