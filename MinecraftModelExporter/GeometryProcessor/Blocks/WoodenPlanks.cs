using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class WoodenPlanks : Block
    {
        public static TextureBuilder Textures = new TextureBuilder().AddForMetadata(0, "planks_oak").AddForMetadata(1, "planks_spruce").AddForMetadata(2, "planks_birch").
            AddForMetadata(3, "planks_jungle");

        public WoodenPlanks(byte id)
        {
            ID = id;
            Name = "WoodenPlanks";
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
