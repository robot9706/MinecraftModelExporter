using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Leaves : Block
    {
        public static TextureBuilder Textures = new TextureBuilder().AddForMetadata(0, "leaves_oak").AddForMetadata(1, "leaves_spruce").AddForMetadata(2, "leaves_birch").
            AddForMetadata(3, "leaves_jungle");

        public Leaves(byte id)
        {
            ID = id;
            Name = "Leaves";
            UseMetadata = true;
        }

        public override bool IsTransparent()
        {
            return true;
        }

        public override string GetTextureForSide(BlockSide side, byte metadata)
        {
            return Textures.GetTexturesForMetadata(metadata)[GetSideInt(side)];
        }

        public override string[] GetTextures(byte metadata)
        {
            return Textures.GetTexturesForMetadata(metadata);
        }
    }
}
