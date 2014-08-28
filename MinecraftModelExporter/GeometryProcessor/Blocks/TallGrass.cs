using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class TallGrass : Block
    {
        private TextureBuilder Textures = new TextureBuilder().AddForMetadata(0, "deadbush").AddForMetadata(1, "tallgrass").AddForMetadata(2, "fern");

        public TallGrass(byte id, string name)
        {
            ID = id;
            Name = name;
            UseMetadata = true;
        }

        public override bool IsFullyCustomModel()
        {
            return true;
        }

        public override string GetTextureForSide(BlockSide side, byte metadata)
        {
            return Textures.GetTexturesForMetadata(metadata)[Block.GetSideInt(side)];
        }

        public override string[] GetTextures(byte metadata)
        {
            return Textures.GetTexturesForMetadata(metadata);
        }

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            List<CustomBlockData> dat = new List<CustomBlockData>();

            float s = 0.707f;
            Vector3 d = new Vector3(-0.02f, 0, -0.02f);

            dat.Add(new CustomBlockData()
            {
                Vertex1 = new Vector3(0, s, 0) - d,
                Vertex2 = new Vector3(s, s, s) - d,
                Vertex3 = new Vector3(s, 0, s) - d,
                Vertex4 = new Vector3(0, 0, 0) - d,

                Normal = new Vector3(1, 0, 1),
                Texture = Textures.GetTexturesForMetadata(metadata)[0]
            }.CreateUVsYFlipped());

            dat.Add(new CustomBlockData()
            {
                Vertex1 = new Vector3(0, s, 0) - d,
                Vertex2 = new Vector3(s, s, s) - d,
                Vertex3 = new Vector3(s, 0, s) - d,
                Vertex4 = new Vector3(0, 0, 0) - d,

                TriFlip = true,

                Normal = new Vector3(-1, 0, -1),
                Texture = Textures.GetTexturesForMetadata(metadata)[0]
            }.CreateUVsYFlipped());

            dat.Add(new CustomBlockData()
            {
                Vertex1 = new Vector3(s, s, 0),
                Vertex2 = new Vector3(0, s, s),
                Vertex3 = new Vector3(0, 0, s),
                Vertex4 = new Vector3(s, 0, 0),

                TriFlip = true,

                Normal = new Vector3(-1, 0, 1),
                Texture = Textures.GetTexturesForMetadata(metadata)[0]
            }.CreateUVsYFlipped());

            dat.Add(new CustomBlockData()
            {
                Vertex1 = new Vector3(s, s, 0),
                Vertex2 = new Vector3(0, s, s),
                Vertex3 = new Vector3(0, 0, s),
                Vertex4 = new Vector3(s, 0, 0),

                TriFlip = true,

                Normal = new Vector3(1, 0, -1),
                Texture = Textures.GetTexturesForMetadata(metadata)[0]
            }.CreateUVsYFlipped());

            return dat;
        }
    }
}
