using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Wool : Block
    {
        public static TextureBuilder Textures = new TextureBuilder().AddForMetadata(0, "wool_colored_white").AddForMetadata(1, "wool_colored_orange").AddForMetadata(2, "wool_colored_magenta")
            .AddForMetadata(3, "wool_colored_light_blue").AddForMetadata(4, "wool_colored_yellow").AddForMetadata(5, "wool_colored_lime").AddForMetadata(6, "wool_colored_pink")
            .AddForMetadata(7, "wool_colored_gray").AddForMetadata(8, "wool_colored_silver").AddForMetadata(9, "wool_colored_cyan").AddForMetadata(10, "wool_colored_purple")
            .AddForMetadata(11, "wool_colored_blue").AddForMetadata(12, "wool_colored_brown").AddForMetadata(13, "wool_colored_green").AddForMetadata(14, "wool_colored_red")
            .AddForMetadata(15, "wool_colored_black");

        public Wool(byte id)
        {
            ID = id;
            Name = "Wool";
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
