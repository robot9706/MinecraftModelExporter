﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class BlockX : Block
    {
        private string _tex;

        public BlockX(byte id, string name, string tex)
        {
            ID = id;
            Name = name;
            _tex = tex;
        }

        public override bool IsFullyCustomModel()
        {
            return true;
        }

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            List<CustomBlockData> dat = new List<CustomBlockData>();

            float s = 0.707f;
            Vector3 d = new Vector3(-0.02f, 0, -0.02f);

            dat.Add(new CustomBlockData()
            {
                Vertex1 = new Vector3(0, 1, 0) - d,
                Vertex2 = new Vector3(s, 1, s) - d,
                Vertex3 = new Vector3(s, 0, s) - d,
                Vertex4 = new Vector3(0, 0, 0) - d,

                Normal = new Vector3(-1, 0, 1),

                TriFlip = true,
                DoubleSided = true,

                Texture = _tex
            }.CreateUVsYFlipped());

            dat.Add(new CustomBlockData()
            {
                Vertex1 = new Vector3(s, 1, 0),
                Vertex2 = new Vector3(0, 1, s),
                Vertex3 = new Vector3(0, 0, s),
                Vertex4 = new Vector3(s, 0, 0),

                Normal = new Vector3(-1, 0, -1),
                Texture = _tex
            }.CreateUVsYFlipped());

            dat.Add(new CustomBlockData()
            {
                Vertex1 = new Vector3(s, 1, 0),
                Vertex2 = new Vector3(0, 1, s),
                Vertex3 = new Vector3(0, 0, s),
                Vertex4 = new Vector3(s, 0, 0),

                TriFlip = true,

                Normal = new Vector3(1, 0, 1),
                Texture = _tex
            }.CreateUVsYFlipped());

            return dat;
        }
    }
}
