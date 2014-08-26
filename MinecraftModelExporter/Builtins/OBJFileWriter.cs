using MinecraftModelExporter.API;
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
                mtlSw = new StreamWriter(file.Replace("obj", "mtl"), false, Encoding.ASCII);

            foreach (DataSet set in data.Data)
            {
                Block block = Block.Blocks[set.BaseData.GetGlobalID()];
                if (block != null)
                {
                    BlockTexture side = BlockTexture.AllSame;
                    switch (set.SideByte)
                    {
                        case 0:
                            side = BlockTexture.Xpos;
                            break;
                        case 1:
                            side = BlockTexture.Xneg;
                            break;

                        case 2:
                            side = BlockTexture.Ypos;
                            break;
                        case 3:
                            side = BlockTexture.Yneg;
                            break;

                        case 4:
                            side = BlockTexture.Zpos;
                            break;
                        case 5:
                            side = BlockTexture.Zneg;
                            break;
                    }

                    sw.WriteLine("o " + block.Name + "_" + side.ToString()); //Material name
                    sw.WriteLine("usemtl " + block.Name + "_" + side.ToString() + "_mat");

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

                    BlockSide ssd = (BlockSide)Enum.Parse(typeof(BlockSide), side.ToString());

                    string tex = block.GetTextureForSide(ssd, set.BaseData.Metadata);

                    sw.WriteLine("g " + tex); //Material name
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
                        WriteMTLEntry(mtlSw, block.Name + "_" + side.ToString() + "_mat", tex + ".png");
                    }
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
