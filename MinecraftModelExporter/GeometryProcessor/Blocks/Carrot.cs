using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Carrot : BlockCrop
    {
        public Carrot(byte id)
            : base(id)
        {
            Name = "Carrot";
        }

        public override string GetTextureForStage(byte stage)
        {
            if (stage == 0 || stage == 1)
                return "carrots_stage_0";
            if (stage == 2 || stage == 3)
                return "carrots_stage_1";
            if (stage == 4 || stage == 5 || stage == 6)
                return "carrots_stage_2";

            return "carrots_stage_3";
        }
    }
}
