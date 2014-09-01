using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Wheat : BlockCrop
    {
        public Wheat(byte id)
            : base(id)
        {
            Name = "Wheat";
        }

        public override string GetTextureForStage(byte stage)
        {
            return "wheat_stage_" + stage.ToString();    
        }
    }
}
