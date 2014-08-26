using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class QuartzBlock  : Block
    {
        public static TextureBuilder Textures = new TextureBuilder()
        .AddForMetadata(0, new TextureMask().AddData("quartz_block_bottom", BlockTexture.Yneg).AddData("quartz_block_top", BlockTexture.Ypos).AddData("quartz_block_side", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures)
        .AddForMetadata(1, new TextureMask().AddData("quartz_block_chiseled_top", BlockTexture.Ypos).AddData("quartz_block_bottom", BlockTexture.Yneg).AddData("quartz_block_side", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures);

        public QuartzBlock(byte id)
        {
            ID = id;
            Name = "Block of Quartz";
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
