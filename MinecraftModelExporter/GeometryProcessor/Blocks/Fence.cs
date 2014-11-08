using MinecraftModelExporter.GeomGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Fence : Block, IGeometryGeneratorSource
    {
        private string _texture;
        private string _textureY;

        public Fence(byte id, string texture, string textureY)
        {
            ID = id;
            Name = "Fence";
            UsesOneTexture = true;
            UseMetadata = false;

            _texture = texture;
            _textureY = textureY;
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

        private bool IsFence(BlockData data)
        {
            Block bl = Block.Blocks[data.GetGlobalID()];
            if (bl == null)
                return false;

            return (bl is Fence);
        }

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            float onepix = 1f / 16f;
            float partStart = onepix * 7;
            float partEnd = onepix * 9;

            float tstart = 0.375f;
            float tend = 1f - tstart;

            List<BoundingBox> boxes = new List<BoundingBox>();

            //Pole
            boxes.Add(new BoundingBox(new Vector3(tstart, 0, tstart), new Vector3(tend, 1f, tend)));

            if (IsFence(Xpos))
            {
                boxes.Add(new BoundingBox(new Vector3(tend, 1f - onepix, partStart), new Vector3(1f, 1f - (onepix * 4), partEnd)));
                boxes.Add(new BoundingBox(new Vector3(tend, 1f - (onepix * 8), partStart), new Vector3(1f, 1f - (onepix * 11), partEnd)));
            }
            if (IsFence(Xneg))
            {
                boxes.Add(new BoundingBox(new Vector3(0, 1f - onepix, partStart), new Vector3(tstart, 1f - (onepix * 4), partEnd)));
                boxes.Add(new BoundingBox(new Vector3(0, 1f - (onepix * 8), partStart), new Vector3(tstart, 1f - (onepix * 11), partEnd)));
            }

            if (IsFence(Zpos))
            {
                boxes.Add(new BoundingBox(new Vector3(partStart, 1f - onepix, tend), new Vector3(partEnd, 1f - (onepix * 4), 1f)));
                boxes.Add(new BoundingBox(new Vector3(partStart, 1f - (onepix * 8), tend), new Vector3(partEnd, 1f - (onepix * 11), 1f)));
            }
            if (IsFence(Zneg))
            {
                boxes.Add(new BoundingBox(new Vector3(partStart, 1f - onepix, 0), new Vector3(partEnd, 1f - (onepix * 4), tstart)));
                boxes.Add(new BoundingBox(new Vector3(partStart, 1f - (onepix * 8), 0), new Vector3(partEnd, 1f - (onepix * 11), tstart)));
            }

            return GeometryGenerator.GenerateModel(boxes, source, blockPosition, false, this);
        }

        private Vector3 Rot(Vector3 a, Vector3 c, float y)
        {
            Vector2 p = new Vector2(a.X, a.Z);
            Vector2 n = Vector2.RotateAround(p, new Vector2(c.X, c.Z), y);

            return new Vector3(n.X, a.Y, n.Y);
        }

        bool IGeometryGeneratorSource.CanBuildSide(BlockData me, BlockData side, Point3 mePos, Point3 sidePos)
        {
            if (IsFence(side))
                return false;

            return !side.IsSolid;
        }

        string IGeometryGeneratorSource.GetTexture(Face face)
        {
            if (face.Normal.Y != 0)
                return _textureY;

            return _texture;
        }

        public Vector2[] GetUVsForTriangle(Vector2[] source, Face face)
        {
            return source;
        }
    }
}
