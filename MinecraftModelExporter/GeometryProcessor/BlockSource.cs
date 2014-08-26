using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor
{
    interface BlockSource
    {
        BlockData GetData(Point3 position);
    }
}
