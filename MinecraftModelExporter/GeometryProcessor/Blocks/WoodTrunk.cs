using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class WoodTrunk : Block
    {
        public static TextureBuilder Textures = new TextureBuilder().AddForMetadata(0, new TextureMask().AddData("log_oak_top", BlockTexture.Ypos | BlockTexture.Yneg).AddData("log_oak", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures)
            .AddForMetadata(1, new TextureMask().AddData("log_spruce_top", BlockTexture.Ypos | BlockTexture.Yneg).AddData("log_spruce", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures)
            .AddForMetadata(2, new TextureMask().AddData("log_birch_top", BlockTexture.Ypos | BlockTexture.Yneg).AddData("log_birch", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures)
            .AddForMetadata(3, new TextureMask().AddData("log_jungle_top", BlockTexture.Ypos | BlockTexture.Yneg).AddData("log_jungle", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures)

            .AddForMetadata(4, new TextureMask().AddData("log_oak_top", BlockTexture.Xneg | BlockTexture.Xpos).AddData("log_oak_rot", BlockTexture.Yneg | BlockTexture.Ypos | BlockTexture.Zneg | BlockTexture.Zpos).Textures)
            .AddForMetadata(5, new TextureMask().AddData("log_spruce_top", BlockTexture.Xneg | BlockTexture.Xpos).AddData("log_spruce_rot", BlockTexture.Yneg | BlockTexture.Ypos | BlockTexture.Zneg | BlockTexture.Zpos).Textures)
            .AddForMetadata(6, new TextureMask().AddData("log_birch_top", BlockTexture.Xneg | BlockTexture.Xpos).AddData("log_birch_rot", BlockTexture.Yneg | BlockTexture.Ypos | BlockTexture.Zneg | BlockTexture.Zpos).Textures)
            .AddForMetadata(7, new TextureMask().AddData("log_jungle_top", BlockTexture.Xneg | BlockTexture.Xpos).AddData("log_jungle_rot", BlockTexture.Yneg | BlockTexture.Ypos | BlockTexture.Zneg | BlockTexture.Zpos).Textures)

            .AddForMetadata(8, new TextureMask().AddData("log_oak_top", BlockTexture.Zneg | BlockTexture.Zpos).AddData("log_oak", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Yneg | BlockTexture.Ypos).Textures)
            .AddForMetadata(9, new TextureMask().AddData("log_spuce_top", BlockTexture.Zneg | BlockTexture.Zpos).AddData("log_spruce", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Yneg | BlockTexture.Ypos).Textures)
            .AddForMetadata(10, new TextureMask().AddData("log_birch_top", BlockTexture.Zneg | BlockTexture.Zpos).AddData("log_birch", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Yneg | BlockTexture.Ypos).Textures)
            .AddForMetadata(11, new TextureMask().AddData("log_jungle_top", BlockTexture.Zneg | BlockTexture.Zpos).AddData("log_jungle", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Yneg | BlockTexture.Ypos).Textures)

            .AddForMetadata(12, new TextureMask().AddData("log_oak", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).AddData("log_oak_rot", BlockTexture.Yneg | BlockTexture.Ypos).Textures)
            .AddForMetadata(13, new TextureMask().AddData("log_spruce", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).AddData("log_spruce_rot", BlockTexture.Yneg | BlockTexture.Ypos).Textures)
            .AddForMetadata(14, new TextureMask().AddData("log_birch", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).AddData("log_birch_rot", BlockTexture.Yneg | BlockTexture.Ypos).Textures)
            .AddForMetadata(15, new TextureMask().AddData("log_jungle", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).AddData("log_jungle_rot", BlockTexture.Yneg | BlockTexture.Ypos).Textures);

        public WoodTrunk(byte id)
        {
            ID = id;
            Name = "Wood Logs";
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
