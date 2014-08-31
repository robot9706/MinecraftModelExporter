using MinecraftModelExporter.GeomGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    //east: +x
    //west: -x
    //south: +z
    //north: -z

    class Stairs : Block
    {
        private string _texture;
        private string _textureY;

        public Stairs(byte id, string texture)
        {
            ID = id;
            _texture = texture;
            _textureY = texture;
            UseMetadata = true;
            Name = "Stairs";
        }

        public Stairs(byte id, string texture, string textureY)
        {
            ID = id;
            _texture = texture;
            _textureY = textureY;
            UseMetadata = true;
            Name = "Stairs";
        }

        public override bool IsFullyCustomModel()
        {
            return true;
        }

        private byte GetType(byte metadata)
        {
            return (byte)BitHelper.Help(metadata, 6, 2);
        }

        private bool IsUpsideDown(byte metadata)
        {
            return BitHelper.IsBitSet(metadata, 2);
        }

        public override string GetTextureForSide(BlockSide side, byte metadata)
        {
            if (side == BlockSide.Yneg || side == BlockSide.Ypos)
                return _textureY;

            return _texture;
        }

        public override string[] GetTextures(byte metadata)
        {
            return new string[] { _texture, _texture, _textureY, _textureY, _texture, _texture };
        }

        public override bool IsTransparent()
        {
            return true;
        }

        private Vector3 MetaToDirection(byte meta)
        {
            switch (meta) //Inverted values, because we need descending directions
            { 
                case 0: //East
                    return new Vector3(-1,0,0);
                case 1: //West
                    return new Vector3(1, 0, 0);

                case 2: //South
                    return new Vector3(0, 0, -1);
                case 3: //North
                    return new Vector3(0, 0, 1);
            }

            return new Vector3(0, 0, 0);
        }

        private bool IsLeft(byte from, byte to)
        {
            if (from == 0 && to == 3)
                return true;
            if (from == 0 && to == 2)
                return true;

            if (from == 1 && to == 2)
                return true;
            if (from == 1 && to == 3)
                return true;

            if (from == 2 && to == 0)
                return true;
            if (from == 2 && to == 1)
                return true;

            if (from == 3 && to == 1)
                return true;
            if (from == 3 && to == 0)
                return true;

            //Vector3 a = MetaToDirection(from);
            //Vector3 b = MetaToDirection(to);

            //a = Vector3.TransformNormal(a, Matrix.CreateRotationY(-(float)Math.PI / 2));
            //a.X = (float)Math.Round(a.X, 4);
            //a.Z = (float)Math.Round(a.Z, 4);

            //return (a == b);

            return false;
        }

        private int CheckLittleCorner(BlockData me, BlockData backBlock, BlockData leftBlock, BlockData rightBlock, byte direction)
        {
            if (IsStairs(backBlock.ID))
            {
                bool leftOk = (IsStairs(leftBlock.ID) && (leftBlock.Metadata == me.Metadata || IsLeft(me.Metadata, leftBlock.Metadata)));
                bool rightOk = (IsStairs(rightBlock.ID) && (rightBlock.Metadata == me.Metadata || IsLeft(me.Metadata, rightBlock.Metadata)));

                if (leftOk && rightOk)
                {
                    if (leftBlock.Metadata == me.Metadata && rightBlock.Metadata != me.Metadata)
                        rightOk = false;
                    else if (leftBlock.Metadata != me.Metadata && rightBlock.Metadata == me.Metadata)
                        leftOk = false;
                }

                if ((leftOk && !rightOk) || (!leftOk && rightOk))
                {
                    switch (direction)
                    {
                        case 0:
                            if (leftOk)
                                return 0; //2
                            else
                                return 3; //1
                            break;

                        case 1:
                            if (leftOk)
                                return 2; //0
                            else
                                return 1; //3
                            break;

                        case 2:
                            if (leftOk)
                                return 3; //1
                            else
                                return 2; //0
                            break;

                        case 3:
                            if (leftOk)
                                return 1; //3
                            else
                                return 0; //2
                            break;
                    }
                }
            }

            return -1;
        }

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            bool upsideDown = IsUpsideDown(metadata);

            List<BoundingBox> boxesToExport = new List<BoundingBox>();

            byte direction = GetType(metadata);

            float bottomBasePos = (upsideDown ? 0.5f : 0f);
            float topBasePos = (upsideDown ? 0f : 0.5f);

            boxesToExport.Add(new BoundingBox(new Vector3(0, bottomBasePos, 0), new Vector3(1, bottomBasePos + 0.5f, 1)));

            //Collect data from environment
            Vector3 descendingDir = MetaToDirection(direction);
            Vector3 rightFloatDir = Vector3.TransformNormal(descendingDir, Matrix.CreateRotationY((float)Math.PI / 2));
            rightFloatDir.X = (float)Math.Round(rightFloatDir.X, 4); rightFloatDir.Z = (float)Math.Round(rightFloatDir.Z, 4); //Fix small numbers

            Point3 facingDir = descendingDir.ToPoint3();
            Point3 rightDir = rightFloatDir.ToPoint3();

            BlockData frontBlock = source.GetData(blockPosition + facingDir);
            BlockData backBlock = source.GetData(blockPosition - facingDir);
            BlockData rightBlock = source.GetData(blockPosition + rightDir);
            BlockData leftBlock = source.GetData(blockPosition - rightDir);

            bool renderBaseStep = true;

            int littleCornerModel = -1;

            //Check if we need to make a corner
            {
                if (IsStairs(frontBlock.ID))
                {
                    if (IsLeft(me.Metadata, frontBlock.Metadata))
                    {
                        bool leftOk = (IsStairs(leftBlock.ID) && me.Metadata == leftBlock.Metadata);
                        bool rightOk = (IsStairs(rightBlock.ID) && me.Metadata == rightBlock.Metadata);

                        if ((leftOk && !rightOk) || (!leftOk && rightOk))
                        {
                            int frontLittleStep = -1;
                            {
                                Point3 frontPos = blockPosition + facingDir;
                                
                                BlockData me2 = source.GetData(frontPos);
                                byte direction2 = GetType(me2.Metadata);

                                Vector3 descendingDir2 = MetaToDirection(direction2);
                                Vector3 rightFloatDir2 = Vector3.TransformNormal(descendingDir2, Matrix.CreateRotationY((float)Math.PI / 2));
                                rightFloatDir2.X = (float)Math.Round(rightFloatDir2.X, 4); rightFloatDir2.Z = (float)Math.Round(rightFloatDir2.Z, 4); //Fix small numbers

                                Point3 facingDir2 = descendingDir2.ToPoint3();
                                Point3 rightDir2 = rightFloatDir2.ToPoint3();

                                BlockData backBlock2 = source.GetData(frontPos - facingDir2);
                                BlockData rightBlock2 = source.GetData(frontPos + rightDir2);
                                BlockData leftBlock2 = source.GetData(frontPos - rightDir2);

                                frontLittleStep = CheckLittleCorner(me2, backBlock2, leftBlock2, rightBlock2, direction2);
  
                            }

                            if (frontLittleStep == -1)
                            {
                                switch (direction)
                                {
                                    case 0:
                                        if (leftOk)
                                            littleCornerModel = 2;
                                        else
                                            littleCornerModel = 1;
                                        break;

                                    case 1:
                                        if (leftOk)
                                            littleCornerModel = 0;
                                        else
                                            littleCornerModel = 3;
                                        break;

                                    case 2:
                                        if (leftOk)
                                            littleCornerModel = 1;
                                        else
                                            littleCornerModel = 0;
                                        break;

                                    case 3:
                                        if (leftOk)
                                            littleCornerModel = 3;
                                        else
                                            littleCornerModel = 2;
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            //Check if we need to make a small corner
            if (littleCornerModel == -1)  //It's not a corner so maybe..
            {
                littleCornerModel = CheckLittleCorner(me, backBlock, leftBlock, rightBlock, direction);
                renderBaseStep = (littleCornerModel == -1);
            } 

            if (littleCornerModel != -1)
            {
                switch (littleCornerModel)
                {
                    case 0:
                        boxesToExport.Add(new BoundingBox(new Vector3(.5f, topBasePos, 0), new Vector3(1f, topBasePos + 0.5f, .5f)));
                        break;
                    case 1:
                        boxesToExport.Add(new BoundingBox(new Vector3(0, topBasePos, 0), new Vector3(.5f, topBasePos + 0.5f, .5f)));
                        break;

                    case 2:
                        boxesToExport.Add(new BoundingBox(new Vector3(0, topBasePos, 0.5f), new Vector3(.5f, topBasePos + 0.5f, 1)));
                        break;
                    case 3:
                        boxesToExport.Add(new BoundingBox(new Vector3(.5f, topBasePos, .5f), new Vector3(1f, topBasePos + 0.5f, 1f)));
                        break;
                }
            }

            if (renderBaseStep)
            {
                switch (direction) //Ascending...
                {
                    case 0: //east = +X
                        boxesToExport.Add(new BoundingBox(new Vector3(.5f, topBasePos, 0), new Vector3(1f, topBasePos + 0.5f, 1)));
                        break;
                    case 1: //west = -X
                        boxesToExport.Add(new BoundingBox(new Vector3(0, topBasePos, 0), new Vector3(.5f, topBasePos + 0.5f, 1)));
                        break;

                    case 2: //south = +Z
                        boxesToExport.Add(new BoundingBox(new Vector3(0, topBasePos, 0.5f), new Vector3(1f, topBasePos + 0.5f, 1)));
                        break;
                    case 3: //north = -Z
                        boxesToExport.Add(new BoundingBox(new Vector3(0, topBasePos, 0), new Vector3(1f, topBasePos + 0.5f, .5f)));
                        break;
                }
            }

            GeometryGenerator geomGen = new GeometryGenerator();

            return geomGen.GenerateModel(boxesToExport, source, blockPosition, _texture, _textureY, CanBuildSide);
        }

        private bool IsStairs(uint id)
        {
            return (id == 53 || id == 67 || id == 108 || id == 109 || id == 114 || id == 128 || id == 134 || id == 135 || id == 136 || id == 156);
        }

        bool CanBuildSide(BlockData data, BlockData me)
        {
            if (Block.Blocks[data.GetGlobalID()] == null)
                return true;

            if (Block.Blocks[data.GetGlobalID()].IsTransparent())
            {
                if (IsStairs(data.ID))
                {
                    bool ud = IsUpsideDown(data.Metadata);
                    bool od = IsUpsideDown(me.Metadata);

                    if (ud != od)
                    {
                        if (GetType(me.Metadata) == GetType(data.Metadata))
                            return true;
                        else
                            return false;
                    }
                    return false;
                }

                return true;
            }

            return true;
        }
    }
}
