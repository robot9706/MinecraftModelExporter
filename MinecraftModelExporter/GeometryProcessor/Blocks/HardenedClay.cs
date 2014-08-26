using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class HardenedClay : Block
    {
        public static TextureBuilder Textures = new TextureBuilder().AddForMetadata(0, "hardened_clay").AddForMetadata(1, "hardened_clay_stained_orange").AddForMetadata(2, "hardened_clay_stained_magenta")
            .AddForMetadata(3, "hardened_clay_stained_light_blue").AddForMetadata(4, "hardened_clay_stained_yellow").AddForMetadata(5, "hardened_clay_stained_lime").AddForMetadata(6, "hardened_clay_stained_pink")
            .AddForMetadata(7, "hardened_clay_stained_gray").AddForMetadata(8, "hardened_clay_stained_silver").AddForMetadata(9, "hardened_clay_stained_cyan").AddForMetadata(10, "hardened_clay_stained_purple")
            .AddForMetadata(11, "hardened_clay_stained_blue").AddForMetadata(12, "hardened_clay_stained_brown").AddForMetadata(13, "hardened_clay_stained_green").AddForMetadata(14, "hardened_clay_stained_red")
            .AddForMetadata(15, "hardened_clay_stained_black");

        public HardenedClay(byte id)
        {
            ID = id;
            Name = "Hardened clay";
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
