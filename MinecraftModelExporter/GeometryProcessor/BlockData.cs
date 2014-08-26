using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeometryProcessor
{
    public class BlockData
    {
        public uint ID;
        public byte Metadata;

        public bool IsAir
        {
            get { return (ID == 0); }
        }

        public bool IsSolid
        {
            get { if (IsAir) return false; return (!Block.Blocks[GetGlobalID()].IsTransparent()); }
        }

        public BlockData(uint Id, byte mt)
        {
            ID = Id;
            Metadata = mt;
        }

        public uint GetGlobalID()
        {
            return GetGlobalID(ID, Metadata);
        }

        public static uint GetGlobalID(uint ID, byte Metadata)
        {
            return (uint)((ID * Byte.MaxValue) + Metadata);
        }

        public bool EqualsFull(BlockData bd)
        {
            if (bd == null)
                return false;

            return (ID == bd.ID && Metadata == bd.Metadata);
        }

        public bool EqualsID(BlockData bd)
        {
            if (bd == null)
                return false;

            return (ID == bd.ID);
        }
    }
}
