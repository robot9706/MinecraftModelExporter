using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor.Blocks
{
    class Liquid : Block
    {
        private bool _still = false;
        private string _tex;
        private int _maxLevel = 7;

        public Liquid(string name, byte id, string tex, bool still, int lvl)
        {
            _maxLevel = lvl;
            _tex = tex;
            Name = name;
            ID = id;
            UseMetadata = true;
            _still = still;
        }

        public override bool IsTransparent()
        {
            return true;
        }

        public override bool IsFullSide(BlockSide side)
        {
            return false;
        }

        public override string GetTextureForSide(BlockSide side, byte metadata)
        {
            return _tex;
        }

        public override string[] GetTextures(byte metadata)
        {
            return new string[] { _tex, _tex, _tex, _tex, _tex, _tex };
        }

        public override bool CanGenerateSide(BlockSide side, byte metadata)
        {
            bool[] bits = BitHelper.GetBits(metadata);

            return base.CanGenerateSide(side, metadata);
        }

        private float CalculateLevel(BlockData dat) 
        {
            bool[] bits = BitHelper.GetBits(dat.Metadata);

            float level = (0.7f * ((_maxLevel - ((float)dat.Metadata)) / _maxLevel)) + 0.1f;

            if (_still)
                level = 0.8f;

            if (bits[0])
                level = 1;

            return level;
        }

        public override List<CustomBlockData> GenerateSide(BlockSide side, byte metadata, BlockData me, BlockData Xpos, BlockData Xneg, BlockData Ypos, BlockData Yneg, BlockData Zpos, BlockData Zneg)
        {
            List<CustomBlockData> s = new List<CustomBlockData>();

            float level = CalculateLevel(me);

            if (side == BlockSide.Ypos)
            {
                s.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, level, 0),
                    Vertex2 = new Vector3(1, level, 0),
                    Vertex3 = new Vector3(1, level, 1),
                    Vertex4 = new Vector3(0, level, 1),

                    Normal = new Vector3(0, 1, 0),

                    Texture = _tex
                }.CreateUVs());
            }
            if (side == BlockSide.Yneg)
            {
                s.Add(new CustomBlockData()
                {
                    Vertex1 = new Vector3(0, 0, 0),
                    Vertex2 = new Vector3(1, 0, 0),
                    Vertex3 = new Vector3(1, 0, 1),
                    Vertex4 = new Vector3(0, 0, 1),

                    TriFlip = true,

                    Normal = new Vector3(0, 1, 0),

                    Texture = _tex
                }.CreateUVs());
            }

            if (side == BlockSide.Xpos)
            {
                float xpl = level;
                if (Xpos.ID == 8 || Xpos.ID == 9)
                {
                    xpl = CalculateLevel(Xpos);

                    s.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(1, level, 1),
                        Vertex2 = new Vector3(1, level, 0),
                        Vertex3 = new Vector3(1, xpl, 0),
                        Vertex4 = new Vector3(1, xpl, 1),

                        UV1 = new Vector2(0, xpl),
                        UV2 = new Vector2(1, xpl),
                        UV3 = new Vector2(1, level),
                        UV4 = new Vector2(0, level),

                        Normal = new Vector3(1, 0, 0),

                        Texture = _tex
                    });
                }
                else
                {
                    s.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(1, xpl, 1),
                        Vertex2 = new Vector3(1, xpl, 0),
                        Vertex3 = new Vector3(1, 0, 0),
                        Vertex4 = new Vector3(1, 0, 1),

                        UV1 = new Vector2(0, xpl),
                        UV2 = new Vector2(1, xpl),
                        UV3 = new Vector2(1, 0),
                        UV4 = new Vector2(0, 0),

                        Normal = new Vector3(1, 0, 0),

                        Texture = _tex
                    });
                }
            }

            if (side == BlockSide.Xneg)
            {
                float xpl = level;
                if (Xneg.ID == 8 || Xneg.ID == 9)
                {
                    xpl = CalculateLevel(Xneg);

                    s.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(0, level, 1),
                        Vertex2 = new Vector3(0, level, 0),
                        Vertex3 = new Vector3(0, xpl, 0),
                        Vertex4 = new Vector3(0, xpl, 1),

                        UV1 = new Vector2(0, xpl),
                        UV2 = new Vector2(1, xpl),
                        UV3 = new Vector2(1, level),
                        UV4 = new Vector2(0, level),

                        Normal = new Vector3(-1, 0, 0),

                        Texture = _tex
                    });
                }
                else
                {
                    s.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(0, xpl, 1),
                        Vertex2 = new Vector3(0, xpl, 0),
                        Vertex3 = new Vector3(0, 0, 0),
                        Vertex4 = new Vector3(0, 0, 1),

                        UV1 = new Vector2(0, xpl),
                        UV2 = new Vector2(1, xpl),
                        UV3 = new Vector2(1, 0),
                        UV4 = new Vector2(0, 0),

                        Normal = new Vector3(-1, 0, 0),

                        Texture = _tex
                    });
                }
            }

            if (side == BlockSide.Zpos)
            {
                float xpl = level;
                if (Zpos.ID == 8 || Zpos.ID == 9)
                {
                    xpl = CalculateLevel(Zpos);

                    s.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(1, level, 1),
                        Vertex2 = new Vector3(0, level, 1),
                        Vertex3 = new Vector3(0, xpl, 1),
                        Vertex4 = new Vector3(1, xpl, 1),

                        UV1 = new Vector2(0, xpl),
                        UV2 = new Vector2(1, xpl),
                        UV3 = new Vector2(1, level),
                        UV4 = new Vector2(0, level),

                        Normal = new Vector3(0, 0, 1),

                        Texture = _tex
                    });
                }
                else
                {
                    s.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(1, xpl, 1),
                        Vertex2 = new Vector3(0, xpl, 1),
                        Vertex3 = new Vector3(0, 0, 1),
                        Vertex4 = new Vector3(1, 0, 1),

                        UV1 = new Vector2(0, xpl),
                        UV2 = new Vector2(1, xpl),
                        UV3 = new Vector2(1, 0),
                        UV4 = new Vector2(0, 0),

                        Normal = new Vector3(0, 0, 1),

                        Texture = _tex
                    });
                }
            }


            if (side == BlockSide.Zneg)
            {
                float xpl = level;
                if (Zneg.ID == 8 || Zneg.ID == 9)
                {
                    xpl = CalculateLevel(Zneg);

                    s.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(1, level, 0),
                        Vertex2 = new Vector3(0, level, 0),
                        Vertex3 = new Vector3(0, xpl, 0),
                        Vertex4 = new Vector3(1, xpl, 0),

                        UV1 = new Vector2(0, xpl),
                        UV2 = new Vector2(1, xpl),
                        UV3 = new Vector2(1, level),
                        UV4 = new Vector2(0, level),

                        Normal = new Vector3(0, 0, -1),

                        Texture = _tex
                    });
                }
                else
                {
                    s.Add(new CustomBlockData()
                    {
                        Vertex1 = new Vector3(1, xpl, 0),
                        Vertex2 = new Vector3(0, xpl, 0),
                        Vertex3 = new Vector3(0, 0, 0),
                        Vertex4 = new Vector3(1, 0, 0),

                        UV1 = new Vector2(0, xpl),
                        UV2 = new Vector2(1, xpl),
                        UV3 = new Vector2(1, 0),
                        UV4 = new Vector2(0, 0),

                        Normal = new Vector3(0, 0, -1),

                        Texture = _tex
                    });
                }
            }

            return s;
        }
    }
}
