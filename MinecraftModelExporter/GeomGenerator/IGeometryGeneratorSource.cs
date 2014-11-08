using MinecraftModelExporter.GeometryProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeomGenerator
{
    interface IGeometryGeneratorSource
    {
        bool CanBuildSide(BlockData me, BlockData side, Point3 mePos, Point3 sidePos);
        string GetTexture(Face face);
        Vector2[] GetUVsForTriangle(Vector2[] source, Face face);
    }
}
