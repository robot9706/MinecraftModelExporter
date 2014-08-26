using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Dropper  : Block
    {
        public static TextureBuilder Textures = new TextureBuilder().AddForMetadata(0, new TextureMask().AddData("furnace_side", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).AddData("dropper_front_vertical", BlockTexture.Yneg).AddData("furnace_top", BlockTexture.Ypos).Textures).
            AddForMetadata(1, new TextureMask().AddData("furnace_side", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).AddData("dropper_front_vertical", BlockTexture.Ypos).AddData("furnace_top", BlockTexture.Yneg).Textures)
            .AddForMetadata(3, new TextureMask().AddData("dropper_front_horizontal", BlockTexture.Zneg).AddData("furnace_top", BlockTexture.Yneg | BlockTexture.Ypos).AddData("furnace_side", BlockTexture.Zpos | BlockTexture.Xneg | BlockTexture.Xpos).Textures)
            .AddForMetadata(2, new TextureMask().AddData("dropper_front_horizontal", BlockTexture.Zpos).AddData("furnace_top", BlockTexture.Yneg | BlockTexture.Ypos).AddData("furnace_side", BlockTexture.Zneg | BlockTexture.Xneg | BlockTexture.Xpos).Textures)
            .AddForMetadata(5, new TextureMask().AddData("dropper_front_horizontal", BlockTexture.Xneg).AddData("furnace_top", BlockTexture.Yneg | BlockTexture.Ypos).AddData("furnace_side", BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures)
            .AddForMetadata(4, new TextureMask().AddData("dropper_front_horizontal", BlockTexture.Xpos).AddData("furnace_top", BlockTexture.Yneg | BlockTexture.Ypos).AddData("furnace_side", BlockTexture.Xneg | BlockTexture.Zneg | BlockTexture.Zpos).Textures);

        public Dropper(byte id)
        {
            ID = id;
            Name = "Dropper";
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
