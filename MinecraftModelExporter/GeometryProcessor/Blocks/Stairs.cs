using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    //east: -x
    //west: +x
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

        private Vector3 RotY(Vector3 a, float y)
        {
            return RotY(a, new Vector3(0, 0, 0), y);
        }

        private Vector3 RotY(Vector3 a, Vector3 c, float y)
        {
            Vector2 p = new Vector2(a.X, a.Z);
            Vector2 n = Vector2.RotateAround(p, new Vector2(c.X, c.Z), y);

            return new Vector3(n.X, a.Y, n.Y);
        }

        private bool Check(byte a, byte b)
        {
            int ia = (int)a;
            int ib = (int)b;

            if (ia == ib)
                return true;

            ia = (ia + 1) % 4;
            if (ia == ib)
                return true;

            ia = (ia - 2);
            if (ia < 0)
                ia = 4 - ia;

            return (ia == ib);
        }

        private Vector3 MetaToDir(byte meta)
        {
            switch (meta)
            {
                case 0:
                    return new Vector3(-1, 0, 0);
                case 1:
                    return new Vector3(1, 0, 0);
                case 2:
                    return new Vector3(0, 0, -1);
                case 3:
                    return new Vector3(0, 0, 1);
            }

            return new Vector3(0, 0, 0);
        }

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            List<CustomBlockData> l = new List<CustomBlockData>();

            CustomBlockData front1 = null;
            CustomBlockData front2 = null;

            CustomBlockData back = null;
            CustomBlockData cornerBack = null;

            CustomBlockData sidepa = null;
            CustomBlockData sidena = null;

            CustomBlockData sidepb = null;
            CustomBlockData sidenb = null;

            CustomBlockData connectPart = null;

            #region Convert metadata to actual info 
            byte direction = GetType(metadata);
            bool upsideDown = IsUpsideDown(metadata);

            Vector3 descendingDir = new Vector3(0,0,0);
            float degDirection = 0;
            switch (direction)
            { 
                case 0:
                    descendingDir = new Vector3(-1, 0, 0);
                    degDirection = 0;
                    break;
                case 1:
                    descendingDir = new Vector3(1, 0, 0);
                    degDirection = 180;
                    break;
                case 2:
                    descendingDir = new Vector3(0, 0, -1);
                    degDirection = 270;
                    break;
                case 3:
                    descendingDir = new Vector3(0, 0, 1);
                    degDirection = 90;
                    break;
            }
            #endregion

            #region Face culling
            Vector3 frontDir = descendingDir;
            Vector3 backDir = -descendingDir;
            Vector3 leftDir = RotY(descendingDir, 90);
            Vector3 rightDir = RotY(descendingDir, -90);
            Vector3 upDir = new Vector3(0, (upsideDown ? -1 : 1), 0);
            Vector3 downDir = -upDir;

            BlockData frontBlock = source.GetData(blockPosition + frontDir.ToPoint3());
            BlockData backBlock = source.GetData(blockPosition + backDir.ToPoint3());
            BlockData leftBlock = source.GetData(blockPosition + leftDir.ToPoint3());
            BlockData rightBlock = source.GetData(blockPosition + rightDir.ToPoint3());
            BlockData topBlock = source.GetData(blockPosition + upDir.ToPoint3());
            BlockData downBlock = source.GetData(blockPosition + downDir.ToPoint3());

            bool leftStairs = false;
            bool rightStairs = false;

            bool useCornerModel = false;

            if (IsStairs(leftBlock.ID))
            {
                byte type = GetType(leftBlock.Metadata);
                if (type == direction)
                {
                    leftStairs = true;
                }
            }

            if (IsStairs(rightBlock.ID))
            {
                byte type = GetType(rightBlock.Metadata);
                if (type == direction)
                {
                    rightStairs = true;
                }
            }

            if (IsStairs(frontBlock.ID))
            {
                useCornerModel = ((leftStairs && !rightStairs) || (!leftStairs && rightStairs));
            }

            bool useLittleCornerModel = false;
            if (!useCornerModel)
            {
                if (IsStairs(backBlock.ID))
                {
                    leftStairs = false;
                    if (IsStairs(leftBlock.ID))
                    {
                        byte type = GetType(leftBlock.Metadata);
                        //if (type == direction || Check(type, direction))
                        {
                            leftStairs = true;
                        }
                    }

                    rightStairs = false;
                    if (IsStairs(rightBlock.ID))
                    {
                        byte type = GetType(rightBlock.Metadata);
                        //if (type == direction || Check(type, direction))
                        {
                            rightStairs = true;
                        }
                    }

                    byte backBlockDir = GetType(backBlock.Metadata);

                    if (!leftStairs && rightStairs)
                    {
                        byte rightType = GetType(rightBlock.Metadata);
                        //if (rightType == direction || Check(rightType, direction))
                        {
                            useLittleCornerModel = true;
                        }
                    }
                    else if (leftStairs && !rightStairs)
                    {
                        byte leftType = GetType(leftBlock.Metadata);
                        //if (leftType == direction || Check(leftType, direction))
                        {
                            useLittleCornerModel = true;
                        }
                    }

                    if (!useLittleCornerModel)
                    {
                        if (backBlockDir == direction && ((leftStairs && !rightStairs) || (!leftStairs && rightStairs)))
                        {
                            useLittleCornerModel = true;
                        }
                    }

                    if (leftStairs && rightStairs)
                        useLittleCornerModel = false;
                }
            }
            #endregion

            #region Corner model facing
            if (useCornerModel)
            {
                if (rightStairs)
                    degDirection += 90;
            }

            if (useLittleCornerModel)
            {
                if (rightStairs)
                    degDirection -= 90;
            }

            if (degDirection < 0)
                degDirection += 360;
            if (degDirection >= 360)
                degDirection = degDirection % 360;
            #endregion

            #region Build the model
            //Bottom
            if (CanBuildSide(downBlock, me))
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, 0, 0),
                    Vertex2 = new Vector3(1, 0, 0),
                    Vertex3 = new Vector3(1, 0, 1),
                    Vertex4 = new Vector3(0, 0, 1),

                    Normal = new Vector3(0, -1, 0),
                    Texture = _textureY
                }.CreateUVs());
            }

            //Top-1
            if (CanBuildSide(topBlock, me))
            {
                float top1Size = (useLittleCornerModel ? .5f : 1);

                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(.5f, 1, 0),
                    Vertex2 = new Vector3(1f, 1, 0),
                    Vertex3 = new Vector3(1f, 1, top1Size),
                    Vertex4 = new Vector3(.5f, 1, top1Size),

                    Normal = new Vector3(0, 1, 0),
                    Texture = _textureY
                }.CreateUVs(.5f, 0, 1, top1Size));
            }

            //Top-2(lower)
            float top2Size = (useCornerModel ? 0.5f : 1f);
            l.Add(new CustomBlockData()
            {
                Vertex1 = new Vector3(0, .5f, 0),
                Vertex2 = new Vector3(.5f, .5f, 0),
                Vertex3 = new Vector3(.5f, .5f, top2Size),
                Vertex4 = new Vector3(0, .5f, top2Size),

                Normal = new Vector3(0, 1, 0),
                Texture = _textureY
            }.CreateUVs(0, 0, .5f, top2Size));

            //Back
            if (CanBuildSide(backBlock, me))
            {
                back = (new CustomBlockData()
                {
                    Vertex1 = new Vector3(1, 0, 0),
                    Vertex2 = new Vector3(1, 1, 0),
                    Vertex3 = new Vector3(1, 1, 1),
                    Vertex4 = new Vector3(1, 0, 1),

                    Normal = new Vector3(1, 0, 0),
                    Texture = _texture,
                    TriFlip = true
                }.CreateUVs().RotateUVs(90));
                l.Add(back);
            }

            //Front-bottom
            if (CanBuildSide(frontBlock, me))
            {
                front1 = new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, 0, 0),
                    Vertex2 = new Vector3(0, .5f, 0),
                    Vertex3 = new Vector3(0, .5f, 1),
                    Vertex4 = new Vector3(0, 0, 1),

                    Normal = new Vector3(-1, 0, 0),
                    Texture = _texture,
                    TriFlip = true,
                }.CreateUVs(0, 0, 0.5f, 1f).RotateUVs(90);
                l.Add(front1);
            }

            //Front-top
            float frontTopSize = ((useCornerModel || useLittleCornerModel) ? 0.5f : 1f);
            front2 = new CustomBlockData()
            {
                Vertex1 = new Vector3(.5f, .5f, 0),
                Vertex2 = new Vector3(.5f, 1, 0),
                Vertex3 = new Vector3(.5f, 1, frontTopSize),
                Vertex4 = new Vector3(.5f, .5f, frontTopSize),

                Normal = new Vector3(-1, 0, 0),
                Texture = _texture,

                TriFlip = true,
            }.CreateUVs(0, 0, 0.5f, frontTopSize).RotateUVs(90);
            l.Add(front2);

            //Front-top3 & Xpos
            if (useCornerModel)
            {
                l.Add(new CustomBlockData()
                {
                    Normal = new Vector3(0, 1, 0),
                    Texture = _texture,

                    Vertex1 = new Vector3(0f, 1f, .5f),
                    Vertex2 = new Vector3(.5f, 1f, .5f),
                    Vertex3 = new Vector3(.5f, 1f, 1f),
                    Vertex4 = new Vector3(0f, 1f, 1f),
                }.CreateUVs(0, 0, .5f, .5f));

                connectPart = (new CustomBlockData()
                {
                    Normal = new Vector3(0,0,-1),
                    Texture = _texture,

                    Vertex1 = new Vector3(0, .5f, 0.5f),
                    Vertex2 = new Vector3(0.5f, .5f, .5f),
                    Vertex3 = new Vector3(0.5f, 1f, .5f),
                    Vertex4 = new Vector3(0f, 1f, 0.5f),
                }.CreateUVs(0, 0, .5f, .5f));
                l.Add(connectPart);
            }

            //Top-3(lower)
            if (useLittleCornerModel)
            {
                l.Add(new CustomBlockData()
                {
                    Texture = _texture,
                    Normal = new Vector3(0, 1, 0),

                    Vertex1 = new Vector3(.5f, .5f, .5f),
                    Vertex2 = new Vector3(1f, .5f, .5f),
                    Vertex3 = new Vector3(1f, .5f, 1f),
                    Vertex4 = new Vector3(.5f, .5f, 1f),
                }.CreateUVs(0, 0, .5f, .5f));
            }
            
            //Zpos
            if (useCornerModel)
            {
                cornerBack = (new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, 0, 1),
                    Vertex2 = new Vector3(1, 0, 1),
                    Vertex3 = new Vector3(1, 1, 1),
                    Vertex4 = new Vector3(0, 1, 1),

                    Normal = new Vector3(0, 0, 1),
                    Texture = _texture,
                    TriFlip = true,
                }.CreateUVs());

                l.Add(cornerBack);
            }
            else if (CanBuildSide(rightBlock, me) || useLittleCornerModel)
            {
                float side1Pos = (useLittleCornerModel ? 0.5f : 1f);
                sidepa = (new CustomBlockData()
                {
                    Vertex1 = new Vector3(.5f, .5f, side1Pos),
                    Vertex2 = new Vector3(.5f, 1, side1Pos),
                    Vertex3 = new Vector3(1, 1, side1Pos),
                    Vertex4 = new Vector3(1, .5f, side1Pos),

                    Normal = new Vector3(0, 0, 1),
                    Texture = _texture,
                    TriFlip = true,

                    UV1 = new Vector2(.5f, 0),
                    UV2 = new Vector2(.5f, .5f),
                    UV3 = new Vector2(1, .5f),
                    UV4 = new Vector2(1, 0)
                });
                l.Add(sidepa);

                sidepb = (new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, 0, 1),
                    Vertex2 = new Vector3(0, .5f, 1),
                    Vertex3 = new Vector3(1, .5f, 1),
                    Vertex4 = new Vector3(1, 0, 1),

                    Normal = new Vector3(0, 0, 1),
                    Texture = _texture,
                    TriFlip = true,

                    UV1 = new Vector2(0, 0),
                    UV2 = new Vector2(0, .5f),
                    UV3 = new Vector2(1f, .5f),
                    UV4 = new Vector2(1f, 0)
                });
                l.Add(sidepb);
            }

            //Zneg
            if ((CanBuildSide(leftBlock, me) && !useLittleCornerModel))
            {
                sidena = (new CustomBlockData()
                {
                    Vertex1 = new Vector3(.5f, 0, 0),
                    Vertex2 = new Vector3(.5f, 1, 0),
                    Vertex3 = new Vector3(1, 1, 0),
                    Vertex4 = new Vector3(1, 0, 0),

                    Normal = new Vector3(0, 0, -1),
                    Texture = _texture,
                    TriFlip = true,

                    UV1 = new Vector2(.5f, 0),
                    UV2 = new Vector2(.5f, 1),
                    UV3 = new Vector2(1, 1),
                    UV4 = new Vector2(1, 0)
                });
                l.Add(sidena);
                sidenb = (new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, 0, 0),
                    Vertex2 = new Vector3(0, .5f, 0),
                    Vertex3 = new Vector3(.5f, .5f, 0),
                    Vertex4 = new Vector3(.5f, 0, 0),

                    Normal = new Vector3(0, 0, -1),
                    Texture = _texture,
                    TriFlip = true,

                    UV1 = new Vector2(0, 0),
                    UV2 = new Vector2(0, .5f),
                    UV3 = new Vector2(.5f, .5f),
                    UV4 = new Vector2(.5f, 0)
                });
                l.Add(sidenb);
            }
            #endregion

            #region Rotate the model
            Vector3 rotationCenter = new Vector3(0.5f, 0.5f, 0.5f);

            if (upsideDown)
            {
                for (int x = 0; x < l.Count; x++)
                {
                    CustomBlockData data = l[x];

                    data.Vertex1 = Vector3.RotateX(data.Vertex1, rotationCenter, 180);
                    data.Vertex2 = Vector3.RotateX(data.Vertex2, rotationCenter, 180);
                    data.Vertex3 = Vector3.RotateX(data.Vertex3, rotationCenter, 180);
                    data.Vertex4 = Vector3.RotateX(data.Vertex4, rotationCenter, 180);
                    data.Normal.Y = -data.Normal.Y;
                    if (data.Normal.Y != 0)
                        data.TriFlip = true;
                }
            }

            Matrix rotate = Matrix.CreateRotationY(((float)Math.PI / 180f) * degDirection);

            for (int x = 0; x < l.Count; x++)
            {
                CustomBlockData data = l[x];

                data.Vertex1 = Vector3.TransformNormal(data.Vertex1 - rotationCenter, rotate) + rotationCenter;
                data.Vertex2 = Vector3.TransformNormal(data.Vertex2 - rotationCenter, rotate) + rotationCenter;
                data.Vertex3 = Vector3.TransformNormal(data.Vertex3 - rotationCenter, rotate) + rotationCenter;
                data.Vertex4 = Vector3.TransformNormal(data.Vertex4 - rotationCenter, rotate) + rotationCenter;
                data.Normal = Vector3.TransformNormal(data.Normal, rotate);
            }

            if (degDirection != 0)
            {
                if (front1 != null)
                    front1.TriFlip = false;
                if (front2 != null)
                    front2.TriFlip = false;

                if (degDirection == 180)
                {
                    if(back != null)
                        back.TriFlip = false;

                    if (sidepa != null)
                    {
                        sidepa.TriFlip = false;
                        sidepb.TriFlip = false;
                    }
                }
                if (degDirection == 90)
                {
                    if (sidena != null)
                    {
                        sidena.TriFlip = false;
                        sidenb.TriFlip = false;
                    }
                    if (sidepa != null)
                    {
                        sidepa.TriFlip = false;
                        sidepb.TriFlip = false;
                    }
                }
            }

            if (connectPart != null && degDirection == 90)
            {
                connectPart.TriFlip = true;
            }

            if (cornerBack != null)
            {
                if (Math.Round(cornerBack.Normal.X, 4) == -1 || Math.Round(cornerBack.Normal.Z, 4) == 1)
                {
                    cornerBack.TriFlip = false;
                }
            }
            #endregion

            return l;
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
