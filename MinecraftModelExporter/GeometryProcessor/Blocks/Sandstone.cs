using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Sandstone : Block
    {
        public static TextureBuilder Textures = new TextureBuilder().AddForMetadata(0, new TextureMask().AddData("sandstone_top", BlockTexture.Ypos).AddData("sandstone_normal", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).AddData("sandstone_bottom", BlockTexture.Yneg).Textures)
            .AddForMetadata(1, new TextureMask().AddData("sandstone_top", BlockTexture.Ypos).AddData("sandstone_top", BlockTexture.Yneg).AddData("sandstone_carved", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures)
            .AddForMetadata(2, new TextureMask().AddData("sandstone_top", BlockTexture.Ypos).AddData("sandstone_top", BlockTexture.Yneg).AddData("sandstone_smooth", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures);

        public Sandstone(byte id)
        {
            ID = id;
            Name = "Sandstone";
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
