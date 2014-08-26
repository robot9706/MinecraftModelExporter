﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor
{
    class MetadataSolidBlock : Block
    {    
        public MetadataSolidBlock(string name, byte id, byte mt, string[] tex)
        {
            Name = name;
            ID = id;
            Metadata = mt;
            texture = tex;
            UseMetadata = true;

            if (texture.Length == 1)
            {
                UsesOneTexture = true;
                texture = new string[] { texture[0], texture[0], texture[0], texture[0], texture[0], texture[0], };
            }
            else
            {
                UsesOneTexture = false;
            }
        }

        public MetadataSolidBlock(string name, byte id, string tex)
        {
            Name = name;
            ID = id;
            Metadata = 0;
            UseMetadata = true;

            UsesOneTexture = true;
            texture = new string[] { tex, tex, tex, tex, tex, tex };
        }

        public MetadataSolidBlock(string name, byte id, string[] text)
        {
            Name = name;
            ID = id;
            texture = text;
            Metadata = 0;
            UseMetadata = true;

            if (texture.Length == 1)
            {
                UsesOneTexture = true;
                string tex = text[0];
                texture = new string[] { tex, tex, tex, tex, tex, tex };
            }
            else
            {
                UsesOneTexture = false;
            }
        }
    }
}
