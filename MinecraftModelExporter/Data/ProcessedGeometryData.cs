using MinecraftModelExporter.GeometryProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.Data
{
    public class ProcessedGeometryData
    {
        private List<DataSet> _data;
        public List<DataSet> Data
        {
            get { return _data; }
            set { _data = value; }
        }

        private ExportConfig _exportConfig;
        public ExportConfig ExportConfig
        {
            get { return _exportConfig; }
            set { _exportConfig = value; }
        }

        public ProcessedGeometryData()
        {
            _data = new List<DataSet>();
        }
    }
}
