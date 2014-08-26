using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor
{
    class MetadataSolidTextureBlock : Block
    {
        private TextureBuilder _b;

        public MetadataSolidTextureBlock(string name, byte id, TextureBuilder builder)
        {
            _b = builder;
            Name = name;
            ID = id;
            UseMetadata = true;
        }

        public override string GetTextureForSide(BlockSide side, byte metadata)
        {
            return _b.GetTexturesForMetadata(metadata)[Block.GetSideInt(side)];
        }

        public override string[] GetTextures(byte metadata)
        {
            return _b.GetTexturesForMetadata(metadata);
        }
    }
}
