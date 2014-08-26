using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Furnace : Block
    {
        public static TextureBuilder Textures = new TextureBuilder().AddForMetadata(5, new TextureMask().AddData("furnace_front_off", BlockTexture.Xpos).AddData("furnace_side", BlockTexture.Xneg | BlockTexture.Zneg | BlockTexture.Zpos).AddData("furnace_top", BlockTexture.Yneg | BlockTexture.Ypos).Textures)
            .AddForMetadata(4, new TextureMask().AddData("furnace_front_off", BlockTexture.Xneg).AddData("furnace_side", BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).AddData("furnace_top", BlockTexture.Yneg | BlockTexture.Ypos).Textures)
            .AddForMetadata(3, new TextureMask().AddData("furnace_front_off", BlockTexture.Zpos).AddData("furnace_side", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg).AddData("furnace_top", BlockTexture.Yneg | BlockTexture.Ypos).Textures)
            .AddForMetadata(2, new TextureMask().AddData("furnace_front_off", BlockTexture.Zneg).AddData("furnace_side", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zpos).AddData("furnace_top", BlockTexture.Yneg | BlockTexture.Ypos).Textures);

        public Furnace(byte id)
        {
            ID = id;
            Name = "Furnace";
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
