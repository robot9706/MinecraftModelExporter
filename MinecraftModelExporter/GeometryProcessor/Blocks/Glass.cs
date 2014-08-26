using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Glass : SolidBlock
    {
        public Glass(string name, byte id, byte mt, string[] tex)
            : base(name, id, mt, tex)
        {
        }

        public Glass(string name, byte id, string tex)
            : base(name, id, tex)
        {
        }

        public Glass(string name, byte id, string[] text)
            : base(name, id, text)
        {
        }

        public override bool IsTransparent()
        {
            return true;
        }
    }
}
