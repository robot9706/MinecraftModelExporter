using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class PlusShapedBlock : Block
    {
        private string _tex;
        private string _texY;

        public PlusShapedBlock(byte id, string name, string texture)
        {
            _tex = texture;
            _texY = texture;

            ID = id;
            UsesOneTexture = true;
            UseMetadata = false;
            Name = name;
        }

        public PlusShapedBlock(byte id, string name, string texture, string texY)
        {
            _tex = texture;
            _texY = texY;

            ID = id;
            UsesOneTexture = true;
            UseMetadata = false;
            Name = name;
        }

        public override string GetTextureForSide(BlockSide side, byte metadata)
        {
            if (side == BlockSide.Yneg || side == BlockSide.Ypos)
                return _texY;

            return _tex;
        }

        public override string[] GetTextures(byte metadata)
        {
            return new string[] { _tex, _tex, _texY, _texY, _tex, _tex };
        }

        public override bool IsTransparent()
        {
            return true;
        }

        public override bool IsFullyCustomModel()
        {
            return true;
        }

        public override List<CustomBlockData> GenerateModel(byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg, BlockSource source, Point3 blockPosition)
        {
            List<CustomBlockData> mdl = new List<CustomBlockData>();

            bool zPos = Zpos.IsSolid || Zpos.ID == me.ID;
            bool zNeg = Zneg.IsSolid || Zneg.ID == me.ID;
            bool xPos = Xpos.IsSolid || Xpos.ID == me.ID;
            bool xNeg = Xneg.IsSolid || Xneg.ID == me.ID;

            float xStart = 0;
            float xEnd = 1;
            float zStart = 0;
            float zEnd = 1;

            if (!xPos && xNeg)
            {
                xStart = 0;
                xEnd = 0.5f;
            }
            else if (xPos && !xNeg)
            {
                xStart = .5f;
                xEnd = 1;
            }
            else if (xPos && xNeg)
            {
                xStart = 0;
                xEnd = 1;
            }
            else if (!xPos && !xNeg)
            {
                xStart = 0;
                xEnd = 0;
            }

            if (!zPos && zNeg)
            {
                zStart = 0;
                zEnd = 0.5f;
            }
            else if (zPos && !zNeg)
            {
                zStart = .5f;
                zEnd = 1;
            }
            else if (zPos && zNeg)
            {
                zStart = 0;
                zEnd = 1;
            }
            else if (!zPos && !zNeg)
            {
                zStart = 0;
                zEnd = 0;
            }

            if (!zPos && !zNeg && !xPos && !xNeg)
            {
                xStart = 0;
                xEnd = 1;
                zStart = 0;
                zEnd = 1;
            }

            #region X sides
            if (xStart != 0 || xEnd != 0)
            {
                mdl.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(xStart, 0, .5f),
                    Vertex2 = new Vector3(xStart, 1, .5f),
                    Vertex3 = new Vector3(xEnd, 1, .5f),
                    Vertex4 = new Vector3(xEnd, 0, .5f),

                    Normal = new Vector3(1, 0, 0),

                    Texture = _tex
                }.CreateUVsRotated90(xStart, 0, xEnd, 1));
                mdl.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(xStart, 0, .5f),
                    Vertex2 = new Vector3(xStart, 1, .5f),
                    Vertex3 = new Vector3(xEnd, 1, .5f),
                    Vertex4 = new Vector3(xEnd, 0, .5f),

                    Normal = new Vector3(1, 0, 0),

                    TriFlip = true,

                    Texture = _tex
                }.CreateUVsRotated90(xStart, 0, xEnd, 1));

                if (!source.GetData(new Point3(blockPosition.X, blockPosition.Y + 1, blockPosition.Z)).EqualsID(me))
                {
                    mdl.Add(new CustomBlockData()
                           {
                               Vertex1 = new Vector3(xStart, 0.99f, 0.45f),
                               Vertex2 = new Vector3(xEnd, 0.99f, 0.45f),
                               Vertex3 = new Vector3(xEnd, 0.99f, .55f),
                               Vertex4 = new Vector3(xStart, 0.99f, .55f),

                               Normal = new Vector3(0, 1, 0),

                               UV1 = new Vector2(.45f, 0),
                               UV2 = new Vector2(.45f, 1),
                               UV3 = new Vector2(.55f, 1),
                               UV4 = new Vector2(.55f, 0),

                               Texture = _tex

                           });
                }

                if (!source.GetData(new Point3(blockPosition.X, blockPosition.Y - 1, blockPosition.Z)).EqualsID(me))
                {
                    mdl.Add(new CustomBlockData()
                        {
                            Vertex1 = new Vector3(xStart, 0.01f, 0.45f),
                            Vertex2 = new Vector3(xEnd, 0.01f, 0.45f),
                            Vertex3 = new Vector3(xEnd, 0.01f, .55f),
                            Vertex4 = new Vector3(xStart, 0.01f, .55f),

                            Normal = new Vector3(0, 1, 0),

                            UV1 = new Vector2(.45f, 0),
                            UV2 = new Vector2(.45f, 1),
                            UV3 = new Vector2(.55f, 1),
                            UV4 = new Vector2(.55f, 0),

                            Texture = _tex

                        });
                }

                if (xPos && !xNeg)
                {
                    mdl.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(.5f, 0, .45f),
                        Vertex2 = new Vector3(.5f, .99f, .45f),
                        Vertex3 = new Vector3(.5f, .99f, .55f),
                        Vertex4 = new Vector3(.5f, 0, .55f),

                        Normal = new Vector3(1, 0, 0),

                        UV1 = new Vector2(.45f, 0),
                        UV2 = new Vector2(.45f, 1),
                        UV3 = new Vector2(.55f, 1),
                        UV4 = new Vector2(.55f, 0),

                        Texture = _texY
                    });
                }
                else if (!xPos && xNeg)
                {
                    mdl.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(.5f, 0, .45f),
                        Vertex2 = new Vector3(.5f, .99f, .45f),
                        Vertex3 = new Vector3(.5f, .99f, .55f),
                        Vertex4 = new Vector3(.5f, 0, .55f),

                        Normal = new Vector3(1, 0, 0),

                        UV1 = new Vector2(.45f, 0),
                        UV2 = new Vector2(.45f, 1),
                        UV3 = new Vector2(.55f, 1),
                        UV4 = new Vector2(.55f, 0),

                        Texture = _texY
                    });
                }
            }
            #endregion

            #region Z sides
            if (zStart != 0 || zEnd != 0)
            {
                mdl.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(.5f, 0, zStart),
                    Vertex2 = new Vector3(.5f, 1, zStart),
                    Vertex3 = new Vector3(.5f, 1, zEnd),
                    Vertex4 = new Vector3(.5f, 0, zEnd),

                    Normal = new Vector3(0, 0, 1),

                    Texture = _tex
                }.CreateUVsRotated90(zStart, 0, zEnd, 1));
                mdl.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(.5f, 0, zStart),
                    Vertex2 = new Vector3(.5f, 1, zStart),
                    Vertex3 = new Vector3(.5f, 1, zEnd),
                    Vertex4 = new Vector3(.5f, 0, zEnd),

                    Normal = new Vector3(0, 0, 1),

                    TriFlip = true,

                    Texture = _tex
                }.CreateUVsRotated90(zStart, 0, zEnd, 1));

                if (!source.GetData(new Point3(blockPosition.X, blockPosition.Y + 1, blockPosition.Z)).EqualsID(me))
                {
                    mdl.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(.45f, 1, zStart),
                        Vertex2 = new Vector3(.45f, 1, zEnd),
                        Vertex3 = new Vector3(.55f, 1, zEnd),
                        Vertex4 = new Vector3(.55f, 1, zStart),

                        Normal = new Vector3(0, 1, 0),

                        UV1 = new Vector2(.45f, zStart),
                        UV2 = new Vector2(.45f, zEnd),
                        UV3 = new Vector2(.55f, zEnd),
                        UV4 = new Vector2(.55f, zStart),

                        Texture = _tex
                    });
                }

                if (!source.GetData(new Point3(blockPosition.X, blockPosition.Y - 1, blockPosition.Z)).EqualsID(me))
                {
                    mdl.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(.45f, 0, zStart),
                        Vertex2 = new Vector3(.45f, 0, zEnd),
                        Vertex3 = new Vector3(.55f, 0, zEnd),
                        Vertex4 = new Vector3(.55f, 0, zStart),

                        Normal = new Vector3(0, 1, 0),

                        UV1 = new Vector2(.45f, zStart),
                        UV2 = new Vector2(.45f, zEnd),
                        UV3 = new Vector2(.55f, zEnd),
                        UV4 = new Vector2(.55f, zStart),

                        Texture = _tex
                    });
                }

                if (zPos && !zNeg)
                {
                    mdl.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(.45f, 0, .5f),
                        Vertex2 = new Vector3(.45f, 1, .5f),
                        Vertex3 = new Vector3(.55f, 1, .5f),
                        Vertex4 = new Vector3(.55f, 0, .5f),

                        Normal = new Vector3(1, 0, 0),

                        UV1 = new Vector2(.45f, 0),
                        UV2 = new Vector2(.45f, 1),
                        UV3 = new Vector2(.55f, 1),
                        UV4 = new Vector2(.55f, 0),

                        Texture = _texY
                    });
                }
                else if (!zPos && zNeg)
                {
                    mdl.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(.45f, 0, .5f),
                        Vertex2 = new Vector3(.45f, 1, .5f),
                        Vertex3 = new Vector3(.55f, 1, .5f),
                        Vertex4 = new Vector3(.55f, 0, .5f),

                        Normal = new Vector3(1, 0, 0),

                        UV1 = new Vector2(.45f, 0),
                        UV2 = new Vector2(.45f, 1),
                        UV3 = new Vector2(.55f, 1),
                        UV4 = new Vector2(.55f, 0),

                        Texture = _texY
                    });
                }
            }
            #endregion

            return mdl;
        }
    }
}
