using MinecraftModelExporter.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.IO
{
    public class FileReader
    {
        public virtual ImportedData ReadFile(string filePath, TaskProgressReport progressReport)
        {
            return null;
        }
    }
}
