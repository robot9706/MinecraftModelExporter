using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Stonebricks  : Block
    {
        public static TextureBuilder Textures = new TextureBuilder().AddForMetadata(0, "stonebrick").AddForMetadata(1, "stonebrick_mossy").AddForMetadata(2, "stonebrick_cracked").AddForMetadata(3, "stonebrick_carved");

        public Stonebricks(byte id)
        {
            ID = id;
            Name = "Stonebricks";
            UseMetadata = true;
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
