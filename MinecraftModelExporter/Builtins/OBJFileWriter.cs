﻿using MinecraftModelExporter.API;
using MinecraftModelExporter.Data;
using MinecraftModelExporter.GeometryProcessor;
using MinecraftModelExporter.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.Builtins
{
    [FileWriterInfo(Author = "Robot9706", Name="OBJ", OutputFormat="obj")]
    public class OBJFileWriter : FileWriter
    {
        public override bool Write(string file, ProcessedGeometryData data)
        {
            int tr = 0;
            int svt = 0;

            StreamWriter sw = new StreamWriter(file, false, Encoding.ASCII);

            StreamWriter mtlSw = null;
            if (data.ExportConfig.ExportMaterials)
            {
                string mtlFile = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)) + ".mtl";
                mtlSw = new StreamWriter(mtlFile, false, Encoding.ASCII);
            }

            foreach (DataSet set in data.Data)
            {
                sw.WriteLine("o " + set.Texture); //Material name
                sw.WriteLine("usemtl " + set.Texture + "_mat");

                sw.WriteLine(" ");
                sw.WriteLine("#Vertices");
                for (int v = 0; v < set.verts.Count; v++)
                {
                    sw.WriteLine("v " + set.verts[v].RawToString());
                    tr++;
                }

                if (data.ExportConfig.ExportUVs)
                {
                    sw.WriteLine(" ");
                    sw.WriteLine("#UVs");
                    for (int t = 0; t < set.uvs.Count; t++)
                    {
                        sw.WriteLine("vt " + set.uvs[t].RawToString());
                    }
                }

                if (data.ExportConfig.ExportNormals)
                {
                    sw.WriteLine(" ");
                    sw.WriteLine("#Normals");
                    for (int n = 0; n < set.normals.Count; n++)
                    {
                        sw.WriteLine("vn " + set.normals[n].RawToString());
                    }
                }

                sw.WriteLine(" ");
                sw.WriteLine("#Faces");

                sw.WriteLine("g " + set.Texture); //Material name
                sw.WriteLine("s 1");
                for (int f = svt; f < tr; f += 3)
                {
                    int ind = f + 1;
                    int ind2 = f + 2;
                    int ind3 = f + 3;

                    Vector3 normal = set.normals[f - svt];
                    if (normal.X > 0 || normal.Y > 0 || normal.Z < 0)
                    {
                        ind = f + 1;
                        ind2 = f + 3;
                        ind3 = f + 2;
                    }

                    sw.WriteLine("f " + ind.ToString() + "/" + ind.ToString() + "/" + ind.ToString() + " " +
                        ind2.ToString() + "/" + ind2.ToString() + "/" + ind2.ToString() + " " +
                        ind3.ToString() + "/" + ind3.ToString() + "/" + ind3.ToString());
                }

                sw.WriteLine(" ");

                svt = tr;

                if (data.ExportConfig.ExportMaterials)
                {
                    WriteMTLEntry(mtlSw, set.Texture + "_mat", set.Texture + ".png");
                }
            }

            sw.Close();
            if (mtlSw != null)
                mtlSw.Close();
            
            return true;
        }

        private void WriteMTLEntry(StreamWriter mtl, string name, string tex)
        {
            mtl.WriteLine("newmtl " + name);
            mtl.WriteLine("Ns 100");
            mtl.WriteLine("d 1");
            mtl.WriteLine("illum 2");
            mtl.WriteLine("Kd 1 1 1");
            mtl.WriteLine("Ka 1 1 1");
            mtl.WriteLine("Ks 1 1 1");
            mtl.WriteLine("Ke 0 0 0");
            mtl.WriteLine("map_Kd " + tex);
            mtl.WriteLine(" ");
        }
    }
}
