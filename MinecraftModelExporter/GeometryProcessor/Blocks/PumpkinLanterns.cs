using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class PumpkinLanterns  : Block
    {
        public static TextureBuilder Textures = new TextureBuilder()
            /*south*/.AddForMetadata(2, new TextureMask().AddData("pumpkin_face_on", BlockTexture.Zneg).AddData("pumpkin_top", BlockTexture.Yneg | BlockTexture.Ypos).AddData("pumpkin_side", BlockTexture.Zpos | BlockTexture.Xneg | BlockTexture.Xpos).Textures)
            /*north*/ .AddForMetadata(0, new TextureMask().AddData("pumpkin_face_on", BlockTexture.Zpos).AddData("pumpkin_top", BlockTexture.Yneg | BlockTexture.Ypos).AddData("pumpkin_side", BlockTexture.Zneg | BlockTexture.Xneg | BlockTexture.Xpos).Textures)
            /*east*/.AddForMetadata(1, new TextureMask().AddData("pumpkin_face_on", BlockTexture.Xneg).AddData("pumpkin_top", BlockTexture.Yneg | BlockTexture.Ypos).AddData("pumpkin_side", BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures)
            /*west*/.AddForMetadata(3, new TextureMask().AddData("pumpkin_face_on", BlockTexture.Xpos).AddData("pumpkin_top", BlockTexture.Yneg | BlockTexture.Ypos).AddData("pumpkin_side", BlockTexture.Xneg | BlockTexture.Zneg | BlockTexture.Zpos).Textures)
            .AddForMetadata(4, new TextureMask().AddData("pumpkin_top", BlockTexture.Yneg | BlockTexture.Ypos).AddData("pumpkin_side", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).Textures);

        public PumpkinLanterns(byte id)
        {
            ID = id;
            Name = "Jack 'o' Lantern";
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
