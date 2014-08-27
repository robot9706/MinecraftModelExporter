using MinecraftModelExporter.Data;
using MinecraftModelExporter.GeometryProcessor;
using MinecraftModelExporter.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MinecraftModelExporter
{
    class ExportTask : BlockSource
    {
        public int ExportedVertices
        {
            get { return _vertices; }
        }

        public int ExportedTriangles
        {
            get { return _vertices / 3; }
        }

        private ImportedData _in;

        private FileWriter _outputWriter;
        private string _outputFile;

        private ExportConfig _cfg;

        private int _vertices;

        public ExportTask(ImportedData input, FileWriter writer, string outputFile, ExportConfig config)
        {
            _in = input;

            _cfg = config;

            _outputWriter = writer;
            _outputFile = outputFile;
        }

        public bool Export(object arg, TaskProgressReport p)
        {
            int maxTotalTask = 8; //6 normals + geom build + file write
            int currentTotalTask = 0;

            PartTaskProgressReport rep = (PartTaskProgressReport)p;

            rep.SetTitle("Preparing input data");
            rep.Report();

            Dictionary<Vector4, BlockRange[]> ranges = new Dictionary<Vector4, BlockRange[]>();
            List<CustomBlockData> customData = new List<CustomBlockData>();

            Vector3[] normalsToFollow = new Vector3[] { new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, -1, 0), new Vector3(0, 0, 1), new Vector3(0, 0, -1) };

            bool[, ,] customBuilt = new bool[_in.BlockIDs.GetLength(0), _in.BlockIDs.GetLength(1), _in.BlockIDs.GetLength(2)];
            for (int n = 0; n < normalsToFollow.Length; n++)
            {
                Vector3 normal = normalsToFollow[n];
                BlockSide normalSide = Block.GetSideFromNormal(normal);

                Vector2 xy = GetXYByNormal(normal);
                Point3 xyLength = GetLengthAxisByNormal(normal); //X:x, Y:y, Z:level

                int xs = _in.BlockIDs.GetLength(xyLength.X);
                int ys = _in.BlockIDs.GetLength(xyLength.Y);
                int zs = _in.BlockIDs.GetLength(xyLength.Z);

                rep.SetTitle("Processing side: " + TranslateNormal(normal));
                rep.Report();

                for (int lvl = 0; lvl < _in.BlockIDs.GetLength(xyLength.Z); lvl++)
                {
                    BlockData[,] lvlData = new BlockData[xs, ys];

                    int partMax = xs * ys;
                    int partProg = 0;

                    for (int a = 0; a < _in.BlockIDs.GetLength(xyLength.X); a++)
                    {
                        for (int b = 0; b < _in.BlockIDs.GetLength(xyLength.Y); b++)
                        {
                            Point3 pos = ConvertLevelAndXYToPos(a, b, lvl, xyLength);

                            if (_in.BlockIDs[pos.X, pos.Y, pos.Z] != 0)
                            {
                                BlockData bd = GetBlockDataAt(pos);
                                uint block = bd.GetGlobalID();
                                Block bl = Block.Blocks[block];
                                if (bl != null && !customBuilt[pos.X, pos.Y, pos.Z])
                                {
                                    bool canExportFace = (!_cfg.DontExportOuterFaces && !_cfg.InteriorOnly);

                                    if (_cfg.DontExportOuterFaces && CheckFace(pos, normal))
                                        canExportFace = true;
                                    else if (_cfg.InteriorOnly && CheckFaceInterior(pos, normal))
                                        canExportFace = true;

                                    if (canExportFace)
                                    {
                                        if (bl.IsFullyCustomModel())
                                        {
                                            List<CustomBlockData> dat = bl.GenerateModel(_in.BlockMetadatas[pos.X, pos.Y, pos.Z], bd, GetBlockDataAt(new Point3(pos.X + 1, pos.Y, pos.Z)), GetBlockDataAt(new Point3(pos.X - 1, pos.Y, pos.Z)), GetBlockDataAt(new Point3(pos.X, pos.Y + 1, pos.Z)), GetBlockDataAt(new Point3(pos.X, pos.Y - 1, pos.Z)), GetBlockDataAt(new Point3(pos.X, pos.Y, pos.Z + 1)), GetBlockDataAt(new Point3(pos.X, pos.Y, pos.Z - 1)), this, pos);
                                            for (int x = 0; x < dat.Count; x++)
                                            {
                                                dat[x].Vertex1 += pos.ToVector3();
                                                dat[x].Vertex2 += pos.ToVector3();
                                                dat[x].Vertex3 += pos.ToVector3();
                                                dat[x].Vertex4 += pos.ToVector3();
                                                dat[x].Source = bd;
                                            }
                                            customData.AddRange(dat);
                                            customBuilt[pos.X, pos.Y, pos.Z] = true;
                                        }
                                        else
                                        {
                                            if (bl.IsFullSide(normalSide))
                                            {
                                                if (CanGenerateSide(pos, normal))
                                                {
                                                    lvlData[a, b] = bd;
                                                }
                                            }
                                            else
                                            {
                                                List<CustomBlockData> dat = bl.GenerateSide(normalSide, _in.BlockMetadatas[pos.X, pos.Y, pos.Z], bd, GetBlockDataAt(new Point3(pos.X + 1, pos.Y, pos.Z)), GetBlockDataAt(new Point3(pos.X - 1, pos.Y, pos.Z)), GetBlockDataAt(new Point3(pos.X, pos.Y + 1, pos.Z)), GetBlockDataAt(new Point3(pos.X, pos.Y - 1, pos.Z)), GetBlockDataAt(new Point3(pos.X, pos.Y, pos.Z + 1)), GetBlockDataAt(new Point3(pos.X, pos.Y, pos.Z - 1)));
                                                for (int x = 0; x < dat.Count; x++)
                                                {
                                                    dat[x].Vertex1 += pos.ToVector3();
                                                    dat[x].Vertex2 += pos.ToVector3();
                                                    dat[x].Vertex3 += pos.ToVector3();
                                                    dat[x].Vertex4 += pos.ToVector3();
                                                    dat[x].Source = bd;
                                                }
                                                customData.AddRange(dat);
                                            }
                                        }
                                    }
                                }
                            }

                            partProg++;
                            rep.SetPartPercent((int)(((float)partProg / (float)partMax) * 100f));
                            rep.Report();
                        }
                    }

                    rep.SetTitle("Square-angulating level: " + lvl.ToString());
                    rep.Report();

                    if (_cfg.OptimizeModel)
                    {
                        List<BlockRange> sq = Squareangulate(lvlData);
                        if (sq.Count > 0)
                        {
                            ranges.Add(new Vector4(normal.X, normal.Y, normal.Z, lvl), sq.ToArray());
                        }
                    }
                    else
                    {
                        List<BlockRange> converted = new List<BlockRange>();
                        for (int x = 0; x < lvlData.GetLength(0); x++)
                        {
                            for (int y = 0; y < lvlData.GetLength(1); y++)
                            {
                                BlockData bd = lvlData[x, y];

                                if (bd == null)
                                    continue;

                                BlockRange range = new BlockRange();
                                range.Block = bd;
                                range.From = new PointF(x, y);
                                range.To = new PointF(x, y);

                                converted.Add(range);
                            }
                        }

                        ranges.Add(new Vector4(normal.X, normal.Y, normal.Z, lvl), converted.ToArray());
                    }
                }

                currentTotalTask++;
                rep.SetTotalPercent((int)(((float)currentTotalTask / (float)maxTotalTask) * 100f));
                rep.Report();
            }

            rep.SetTitle("Building geometry");
            rep.Report();

            Dictionary<KeyStruct, DataSet> datas = new Dictionary<KeyStruct, DataSet>();

            int pairIndex = 0;
            foreach (KeyValuePair<Vector4, BlockRange[]> pair in ranges)
            {
                Vector3 normal = new Vector3(pair.Key.X, pair.Key.Y, pair.Key.Z);
                int level = (int)pair.Key.Level;

                Vector3 addNormal = new Vector3(normal.X, normal.Y, normal.Z);
                if (addNormal.X < 0)
                    addNormal.X = 0;
                if (addNormal.Y < 0)
                    addNormal.Y = 0;
                if (addNormal.Z < 0)
                    addNormal.Z = 0;

                for (int x = 0; x < pair.Value.Length; x++)
                {
                    WriteRange(pair.Value[x], normal, level, addNormal, ref datas);
                }

                pairIndex++;
                rep.SetTotalPercent((int)(((float)pairIndex / (float)ranges.Count) * 100f));
                rep.Report();
            }

            for (int x = 0; x < customData.Count; x++)
            {
                WriteCustomData(customData[x], ref datas);
            }

            rep.SetTotalPercent(100);
            rep.Report();

            if (_cfg.CenterObject)
            {
                rep.SetTitle("Centering geometry");
                rep.Report();

                Vector3 min = new Vector3(0, 0, 0);
                Vector3 max = new Vector3(0, 0, 0);
                foreach (KeyValuePair<KeyStruct, DataSet> pair2 in datas)
                {
                    foreach (Vector3 v in pair2.Value.verts)
                    {
                        min.X = Math.Min(min.X, v.X);
                        min.Y = Math.Min(min.Y, v.Y);
                        min.Z = Math.Min(min.Z, v.Z);

                        max.X = Math.Max(max.X, v.X);
                        max.Y = Math.Max(max.Y, v.Y);
                        max.Z = Math.Max(max.Z, v.Z);
                    }
                }

                Vector3 move = (max - min) / 2;
                foreach (KeyValuePair<KeyStruct, DataSet> pair2 in datas)
                {
                    for (int x = 0; x < pair2.Value.verts.Count; x++)
                    {
                        pair2.Value.verts[x] -= move;
                    }
                }
            }

            currentTotalTask++;
            rep.SetTotalPercent((int)(((float)currentTotalTask / (float)maxTotalTask) * 100f));

            rep.SetTitle("Creating vertex data");
            rep.Report();

            ProcessedGeometryData geom = new ProcessedGeometryData();
            geom.ExportConfig = _cfg;

            foreach (KeyValuePair<KeyStruct, DataSet> pair2 in datas)
            {
                geom.Data.Add(pair2.Value);
            }

            //Export model file
            _outputWriter.Write(_outputFile, geom);

            //Export textures
            if (_cfg.ExportTextures)
            {
                rep.SetTitle("Exporting textures");
                rep.Report();

                List<string> failedTextures = new List<string>();

                string textureOutput = Path.Combine(Path.GetDirectoryName(_outputFile), _cfg.TextureOutputFolder);

                ResourcePack rs = new ResourcePack(_cfg.ResourcePack);
                rs.Open();

                foreach (KeyValuePair<KeyStruct, DataSet> pair2 in datas)
                {
                    if (Block.Blocks[pair2.Value.BaseData.GetGlobalID()] != null)
                    {
                        Block b = Block.Blocks[pair2.Value.BaseData.GetGlobalID()];
                        BlockSide side = (BlockSide)pair2.Value.SideByte;

                        string tex = b.GetTextureForSide(side, pair2.Value.BaseData.Metadata);

                        if (!rs.SaveBlockTexture(tex, textureOutput))
                            failedTextures.Add(tex);
                    }
                }

                rs.Close();
            }

            currentTotalTask++;
            rep.SetTotalPercent((int)(((float)currentTotalTask / (float)maxTotalTask) * 100f));

            return true;
        }

        private string TranslateNormal(Vector3 norm)
        {
            if (norm.X == 1)
                return "X+";
            if (norm.X == -1)
                return "X-";

            if (norm.Y == 1)
                return "Y+";
            if (norm.Y == -1)
                return "Y-";

            if (norm.Z == 1)
                return "Z+";
            if (norm.Z == -1)
                return "Z-";

            return "";
        }

        #region Helpers
        private bool CheckFace(Point3 block, Vector3 normal)
        {
            Point3 dir = normal.ToPoint3Normal();

            Point3 pos = block + dir;

            if (pos.X < 0 || pos.Y < 0 || pos.Z < 0 || pos.X >= _in.BlockIDs.GetLength(0) || pos.Y >= _in.BlockIDs.GetLength(1) || pos.Z >= _in.BlockIDs.GetLength(2))
                return false;

            return true;
        }

        private bool CheckFaceInterior(Point3 block, Vector3 normal)
        {
            Point3 dir = normal.ToPoint3Normal();

            Point3 pos = block;

            pos += dir;

            if (!(pos.X >= 0 && pos.Y >= 0 && pos.Z >= 0 && pos.X < _in.BlockIDs.GetLength(0) && pos.Y < _in.BlockIDs.GetLength(1) && pos.Z < _in.BlockIDs.GetLength(2)))
                return false;

            while (pos.X >= 0 && pos.Y >= 0 && pos.Z >= 0 && pos.X < _in.BlockIDs.GetLength(0) && pos.Y < _in.BlockIDs.GetLength(1) && pos.Z < _in.BlockIDs.GetLength(2))
            {
                if (_in.BlockIDs[pos.X, pos.Y, pos.Z] != 0)
                {
                    return true;
                }

                pos += dir;
            }

            return false;
        }

        private Dictionary<Vector4, BlockRange[]> SetupModelData(Dictionary<Vector3, List<BlockRange>> data, Point3 position, float level)
        {
            Dictionary<Vector4, List<BlockRange>> rt = new Dictionary<Vector4, List<BlockRange>>();

            foreach (KeyValuePair<Vector3, List<BlockRange>> pair in data)
            {
                Vector4 key = new Vector4(pair.Key.X, pair.Key.Y, pair.Key.Z, level);

                PointF xy = GetXYByNormal(pair.Key, position);

                List<BlockRange> rg = pair.Value;

                for (int x = 0; x < rg.Count; x++)
                    rg[x].Shift(xy);

                if (rt.ContainsKey(key))
                {
                    rt[key].AddRange(pair.Value);
                }
                else
                {
                    rt.Add(key, rg);
                }
            }

            Dictionary<Vector4, BlockRange[]> e = new Dictionary<Vector4, BlockRange[]>();

            foreach (KeyValuePair<Vector4, List<BlockRange>> pair in rt)
            {
                e.Add(pair.Key, pair.Value.ToArray());
            }

            return e;
        }

        private BlockData GetBlockDataAt(Point3 pos)
        {
            if (pos.X >= 0 && pos.Y >= 0 && pos.Z >= 0 && pos.X < _in.BlockIDs.GetLength(0) && pos.Y < _in.BlockIDs.GetLength(1) && pos.Z < _in.BlockIDs.GetLength(2))
            {
                if (_in.BlockIDs[pos.X, pos.Y, pos.Z] == 0)
                    return new BlockData(0, 0);
                return new BlockData(_in.BlockIDs[pos.X, pos.Y, pos.Z], _in.BlockMetadatas[pos.X, pos.Y, pos.Z]);
            }

            return new BlockData(0, 0);
        }

        private bool CanGenerateSide(Point3 pos, Vector3 norm)
        {
            BlockData me = GetBlockDataAt(pos);

            if (me == null)
                return false;

            Point3 p3normal = new Point3((int)norm.X, (int)norm.Y, (int)norm.Z);

            BlockData atSide = GetBlockDataAt(pos + p3normal);
            if (atSide == null)
                return true;

            if (atSide.EqualsFull(me))
                return false;

            uint i = BlockData.GetGlobalID(atSide.ID, 0);
            Block b = Block.Blocks[i];

            if (b != null)
            {
                Block bs = null;

                if (b.UseMetadata)
                {
                    bs = Block.Blocks[atSide.GetGlobalID()];
                }
                else
                {
                    bs = b;
                }

                return bs.IsTransparent();
            }

            return true;
        }

        private Point3 ConvertLevelAndXYToPos(int a, int b, int level, Point3 xyzLength)
        {
            if (xyzLength.X == 2 && xyzLength.Y == 1 && xyzLength.Z == 0)
                return new Point3(level, b, a);
            if (xyzLength.X == 0 && xyzLength.Y == 2 && xyzLength.Z == 1)
                return new Point3(a, level, b);
            if (xyzLength.X == 0 && xyzLength.Y == 1 && xyzLength.Z == 2)
                return new Point3(a, b, level);

            return new Point3(0, 0, 0);
        }

        private Point3 GetLengthAxisByNormal(Vector3 normal)
        {
            if (normal.X != 0 && normal.Y == 0 && normal.Z == 0)
            {
                return new Point3(2, 1, 0);
            }
            if (normal.X == 0 && normal.Y != 0 && normal.Z == 0)
            {
                return new Point3(0, 2, 1);
            }
            if (normal.X == 0 && normal.Y == 0 && normal.Z != 0)
            {
                return new Point3(0, 1, 2);
            }

            return new Point3(0, 0, 0);
        }

        private Vector2 GetXYByNormal(Vector3 normal)
        {
            if (normal.X != 0 && normal.Y == 0 && normal.Z == 0)
            {
                return new Vector2(normal.Z, normal.Y);
            }
            if (normal.X == 0 && normal.Y != 0 && normal.Z == 0)
            {
                return new Vector2(normal.X, normal.Z);
            }
            if (normal.X == 0 && normal.Y == 0 && normal.Z != 0)
            {
                return new Vector2(normal.X, normal.Y);
            }

            return new Vector2(0, 0);
        }

        private PointF GetXYByNormal(Vector3 normal, Point3 position)
        {
            if (normal.X != 0 && normal.Y == 0 && normal.Z == 0)
            {
                return new PointF(position.Z, position.Y);
            }
            if (normal.X == 0 && normal.Y != 0 && normal.Z == 0)
            {
                return new PointF(position.X, position.Z);
            }
            if (normal.X == 0 && normal.Y == 0 && normal.Z != 0)
            {
                return new PointF(position.X, position.Y);
            }

            return new PointF(0, 0);
        }

        private float GetLevelByNormal(Vector3 normal, Vector3 pos)
        {
            if (normal.X != 0 && normal.Y == 0 && normal.Z == 0)
            {
                return pos.X;
            }
            if (normal.X == 0 && normal.Y != 0 && normal.Z == 0)
            {
                return pos.Y;
            }
            if (normal.X == 0 && normal.Y == 0 && normal.Z != 0)
            {
                return pos.Z;
            }

            return 0;
        }
        #endregion

        #region Square-angulate

        private List<BlockRange> Squareangulate(BlockData[,] data)
        {
            List<BlockRange> ql = new List<BlockRange>();

            Dictionary<Tuple<uint, Byte>, BlockData[,]> types = CollectTypes(data);

            foreach (KeyValuePair<Tuple<uint, Byte>, BlockData[,]> pair in types)
            {
                BlockData[,] levelData = pair.Value;

                int rem = CountRem(levelData);
                if (rem > 0)
                {
                    bool[,] proc = new bool[levelData.GetLength(0), levelData.GetLength(1)];

                    while (rem > 0)
                    {
                        Point s = FindStart(levelData, proc);

                        if (s.X == -1 && s.Y == -1)
                            break;

                        Find(ref ql, ref levelData, ref proc, s, ref rem, levelData[s.X, s.Y]);
                    }
                }
            }

            return ql;
        }

        private Dictionary<Tuple<uint, byte>, BlockData[,]> CollectTypes(BlockData[,] source)
        {
            //ID, META : Level data
            Dictionary<Tuple<uint, byte>, BlockData[,]> dat = new Dictionary<Tuple<uint, byte>, BlockData[,]>();

            for (int x = 0; x < source.GetLength(0); x++)
            {
                for (int y = 0; y < source.GetLength(1); y++)
                {
                    BlockData d = source[x, y];

                    if (d != null)
                    {
                        Tuple<uint, byte> nd = new Tuple<uint, byte>(d.ID, d.Metadata);
                        if (!dat.ContainsKey(nd))
                        {
                            dat.Add(nd, new BlockData[source.GetLength(0), source.GetLength(1)]);
                        }

                        dat[nd][x, y] = d;
                    }
                }
            }

            return dat;
        }

        private void Find(ref List<BlockRange> ql, ref BlockData[,] oData, ref bool[,] proc, Point s, ref int remains, BlockData data)
        {
            //Find square
            int fsx = 0;
            int fsy = 0;
            bool find = true;
            bool b1 = false;
            bool b2 = false;
            while (find)
            {
                if (!b1 && s.X + fsx + 1 < oData.GetLength(0) && s.Y + fsy < oData.GetLength(1) &&
                    oData[s.X + fsx + 1, s.Y + fsy] != null && !proc[s.X + fsx + 1, s.Y + fsy])
                {
                    bool canAdd = true;

                    for (int cx = s.X; cx <= s.X + fsx + 1; cx++)
                    {
                        for (int cy = s.Y; cy <= s.Y + fsy; cy++)
                        {
                            if (oData[cx, cy] == null || proc[cx, cy])
                            {
                                canAdd = false;
                                break;
                            }
                        }
                    }

                    if (canAdd)
                        fsx++;
                    else
                        b1 = true;
                }
                else
                    b1 = true;

                if (!b2 && s.X + fsx < oData.GetLength(0) && s.Y + fsy + 1 < oData.GetLength(1) &&
                   oData[s.X + fsx, s.Y + fsy + 1] != null && !proc[s.X + fsx, s.Y + fsy + 1])
                {
                    bool canAdd = true;

                    for (int cy = s.Y; cy <= s.Y + fsy + 1; cy++)
                    {
                        for (int cx = s.X; cx <= s.X + fsx; cx++)
                        {
                            if (oData[cx, cy] == null || proc[cx, cy])
                            {
                                canAdd = false;
                                break;
                            }
                        }
                    }

                    if (canAdd)
                        fsy++;
                    else
                        b2 = true;
                }
                else
                    b2 = true;

                if (b1 && b2)
                {
                    find = false;

                    BlockRange br = new BlockRange();
                    br.From = s;
                    br.To = new Point(s.X + fsx, s.Y + fsy);
                    br.Block = data;
                    ql.Add(br);

                    for (int rx = s.X; rx <= br.To.X; rx++)
                        for (int ry = s.Y; ry <= br.To.Y; ry++)
                        {
                            remains--;
                            proc[rx, ry] = true;
                        }
                }
            }
        }

        private Point FindStart(BlockData[,] grid, bool[,] exclude)
        {
            Point s = new Point(-1, -1);

            for (int y = 0; y < grid.GetLength(1); y++)
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    if (grid[x, y] != null && !exclude[x, y])
                        return new Point(x, y);
                }

            return s;
        }

        private int CountRem(BlockData[,] datas)
        {
            int r = 0;

            for (int x = 0; x < datas.GetLength(0); x++)
                for (int y = 0; y < datas.GetLength(1); y++)
                    if (datas[x, y] != null)
                        r++;

            return r;
        }

        #endregion

        #region Interface
        public BlockData GetData(Point3 position)
        {
            return GetBlockDataAt(position);
        }
        #endregion

        #region GeomBuild
        private void WriteTriangle(DataSet ds, Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector3 normal)
        {
            ds.verts.Add(v1);
            ds.verts.Add(v2);
            ds.verts.Add(v3);
            _vertices += 3;

            ds.uvs.Add(uv1);
            ds.uvs.Add(uv2);
            ds.uvs.Add(uv3);

            ds.normals.Add(normal);
            ds.normals.Add(normal);
            ds.normals.Add(normal);
        }

        private void WriteRange(BlockRange range, Vector3 normal, float level, Vector3 addNormal, ref Dictionary<KeyStruct, DataSet> datas)
        {
            BlockSide side = Block.GetSideFromNormal(normal);
            Block bl = Block.Blocks[range.Block.GetGlobalID()];
            byte sideByte = 6;
            DataSet foundSet = null;
            sideByte = (byte)Block.GetSideInt(side);
            if (bl.UsesOneTexture)
            {
                side = BlockSide.AllSame;
                sideByte = (byte)Block.GetSideInt(side);
            }
            else
            {
                string tex = bl.GetTextureForSide(side, range.Block.Metadata);
                int[] ids = Find(bl.GetTextures(range.Block.Metadata), tex);

                DataSet[] ds = new DataSet[ids.Length];

                KeyStruct key = new KeyStruct(){
                    ID = range.Block.ID,
                    Metadata = range.Block.Metadata,
                    SideByte = sideByte
                };

                for (int s = 0; s < ids.Length; s++)
                {
                    KeyStruct k = new KeyStruct()
                    {
                        ID = range.Block.ID,
                        Metadata = range.Block.Metadata,
                        SideByte = (byte)ids[s]
                    };

                    if (datas.ContainsKey(k) && key != k)
                    {
                        ds[s] = datas[k];
                    }
                }

                for (int e = 0; e < ds.Length; e++)
                    if (ds[e] != null)
                    {
                        foundSet = ds[e];
                        break;
                    }
            }

            PointF ch = new PointF((range.To.X - range.From.X) + 1, (range.To.Y - range.From.Y) + 1);

            PointF t1 = range.From;
            PointF t2 = new PointF(range.From.X + ch.X, range.From.Y);
            PointF t3 = new PointF(range.From.X + ch.X, range.From.Y + ch.Y);
            PointF t4 = new PointF(range.From.X, range.From.Y + ch.Y);

            Vector2 uv1 = new Vector2(0, 0);
            Vector2 uv2 = new Vector2(ch.X, 0);
            Vector2 uv3 = new Vector2(ch.X, ch.Y);
            Vector2 uv4 = new Vector2(0, ch.Y);

            Vector3 t3d1 = ConvertToPos(t1, normal, level) + addNormal;
            Vector3 t3d2 = ConvertToPos(t2, normal, level) + addNormal;
            Vector3 t3d3 = ConvertToPos(t3, normal, level) + addNormal;
            Vector3 t3d4 = ConvertToPos(t4, normal, level) + addNormal;

            if (foundSet == null)
            {
                KeyStruct key = new KeyStruct()
                {
                    ID = range.Block.ID,
                    Metadata = range.Block.Metadata,
                    SideByte = sideByte
                };

                if (!datas.ContainsKey(key))
                {
                    datas.Add(key, new DataSet());
                    datas[key].BaseData = range.Block;
                    datas[key].SideByte = sideByte;
                }

                WriteTriangle(datas[key], t3d1, t3d2, t3d3, uv1, uv2, uv3, normal);
                WriteTriangle(datas[key], t3d3, t3d4, t3d1, uv3, uv4, uv1, normal);
            }
            else
            {
                WriteTriangle(foundSet, t3d1, t3d2, t3d3, uv1, uv2, uv3, normal);
                WriteTriangle(foundSet, t3d3, t3d4, t3d1, uv3, uv4, uv1, normal);
            }
        }

        private void WriteCustomData(CustomBlockData range, ref Dictionary<KeyStruct, DataSet> datas)
        {
            Vector3 normal = range.Normal;

            BlockSide side = Block.GetSideFromNormal(normal);
            Block bl = Block.Blocks[range.Source.GetGlobalID()];
            byte sideByte = 6;
            DataSet foundSet = null;
            sideByte = (byte)Block.GetSideInt(side);
            if (bl.UsesOneTexture)
            {
                side = BlockSide.AllSame;
                sideByte = (byte)Block.GetSideInt(side);
            }
            else
            {
                string tex = range.Texture;//bl.GetTextureForSide(side, range.Source.Metadata);
                int[] ids = Find(bl.GetTextures(range.Source.Metadata), tex);

                DataSet[] ds = new DataSet[ids.Length];

                KeyStruct key = new KeyStruct()
                {
                    ID = range.Source.ID,
                    Metadata = range.Source.Metadata,
                    SideByte = sideByte
                };

                for (int s = 0; s < ids.Length; s++)
                {
                    KeyStruct k = new KeyStruct()
                    {
                        ID = range.Source.ID,
                        Metadata = range.Source.Metadata,
                        SideByte = (byte)ids[s]
                    };

                    if (datas.ContainsKey(k) && key != k)
                    {
                        ds[s] = datas[k];
                    }
                }

                for (int e = 0; e < ds.Length; e++)
                    if (ds[e] != null)
                    {
                        foundSet = ds[e];
                        break;
                    }
            }

            if (foundSet == null)
            {
                KeyStruct key = new KeyStruct()
                {
                    ID = range.Source.ID,
                    Metadata = range.Source.Metadata,
                    SideByte = sideByte
                };

                if (!datas.ContainsKey(key))
                {
                    datas.Add(key, new DataSet());
                    datas[key].BaseData = range.Source;
                    datas[key].SideByte = sideByte;

                    foundSet = datas[key];
                }
                else
                {
                    foundSet = datas[key];
                }
            }

            //tri1: 1,2,3
            //tri2: 3,4,1

            //Vector3 tri1Norm = Vector3.Cross((range.Vertex2 - range.Vertex1), (range.Vertex3 - range.Vertex1));
            //Vector3 tri2Norm = Vector3.Cross((range.Vertex3 - range.Vertex1), (range.Vertex4 - range.Vertex1));
            Vector3 tri1Norm = normal;
            Vector3 tri2Norm = normal;

            if (range.TriFlip)// || (!range.TriFlip && (normal.X < 0 || normal.Y < 0 || normal.Z < 0)))
            {
                WriteTriangle(foundSet, range.Vertex3, range.Vertex2, range.Vertex1, range.UV3, range.UV2, range.UV1, tri1Norm);
                WriteTriangle(foundSet, range.Vertex1, range.Vertex4, range.Vertex3, range.UV1, range.UV4, range.UV3, tri2Norm);
            }
            else
            {
                WriteTriangle(foundSet, range.Vertex1, range.Vertex2, range.Vertex3, range.UV1, range.UV2, range.UV3, tri1Norm);
                WriteTriangle(foundSet, range.Vertex3, range.Vertex4, range.Vertex1, range.UV3, range.UV4, range.UV1, tri2Norm);
            }
        }

        private int[] Find(string[] array, string what)
        {
            List<int> w = new List<int>();

            for (int x = 0; x < array.Length; x++)
            {
                if (array[x] == what)
                    w.Add(x);
            }

            return w.ToArray();
        }

        public Vector3 ConvertToPos(PointF p, Vector3 normal, float level)
        {
            if (normal.X != 0 && normal.Y == 0 && normal.Z == 0)
            {
                return new Vector3(level, p.Y, p.X);
            }
            if (normal.X == 0 && normal.Y != 0 && normal.Z == 0)
            {
                return new Vector3(p.X, level, p.Y);
            }
            if (normal.X == 0 && normal.Y == 0 && normal.Z != 0)
            {
                return new Vector3(p.X, p.Y, level);
            }

            return new Vector3(0, 0, 0);
        }
        #endregion
    }

    public class DataSet
    {
        public BlockData BaseData;
        public byte SideByte;

        public List<Vector3> verts = new List<Vector3>();
        public List<Vector2> uvs = new List<Vector2>();
        public List<Vector3> normals = new List<Vector3>();
    }

    struct KeyStruct
    {
        public uint ID;
        public byte Metadata;
        public byte SideByte;

        public override bool Equals(object obj)
        {
            if (obj is KeyStruct)
            {
                KeyStruct o = (KeyStruct)obj;

                return (ID == o.ID && Metadata == o.Metadata && SideByte == o.SideByte);
            }
            return false;
        }

        public static bool operator ==(KeyStruct a, KeyStruct b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(KeyStruct a, KeyStruct b)
        {
            return !a.Equals(b);
        }
    }
}