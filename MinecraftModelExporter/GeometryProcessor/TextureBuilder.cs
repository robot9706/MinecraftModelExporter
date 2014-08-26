using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor
{
    class TextureBuilder
    {
        private Dictionary<byte, string[]> _dats = new Dictionary<byte, string[]>();

        public TextureBuilder AddForMetadata(byte mt, string[] tex)
        {
            if (tex.Length != 6)
                throw new Exception("6 textures needed!");

            _dats.Add(mt, tex);

            return this;
        }

        public TextureBuilder AddForMetadata(byte mt, string tex)
        {
            _dats.Add(mt, new string[]{tex,tex,tex,tex,tex,tex});

            return this;
        }

        public string[] GetTexturesForMetadata(byte mt)
        {
            if (!_dats.ContainsKey(mt))
                return new string[] { "stone", "stone", "stone", "stone", "stone", "stone" };
                //throw new Exception("Textures for metadata " + mt.ToString() + " is not added!");

            return _dats[mt];
        }
    }

    class TextureMask
    {
        public string[] Textures = new string[6];

        public TextureMask AddData(string texture, BlockTexture side)
        {
            if (side.HasFlag(BlockTexture.Xneg))
            {
                Textures[Block.GetSideInt(BlockSide.Xneg)] = texture;
            }
            if (side.HasFlag(BlockTexture.Xpos))
            {
                Textures[Block.GetSideInt(BlockSide.Xpos)] = texture;
            }

            if (side.HasFlag(BlockTexture.Yneg))
            {
                Textures[Block.GetSideInt(BlockSide.Yneg)] = texture;
            }
            if (side.HasFlag(BlockTexture.Ypos))
            {
                Textures[Block.GetSideInt(BlockSide.Ypos)] = texture;
            }

            if (side.HasFlag(BlockTexture.Zneg))
            {
                Textures[Block.GetSideInt(BlockSide.Zneg)] = texture;
            }
            if (side.HasFlag(BlockTexture.Zpos))
            {
                Textures[Block.GetSideInt(BlockSide.Zpos)] = texture;
            }

            return this;
        }
    }
}
