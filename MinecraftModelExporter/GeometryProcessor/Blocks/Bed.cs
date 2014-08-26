using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Bed : Block
    {
        public Bed(byte id)
        {
            ID = id;
            UseMetadata = true;
            Name = "Bed";
        }

        public override bool IsTransparent()
        {
            return true;
        }

        public override string GetTextureForSide(BlockSide side, byte metadata)
        {
            return "bed_feet_top";
        }

        public override bool IsFullyCustomModel()
        {
            return true;
        }

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            List<CustomBlockData> d = new List<CustomBlockData>();

            bool head = BitHelper.IsBitSet(metadata, 0x8);

            bool[] bits = BitHelper.GetBits(metadata);
            int dat = BitHelper.Help(metadata, 0, 3);

            return d;
        }
    }
}
