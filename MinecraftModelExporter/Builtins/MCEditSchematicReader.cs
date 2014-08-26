using MinecraftModelExporter.API;
using MinecraftModelExporter.Data;
using MinecraftModelExporter.IO;
using NamedBinaryTag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MinecraftModelExporter.Builtins
{
    [FileReaderInfo(Name = "MCEdit schematic", Author = "Robot9706", SupportedFiles = new string[] { "schematic" })]
    class MCEditSchematicReader : FileReader
    {
        public override ImportedData ReadFile(string filePath, TaskProgressReport progressReport)
        {
            string name;

            TagCompound main = new TagCompound();
            progressReport.Report(0, "Opening NBT file");
            main.Load(filePath, out name);

            if (name.ToLower() == "schematic")
            {
                TagString mat = (TagString)main["Materials"];
                if (((string)mat.Value).ToLower() == "alpha")
                {
                    TagShort w = (TagShort)main["Width"];
                    TagShort h = (TagShort)main["Height"];
                    TagShort l = (TagShort)main["Length"];

                    progressReport.Report(0, "Resolving model type and size");

                    if (main["Blocks"] != null && main["Data"] != null)
                    {
                        Point3 size = new Point3((short)w.Value, (short)h.Value, (short)l.Value);
                        if (size.X <= 0 || size.Y <= 0 || size.Z <= 0)
                        {
                            throw new Exception("Invalid schematic size!");
                        }
                        else
                        {
                            progressReport.Report(0, "Loading data");

                            ImportedData data = new ImportedData(size);

                            TagByteArray blockIds = (TagByteArray)main["Blocks"];
                            byte[] ids = (byte[])blockIds.Value;

                            TagByteArray blockDatas = (TagByteArray)main["Data"];
                            byte[] datas = (byte[])blockDatas.Value;

                            int max = size.X * size.Y * size.Z;
                            int prog = 0;

                            for (int y = 0; y < size.Y; y++)
                            {
                                for (int z = 0; z < size.Z; z++)
                                {
                                    for (int x = 0; x < size.X; x++)
                                    {
                                        int offset = y * size.X * size.Z + z * size.X + x;

                                        data.SetBlock(x, y, z, ids[offset], datas[offset]);

                                        prog++;
                                        progressReport.Report((int)(((float)prog / (float)max) * 100f), "Loading data");
                                    }
                                }
                            }

                            return data;
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid or corrupted file, missing \"Blocks\" and \"Data\" tags!");
                    }
                }
                else
                {
                    throw new Exception("Minecraft classic schematics are not supported!");
                }
            }
            else
            {
                throw new Exception("Invalid NBT file!");
            }
        }
    }
}
