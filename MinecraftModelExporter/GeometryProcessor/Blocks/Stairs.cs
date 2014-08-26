using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
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

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            List<CustomBlockData> l = new List<CustomBlockData>();

            Vector3 cent = new Vector3(0, 0, 0);

            Vector3 zPos = new Vector3(0, 0, 1);
            Vector3 zNeg = new Vector3(0, 0, -1);
            Vector3 xPos = new Vector3(1, 0, 0);
            Vector3 xNeg = new Vector3(-1, 0, 0);

            byte t = GetType(metadata);
            bool ud = IsUpsideDown(metadata);

            if (t == 0)
            {
                zPos = Vector3.RotateY(zPos, cent, 180);
                zNeg = Vector3.RotateY(zNeg, cent, 180);
                xPos = Vector3.RotateY(xPos, cent, 180);
                xNeg = Vector3.RotateY(xNeg, cent, 180);
            }
            if (t == 3)
            {
                zPos = Vector3.RotateY(zPos - cent, 270) + cent;
                zNeg = Vector3.RotateY(zNeg - cent, 270) + cent;
                xPos = Vector3.RotateY(xPos - cent, 270) + cent;
                xNeg = Vector3.RotateY(xNeg - cent, 270) + cent;
            }
            if (t == 2)
            {
                zPos = Vector3.RotateY(zPos - cent, 90) + cent;
                zNeg = Vector3.RotateY(zNeg - cent, 90) + cent;
                xPos = Vector3.RotateY(xPos - cent, 90) + cent;
                xNeg = Vector3.RotateY(xNeg - cent, 90) + cent;
            }

            Zpos = source.GetData(zPos.ToPoint3());
            Zneg = source.GetData(zNeg.ToPoint3());
            Xpos = source.GetData(xPos.ToPoint3());
            Xneg = source.GetData(xNeg.ToPoint3());

            #region Build the model
            //Bottom
            if (CanBuild(Yneg, me))
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

            //Top
            if (CanBuild(Ypos, me))
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(.5f, 1, 0),
                    Vertex2 = new Vector3(1, 1, 0),
                    Vertex3 = new Vector3(1, 1, 1),
                    Vertex4 = new Vector3(.5f, 1, 1),

                    Normal = new Vector3(0, 1, 0),
                    Texture = _textureY
                }.CreateUVs(.5f, 0, 1, 1));
            }

            //Xpos
            if (CanBuild(Xpos, me))
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(1, 0, 0),
                    Vertex2 = new Vector3(1, 1, 0),
                    Vertex3 = new Vector3(1, 1, 1),
                    Vertex4 = new Vector3(1, 0, 1),

                    Normal = new Vector3(1, 0, 0),
                    Texture = _texture,
                    TriFlip = true
                }.CreateUVs());
            }

            //Xneg
            if (CanBuild(Xneg, me))
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, 0, 0),
                    Vertex2 = new Vector3(0, .5f, 0),
                    Vertex3 = new Vector3(0, .5f, 1),
                    Vertex4 = new Vector3(0, 0, 1),

                    Normal = new Vector3(-1, 0, 0),
                    Texture = _texture,
                    TriFlip = true,
                }.CreateUVs());
            }

            //Zpos
            if (CanBuild(Zpos, me))
            {
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(.5f, 0, 1),
                    Vertex2 = new Vector3(.5f, 1, 1),
                    Vertex3 = new Vector3(1, 1, 1),
                    Vertex4 = new Vector3(1, 0, 1),

                    Normal = new Vector3(0, 0, 1),
                    Texture = _texture,
                    TriFlip = true,

                    UV1 = new Vector2(.5f, 0),
                    UV2 = new Vector2(.5f, 1),
                    UV3 = new Vector2(1, 1),
                    UV4 = new Vector2(1, 0)
                });
                l.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, 0, 1),
                    Vertex2 = new Vector3(0, .5f, 1),
                    Vertex3 = new Vector3(.5f, .5f, 1),
                    Vertex4 = new Vector3(.5f, 0, 1),

                    Normal = new Vector3(0, 0, 1),
                    Texture = _texture,
                    TriFlip = true,

                    UV1 = new Vector2(0, 0),
                    UV2 = new Vector2(0, .5f),
                    UV3 = new Vector2(.5f, .5f),
                    UV4 = new Vector2(.5f, 0)
                });
            }

            //Zneg
            if (CanBuild(Zneg, me))
            {
                l.Add(new CustomBlockData()
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
                l.Add(new CustomBlockData()
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
            }

            //Stair Xneg
            l.Add(new CustomBlockData()
            {
                Vertex1 = new Vector3(.5f, .5f, 0),
                Vertex2 = new Vector3(.5f, 1, 0),
                Vertex3 = new Vector3(.5f, 1, 1),
                Vertex4 = new Vector3(.5f, .5f, 1),

                Normal = new Vector3(-1, 0, 0),
                Texture = _texture,

                TriFlip = true,

                UV1 = new Vector2(0, .5f),
                UV2 = new Vector2(0, 1),
                UV3 = new Vector2(1, 1),
                UV4 = new Vector2(1, .5f)
            });

            //Stair top2
            l.Add(new CustomBlockData()
            {
                Vertex1 = new Vector3(0, .5f, 0),
                Vertex2 = new Vector3(.5f, .5f, 0),
                Vertex3 = new Vector3(.5f, .5f, 1),
                Vertex4 = new Vector3(0, .5f, 1),

                Normal = new Vector3(0, 1, 0),
                Texture = _textureY
            }.CreateUVs(0, 0, .5f, 1));
            #endregion

            Vector3 c = new Vector3(0, 0, 0);
            Vector3 c2 = new Vector3(.5f, .5f, .5f);
            if (ud)
            {
                for (int x = 0; x < l.Count; x++)
                {
                    l[x].Vertex1 = Vector3.RotateZ(l[x].Vertex1, c2, 180);
                    l[x].Vertex2 = Vector3.RotateZ(l[x].Vertex2, c2, 180);
                    l[x].Vertex3 = Vector3.RotateZ(l[x].Vertex3, c2, 180);
                    l[x].Vertex4 = Vector3.RotateZ(l[x].Vertex4, c2, 180);
                    //l[x].Normal = Vector3.RotateZ(l[x].Normal, 180);
                    l[x].Normal = l[x].Normal * -1;
                    l[x].TriFlip = !l[x].TriFlip;
                }
            }
            else
            {
                for (int x = 0; x < l.Count; x++)
                {
                    l[x].Vertex1 = Vector3.RotateY(l[x].Vertex1, c2, -180);
                    l[x].Vertex2 = Vector3.RotateY(l[x].Vertex2, c2, -180);
                    l[x].Vertex3 = Vector3.RotateY(l[x].Vertex3, c2, -180);
                    l[x].Vertex4 = Vector3.RotateY(l[x].Vertex4, c2, -180);
                }
            }

            //if (t == 1)
            //{
            //    for (int x = 0; x < l.Count; x++)
            //    {
            //        l[x].Vertex1 = Vector3.RotateY(l[x].Vertex1, c2, 180);
            //        l[x].Vertex2 = Vector3.RotateY(l[x].Vertex2, c2, 180);
            //        l[x].Vertex3 = Vector3.RotateY(l[x].Vertex3, c2, 180);
            //        l[x].Vertex4 = Vector3.RotateY(l[x].Vertex4, c2, 180);
            //    }
            //}
            if (t == 0)
            {
                for (int x = 0; x < l.Count; x++)
                {
                    l[x].Vertex1 = Vector3.RotateY(l[x].Vertex1, c2, 180);
                    l[x].Vertex2 = Vector3.RotateY(l[x].Vertex2, c2, 180);
                    l[x].Vertex3 = Vector3.RotateY(l[x].Vertex3, c2, 180);
                    l[x].Vertex4 = Vector3.RotateY(l[x].Vertex4, c2, 180);
                }
            }
            if (t == 3)
            {
                for (int x = 0; x < l.Count; x++)
                {
                    l[x].Vertex1 = Vector3.RotateY(l[x].Vertex1 - c2, 270) + c2;
                    l[x].Vertex2 = Vector3.RotateY(l[x].Vertex2 - c2, 270) + c2;
                    l[x].Vertex3 = Vector3.RotateY(l[x].Vertex3 - c2, 270) + c2;
                    l[x].Vertex4 = Vector3.RotateY(l[x].Vertex4 - c2, 270) + c2;
                }
            }
            if (t == 2)
            {
                for (int x = 0; x < l.Count; x++)
                {
                    l[x].Vertex1 = Vector3.RotateY(l[x].Vertex1 - c2, 90) + c2;
                    l[x].Vertex2 = Vector3.RotateY(l[x].Vertex2 - c2, 90) + c2;
                    l[x].Vertex3 = Vector3.RotateY(l[x].Vertex3 - c2, 90) + c2;
                    l[x].Vertex4 = Vector3.RotateY(l[x].Vertex4 - c2, 90) + c2;
                }
            }

            return l;
        }

        private bool IsStairs(uint id)
        {
            return (id == 53 || id == 67 || id == 108 || id == 109 || id == 114 || id == 128 || id == 134 || id == 135 || id == 136 || id == 156);
        }

        public bool CanBuild(BlockData data, BlockData me)
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
