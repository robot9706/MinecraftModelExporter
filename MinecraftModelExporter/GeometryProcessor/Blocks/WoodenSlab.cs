﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class WoodenSlab : Block
    {
        public WoodenSlab(byte id)
        {
            ID = id;
            UseMetadata = true;
            Name = "Wooden slab";
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
            int i = BitHelper.Help(metadata, 5, 3);

            switch (i)
            { 
                case 0:
                    return "planks_oak";
                case 1:
                    return "planks_spruce";
                case 2:
                    return "planks_birch";
                case 3:
                    return "planks_jungle";
            }

            return "stone";
        }

        public override string[] GetTextures(byte metadata)
        {
            return new string[] { GetTextureForSide(BlockSide.Xpos, metadata), GetTextureForSide(BlockSide.Xneg, metadata), GetTextureForSide(BlockSide.Ypos, metadata), GetTextureForSide(BlockSide.Yneg, metadata), GetTextureForSide(BlockSide.Zpos, metadata), GetTextureForSide(BlockSide.Zneg, metadata) };
        }

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            List<CustomBlockData> r = new List<CustomBlockData>();

            bool top = BitHelper.IsBitSet(metadata, 3);

            float h = .5f;
            float l = 0;

            if (top)
            {
                h = 1;
                l = .5f;
            }

            if (Block.CanBuild(Ypos, me))
            {
                r.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, h, 0),
                    Vertex2 = new Vector3(1, h, 0),
                    Vertex3 = new Vector3(1, h, 1),
                    Vertex4 = new Vector3(0, h, 1),

                    Texture = GetTextureForSide(BlockSide.Ypos, metadata),
                    Normal = new Vector3(0, 1, 0)
                }.CreateUVs());
            }

            if (Block.CanBuild(Yneg, me))
            {
                r.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(1, l, 1),
                    Vertex2 = new Vector3(0, l, 1),
                    Vertex3 = new Vector3(0, l, 0),
                    Vertex4 = new Vector3(1, l, 0),

                    Texture = GetTextureForSide(BlockSide.Yneg, metadata),
                    Normal = new Vector3(0, -1, 0)
                }.CreateUVs());
            }

            if (Block.CanBuild(Zneg, me))
            {
                r.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, l, 0),
                    Vertex2 = new Vector3(1, l, 0),
                    Vertex3 = new Vector3(1, h, 0),
                    Vertex4 = new Vector3(0, h, 0),

                    UV1 = new Vector2(0, l),
                    UV2 = new Vector2(1, l),
                    UV3 = new Vector2(1, h),
                    UV4 = new Vector2(0, h),

                    Texture = GetTextureForSide(BlockSide.Zneg, metadata),
                    Normal = new Vector3(0, 0, -1)
                });
            }

            if (Block.CanBuild(Zpos, me))
            {
                r.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, l, 1),
                    Vertex2 = new Vector3(1, l, 1),
                    Vertex3 = new Vector3(1, h, 1),
                    Vertex4 = new Vector3(0, h, 1),

                    UV1 = new Vector2(0, l),
                    UV2 = new Vector2(1, l),
                    UV3 = new Vector2(1, h),
                    UV4 = new Vector2(0, h),

                    Texture = GetTextureForSide(BlockSide.Zpos, metadata),
                    Normal = new Vector3(0, 0, 1)
                });
            }

            if (Block.CanBuild(Xneg, me))
            {
                r.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, l, 0),
                    Vertex2 = new Vector3(0, l, 1),
                    Vertex3 = new Vector3(0, h, 1),
                    Vertex4 = new Vector3(0, h, 0),

                    UV1 = new Vector2(0, l),
                    UV2 = new Vector2(1, l),
                    UV3 = new Vector2(1, h),
                    UV4 = new Vector2(0, h),

                    Texture = GetTextureForSide(BlockSide.Xneg, metadata),
                    Normal = new Vector3(-1, 0, 0)
                });
            }

            if (Block.CanBuild(Xpos, me))
            {
                r.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(1, l, 0),
                    Vertex2 = new Vector3(1, l, 1),
                    Vertex3 = new Vector3(1, h, 1),
                    Vertex4 = new Vector3(1, h, 0),

                    UV1 = new Vector2(0, l),
                    UV2 = new Vector2(1, l),
                    UV3 = new Vector2(1, h),
                    UV4 = new Vector2(0, h),

                    Texture = GetTextureForSide(BlockSide.Xpos, metadata),
                    Normal = new Vector3(1, 0, 0)
                });
            }

            return r;
        }
    }
}
