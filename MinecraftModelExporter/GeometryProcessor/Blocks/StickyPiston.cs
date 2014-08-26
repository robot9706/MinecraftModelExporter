using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class StickyPiston : Block
    {
        private TextureBuilder Textures = new TextureBuilder().AddForMetadata(0, new TextureMask().AddData("piston_bottom", BlockTexture.Ypos).AddData("piston_side_rot2", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).AddData("piston_top_sticky", BlockTexture.Yneg).Textures)
            .AddForMetadata(1, new TextureMask().AddData("piston_bottom", BlockTexture.Yneg).AddData("piston_side", BlockTexture.Xneg | BlockTexture.Xpos | BlockTexture.Zneg | BlockTexture.Zpos).AddData("piston_top_sticky", BlockTexture.Ypos).Textures)
            .AddForMetadata(3, new TextureMask().AddData("piston_top_sticky", BlockTexture.Zpos).AddData("piston_bottom", BlockTexture.Zneg).AddData("piston_side", BlockTexture.Yneg | BlockTexture.Ypos).AddData("piston_side_rot", BlockTexture.Xneg | BlockTexture.Xpos).Textures)
            .AddForMetadata(2, new TextureMask().AddData("piston_top_sticky", BlockTexture.Zneg).AddData("piston_bottom", BlockTexture.Zpos).AddData("piston_side_rot2", BlockTexture.Yneg | BlockTexture.Ypos).AddData("piston_side_rot3", BlockTexture.Xneg | BlockTexture.Xpos).Textures)
            .AddForMetadata(5, new TextureMask().AddData("piston_top_sticky", BlockTexture.Xpos).AddData("piston_bottom", BlockTexture.Xneg).AddData("piston_side_rot", BlockTexture.Yneg | BlockTexture.Ypos | BlockTexture.Zneg | BlockTexture.Zpos).Textures)
            .AddForMetadata(4, new TextureMask().AddData("piston_top_sticky", BlockTexture.Xneg).AddData("piston_bottom", BlockTexture.Xpos).AddData("piston_side_rot3", BlockTexture.Yneg | BlockTexture.Ypos | BlockTexture.Zneg | BlockTexture.Zpos).Textures);

        public StickyPiston(byte id, string name)
        {
            ID = id;
            Name = name;
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
