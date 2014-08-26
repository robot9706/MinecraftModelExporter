using MinecraftModelExporter.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.IO
{
    public class FileWriter
    {
        public virtual bool Write(string file, ProcessedGeometryData data)
        {
            return false;
        }
    }
}
