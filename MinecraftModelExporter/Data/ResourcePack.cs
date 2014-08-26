using ICSharpCode.SharpZipLib.Zip;
using MinecraftModelExporter.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.Data
{
    class ResourcePack
    {
        private string _file;

        private ZipFile _zip;
        private Dictionary<string, long> _blockTextures;

        public ResourcePack(string zipFile)
        {
            _blockTextures = new Dictionary<string, long>();
            _file = zipFile;
        }

        public void Open()
        {
            _blockTextures.Clear();

            if (string.IsNullOrEmpty(_file))
            {
                MemoryStream ms = new MemoryStream(Resources.DefaultTexturePack);
                _zip = new ZipFile(ms);
            }
            else
            {
                _zip = new ZipFile(_file);
            }

            foreach (ZipEntry e in _zip)
            {
                if (e.IsFile)
                {
                    string[] path = Path.GetDirectoryName(e.Name.ToLower()).Split(new string[] { @"\\", @"\", @"/" }, StringSplitOptions.RemoveEmptyEntries);
                    if (path.Length == 4 && path[0] == "assets" && path[1] == "minecraft" && path[2] == "textures" && path[3] == "blocks")
                    {
                        string name = Path.GetFileNameWithoutExtension(e.Name);
                        if (!string.IsNullOrEmpty(name))
                        {
                            _blockTextures.Add(name, e.ZipFileIndex);
                        }
                    }
                }
            }
        }

        public bool SaveBlockTexture(string textureName, string targetFolder)
        {
            if (_blockTextures.ContainsKey(textureName))
            {
                using (Stream stream = _zip.GetInputStream(_blockTextures[textureName]))
                { 
                    string outFile = Path.Combine(targetFolder, textureName + ".png");

                    using (FileStream fs = File.OpenWrite(outFile))
                    {
                        stream.CopyTo(fs);
                    }
                }

                return true;
            }

            return false;
        }

        public void Close()
        {
            _zip.Close();
        }

        public bool Check()
        {
            if (string.IsNullOrEmpty(_file))
                return true;

            bool assetsFound = false;
            bool mcFound = false;
            bool texturesFound = false;
            bool blocksFound = false;

            try
            {
                using (ZipFile zip = new ZipFile(_file))
                {
                    foreach(ZipEntry entry in zip)
                    {
                        if (entry.IsDirectory)
                        {
                            string name = Path.GetDirectoryName(entry.Name);
                            string[] path = name.Split(new string[] { @"\\", @"\", @"/" }, StringSplitOptions.None);

                            for (int x = 0; x < path.Length; x++)
                            {
                                switch (path[x].ToLower())
                                {
                                    case "assets":
                                        assetsFound = true;
                                        break;
                                    case "minecraft":
                                        mcFound = true;
                                        break;
                                    case "textures":
                                        texturesFound = true;
                                        break;
                                    case "blocks":
                                        blocksFound = true;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch
            { 
            }

            return (assetsFound && mcFound && texturesFound && blocksFound);
        }
    }
}
