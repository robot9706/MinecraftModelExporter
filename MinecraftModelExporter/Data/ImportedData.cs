using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.Data
{
    public class ImportedData
    {
        private uint[,,] _blockIDs;
        public uint[,,] BlockIDs
        {
            get { return _blockIDs; }
        }

        private byte[,,] _blockMetadatas;
        public byte[, ,] BlockMetadatas
        {
            get { return _blockMetadatas; }
        }

        private Point3 _modelSize;
        public Point3 ModelSize
        {
            get { return _modelSize; }
        }

        public ImportedData(Point3 modelSize)
        {
            _modelSize = modelSize;
            _blockIDs = new uint[modelSize.X, modelSize.Y, modelSize.Z];
            _blockMetadatas = new byte[modelSize.X, modelSize.Y, modelSize.Z];
        }

        #region ID
        public void SetBlockID(Point3 pos, uint id)
        {
            SetBlockID(pos.X, pos.Y, pos.Z, id);
        }

        public void SetBlockID(int x, int y, int z, uint id)
        {
            if (x < 0 || y < 0 || z < 0 || x >= _modelSize.X || y >= _modelSize.Y || z >= _modelSize.Z)
                throw new ArgumentOutOfRangeException("Invalid position");

            _blockIDs[x, y, z] = id;
        }
        #endregion

        #region Metadata
        public void SetBlockMetadata(Point3 pos, byte meta)
        {
            SetBlockMetadata(pos.X, pos.Y, pos.Z, meta);
        }

        public void SetBlockMetadata(int x, int y, int z, byte meta)
        {
            if (x < 0 || y < 0 || z < 0 || x >= _modelSize.X || y >= _modelSize.Y || z >= _modelSize.Z)
                throw new ArgumentOutOfRangeException("Invalid position");

            _blockMetadatas[x, y, z] = meta;
        }
        #endregion

        #region Both
        public void SetBlock(Point3 pos, uint id, byte meta)
        {
            SetBlock(pos.X, pos.Y, pos.Z, id, meta);
        }

        public void SetBlock(int x, int y, int z, uint id, byte meta)
        {
            if (x < 0 || y < 0 || z < 0 || x >= _modelSize.X || y >= _modelSize.Y || z >= _modelSize.Z)
                throw new ArgumentOutOfRangeException("Invalid position");

            _blockIDs[x, y, z] = id;
            _blockMetadatas[x, y, z] = meta;
        }
        #endregion
    }
}
