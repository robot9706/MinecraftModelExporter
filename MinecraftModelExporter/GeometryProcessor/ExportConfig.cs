using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeometryProcessor
{
    public class ExportConfig
    {
        public bool ExportMaterials;
        public bool OptimizeModel;
        public bool CenterObject;
        public bool DontExportOuterFaces;
        public bool InteriorOnly;

        public bool ExportTextures;
        public string TextureOutputFolder;
        public string ResourcePack;

        public bool ExportNormals;
        public bool ExportUVs;
    }
}
