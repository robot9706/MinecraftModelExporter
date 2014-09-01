using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Potato : BlockCrop
    {
        public Potato(byte id)
            : base(id)
        {
            Name = "Potato";
        }

        public override string GetTextureForStage(byte stage)
        {
            if (stage == 0 || stage == 1)
                return "potatoes_stage_0";
            if (stage == 2 || stage == 3)
                return "potatoes_stage_1";
            if (stage == 4 || stage == 5 || stage == 6)
                return "potatoes_stage_2";

            return "potatoes_stage_3";
        }
    }
}
