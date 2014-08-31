using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftModelExporter.GeomGenerator
{
    public static class Triangulator
    {
        private static TriangulationAlgorithm _defaultAlgorithm = TriangulationAlgorithm.DTSweep;

        public static void Triangulate(PolygonSet ps)
        {
            TriangulationContext tcx = CreateContext(_defaultAlgorithm);
            foreach (Polygon p in ps.Polygons)
            {
                Triangulate(p);
            }
        }


        public static void Triangulate(Polygon p)
        {
            Triangulate(_defaultAlgorithm, p);
        }


        public static void Triangulate(ConstrainedPointSet cps)
        {
            Triangulate(_defaultAlgorithm, cps);
        }


        public static void Triangulate(PointSet ps)
        {
            Triangulate(_defaultAlgorithm, ps);
        }


        public static TriangulationContext CreateContext(TriangulationAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case TriangulationAlgorithm.DTSweep:
                default:
                    return new DTSweepContext();
            }
        }


        public static void Triangulate(TriangulationAlgorithm algorithm, ITriangulatable t)
        {
            TriangulationContext tcx;

            System.Console.WriteLine("Triangulating " + t.FileName);
            //        long time = System.nanoTime();
            tcx = CreateContext(algorithm);
            tcx.PrepareTriangulation(t);
            Triangulate(tcx);
            //        logger.info( "Triangulation of {} points [{}ms]", tcx.getPoints().size(), ( System.nanoTime() - time ) / 1e6 );
        }


        public static void Triangulate(TriangulationContext tcx)
        {
            switch (tcx.Algorithm)
            {
                case TriangulationAlgorithm.DTSweep:
                default:
                    DTSweep.Triangulate((DTSweepContext)tcx);
                    break;
            }
        }


        /// <summary>
        /// Will do a warmup run to let the JVM optimize the triangulation code -- or would if this were Java --MM
        /// </summary>
        public static void Warmup()
        {
#if false
                        /*
                         * After a method is run 10000 times, the Hotspot compiler will compile
                         * it into native code. Periodically, the Hotspot compiler may recompile
                         * the method. After an unspecified amount of time, then the compilation
                         * system should become quiet.
                         */
                        Polygon poly = PolygonGenerator.RandomCircleSweep2(50, 50000);
                        TriangulationProcess process = new TriangulationProcess();
                        process.triangulate(poly);
#endif
        }
    }

    public interface ITriangulatable
    {
        //IList<TriangulationPoint> Points { get; } // MM: Neither of these are used via interface (yet?)
        IList<DelaunayTriangle> Triangles { get; }
        TriangulationMode TriangulationMode { get; }
        string FileName { get; set; }
        bool DisplayFlipX { get; set; }
        bool DisplayFlipY { get; set; }
        float DisplayRotate { get; set; }
        double Precision { get; set; }
        double MinX { get; }
        double MaxX { get; }
        double MinY { get; }
        double MaxY { get; }
        Rect2D Bounds { get; }

        void Prepare(TriangulationContext tcx);
        void AddTriangle(DelaunayTriangle t);
        void AddTriangles(IEnumerable<DelaunayTriangle> list);
        void ClearTriangles();
    }

    public enum Orientation
    {
        CW,
        CCW,
        Collinear
    }

    public enum TriangulationAlgorithm
    {
        DTSweep
    }

    public class Edge
    {
        protected Point2D mP = null;
        protected Point2D mQ = null;

        public Point2D EdgeStart { get { return mP; } set { mP = value; } }
        public Point2D EdgeEnd { get { return mQ; } set { mQ = value; } }

        public Edge() { mP = null; mQ = null; }
        public Edge(Point2D edgeStart, Point2D edgeEnd)
        {
            mP = edgeStart;
            mQ = edgeEnd;
        }
    }


    public class TriangulationConstraint : Edge
    {
        private uint mContraintCode = 0;

        public TriangulationPoint P
        {
            get { return mP as TriangulationPoint; }
            set
            {
                // Note:  intentionally use != instead of !Equals() because we
                // WANT to compare pointer values here rather than VertexCode values
                if (value != null && mP != value)
                {
                    mP = value;
                    CalculateContraintCode();
                }
            }
        }
        public TriangulationPoint Q
        {
            get { return mQ as TriangulationPoint; }
            set
            {
                // Note:  intentionally use != instead of !Equals() because we
                // WANT to compare pointer values here rather than VertexCode values
                if (value != null && mQ != value)
                {
                    mQ = value;
                    CalculateContraintCode();
                }
            }
        }
        public uint ConstraintCode { get { return mContraintCode; } }


        /// <summary>
        /// Give two points in any order. Will always be ordered so
        /// that q.y > p.y and q.x > p.x if same y value 
        /// </summary>
        public TriangulationConstraint(TriangulationPoint p1, TriangulationPoint p2)
        {
            mP = p1;
            mQ = p2;
            if (p1.Y > p2.Y)
            {
                mQ = p1;
                mP = p2;
            }
            else if (p1.Y == p2.Y)
            {
                if (p1.X > p2.X)
                {
                    mQ = p1;
                    mP = p2;
                }
                else if (p1.X == p2.X)
                {
                    //                logger.info( "Failed to create constraint {}={}", p1, p2 );
                    //                throw new DuplicatePointException( p1 + "=" + p2 );
                    //                return;
                }
            }
            CalculateContraintCode();
        }


        public override string ToString()
        {
            return "[P=" + P.ToString() + ", Q=" + Q.ToString() + " : {" + mContraintCode.ToString() + "}]";
        }


        public void CalculateContraintCode()
        {
            mContraintCode = TriangulationConstraint.CalculateContraintCode(P, Q);
        }


        public static uint CalculateContraintCode(TriangulationPoint p, TriangulationPoint q)
        {
            if (p == null || p == null)
            {
                throw new ArgumentNullException();
            }

            uint constraintCode = MathUtil.Jenkins32Hash(BitConverter.GetBytes(p.VertexCode), 0);
            constraintCode = MathUtil.Jenkins32Hash(BitConverter.GetBytes(q.VertexCode), constraintCode);

            return constraintCode;
        }

    }

    public abstract class TriangulationContext
    {
        public TriangulationDebugContext DebugContext { get; protected set; }

        public readonly List<DelaunayTriangle> Triangles = new List<DelaunayTriangle>();
        public readonly List<TriangulationPoint> Points = new List<TriangulationPoint>(200);
        public TriangulationMode TriangulationMode { get; protected set; }
        public ITriangulatable Triangulatable { get; private set; }

        public int StepCount { get; private set; }

        public void Done()
        {
            StepCount++;
        }

        public abstract TriangulationAlgorithm Algorithm { get; }


        public virtual void PrepareTriangulation(ITriangulatable t)
        {
            Triangulatable = t;
            TriangulationMode = t.TriangulationMode;
            t.Prepare(this);

            //List<TriangulationConstraint> constraints = new List<TriangulationConstraint>();

            //Console.WriteLine("Points for " + t.FileName + ":");
            //Console.WriteLine("Idx,X,Y,VC,Edges");
            //int numPoints = Points.Count;
            //for (int i = 0; i < numPoints; ++i)
            //{
            //    StringBuilder sb = new StringBuilder(128);
            //    sb.Append(i.ToString());
            //    sb.Append(",");
            //    sb.Append(Points[i].X.ToString());
            //    sb.Append(",");
            //    sb.Append(Points[i].Y.ToString());
            //    sb.Append(",");
            //    sb.Append(Points[i].VertexCode.ToString());
            //    int numEdges = (Points[i].Edges != null) ? Points[i].Edges.Count : 0;
            //    for (int j = 0; j < numEdges; ++j)
            //    {
            //        TriangulationConstraint tc = Points[i].Edges[j];
            //        sb.Append(",");
            //        sb.Append(tc.ConstraintCode.ToString());
            //        constraints.Add(tc);
            //    }
            //    Console.WriteLine(sb.ToString());
            //}

            //int idx = 0;
            //Console.WriteLine("Constraints " + t.FileName + ":");
            //Console.WriteLine("EdgeIdx,Px,Py,PVC,Qx,Qy,QVC,ConstraintCode,Owner");
            //foreach (TriangulationConstraint tc in constraints)
            //{
            //    StringBuilder sb = new StringBuilder(128);

            //    sb.Append(idx.ToString());
            //    sb.Append(",");
            //    sb.Append(tc.P.X.ToString());
            //    sb.Append(",");
            //    sb.Append(tc.P.Y.ToString());
            //    sb.Append(",");
            //    sb.Append(tc.P.VertexCode.ToString());
            //    sb.Append(",");
            //    sb.Append(tc.Q.X.ToString());
            //    sb.Append(",");
            //    sb.Append(tc.Q.Y.ToString());
            //    sb.Append(",");
            //    sb.Append(tc.Q.VertexCode.ToString());
            //    sb.Append(",");
            //    sb.Append(tc.ConstraintCode.ToString());
            //    sb.Append(",");
            //    if (tc.Q.HasEdge(tc.P))
            //    {
            //        sb.Append("Q");
            //    }
            //    else
            //    {
            //        sb.Append("P");
            //    }
            //    Console.WriteLine(sb.ToString());

            //    ++idx;
            //}
        }


        public abstract TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b);


        public void Update(string message) { }


        public virtual void Clear()
        {
            Points.Clear();
            if (DebugContext != null)
            {
                DebugContext.Clear();
            }
            StepCount = 0;
        }


        public virtual bool IsDebugEnabled { get; protected set; }

        public DTSweepDebugContext DTDebugContext { get { return DebugContext as DTSweepDebugContext; } }
    }

    public abstract class TriangulationDebugContext
    {
        protected TriangulationContext _tcx;

        public TriangulationDebugContext(TriangulationContext tcx)
        {
            _tcx = tcx;
        }

        public abstract void Clear();
    }

    public enum TriangulationMode
    {
        Unconstrained,
        Constrained,
        Polygon
    }

    public class TriangulationPoint : Point2D
    {
        public static readonly double kVertexCodeDefaultPrecision = 3.0;

        public override double X
        {
            get { return mX; }
            set
            {
                if (value != mX)
                {
                    mX = value;
                    mVertexCode = TriangulationPoint.CreateVertexCode(mX, mY, kVertexCodeDefaultPrecision);

                    // Technically, we should change the ConstraintCodes of any edges that contain this point.
                    // We don't for 2 reasons:
                    // 1) Currently the only time we care about Vertex/Constraint Codes is when entering data in the point-set.
                    //    Once the data is being used by the algorithm, the point locations are (currently) not modified.
                    // 2) Since this Point's Edge list will only contain SOME of the edges that this point is a part of, 
                    //    there currently isn't a way to (easily) get any edges that contain this point but are not in this
                    //    point's edge list.
                }
            }
        }
        public override double Y
        {
            get { return mY; }
            set
            {
                if (value != mY)
                {
                    mY = value;
                    mVertexCode = TriangulationPoint.CreateVertexCode(mX, mY, kVertexCodeDefaultPrecision);

                    // Technically, we should change the ConstraintCodes of any edges that contain this point.
                    // We don't for 2 reasons:
                    // 1) Currently the only time we care about Vertex/Constraint Codes is when entering data in the point-set.
                    //    Once the data is being used by the algorithm, the point locations are (currently) not modified.
                    // 2) Since this Point's Edge list will only contain SOME of the edges that this point is a part of, 
                    //    there currently isn't a way to (easily) get any edges that contain this point but are not in this
                    //    point's edge list.
                }
            }
        }

        protected uint mVertexCode = 0;
        public uint VertexCode { get { return mVertexCode; } }

        // List of edges this point constitutes an upper ending point (CDT)
        public List<DTSweepConstraint> Edges { get; private set; }
        public bool HasEdges { get { return Edges != null; } }


        public TriangulationPoint(double x, double y)
            : this(x, y, kVertexCodeDefaultPrecision)
        {
        }


        public TriangulationPoint(double x, double y, double precision)
            : base(x, y)
        {
            mVertexCode = TriangulationPoint.CreateVertexCode(x, y, precision);
        }


        public override string ToString()
        {
            return base.ToString() + ":{" + mVertexCode.ToString() + "}";
        }


        public override int GetHashCode()
        {
            return (int)mVertexCode;
        }


        public override bool Equals(object obj)
        {
            TriangulationPoint p2 = obj as TriangulationPoint;
            if (p2 != null)
            {
                return mVertexCode == p2.VertexCode;
            }
            else
            {
                return base.Equals(obj);
            }
        }


        public override void Set(double x, double y)
        {
            if (x != mX || y != mY)
            {
                mX = x;
                mY = y;
                mVertexCode = TriangulationPoint.CreateVertexCode(mX, mY, kVertexCodeDefaultPrecision);
            }
        }


        public static uint CreateVertexCode(double x, double y, double precision)
        {
            float fx = (float)MathUtil.RoundWithPrecision(x, precision);
            float fy = (float)MathUtil.RoundWithPrecision(y, precision);
            uint vc = MathUtil.Jenkins32Hash(BitConverter.GetBytes(fx), 0);
            vc = MathUtil.Jenkins32Hash(BitConverter.GetBytes(fy), vc);

            return vc;
        }


        public void AddEdge(DTSweepConstraint e)
        {
            if (Edges == null)
            {
                Edges = new List<DTSweepConstraint>();
            }
            Edges.Add(e);
        }


        public bool HasEdge(TriangulationPoint p)
        {
            DTSweepConstraint tmp = null;
            return GetEdge(p, out tmp);
        }


        public bool GetEdge(TriangulationPoint p, out DTSweepConstraint edge)
        {
            edge = null;
            if (Edges == null || Edges.Count < 1 || p == null || p.Equals(this))
            {
                return false;
            }

            foreach (DTSweepConstraint sc in Edges)
            {
                if ((sc.P.Equals(this) && sc.Q.Equals(p)) || (sc.P.Equals(p) && sc.Q.Equals(this)))
                {
                    edge = sc;
                    return true;
                }
            }

            return false;
        }


        public static Point2D ToPoint2D(TriangulationPoint p)
        {
            return p as Point2D;
        }
    }


    public class TriangulationPointEnumerator : IEnumerator<TriangulationPoint>
    {
        protected IList<Point2D> mPoints;
        protected int position = -1;  // Enumerators are positioned before the first element until the first MoveNext() call.


        public TriangulationPointEnumerator(IList<Point2D> points)
        {
            mPoints = points;
        }

        public bool MoveNext()
        {
            position++;
            return (position < mPoints.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        void IDisposable.Dispose() { }

        Object IEnumerator.Current { get { return Current; } }

        public TriangulationPoint Current
        {
            get
            {
                if (position < 0 || position >= mPoints.Count)
                {
                    return null;
                }
                return mPoints[position] as TriangulationPoint;
            }
        }
    }


    public class TriangulationPointList : Point2DList
    {

    }

    public class DelaunayTriangle
    {

        public FixedArray3<TriangulationPoint> Points;
        public FixedArray3<DelaunayTriangle> Neighbors;
        private FixedBitArray3 mEdgeIsConstrained;
        public FixedBitArray3 EdgeIsConstrained { get { return mEdgeIsConstrained; } }
        public FixedBitArray3 EdgeIsDelaunay;
        public bool IsInterior { get; set; }

        public DelaunayTriangle(TriangulationPoint p1, TriangulationPoint p2, TriangulationPoint p3)
        {
            Points[0] = p1;
            Points[1] = p2;
            Points[2] = p3;
        }


        public int IndexOf(TriangulationPoint p)
        {
            int i = Points.IndexOf(p);
            if (i == -1)
            {
                throw new Exception("Calling index with a point that doesn't exist in triangle");
            }

            return i;
        }


        public int IndexCWFrom(TriangulationPoint p)
        {
            return (IndexOf(p) + 2) % 3;
        }


        public int IndexCCWFrom(TriangulationPoint p)
        {
            return (IndexOf(p) + 1) % 3;
        }


        public bool Contains(TriangulationPoint p)
        {
            return Points.Contains(p);
        }


        /// <summary>
        /// Update neighbor pointers
        /// </summary>
        /// <param name="p1">Point 1 of the shared edge</param>
        /// <param name="p2">Point 2 of the shared edge</param>
        /// <param name="t">This triangle's new neighbor</param>
        private void MarkNeighbor(TriangulationPoint p1, TriangulationPoint p2, DelaunayTriangle t)
        {
            int i = EdgeIndex(p1, p2);
            if (i == -1)
            {
                //throw new Exception("Error marking neighbors -- t doesn't contain edge p1-p2!");
            }
            else
            {
                Neighbors[i] = t;
            }
        }


        /// <summary>
        /// Exhaustive search to update neighbor pointers
        /// </summary>
        public void MarkNeighbor(DelaunayTriangle t)
        {
            if (t != null)
            {
                // Points of this triangle also belonging to t
                bool a = t.Contains(Points[0]);
                bool b = t.Contains(Points[1]);
                bool c = t.Contains(Points[2]);

                if (b && c)
                {
                    Neighbors[0] = t;
                    t.MarkNeighbor(Points[1], Points[2], this);
                }
                else if (a && c)
                {
                    Neighbors[1] = t;
                    t.MarkNeighbor(Points[0], Points[2], this);
                }
                else if (a && b)
                {
                    Neighbors[2] = t;
                    t.MarkNeighbor(Points[0], Points[1], this);
                }
                else
                {
                    //throw new Exception("Failed to mark neighbor, doesn't share an edge!");
                }
            }
        }


        public void ClearNeighbors()
        {
            Neighbors[0] = Neighbors[1] = Neighbors[2] = null;
        }


        public void ClearNeighbor(DelaunayTriangle triangle)
        {
            if (Neighbors[0] == triangle)
            {
                Neighbors[0] = null;
            }
            else if (Neighbors[1] == triangle)
            {
                Neighbors[1] = null;
            }
            else if (Neighbors[2] == triangle)
            {
                Neighbors[2] = null;
            }
        }

        /// <summary>
        /// Clears all references to all other triangles and points
        /// </summary>
        public void Clear()
        {
            DelaunayTriangle t;
            for (int i = 0; i < 3; i++)
            {
                t = Neighbors[i];
                if (t != null)
                {
                    t.ClearNeighbor(this);
                }
            }
            ClearNeighbors();
            Points[0] = Points[1] = Points[2] = null;
        }

        /// <param name="t">Opposite triangle</param>
        /// <param name="p">The point in t that isn't shared between the triangles</param>
        public TriangulationPoint OppositePoint(DelaunayTriangle t, TriangulationPoint p)
        {
            return PointCWFrom(t.PointCWFrom(p));
        }


        public DelaunayTriangle NeighborCWFrom(TriangulationPoint point)
        {
            return Neighbors[(Points.IndexOf(point) + 1) % 3];
        }


        public DelaunayTriangle NeighborCCWFrom(TriangulationPoint point)
        {
            return Neighbors[(Points.IndexOf(point) + 2) % 3];
        }


        public DelaunayTriangle NeighborAcrossFrom(TriangulationPoint point)
        {
            return Neighbors[Points.IndexOf(point)];
        }


        public TriangulationPoint PointCCWFrom(TriangulationPoint point)
        {
            return Points[(IndexOf(point) + 1) % 3];
        }


        public TriangulationPoint PointCWFrom(TriangulationPoint point)
        {
            return Points[(IndexOf(point) + 2) % 3];
        }


        private void RotateCW()
        {
            var t = Points[2];
            Points[2] = Points[1];
            Points[1] = Points[0];
            Points[0] = t;
        }


        /// <summary>
        /// Legalize triangle by rotating clockwise around oPoint
        /// </summary>
        /// <param name="oPoint">The origin point to rotate around</param>
        /// <param name="nPoint">???</param>
        public void Legalize(TriangulationPoint oPoint, TriangulationPoint nPoint)
        {
            RotateCW();
            Points[IndexCCWFrom(oPoint)] = nPoint;
        }


        public override string ToString()
        {
            return Points[0] + "," + Points[1] + "," + Points[2];
        }


        /// <summary>
        /// Finalize edge marking
        /// </summary>
        public void MarkNeighborEdges()
        {
            for (int i = 0; i < 3; i++)
            {
                if (EdgeIsConstrained[i] && Neighbors[i] != null)
                {
                    Neighbors[i].MarkConstrainedEdge(Points[(i + 1) % 3], Points[(i + 2) % 3]);
                }
            }
        }


        public void MarkEdge(DelaunayTriangle triangle)
        {
            for (int i = 0; i < 3; i++) if (EdgeIsConstrained[i])
                {
                    triangle.MarkConstrainedEdge(Points[(i + 1) % 3], Points[(i + 2) % 3]);
                }
        }

        public void MarkEdge(List<DelaunayTriangle> tList)
        {
            foreach (DelaunayTriangle t in tList)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (t.EdgeIsConstrained[i])
                    {
                        MarkConstrainedEdge(t.Points[(i + 1) % 3], t.Points[(i + 2) % 3]);
                    }
                }
            }
        }


        public void MarkConstrainedEdge(int index)
        {
            mEdgeIsConstrained[index] = true;
        }


        public void MarkConstrainedEdge(DTSweepConstraint edge)
        {
            MarkConstrainedEdge(edge.P, edge.Q);
        }


        /// <summary>
        /// Mark edge as constrained
        /// </summary>
        public void MarkConstrainedEdge(TriangulationPoint p, TriangulationPoint q)
        {
            int i = EdgeIndex(p, q);
            if (i != -1)
            {
                mEdgeIsConstrained[i] = true;
            }
        }


        public double Area()
        {
            double b = Points[0].X - Points[1].X;
            double h = Points[2].Y - Points[1].Y;

            return Math.Abs((b * h * 0.5f));
        }

        public TriangulationPoint Centroid()
        {
            double cx = (Points[0].X + Points[1].X + Points[2].X) / 3f;
            double cy = (Points[0].Y + Points[1].Y + Points[2].Y) / 3f;
            return new TriangulationPoint(cx, cy);
        }


        /// <summary>
        /// Get the index of the neighbor that shares this edge (or -1 if it isn't shared)
        /// </summary>
        /// <returns>index of the shared edge or -1 if edge isn't shared</returns>
        public int EdgeIndex(TriangulationPoint p1, TriangulationPoint p2)
        {
            int i1 = Points.IndexOf(p1);
            int i2 = Points.IndexOf(p2);

            // Points of this triangle in the edge p1-p2
            bool a = (i1 == 0 || i2 == 0);
            bool b = (i1 == 1 || i2 == 1);
            bool c = (i1 == 2 || i2 == 2);

            if (b && c)
            {
                return 0;
            }
            if (a && c)
            {
                return 1;
            }
            if (a && b)
            {
                return 2;
            }

            return -1;
        }


        public bool GetConstrainedEdgeCCW(TriangulationPoint p) { return EdgeIsConstrained[(IndexOf(p) + 2) % 3]; }
        public bool GetConstrainedEdgeCW(TriangulationPoint p) { return EdgeIsConstrained[(IndexOf(p) + 1) % 3]; }
        public bool GetConstrainedEdgeAcross(TriangulationPoint p) { return EdgeIsConstrained[IndexOf(p)]; }

        protected void SetConstrainedEdge(int idx, bool ce)
        {
            //if (ce == false && EdgeIsConstrained[idx])
            //{
            //    DTSweepConstraint edge = null;
            //    if (GetEdge(idx, out edge))
            //    {
            //        Console.WriteLine("Removing pre-defined constraint from edge " + edge.ToString());
            //    }
            //}
            mEdgeIsConstrained[idx] = ce;
        }
        public void SetConstrainedEdgeCCW(TriangulationPoint p, bool ce)
        {
            int idx = (IndexOf(p) + 2) % 3;
            SetConstrainedEdge(idx, ce);
        }
        public void SetConstrainedEdgeCW(TriangulationPoint p, bool ce)
        {
            int idx = (IndexOf(p) + 1) % 3;
            SetConstrainedEdge(idx, ce);
        }
        public void SetConstrainedEdgeAcross(TriangulationPoint p, bool ce)
        {
            int idx = IndexOf(p);
            SetConstrainedEdge(idx, ce);
        }

        public bool GetDelaunayEdgeCCW(TriangulationPoint p) { return EdgeIsDelaunay[(IndexOf(p) + 2) % 3]; }
        public bool GetDelaunayEdgeCW(TriangulationPoint p) { return EdgeIsDelaunay[(IndexOf(p) + 1) % 3]; }
        public bool GetDelaunayEdgeAcross(TriangulationPoint p) { return EdgeIsDelaunay[IndexOf(p)]; }
        public void SetDelaunayEdgeCCW(TriangulationPoint p, bool ce) { EdgeIsDelaunay[(IndexOf(p) + 2) % 3] = ce; }
        public void SetDelaunayEdgeCW(TriangulationPoint p, bool ce) { EdgeIsDelaunay[(IndexOf(p) + 1) % 3] = ce; }
        public void SetDelaunayEdgeAcross(TriangulationPoint p, bool ce) { EdgeIsDelaunay[IndexOf(p)] = ce; }


        public bool GetEdge(int idx, out DTSweepConstraint edge)
        {
            edge = null;
            if (idx < 0 || idx > 2)
            {
                return false;
            }
            TriangulationPoint p1 = Points[(idx + 1) % 3];
            TriangulationPoint p2 = Points[(idx + 2) % 3];
            if (p1.GetEdge(p2, out edge))
            {
                return true;
            }
            else if (p2.GetEdge(p1, out edge))
            {
                return true;
            }

            return false;
        }


        public bool GetEdgeCCW(TriangulationPoint p, out DTSweepConstraint edge)
        {
            int pointIndex = IndexOf(p);
            int edgeIdx = (pointIndex + 2) % 3;

            return GetEdge(edgeIdx, out edge);
        }

        public bool GetEdgeCW(TriangulationPoint p, out DTSweepConstraint edge)
        {
            int pointIndex = IndexOf(p);
            int edgeIdx = (pointIndex + 1) % 3;

            return GetEdge(edgeIdx, out edge);
        }

        public bool GetEdgeAcross(TriangulationPoint p, out DTSweepConstraint edge)
        {
            int pointIndex = IndexOf(p);
            int edgeIdx = pointIndex;

            return GetEdge(edgeIdx, out edge);
        }

    }

    public class AdvancingFront
    {
        public AdvancingFrontNode Head;
        public AdvancingFrontNode Tail;
        protected AdvancingFrontNode Search;

        public AdvancingFront(AdvancingFrontNode head, AdvancingFrontNode tail)
        {
            this.Head = head;
            this.Tail = tail;
            this.Search = head;
            AddNode(head);
            AddNode(tail);
        }

        public void AddNode(AdvancingFrontNode node) { }
        public void RemoveNode(AdvancingFrontNode node) { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AdvancingFrontNode node = Head;
            while (node != Tail)
            {
                sb.Append(node.Point.X).Append("->");
                node = node.Next;
            }
            sb.Append(Tail.Point.X);
            return sb.ToString();
        }

        /// <summary>
        /// MM:  This seems to be used by LocateNode to guess a position in the implicit linked list of AdvancingFrontNodes near x
        ///      Removed an overload that depended on this being exact
        /// </summary>
        private AdvancingFrontNode FindSearchNode(double x)
        {
            return Search;
        }

        /// <summary>
        /// We use a balancing tree to locate a node smaller or equal to given key value (in theory)
        /// </summary>
        public AdvancingFrontNode LocateNode(TriangulationPoint point)
        {
            return LocateNode(point.X);
        }

        private AdvancingFrontNode LocateNode(double x)
        {
            AdvancingFrontNode node = FindSearchNode(x);
            if (node != null)
            {
                if (x < node.Value)
                {
                    while ((node = node.Prev) != null)
                    {
                        if (x >= node.Value)
                        {
                            Search = node;
                            return node;
                        }
                    }
                }
                else
                {
                    while ((node = node.Next) != null)
                    {
                        if (x < node.Value)
                        {
                            Search = node.Prev;
                            return node.Prev;
                        }
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// This implementation will use simple node traversal algorithm to find a point on the front
        /// </summary>
        public AdvancingFrontNode LocatePoint(TriangulationPoint point)
        {
            double px = point.X;
            AdvancingFrontNode node = FindSearchNode(px);
            if (node != null)
            {
                double nx = node.Point.X;

                if (px == nx)
                {
                    if (point != node.Point)
                    {
                        // We might have two nodes with same x value for a short time
                        if (point == node.Prev.Point)
                        {
                            node = node.Prev;
                        }
                        else if (point == node.Next.Point)
                        {
                            node = node.Next;
                        }
                        else
                        {
                            throw new Exception("Failed to find Node for given afront point");
                        }
                    }
                }
                else if (px < nx)
                {
                    while ((node = node.Prev) != null)
                    {
                        if (point == node.Point)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    while ((node = node.Next) != null)
                    {
                        if (point == node.Point)
                        {
                            break;
                        }
                    }
                }
                Search = node;
            }

            return node;
        }
    }

    public class AdvancingFrontNode
    {
        public AdvancingFrontNode Next;
        public AdvancingFrontNode Prev;
        public double Value;
        public TriangulationPoint Point;
        public DelaunayTriangle Triangle;

        public AdvancingFrontNode(TriangulationPoint point)
        {
            this.Point = point;
            Value = point.X;
        }

        public bool HasNext { get { return Next != null; } }
        public bool HasPrev { get { return Prev != null; } }
    }

    public static class DTSweep
    {
        private const double PI_div2 = Math.PI / 2;
        private const double PI_3div4 = 3 * Math.PI / 4;


        /// <summary>
        /// Triangulate simple polygon with holes
        /// </summary>
        public static void Triangulate(DTSweepContext tcx)
        {
            tcx.CreateAdvancingFront();

            Sweep(tcx);

            FixupConstrainedEdges(tcx);

            // Finalize triangulation
            if (tcx.TriangulationMode == TriangulationMode.Polygon)
            {
                FinalizationPolygon(tcx);
            }
            else
            {
                FinalizationConvexHull(tcx);
                if (tcx.TriangulationMode == TriangulationMode.Constrained)
                {
                    // work in progress.  When it's done, call FinalizationConstraints INSTEAD of tcx.FinalizeTriangulation
                    //FinalizationConstraints(tcx);

                    tcx.FinalizeTriangulation();
                }
                else
                {
                    tcx.FinalizeTriangulation();
                }
            }

            tcx.Done();
        }


        /// <summary>
        /// Start sweeping the Y-sorted point set from bottom to top
        /// </summary>
        private static void Sweep(DTSweepContext tcx)
        {
            var points = tcx.Points;
            TriangulationPoint point;
            AdvancingFrontNode node;

            for (int i = 1; i < points.Count; i++)
            {
                point = points[i];
                node = PointEvent(tcx, point);

                if (node != null && point.HasEdges)
                {
                    foreach (DTSweepConstraint e in point.Edges)
                    {
                        if (tcx.IsDebugEnabled)
                        {
                            tcx.DTDebugContext.ActiveConstraint = e;
                        }
                        EdgeEvent(tcx, e, node);
                    }
                }
                tcx.Update(null);
            }
        }


        private static void FixupConstrainedEdges(DTSweepContext tcx)
        {
            foreach (DelaunayTriangle t in tcx.Triangles)
            {
                for (int i = 0; i < 3; ++i)
                {
                    bool isConstrained = t.GetConstrainedEdgeCCW(t.Points[i]);
                    if (!isConstrained)
                    {
                        DTSweepConstraint edge = null;
                        bool hasConstrainedEdge = t.GetEdgeCCW(t.Points[i], out edge);
                        if (hasConstrainedEdge)
                        {
                            t.MarkConstrainedEdge((i + 2) % 3);
                            //t.MarkConstrainedEdgeCCW(t.Points[i]);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// If this is a Delaunay Triangulation of a pointset we need to fill so the triangle mesh gets a ConvexHull 
        /// </summary>
        private static void FinalizationConvexHull(DTSweepContext tcx)
        {
            AdvancingFrontNode n1, n2;
            DelaunayTriangle t1, t2;
            TriangulationPoint first, p1;

            n1 = tcx.Front.Head.Next;
            n2 = n1.Next;
            first = n1.Point;

            TurnAdvancingFrontConvex(tcx, n1, n2);

            // Lets remove triangles connected to the two "algorithm" points
            // XXX: When the first three nodes are points in a triangle we need to do a flip before
            // removing triangles or we will lose a valid triangle.
            // Same for last three nodes!
            // !!! If I implement ConvexHull for lower right and left boundary this fix should not be
            // needed and the removed triangles will be added again by default

            n1 = tcx.Front.Tail.Prev;
            if (n1.Triangle.Contains(n1.Next.Point) && n1.Triangle.Contains(n1.Prev.Point))
            {
                t1 = n1.Triangle.NeighborAcrossFrom(n1.Point);
                RotateTrianglePair(n1.Triangle, n1.Point, t1, t1.OppositePoint(n1.Triangle, n1.Point));
                tcx.MapTriangleToNodes(n1.Triangle);
                tcx.MapTriangleToNodes(t1);
            }
            n1 = tcx.Front.Head.Next;
            if (n1.Triangle.Contains(n1.Prev.Point) && n1.Triangle.Contains(n1.Next.Point))
            {
                t1 = n1.Triangle.NeighborAcrossFrom(n1.Point);
                RotateTrianglePair(n1.Triangle, n1.Point, t1, t1.OppositePoint(n1.Triangle, n1.Point));
                tcx.MapTriangleToNodes(n1.Triangle);
                tcx.MapTriangleToNodes(t1);
            }

            // Lower right boundary 
            first = tcx.Front.Head.Point;
            n2 = tcx.Front.Tail.Prev;
            t1 = n2.Triangle;
            p1 = n2.Point;
            n2.Triangle = null;
            do
            {
                tcx.RemoveFromList(t1);
                p1 = t1.PointCCWFrom(p1);
                if (p1 == first)
                {
                    break;
                }
                t2 = t1.NeighborCCWFrom(p1);
                t1.Clear();
                t1 = t2;
            } while (true);

            // Lower left boundary
            first = tcx.Front.Head.Next.Point;
            p1 = t1.PointCWFrom(tcx.Front.Head.Point);
            t2 = t1.NeighborCWFrom(tcx.Front.Head.Point);
            t1.Clear();
            t1 = t2;
            while (p1 != first)
            {
                tcx.RemoveFromList(t1);
                p1 = t1.PointCCWFrom(p1);
                t2 = t1.NeighborCCWFrom(p1);
                t1.Clear();
                t1 = t2;
            }

            // Remove current head and tail node now that we have removed all triangles attached
            // to them. Then set new head and tail node points
            tcx.Front.Head = tcx.Front.Head.Next;
            tcx.Front.Head.Prev = null;
            tcx.Front.Tail = tcx.Front.Tail.Prev;
            tcx.Front.Tail.Next = null;
        }


        /// <summary>
        /// We will traverse the entire advancing front and fill it to form a convex hull.
        /// </summary>
        private static void TurnAdvancingFrontConvex(DTSweepContext tcx, AdvancingFrontNode b, AdvancingFrontNode c)
        {
            AdvancingFrontNode first = b;
            while (c != tcx.Front.Tail)
            {
                if (tcx.IsDebugEnabled)
                {
                    tcx.DTDebugContext.ActiveNode = c;
                }

                if (TriangulationUtil.Orient2d(b.Point, c.Point, c.Next.Point) == Orientation.CCW)
                {
                    // [b,c,d] Concave - fill around c
                    Fill(tcx, c);
                    c = c.Next;
                }
                else
                {
                    // [b,c,d] Convex
                    if (b != first && TriangulationUtil.Orient2d(b.Prev.Point, b.Point, c.Point) == Orientation.CCW)
                    {
                        // [a,b,c] Concave - fill around b
                        Fill(tcx, b);
                        b = b.Prev;
                    }
                    else
                    {
                        // [a,b,c] Convex - nothing to fill
                        b = c;
                        c = c.Next;
                    }
                }
            }
        }


        private static void FinalizationPolygon(DTSweepContext tcx)
        {
            // Get an Internal triangle to start with
            DelaunayTriangle t = tcx.Front.Head.Next.Triangle;
            TriangulationPoint p = tcx.Front.Head.Next.Point;
            while (!t.GetConstrainedEdgeCW(p))
            {
                DelaunayTriangle tTmp = t.NeighborCCWFrom(p);
                if (tTmp == null)
                {
                    break;
                }
                t = tTmp;
            }

            // Collect interior triangles constrained by edges
            tcx.MeshClean(t);
        }


        /// <summary>
        /// NOTE: WORK IN PROGRESS - for now this will just clean out all triangles from
        /// inside the outermost holes without paying attention to holes within holes..
        /// hence the work in progress :)
        /// 
        /// Removes triangles inside "holes" (that are not inside of other holes already)
        /// 
        /// In the example below, assume that triangle ABC is a user-defined "hole".  Thus
        /// any triangles inside it (that aren't inside yet another user-defined hole inside
        /// triangle ABC) should get removed.  In this case, since there are no user-defined
        /// holes inside ABC, we would remove triangles ADE, BCE, and CDE.  We would also 
        /// need to combine the appropriate edges so that we end up with just triangle ABC
        ///
        ///          E
        /// A +------+-----+ B              A +-----------+ B
        ///    \    /|    /                    \         /
        ///     \  / |   /                      \       /
        ///    D +   |  /        ======>         \     /
        ///       \  | /                          \   /
        ///        \ |/                            \ /
        ///          +                              +
        ///          C                              C
        ///          
        /// </summary>
        private static void FinalizationConstraints(DTSweepContext tcx)
        {
            // Get an Internal triangle to start with
            DelaunayTriangle t = tcx.Front.Head.Triangle;
            TriangulationPoint p = tcx.Front.Head.Point;
            while (!t.GetConstrainedEdgeCW(p))
            {
                DelaunayTriangle tTmp = t.NeighborCCWFrom(p);
                if (tTmp == null)
                {
                    break;
                }
                t = tTmp;
            }

            // Collect interior triangles constrained by edges
            tcx.MeshClean(t);
        }


        /// <summary>
        /// Find closes node to the left of the new point and
        /// create a new triangle. If needed new holes and basins
        /// will be filled to.
        /// </summary>
        private static AdvancingFrontNode PointEvent(DTSweepContext tcx, TriangulationPoint point)
        {
            AdvancingFrontNode node, newNode;

            node = tcx.LocateNode(point);
            if (tcx.IsDebugEnabled)
            {
                tcx.DTDebugContext.ActiveNode = node;
            }
            if (node == null || point == null)
            {
                return null;
            }
            newNode = NewFrontTriangle(tcx, point, node);

            // Only need to check +epsilon since point never have smaller 
            // x value than node due to how we fetch nodes from the front
            if (point.X <= node.Point.X + MathUtil.EPSILON)
            {
                Fill(tcx, node);
            }

            tcx.AddNode(newNode);

            FillAdvancingFront(tcx, newNode);
            return newNode;
        }


        /// <summary>
        /// Creates a new front triangle and legalize it
        /// </summary>
        private static AdvancingFrontNode NewFrontTriangle(DTSweepContext tcx, TriangulationPoint point, AdvancingFrontNode node)
        {
            AdvancingFrontNode newNode;
            DelaunayTriangle triangle;

            triangle = new DelaunayTriangle(point, node.Point, node.Next.Point);
            triangle.MarkNeighbor(node.Triangle);
            tcx.Triangles.Add(triangle);

            newNode = new AdvancingFrontNode(point);
            newNode.Next = node.Next;
            newNode.Prev = node;
            node.Next.Prev = newNode;
            node.Next = newNode;

            tcx.AddNode(newNode); // XXX: BST

            if (tcx.IsDebugEnabled)
            {
                tcx.DTDebugContext.ActiveNode = newNode;
            }

            if (!Legalize(tcx, triangle))
            {
                tcx.MapTriangleToNodes(triangle);
            }

            return newNode;
        }


        private static void EdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
        {
            try
            {
                tcx.EdgeEvent.ConstrainedEdge = edge;
                tcx.EdgeEvent.Right = edge.P.X > edge.Q.X;

                if (tcx.IsDebugEnabled)
                {
                    tcx.DTDebugContext.PrimaryTriangle = node.Triangle;
                }

                if (IsEdgeSideOfTriangle(node.Triangle, edge.P, edge.Q))
                {
                    return;
                }

                // For now we will do all needed filling
                // TODO: integrate with flip process might give some better performance 
                //       but for now this avoid the issue with cases that needs both flips and fills
                FillEdgeEvent(tcx, edge, node);

                EdgeEvent(tcx, edge.P, edge.Q, node.Triangle, edge.Q);
            }
            catch (PointOnEdgeException)
            {
                //Debug.WriteLine( String.Format( "Warning: Skipping Edge: {0}", e.Message ) );
                throw;
            }
        }


        private static void FillEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
        {
            if (tcx.EdgeEvent.Right)
            {
                FillRightAboveEdgeEvent(tcx, edge, node);
            }
            else
            {
                FillLeftAboveEdgeEvent(tcx, edge, node);
            }
        }


        private static void FillRightConcaveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
        {
            Fill(tcx, node.Next);
            if (node.Next.Point != edge.P)
            {
                // Next above or below edge?
                if (TriangulationUtil.Orient2d(edge.Q, node.Next.Point, edge.P) == Orientation.CCW)
                {
                    // Below
                    if (TriangulationUtil.Orient2d(node.Point, node.Next.Point, node.Next.Next.Point) == Orientation.CCW)
                    {
                        // Next is concave
                        FillRightConcaveEdgeEvent(tcx, edge, node);
                    }
                    else
                    {
                        // Next is convex
                    }
                }
            }
        }


        private static void FillRightConvexEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
        {
            // Next concave or convex?
            if (TriangulationUtil.Orient2d(node.Next.Point, node.Next.Next.Point, node.Next.Next.Next.Point) == Orientation.CCW)
            {
                // Concave
                FillRightConcaveEdgeEvent(tcx, edge, node.Next);
            }
            else
            {
                // Convex
                // Next above or below edge?
                if (TriangulationUtil.Orient2d(edge.Q, node.Next.Next.Point, edge.P) == Orientation.CCW)
                {
                    // Below
                    FillRightConvexEdgeEvent(tcx, edge, node.Next);
                }
                else
                {
                    // Above
                }
            }
        }

        private static void FillRightBelowEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
        {
            if (tcx.IsDebugEnabled)
            {
                tcx.DTDebugContext.ActiveNode = node;
            }

            if (node.Point.X < edge.P.X)
            {
                // needed?
                if (TriangulationUtil.Orient2d(node.Point, node.Next.Point, node.Next.Next.Point) == Orientation.CCW)
                {
                    // Concave 
                    FillRightConcaveEdgeEvent(tcx, edge, node);
                }
                else
                {
                    // Convex
                    FillRightConvexEdgeEvent(tcx, edge, node);
                    // Retry this one
                    FillRightBelowEdgeEvent(tcx, edge, node);
                }
            }
        }


        private static void FillRightAboveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
        {
            while (node.Next.Point.X < edge.P.X)
            {
                if (tcx.IsDebugEnabled) { tcx.DTDebugContext.ActiveNode = node; }
                // Check if next node is below the edge
                Orientation o1 = TriangulationUtil.Orient2d(edge.Q, node.Next.Point, edge.P);
                if (o1 == Orientation.CCW)
                {
                    FillRightBelowEdgeEvent(tcx, edge, node);
                }
                else
                {
                    node = node.Next;
                }
            }
        }


        private static void FillLeftConvexEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
        {
            // Next concave or convex?
            if (TriangulationUtil.Orient2d(node.Prev.Point, node.Prev.Prev.Point, node.Prev.Prev.Prev.Point) == Orientation.CW)
            {
                // Concave
                FillLeftConcaveEdgeEvent(tcx, edge, node.Prev);
            }
            else
            {
                // Convex
                // Next above or below edge?
                if (TriangulationUtil.Orient2d(edge.Q, node.Prev.Prev.Point, edge.P) == Orientation.CW)
                {
                    // Below
                    FillLeftConvexEdgeEvent(tcx, edge, node.Prev);
                }
                else
                {
                    // Above
                }
            }
        }


        private static void FillLeftConcaveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
        {
            Fill(tcx, node.Prev);
            if (node.Prev.Point != edge.P)
            {
                // Next above or below edge?
                if (TriangulationUtil.Orient2d(edge.Q, node.Prev.Point, edge.P) == Orientation.CW)
                {
                    // Below
                    if (TriangulationUtil.Orient2d(node.Point, node.Prev.Point, node.Prev.Prev.Point) == Orientation.CW)
                    {
                        // Next is concave
                        FillLeftConcaveEdgeEvent(tcx, edge, node);
                    }
                    else
                    {
                        // Next is convex
                    }
                }
            }
        }


        private static void FillLeftBelowEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
        {
            if (tcx.IsDebugEnabled)
                tcx.DTDebugContext.ActiveNode = node;

            if (node.Point.X > edge.P.X)
            {
                if (TriangulationUtil.Orient2d(node.Point, node.Prev.Point, node.Prev.Prev.Point) == Orientation.CW)
                {
                    // Concave 
                    FillLeftConcaveEdgeEvent(tcx, edge, node);
                }
                else
                {
                    // Convex
                    FillLeftConvexEdgeEvent(tcx, edge, node);
                    // Retry this one
                    FillLeftBelowEdgeEvent(tcx, edge, node);
                }

            }
        }


        private static void FillLeftAboveEdgeEvent(DTSweepContext tcx, DTSweepConstraint edge, AdvancingFrontNode node)
        {
            while (node.Prev.Point.X > edge.P.X)
            {
                if (tcx.IsDebugEnabled)
                {
                    tcx.DTDebugContext.ActiveNode = node;
                }
                // Check if next node is below the edge
                Orientation o1 = TriangulationUtil.Orient2d(edge.Q, node.Prev.Point, edge.P);
                if (o1 == Orientation.CW)
                {
                    FillLeftBelowEdgeEvent(tcx, edge, node);
                }
                else
                {
                    node = node.Prev;
                }
            }
        }


        private static bool IsEdgeSideOfTriangle(DelaunayTriangle triangle, TriangulationPoint ep, TriangulationPoint eq)
        {
            if (triangle != null)
            {
                int index = triangle.EdgeIndex(ep, eq);
                if (index == -1)
                {
                    return false;
                }
                triangle.MarkConstrainedEdge(index);
                triangle = triangle.Neighbors[index];
                if (triangle != null)
                {
                    triangle.MarkConstrainedEdge(ep, eq);
                }
            }
            return true;
        }


        private static void EdgeEvent(DTSweepContext tcx, TriangulationPoint ep, TriangulationPoint eq, DelaunayTriangle triangle, TriangulationPoint point)
        {
            TriangulationPoint p1, p2;

            if (tcx.IsDebugEnabled)
            {
                tcx.DTDebugContext.PrimaryTriangle = triangle;
            }

            if (IsEdgeSideOfTriangle(triangle, ep, eq))
            {
                return;
            }

            p1 = triangle.PointCCWFrom(point);
            Orientation o1 = TriangulationUtil.Orient2d(eq, p1, ep);
            if (o1 == Orientation.Collinear)
            {
                if (triangle.Contains(eq) && triangle.Contains(p1))
                {
                    triangle.MarkConstrainedEdge(eq, p1);
                    // We are modifying the constraint maybe it would be better to
                    // not change the given constraint and just keep a variable for the new constraint
                    tcx.EdgeEvent.ConstrainedEdge.Q = p1;
                    triangle = triangle.NeighborAcrossFrom(point);
                    EdgeEvent(tcx, ep, p1, triangle, p1);
                }
                else
                {
                    throw new PointOnEdgeException("EdgeEvent - Point on constrained edge not supported yet", ep, eq, p1);
                }
                if (tcx.IsDebugEnabled)
                {
                    Console.WriteLine("EdgeEvent - Point on constrained edge");
                }

                return;
            }

            p2 = triangle.PointCWFrom(point);
            Orientation o2 = TriangulationUtil.Orient2d(eq, p2, ep);
            if (o2 == Orientation.Collinear)
            {
                if (triangle.Contains(eq) && triangle.Contains(p2))
                {
                    triangle.MarkConstrainedEdge(eq, p2);
                    // We are modifying the constraint maybe it would be better to
                    // not change the given constraint and just keep a variable for the new constraint
                    tcx.EdgeEvent.ConstrainedEdge.Q = p2;
                    triangle = triangle.NeighborAcrossFrom(point);
                    EdgeEvent(tcx, ep, p2, triangle, p2);
                }
                else
                {
                    throw new PointOnEdgeException("EdgeEvent - Point on constrained edge not supported yet", ep, eq, p2);
                }
                if (tcx.IsDebugEnabled)
                {
                    Console.WriteLine("EdgeEvent - Point on constrained edge");
                }

                return;
            }

            if (o1 == o2)
            {
                // Need to decide if we are rotating CW or CCW to get to a triangle
                // that will cross edge
                if (o1 == Orientation.CW)
                {
                    triangle = triangle.NeighborCCWFrom(point);
                }
                else
                {
                    triangle = triangle.NeighborCWFrom(point);
                }
                EdgeEvent(tcx, ep, eq, triangle, point);
            }
            else
            {
                // This triangle crosses constraint so lets flippin start!
                FlipEdgeEvent(tcx, ep, eq, triangle, point);
            }
        }


        private static void FlipEdgeEvent(DTSweepContext tcx, TriangulationPoint ep, TriangulationPoint eq, DelaunayTriangle t, TriangulationPoint p)
        {
            DelaunayTriangle ot = t.NeighborAcrossFrom(p);
            TriangulationPoint op = ot.OppositePoint(t, p);

            if (ot == null)
            {
                // If we want to integrate the fillEdgeEvent do it here
                // With current implementation we should never get here
                throw new InvalidOperationException("[BUG:FIXME] FLIP failed due to missing triangle");
            }

            if (tcx.IsDebugEnabled)
            {
                tcx.DTDebugContext.PrimaryTriangle = t;
                tcx.DTDebugContext.SecondaryTriangle = ot;
            } // TODO: remove

            bool inScanArea = TriangulationUtil.InScanArea(p, t.PointCCWFrom(p), t.PointCWFrom(p), op);
            if (inScanArea)
            {
                // Lets rotate shared edge one vertex CW
                RotateTrianglePair(t, p, ot, op);
                tcx.MapTriangleToNodes(t);
                tcx.MapTriangleToNodes(ot);

                if (p == eq && op == ep)
                {
                    if (eq == tcx.EdgeEvent.ConstrainedEdge.Q && ep == tcx.EdgeEvent.ConstrainedEdge.P)
                    {
                        if (tcx.IsDebugEnabled)
                        {
                            Console.WriteLine("[FLIP] - constrained edge done"); // TODO: remove
                        }
                        t.MarkConstrainedEdge(ep, eq);
                        ot.MarkConstrainedEdge(ep, eq);
                        Legalize(tcx, t);
                        Legalize(tcx, ot);
                    }
                    else
                    {
                        if (tcx.IsDebugEnabled)
                        {
                            Console.WriteLine("[FLIP] - subedge done"); // TODO: remove
                        }
                        // XXX: I think one of the triangles should be legalized here?
                    }
                }
                else
                {
                    if (tcx.IsDebugEnabled)
                    {
                        Console.WriteLine("[FLIP] - flipping and continuing with triangle still crossing edge"); // TODO: remove
                    }
                    Orientation o = TriangulationUtil.Orient2d(eq, op, ep);
                    t = NextFlipTriangle(tcx, o, t, ot, p, op);
                    FlipEdgeEvent(tcx, ep, eq, t, p);
                }
            }
            else
            {
                TriangulationPoint newP = null;
                if (NextFlipPoint(ep, eq, ot, op, out newP))
                {
                    FlipScanEdgeEvent(tcx, ep, eq, t, ot, newP);
                    EdgeEvent(tcx, ep, eq, t, p);
                }
            }
        }


        /// <summary>
        /// When we need to traverse from one triangle to the next we need 
        /// the point in current triangle that is the opposite point to the next
        /// triangle. 
        /// </summary>
        private static bool NextFlipPoint(TriangulationPoint ep, TriangulationPoint eq, DelaunayTriangle ot, TriangulationPoint op, out TriangulationPoint newP)
        {
            newP = null;
            Orientation o2d = TriangulationUtil.Orient2d(eq, op, ep);
            switch (o2d)
            {
                case Orientation.CW:
                    newP = ot.PointCCWFrom(op);
                    return true;
                case Orientation.CCW:
                    newP = ot.PointCWFrom(op);
                    return true;
                case Orientation.Collinear:
                    // TODO: implement support for point on constraint edge
                    //throw new PointOnEdgeException("Point on constrained edge not supported yet", eq, op, ep);
                    return false;
                default:
                    throw new NotImplementedException("Orientation not handled");
            }
        }


        /// <summary>
        /// After a flip we have two triangles and know that only one will still be
        /// intersecting the edge. So decide which to contiune with and legalize the other
        /// </summary>
        /// <param name="tcx"></param>
        /// <param name="o">should be the result of an TriangulationUtil.orient2d( eq, op, ep )</param>
        /// <param name="t">triangle 1</param>
        /// <param name="ot">triangle 2</param>
        /// <param name="p">a point shared by both triangles</param>
        /// <param name="op">another point shared by both triangles</param>
        /// <returns>returns the triangle still intersecting the edge</returns>
        private static DelaunayTriangle NextFlipTriangle(DTSweepContext tcx, Orientation o, DelaunayTriangle t, DelaunayTriangle ot, TriangulationPoint p, TriangulationPoint op)
        {
            int edgeIndex;
            if (o == Orientation.CCW)
            {
                // ot is not crossing edge after flip
                edgeIndex = ot.EdgeIndex(p, op);
                ot.EdgeIsDelaunay[edgeIndex] = true;
                Legalize(tcx, ot);
                ot.EdgeIsDelaunay.Clear();
                return t;
            }
            // t is not crossing edge after flip
            edgeIndex = t.EdgeIndex(p, op);
            t.EdgeIsDelaunay[edgeIndex] = true;
            Legalize(tcx, t);
            t.EdgeIsDelaunay.Clear();
            return ot;
        }


        /// <summary>
        /// Scan part of the FlipScan algorithm<br>
        /// When a triangle pair isn't flippable we will scan for the next 
        /// point that is inside the flip triangle scan area. When found 
        /// we generate a new flipEdgeEvent
        /// </summary>
        /// <param name="tcx"></param>
        /// <param name="ep">last point on the edge we are traversing</param>
        /// <param name="eq">first point on the edge we are traversing</param>
        /// <param name="flipTriangle">the current triangle sharing the point eq with edge</param>
        /// <param name="t"></param>
        /// <param name="p"></param>
        private static void FlipScanEdgeEvent(DTSweepContext tcx, TriangulationPoint ep, TriangulationPoint eq, DelaunayTriangle flipTriangle, DelaunayTriangle t, TriangulationPoint p)
        {
            DelaunayTriangle ot;
            TriangulationPoint op, newP;
            bool inScanArea;

            ot = t.NeighborAcrossFrom(p);
            op = ot.OppositePoint(t, p);

            if (ot == null)
            {
                // If we want to integrate the fillEdgeEvent do it here
                // With current implementation we should never get here
                throw new Exception("[BUG:FIXME] FLIP failed due to missing triangle");
            }

            if (tcx.IsDebugEnabled)
            {
                Console.WriteLine("[FLIP:SCAN] - scan next point"); // TODO: remove
                tcx.DTDebugContext.PrimaryTriangle = t;
                tcx.DTDebugContext.SecondaryTriangle = ot;
            }

            inScanArea = TriangulationUtil.InScanArea(eq, flipTriangle.PointCCWFrom(eq), flipTriangle.PointCWFrom(eq), op);
            if (inScanArea)
            {
                // flip with new edge op->eq
                FlipEdgeEvent(tcx, eq, op, ot, op);
                // TODO: Actually I just figured out that it should be possible to 
                //       improve this by getting the next ot and op before the the above 
                //       flip and continue the flipScanEdgeEvent here
                // set new ot and op here and loop back to inScanArea test
                // also need to set a new flipTriangle first
                // Turns out at first glance that this is somewhat complicated
                // so it will have to wait.
            }
            else
            {
                if (NextFlipPoint(ep, eq, ot, op, out newP))
                {
                    FlipScanEdgeEvent(tcx, ep, eq, flipTriangle, ot, newP);
                }
                //newP = NextFlipPoint(ep, eq, ot, op);
            }
        }


        /// <summary>
        /// Fills holes in the Advancing Front
        /// </summary>
        private static void FillAdvancingFront(DTSweepContext tcx, AdvancingFrontNode n)
        {
            AdvancingFrontNode node;
            double angle;

            // Fill right holes
            node = n.Next;
            while (node.HasNext)
            {
                angle = HoleAngle(node);
                if (angle > PI_div2 || angle < -PI_div2)
                {
                    break;
                }
                Fill(tcx, node);
                node = node.Next;
            }

            // Fill left holes
            node = n.Prev;
            while (node.HasPrev)
            {
                angle = HoleAngle(node);
                if (angle > PI_div2 || angle < -PI_div2)
                {
                    break;
                }
                Fill(tcx, node);
                node = node.Prev;
            }

            // Fill right basins
            if (n.HasNext && n.Next.HasNext)
            {
                angle = BasinAngle(n);
                if (angle < PI_3div4)
                {
                    FillBasin(tcx, n);
                }
            }
        }


        /// <summary>
        /// Fills a basin that has formed on the Advancing Front to the right
        /// of given node.<br>
        /// First we decide a left,bottom and right node that forms the 
        /// boundaries of the basin. Then we do a reqursive fill.
        /// </summary>
        /// <param name="tcx"></param>
        /// <param name="node">starting node, this or next node will be left node</param>
        private static void FillBasin(DTSweepContext tcx, AdvancingFrontNode node)
        {
            if (TriangulationUtil.Orient2d(node.Point, node.Next.Point, node.Next.Next.Point) == Orientation.CCW)
            {
                // tcx.basin.leftNode = node.next.next;
                tcx.Basin.leftNode = node;
            }
            else
            {
                tcx.Basin.leftNode = node.Next;
            }

            // Find the bottom and right node
            tcx.Basin.bottomNode = tcx.Basin.leftNode;
            while (tcx.Basin.bottomNode.HasNext && tcx.Basin.bottomNode.Point.Y >= tcx.Basin.bottomNode.Next.Point.Y)
            {
                tcx.Basin.bottomNode = tcx.Basin.bottomNode.Next;
            }

            if (tcx.Basin.bottomNode == tcx.Basin.leftNode)
            {
                return; // No valid basin
            }

            tcx.Basin.rightNode = tcx.Basin.bottomNode;
            while (tcx.Basin.rightNode.HasNext && tcx.Basin.rightNode.Point.Y < tcx.Basin.rightNode.Next.Point.Y)
            {
                tcx.Basin.rightNode = tcx.Basin.rightNode.Next;
            }

            if (tcx.Basin.rightNode == tcx.Basin.bottomNode)
            {
                return; // No valid basins
            }

            tcx.Basin.width = tcx.Basin.rightNode.Point.X - tcx.Basin.leftNode.Point.X;
            tcx.Basin.leftHighest = tcx.Basin.leftNode.Point.Y > tcx.Basin.rightNode.Point.Y;

            FillBasinReq(tcx, tcx.Basin.bottomNode);
        }


        /// <summary>
        /// Recursive algorithm to fill a Basin with triangles
        /// </summary>
        private static void FillBasinReq(DTSweepContext tcx, AdvancingFrontNode node)
        {
            if (IsShallow(tcx, node))
            {
                return; // if shallow stop filling
            }

            Fill(tcx, node);
            if (node.Prev == tcx.Basin.leftNode && node.Next == tcx.Basin.rightNode)
            {
                return;
            }
            else if (node.Prev == tcx.Basin.leftNode)
            {
                Orientation o = TriangulationUtil.Orient2d(node.Point, node.Next.Point, node.Next.Next.Point);
                if (o == Orientation.CW)
                {
                    return;
                }
                node = node.Next;
            }
            else if (node.Next == tcx.Basin.rightNode)
            {
                Orientation o = TriangulationUtil.Orient2d(node.Point, node.Prev.Point, node.Prev.Prev.Point);
                if (o == Orientation.CCW)
                {
                    return;
                }
                node = node.Prev;
            }
            else
            {
                // Continue with the neighbor node with lowest Y value
                if (node.Prev.Point.Y < node.Next.Point.Y)
                {
                    node = node.Prev;
                }
                else
                {
                    node = node.Next;
                }
            }
            FillBasinReq(tcx, node);
        }


        private static bool IsShallow(DTSweepContext tcx, AdvancingFrontNode node)
        {
            double height;

            if (tcx.Basin.leftHighest)
            {
                height = tcx.Basin.leftNode.Point.Y - node.Point.Y;
            }
            else
            {
                height = tcx.Basin.rightNode.Point.Y - node.Point.Y;
            }
            if (tcx.Basin.width > height)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// ???
        /// </summary>
        /// <param name="node">middle node</param>
        /// <returns>the angle between 3 front nodes</returns>
        private static double HoleAngle(AdvancingFrontNode node)
        {
            // XXX: do we really need a signed angle for holeAngle?
            //      could possible save some cycles here
            /* Complex plane
             * ab = cosA +i*sinA
             * ab = (ax + ay*i)(bx + by*i) = (ax*bx + ay*by) + i(ax*by-ay*bx)
             * atan2(y,x) computes the principal value of the argument function
             * applied to the complex number x+iy
             * Where x = ax*bx + ay*by
             *       y = ax*by - ay*bx
             */
            double px = node.Point.X;
            double py = node.Point.Y;
            double ax = node.Next.Point.X - px;
            double ay = node.Next.Point.Y - py;
            double bx = node.Prev.Point.X - px;
            double by = node.Prev.Point.Y - py;
            return Math.Atan2((ax * by) - (ay * bx), (ax * bx) + (ay * by));
        }


        /// <summary>
        /// The basin angle is decided against the horizontal line [1,0]
        /// </summary>
        private static double BasinAngle(AdvancingFrontNode node)
        {
            double ax = node.Point.X - node.Next.Next.Point.X;
            double ay = node.Point.Y - node.Next.Next.Point.Y;
            return Math.Atan2(ay, ax);
        }


        /// <summary>
        /// Adds a triangle to the advancing front to fill a hole.
        /// </summary>
        /// <param name="tcx"></param>
        /// <param name="node">middle node, that is the bottom of the hole</param>
        private static void Fill(DTSweepContext tcx, AdvancingFrontNode node)
        {
            DelaunayTriangle triangle = new DelaunayTriangle(node.Prev.Point, node.Point, node.Next.Point);
            // TODO: should copy the cEdge value from neighbor triangles
            //       for now cEdge values are copied during the legalize 
            triangle.MarkNeighbor(node.Prev.Triangle);
            triangle.MarkNeighbor(node.Triangle);
            tcx.Triangles.Add(triangle);

            // Update the advancing front
            node.Prev.Next = node.Next;
            node.Next.Prev = node.Prev;
            tcx.RemoveNode(node);

            // If it was legalized the triangle has already been mapped
            if (!Legalize(tcx, triangle))
            {
                tcx.MapTriangleToNodes(triangle);
            }
        }


        /// <summary>
        /// Returns true if triangle was legalized
        /// </summary>
        private static bool Legalize(DTSweepContext tcx, DelaunayTriangle t)
        {
            // To legalize a triangle we start by finding if any of the three edges
            // violate the Delaunay condition
            for (int i = 0; i < 3; i++)
            {
                // TODO: fix so that cEdge is always valid when creating new triangles then we can check it here
                //       instead of below with ot
                if (t.EdgeIsDelaunay[i])
                {
                    continue;
                }

                DelaunayTriangle ot = t.Neighbors[i];
                if (ot == null)
                {
                    continue;
                }

                TriangulationPoint p = t.Points[i];
                TriangulationPoint op = ot.OppositePoint(t, p);
                int oi = ot.IndexOf(op);
                // If this is a Constrained Edge or a Delaunay Edge(only during recursive legalization)
                // then we should not try to legalize
                if (ot.EdgeIsConstrained[oi] || ot.EdgeIsDelaunay[oi])
                {
                    t.SetConstrainedEdgeAcross(p, ot.EdgeIsConstrained[oi]); // XXX: have no good way of setting this property when creating new triangles so lets set it here
                    continue;
                }

                if (!TriangulationUtil.SmartIncircle(p, t.PointCCWFrom(p), t.PointCWFrom(p), op))
                {
                    continue;
                }

                // Lets mark this shared edge as Delaunay 
                t.EdgeIsDelaunay[i] = true;
                ot.EdgeIsDelaunay[oi] = true;

                // Lets rotate shared edge one vertex CW to legalize it
                RotateTrianglePair(t, p, ot, op);

                // We now got one valid Delaunay Edge shared by two triangles
                // This gives us 4 new edges to check for Delaunay

                // Make sure that triangle to node mapping is done only one time for a specific triangle
                if (!Legalize(tcx, t))
                {
                    tcx.MapTriangleToNodes(t);
                }
                if (!Legalize(tcx, ot))
                {
                    tcx.MapTriangleToNodes(ot);
                }

                // Reset the Delaunay edges, since they only are valid Delaunay edges
                // until we add a new triangle or point.
                // XXX: need to think about this. Can these edges be tried after we 
                //      return to previous recursive level?
                t.EdgeIsDelaunay[i] = false;
                ot.EdgeIsDelaunay[oi] = false;

                // If triangle have been legalized no need to check the other edges since
                // the recursive legalization will handles those so we can end here.
                return true;
            }
            return false;
        }


        /// <summary>
        /// Rotates a triangle pair one vertex CW
        ///       n2                    n2
        ///  P +-----+             P +-----+
        ///    | t  /|               |\  t |  
        ///    |   / |               | \   |
        ///  n1|  /  |n3           n1|  \  |n3
        ///    | /   |    after CW   |   \ |
        ///    |/ oT |               | oT \|
        ///    +-----+ oP            +-----+
        ///       n4                    n4
        /// </summary>
        private static void RotateTrianglePair(DelaunayTriangle t, TriangulationPoint p, DelaunayTriangle ot, TriangulationPoint op)
        {
            DelaunayTriangle n1, n2, n3, n4;
            n1 = t.NeighborCCWFrom(p);
            n2 = t.NeighborCWFrom(p);
            n3 = ot.NeighborCCWFrom(op);
            n4 = ot.NeighborCWFrom(op);

            bool ce1, ce2, ce3, ce4;
            ce1 = t.GetConstrainedEdgeCCW(p);
            ce2 = t.GetConstrainedEdgeCW(p);
            ce3 = ot.GetConstrainedEdgeCCW(op);
            ce4 = ot.GetConstrainedEdgeCW(op);

            bool de1, de2, de3, de4;
            de1 = t.GetDelaunayEdgeCCW(p);
            de2 = t.GetDelaunayEdgeCW(p);
            de3 = ot.GetDelaunayEdgeCCW(op);
            de4 = ot.GetDelaunayEdgeCW(op);

            t.Legalize(p, op);
            ot.Legalize(op, p);

            // Remap dEdge
            ot.SetDelaunayEdgeCCW(p, de1);
            t.SetDelaunayEdgeCW(p, de2);
            t.SetDelaunayEdgeCCW(op, de3);
            ot.SetDelaunayEdgeCW(op, de4);

            // Remap cEdge
            ot.SetConstrainedEdgeCCW(p, ce1);
            t.SetConstrainedEdgeCW(p, ce2);
            t.SetConstrainedEdgeCCW(op, ce3);
            ot.SetConstrainedEdgeCW(op, ce4);

            // Remap neighbors
            // XXX: might optimize the markNeighbor by keeping track of
            //      what side should be assigned to what neighbor after the 
            //      rotation. Now mark neighbor does lots of testing to find 
            //      the right side.
            t.Neighbors.Clear();
            ot.Neighbors.Clear();
            if (n1 != null)
            {
                ot.MarkNeighbor(n1);
            }
            if (n2 != null)
            {
                t.MarkNeighbor(n2);
            }
            if (n3 != null)
            {
                t.MarkNeighbor(n3);
            }
            if (n4 != null)
            {
                ot.MarkNeighbor(n4);
            }
            t.MarkNeighbor(ot);
        }
    }

    public class DTSweepBasin
    {
        public AdvancingFrontNode leftNode;
        public AdvancingFrontNode bottomNode;
        public AdvancingFrontNode rightNode;
        public double width;
        public bool leftHighest;
    }

    public class DTSweepConstraint : TriangulationConstraint
    {
        /// <summary>
        /// Give two points in any order. Will always be ordered so
        /// that q.y > p.y and q.x > p.x if same y value 
        /// </summary>
        public DTSweepConstraint(TriangulationPoint p1, TriangulationPoint p2)
            : base(p1, p2)
        {
            Q.AddEdge(this);
        }
    }

    public class DTSweepContext : TriangulationContext
    {
        // Inital triangle factor, seed triangle will extend 30% of 
        // PointSet width to both left and right.
        private readonly float ALPHA = 0.3f;

        public AdvancingFront Front;
        public TriangulationPoint Head { get; set; }
        public TriangulationPoint Tail { get; set; }

        public DTSweepBasin Basin = new DTSweepBasin();
        public DTSweepEdgeEvent EdgeEvent = new DTSweepEdgeEvent();

        private DTSweepPointComparator _comparator = new DTSweepPointComparator();

        public override TriangulationAlgorithm Algorithm { get { return TriangulationAlgorithm.DTSweep; } }


        public DTSweepContext()
        {
            Clear();
        }


        public override bool IsDebugEnabled
        {
            get
            {
                return base.IsDebugEnabled;
            }
            protected set
            {
                if (value && DebugContext == null)
                {
                    DebugContext = new DTSweepDebugContext(this);
                }
                base.IsDebugEnabled = value;
            }
        }


        public void RemoveFromList(DelaunayTriangle triangle)
        {
            Triangles.Remove(triangle);
            // TODO: remove all neighbor pointers to this triangle
            //        for( int i=0; i<3; i++ )
            //        {
            //            if( triangle.neighbors[i] != null )
            //            {
            //                triangle.neighbors[i].clearNeighbor( triangle );
            //            }
            //        }
            //        triangle.clearNeighbors();
        }


        public void MeshClean(DelaunayTriangle triangle)
        {
            MeshCleanReq(triangle);
        }


        private void MeshCleanReq(DelaunayTriangle triangle)
        {
            if (triangle != null && !triangle.IsInterior)
            {
                triangle.IsInterior = true;
                Triangulatable.AddTriangle(triangle);

                for (int i = 0; i < 3; i++)
                {
                    if (!triangle.EdgeIsConstrained[i])
                    {
                        MeshCleanReq(triangle.Neighbors[i]);
                    }
                }
            }
        }


        public override void Clear()
        {
            base.Clear();
            Triangles.Clear();
        }


        public void AddNode(AdvancingFrontNode node)
        {
            //        Console.WriteLine( "add:" + node.key + ":" + System.identityHashCode(node.key));
            //        m_nodeTree.put( node.getKey(), node );
            Front.AddNode(node);
        }


        public void RemoveNode(AdvancingFrontNode node)
        {
            //        Console.WriteLine( "remove:" + node.key + ":" + System.identityHashCode(node.key));
            //        m_nodeTree.delete( node.getKey() );
            Front.RemoveNode(node);
        }


        public AdvancingFrontNode LocateNode(TriangulationPoint point)
        {
            return Front.LocateNode(point);
        }


        public void CreateAdvancingFront()
        {
            AdvancingFrontNode head, tail, middle;
            // Initial triangle
            DelaunayTriangle iTriangle = new DelaunayTriangle(Points[0], Tail, Head);
            Triangles.Add(iTriangle);

            head = new AdvancingFrontNode(iTriangle.Points[1]);
            head.Triangle = iTriangle;
            middle = new AdvancingFrontNode(iTriangle.Points[0]);
            middle.Triangle = iTriangle;
            tail = new AdvancingFrontNode(iTriangle.Points[2]);

            Front = new AdvancingFront(head, tail);
            Front.AddNode(middle);

            // TODO: I think it would be more intuitive if head is middles next and not previous
            //       so swap head and tail
            Front.Head.Next = middle;
            middle.Next = Front.Tail;
            middle.Prev = Front.Head;
            Front.Tail.Prev = middle;
        }


        /// <summary>
        /// Try to map a node to all sides of this triangle that don't have 
        /// a neighbor.
        /// </summary>
        public void MapTriangleToNodes(DelaunayTriangle t)
        {
            for (int i = 0; i < 3; i++)
            {
                if (t.Neighbors[i] == null)
                {
                    AdvancingFrontNode n = Front.LocatePoint(t.PointCWFrom(t.Points[i]));
                    if (n != null)
                    {
                        n.Triangle = t;
                    }
                }
            }
        }


        public override void PrepareTriangulation(ITriangulatable t)
        {
            base.PrepareTriangulation(t);

            double xmax, xmin;
            double ymax, ymin;

            xmax = xmin = Points[0].X;
            ymax = ymin = Points[0].Y;

            // Calculate bounds. Should be combined with the sorting
            foreach (TriangulationPoint p in Points)
            {
                if (p.X > xmax)
                {
                    xmax = p.X;
                }
                if (p.X < xmin)
                {
                    xmin = p.X;
                }
                if (p.Y > ymax)
                {
                    ymax = p.Y;
                }
                if (p.Y < ymin)
                {
                    ymin = p.Y;
                }
            }

            double deltaX = ALPHA * (xmax - xmin);
            double deltaY = ALPHA * (ymax - ymin);
            TriangulationPoint p1 = new TriangulationPoint(xmax + deltaX, ymin - deltaY);
            TriangulationPoint p2 = new TriangulationPoint(xmin - deltaX, ymin - deltaY);

            Head = p1;
            Tail = p2;

            //        long time = System.nanoTime();
            // Sort the points along y-axis
            Points.Sort(_comparator);
            //        logger.info( "Triangulation setup [{}ms]", ( System.nanoTime() - time ) / 1e6 );
        }


        public void FinalizeTriangulation()
        {
            Triangulatable.AddTriangles(Triangles);
            Triangles.Clear();
        }


        public override TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b)
        {
            return new DTSweepConstraint(a, b);
        }

    }


    public class DTSweepDebugContext : TriangulationDebugContext
    {
        /*
         * Fields used for visual representation of current triangulation
         */

        public DelaunayTriangle PrimaryTriangle { get { return _primaryTriangle; } set { _primaryTriangle = value; _tcx.Update("set PrimaryTriangle"); } }
        public DelaunayTriangle SecondaryTriangle { get { return _secondaryTriangle; } set { _secondaryTriangle = value; _tcx.Update("set SecondaryTriangle"); } }
        public TriangulationPoint ActivePoint { get { return _activePoint; } set { _activePoint = value; _tcx.Update("set ActivePoint"); } }
        public AdvancingFrontNode ActiveNode { get { return _activeNode; } set { _activeNode = value; _tcx.Update("set ActiveNode"); } }
        public DTSweepConstraint ActiveConstraint { get { return _activeConstraint; } set { _activeConstraint = value; _tcx.Update("set ActiveConstraint"); } }

        public DTSweepDebugContext(DTSweepContext tcx) : base(tcx) { }

        public bool IsDebugContext { get { return true; } }

        public override void Clear()
        {
            PrimaryTriangle = null;
            SecondaryTriangle = null;
            ActivePoint = null;
            ActiveNode = null;
            ActiveConstraint = null;
        }

        private DelaunayTriangle _primaryTriangle;
        private DelaunayTriangle _secondaryTriangle;
        private TriangulationPoint _activePoint;
        private AdvancingFrontNode _activeNode;
        private DTSweepConstraint _activeConstraint;
    }

    public class DTSweepEdgeEvent
    {
        public DTSweepConstraint ConstrainedEdge;
        public bool Right;
    }

    public class DTSweepPointComparator : IComparer<TriangulationPoint>
    {
        public int Compare(TriangulationPoint p1, TriangulationPoint p2)
        {
            if (p1.Y < p2.Y)
            {
                return -1;
            }
            else if (p1.Y > p2.Y)
            {
                return 1;
            }
            else
            {
                if (p1.X < p2.X)
                {
                    return -1;
                }
                else if (p1.X > p2.X)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    public class PointOnEdgeException : NotImplementedException
    {
        public readonly TriangulationPoint A, B, C;

        public PointOnEdgeException(string message, TriangulationPoint a, TriangulationPoint b, TriangulationPoint c)
            : base(message)
        {
            A = a;
            B = b;
            C = c;
        }
    }

    public class Contour : Point2DList, ITriangulatable, IEnumerable<TriangulationPoint>, IList<TriangulationPoint>
    {
        private List<Contour> mHoles = new List<Contour>();
        private ITriangulatable mParent = null;
        private string mName = "";

        public new TriangulationPoint this[int index]
        {
            get { return mPoints[index] as TriangulationPoint; }
            set { mPoints[index] = value; }
        }
        public string Name { get { return mName; } set { mName = value; } }


        public IList<DelaunayTriangle> Triangles
        {
            get
            {
                throw new NotImplementedException("PolyHole.Triangles should never get called");
            }
            private set { }
        }
        public TriangulationMode TriangulationMode { get { return mParent.TriangulationMode; } }
        public string FileName { get { return mParent.FileName; } set { } }
        public bool DisplayFlipX { get { return mParent.DisplayFlipX; } set { } }
        public bool DisplayFlipY { get { return mParent.DisplayFlipY; } set { } }
        public float DisplayRotate { get { return mParent.DisplayRotate; } set { } }
        public double Precision { get { return mParent.Precision; } set { } }
        public double MinX { get { return mBoundingBox.MinX; } }
        public double MaxX { get { return mBoundingBox.MaxX; } }
        public double MinY { get { return mBoundingBox.MinY; } }
        public double MaxY { get { return mBoundingBox.MaxY; } }
        public Rect2D Bounds { get { return mBoundingBox; } }


        public Contour(ITriangulatable parent)
        {
            mParent = parent;
        }


        public Contour(ITriangulatable parent, IList<TriangulationPoint> points, Point2DList.WindingOrderType windingOrder)
        {
            // Currently assumes that input is pre-checked for validity
            mParent = parent;
            AddRange(points, windingOrder);
        }


        public override string ToString()
        {
            return mName + " : " + base.ToString();
        }


        IEnumerator<TriangulationPoint> IEnumerable<TriangulationPoint>.GetEnumerator()
        {
            return new TriangulationPointEnumerator(mPoints);
        }


        public int IndexOf(TriangulationPoint p)
        {
            return mPoints.IndexOf(p);
        }


        public void Add(TriangulationPoint p)
        {
            Add(p, -1, true);
        }


        protected override void Add(Point2D p, int idx, bool bCalcWindingOrderAndEpsilon)
        {
            TriangulationPoint pt = null;
            if (p is TriangulationPoint)
            {
                pt = p as TriangulationPoint;
            }
            else
            {
                pt = new TriangulationPoint(p.X, p.Y);
            }
            if (idx < 0)
            {
                mPoints.Add(pt);
            }
            else
            {
                mPoints.Insert(idx, pt);
            }
            mBoundingBox.AddPoint(pt);
            if (bCalcWindingOrderAndEpsilon)
            {
                if (mWindingOrder == WindingOrderType.Unknown)
                {
                    mWindingOrder = CalculateWindingOrder();
                }
                mEpsilon = CalculateEpsilon();
            }
        }


        public override void AddRange(IEnumerator<Point2D> iter, WindingOrderType windingOrder)
        {
            if (iter == null)
            {
                return;
            }

            if (mWindingOrder == WindingOrderType.Unknown && Count == 0)
            {
                mWindingOrder = windingOrder;
            }
            bool bReverseReadOrder = (WindingOrder != WindingOrderType.Unknown) && (windingOrder != WindingOrderType.Unknown) && (WindingOrder != windingOrder);
            bool bAddedFirst = true;
            int startCount = mPoints.Count;
            iter.Reset();
            while (iter.MoveNext())
            {
                TriangulationPoint pt = null;
                if (iter.Current is TriangulationPoint)
                {
                    pt = iter.Current as TriangulationPoint;
                }
                else
                {
                    pt = new TriangulationPoint(iter.Current.X, iter.Current.Y);
                }
                if (!bAddedFirst)
                {
                    bAddedFirst = true;
                    mPoints.Add(pt);
                }
                else if (bReverseReadOrder)
                {
                    mPoints.Insert(startCount, pt);
                }
                else
                {
                    mPoints.Add(pt);
                }
                mBoundingBox.AddPoint(iter.Current);
            }
            if (mWindingOrder == WindingOrderType.Unknown && windingOrder == WindingOrderType.Unknown)
            {
                mWindingOrder = CalculateWindingOrder();
            }
            mEpsilon = CalculateEpsilon();
        }


        public void AddRange(IList<TriangulationPoint> points, Point2DList.WindingOrderType windingOrder)
        {
            if (points == null || points.Count < 1)
            {
                return;
            }

            if (mWindingOrder == Point2DList.WindingOrderType.Unknown && Count == 0)
            {
                mWindingOrder = windingOrder;
            }

            int numPoints = points.Count;
            bool bReverseReadOrder = (WindingOrder != WindingOrderType.Unknown) && (windingOrder != WindingOrderType.Unknown) && (WindingOrder != windingOrder);
            for (int i = 0; i < numPoints; ++i)
            {
                int idx = i;
                if (bReverseReadOrder)
                {
                    idx = points.Count - i - 1;
                }
                Add(points[idx], -1, false);
            }
            if (mWindingOrder == WindingOrderType.Unknown)
            {
                mWindingOrder = CalculateWindingOrder();
            }
            mEpsilon = CalculateEpsilon();
        }


        public void Insert(int idx, TriangulationPoint p)
        {
            Add(p, idx, true);
        }


        public bool Remove(TriangulationPoint p)
        {
            return Remove(p as Point2D);
        }


        public bool Contains(TriangulationPoint p)
        {
            return mPoints.Contains(p);
        }


        public void CopyTo(TriangulationPoint[] array, int arrayIndex)
        {
            int numElementsToCopy = Math.Min(Count, array.Length - arrayIndex);
            for (int i = 0; i < numElementsToCopy; ++i)
            {
                array[arrayIndex + i] = mPoints[i] as TriangulationPoint;
            }
        }


        protected void AddHole(Contour c)
        {
            // no checking is done here as we rely on InitializeHoles for that
            c.mParent = this;
            mHoles.Add(c);
        }


        /// <summary>
        /// returns number of holes that are actually holes, including all children of children, etc.   Does NOT
        /// include holes that are not actually holes.   For example, if the parent is not a hole and this contour has
        /// a hole that contains a hole, then the number of holes returned would be 2 - one for the current hole (because
        /// the parent is NOT a hole and thus this hole IS a hole), and 1 for the child of the child.
        /// </summary>
        /// <param name="parentIsHole"></param>
        /// <returns></returns>
        public int GetNumHoles(bool parentIsHole)
        {
            int numHoles = parentIsHole ? 0 : 1;
            foreach (Contour c in mHoles)
            {
                numHoles += c.GetNumHoles(!parentIsHole);
            }

            return numHoles;
        }


        /// <summary>
        /// returns the basic number of child holes of THIS contour, not including any children of children, etc nor
        /// examining whether any children are actual holes.
        /// </summary>
        /// <returns></returns>
        public int GetNumHoles()
        {
            return mHoles.Count;
        }


        public Contour GetHole(int idx)
        {
            if (idx < 0 || idx >= mHoles.Count)
            {
                return null;
            }

            return mHoles[idx];
        }


        public void GetActualHoles(bool parentIsHole, ref List<Contour> holes)
        {
            if (parentIsHole)
            {
                holes.Add(this);
            }

            foreach (Contour c in mHoles)
            {
                c.GetActualHoles(!parentIsHole, ref holes);
            }
        }


        public List<Contour>.Enumerator GetHoleEnumerator()
        {
            return mHoles.GetEnumerator();
        }


        public void InitializeHoles(ConstrainedPointSet cps)
        {
            Contour.InitializeHoles(mHoles, this, cps);
            foreach (Contour c in mHoles)
            {
                c.InitializeHoles(cps);
            }
        }


        public static void InitializeHoles(List<Contour> holes, ITriangulatable parent, ConstrainedPointSet cps)
        {
            int numHoles = holes.Count;
            int holeIdx = 0;

            // pass 1 - remove duplicates
            while (holeIdx < numHoles)
            {
                int hole2Idx = holeIdx + 1;
                while (hole2Idx < numHoles)
                {
                    bool bSamePolygon = PolygonUtil.PolygonsAreSame2D(holes[holeIdx], holes[hole2Idx]);
                    if (bSamePolygon)
                    {
                        // remove one of them
                        holes.RemoveAt(hole2Idx);
                        --numHoles;
                    }
                    else
                    {
                        ++hole2Idx;
                    }
                }
                ++holeIdx;
            }

            // pass 2: Intersections and Containment
            holeIdx = 0;
            while (holeIdx < numHoles)
            {
                bool bIncrementHoleIdx = true;
                int hole2Idx = holeIdx + 1;
                while (hole2Idx < numHoles)
                {
                    if (PolygonUtil.PolygonContainsPolygon(holes[holeIdx], holes[holeIdx].Bounds, holes[hole2Idx], holes[hole2Idx].Bounds, false))
                    {
                        holes[holeIdx].AddHole(holes[hole2Idx]);
                        holes.RemoveAt(hole2Idx);
                        --numHoles;
                    }
                    else if (PolygonUtil.PolygonContainsPolygon(holes[hole2Idx], holes[hole2Idx].Bounds, holes[holeIdx], holes[holeIdx].Bounds, false))
                    {
                        holes[hole2Idx].AddHole(holes[holeIdx]);
                        holes.RemoveAt(holeIdx);
                        --numHoles;
                        bIncrementHoleIdx = false;
                        break;
                    }
                    else
                    {
                        bool bIntersect = PolygonUtil.PolygonsIntersect2D(holes[holeIdx], holes[holeIdx].Bounds, holes[hole2Idx], holes[hole2Idx].Bounds);
                        if (bIntersect)
                        {
                            // this is actually an error condition
                            // fix by merging hole1 and hole2 into hole1 (including the holes inside hole2!) and delete hole2
                            // Then, because hole1 is now changed, restart it's check.
                            PolygonOperationContext ctx = new PolygonOperationContext();
                            if (!ctx.Init(PolygonUtil.PolyOperation.Union | PolygonUtil.PolyOperation.Intersect, holes[holeIdx], holes[hole2Idx]))
                            {
                                if (ctx.mError == PolygonUtil.PolyUnionError.Poly1InsidePoly2)
                                {
                                    holes[hole2Idx].AddHole(holes[holeIdx]);
                                    holes.RemoveAt(holeIdx);
                                    --numHoles;
                                    bIncrementHoleIdx = false;
                                    break;
                                }
                                else
                                {
                                    throw new Exception("PolygonOperationContext.Init had an error during initialization");
                                }
                            }
                            PolygonUtil.PolyUnionError pue = PolygonUtil.PolygonOperation(ctx);
                            if (pue == PolygonUtil.PolyUnionError.None)
                            {
                                Point2DList union = ctx.Union;
                                Point2DList intersection = ctx.Intersect;

                                // create a new contour for the union
                                Contour c = new Contour(parent);
                                c.AddRange(union);
                                c.Name = "(" + holes[holeIdx].Name + " UNION " + holes[hole2Idx].Name + ")";
                                c.WindingOrder = Point2DList.WindingOrderType.Default;

                                // add children from both of the merged contours
                                int numChildHoles = holes[holeIdx].GetNumHoles();
                                for (int i = 0; i < numChildHoles; ++i)
                                {
                                    c.AddHole(holes[holeIdx].GetHole(i));
                                }
                                numChildHoles = holes[hole2Idx].GetNumHoles();
                                for (int i = 0; i < numChildHoles; ++i)
                                {
                                    c.AddHole(holes[hole2Idx].GetHole(i));
                                }

                                // make sure we preserve the contours of the intersection
                                Contour cInt = new Contour(c);
                                cInt.AddRange(intersection);
                                cInt.Name = "(" + holes[holeIdx].Name + " INTERSECT " + holes[hole2Idx].Name + ")";
                                cInt.WindingOrder = Point2DList.WindingOrderType.Default;
                                c.AddHole(cInt);

                                // replace the current contour with the merged contour
                                holes[holeIdx] = c;

                                // toss the second contour
                                holes.RemoveAt(hole2Idx);
                                --numHoles;

                                // current hole is "examined", so move to the next one
                                hole2Idx = holeIdx + 1;
                            }
                            else
                            {
                                throw new Exception("PolygonOperation had an error!");
                            }
                        }
                        else
                        {
                            ++hole2Idx;
                        }
                    }
                }
                if (bIncrementHoleIdx)
                {
                    ++holeIdx;
                }
            }

            numHoles = holes.Count;
            holeIdx = 0;
            while (holeIdx < numHoles)
            {
                int numPoints = holes[holeIdx].Count;
                for (int i = 0; i < numPoints; ++i)
                {
                    int j = holes[holeIdx].NextIndex(i);
                    uint constraintCode = TriangulationConstraint.CalculateContraintCode(holes[holeIdx][i], holes[holeIdx][j]);
                    TriangulationConstraint tc = null;
                    if (!cps.TryGetConstraint(constraintCode, out tc))
                    {
                        tc = new TriangulationConstraint(holes[holeIdx][i], holes[holeIdx][j]);
                        cps.AddConstraint(tc);
                    }

                    // replace the points in the holes with valid points
                    if (holes[holeIdx][i].VertexCode == tc.P.VertexCode)
                    {
                        holes[holeIdx][i] = tc.P;
                    }
                    else if (holes[holeIdx][j].VertexCode == tc.P.VertexCode)
                    {
                        holes[holeIdx][j] = tc.P;
                    }
                    if (holes[holeIdx][i].VertexCode == tc.Q.VertexCode)
                    {
                        holes[holeIdx][i] = tc.Q;
                    }
                    else if (holes[holeIdx][j].VertexCode == tc.Q.VertexCode)
                    {
                        holes[holeIdx][j] = tc.Q;
                    }
                }
                ++holeIdx;
            }
        }


        public void Prepare(TriangulationContext tcx)
        {
            throw new NotImplementedException("PolyHole.Prepare should never get called");
        }


        public void AddTriangle(DelaunayTriangle t)
        {
            throw new NotImplementedException("PolyHole.AddTriangle should never get called");
        }


        public void AddTriangles(IEnumerable<DelaunayTriangle> list)
        {
            throw new NotImplementedException("PolyHole.AddTriangles should never get called");
        }


        public void ClearTriangles()
        {
            throw new NotImplementedException("PolyHole.ClearTriangles should never get called");
        }


        public Point2D FindPointInContour()
        {
            if (Count < 3)
            {
                return null;
            }

            // first try the simple approach:
            Point2D p = GetCentroid();
            if (IsPointInsideContour(p))
            {
                return p;
            }

            // brute force it...
            Random random = new Random();
            while (true)
            {
                p.X = (random.NextDouble() * (MaxX - MinX)) + MinX;
                p.Y = (random.NextDouble() * (MaxY - MinY)) + MinY;
                if (IsPointInsideContour(p))
                {
                    return p;
                }
            }
        }


        public bool IsPointInsideContour(Point2D p)
        {
            if (PolygonUtil.PointInPolygon2D(this, p))
            {
                foreach (Contour c in mHoles)
                {
                    if (c.IsPointInsideContour(p))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

    }

    public class Polygon : Point2DList, ITriangulatable, IEnumerable<TriangulationPoint>, IList<TriangulationPoint>
    {
        // ITriangulatable Implementation
        protected Dictionary<uint, TriangulationPoint> mPointMap = new Dictionary<uint, TriangulationPoint>();
        public IList<TriangulationPoint> Points { get { return this; } }
        protected List<DelaunayTriangle> mTriangles;
        public IList<DelaunayTriangle> Triangles { get { return mTriangles; } }
        public TriangulationMode TriangulationMode { get { return TriangulationMode.Polygon; } }
        public string FileName { get; set; }
        public bool DisplayFlipX { get; set; }
        public bool DisplayFlipY { get; set; }
        public float DisplayRotate { get; set; }
        private double mPrecision = TriangulationPoint.kVertexCodeDefaultPrecision;
        public double Precision { get { return mPrecision; } set { mPrecision = value; } }
        public double MinX { get { return mBoundingBox.MinX; } }
        public double MaxX { get { return mBoundingBox.MaxX; } }
        public double MinY { get { return mBoundingBox.MinY; } }
        public double MaxY { get { return mBoundingBox.MaxY; } }
        public Rect2D Bounds { get { return mBoundingBox; } }

        // Point2DList overrides
        public new TriangulationPoint this[int index]
        {
            get { return mPoints[index] as TriangulationPoint; }
            set { mPoints[index] = value; }
        }

        // Polygon Implementation
        protected List<Polygon> mHoles;
        public IList<Polygon> Holes { get { return mHoles; } }
        protected List<TriangulationPoint> mSteinerPoints;
        protected PolygonPoint _last;



        /// <summary>
        /// Create a polygon from a list of at least 3 points with no duplicates.
        /// </summary>
        /// <param name="points">A list of unique points</param>
        public Polygon(IList<PolygonPoint> points)
        {
            if (points.Count < 3)
            {
                throw new ArgumentException("List has fewer than 3 points", "points");
            }

            AddRange(points, WindingOrderType.Unknown);
        }


        /// <summary>
        /// Create a polygon from a list of at least 3 points with no duplicates.
        /// </summary>
        /// <param name="points">A list of unique points.</param>
        public Polygon(IEnumerable<PolygonPoint> points)
            : this((points as IList<PolygonPoint>) ?? points.ToArray())
        { }


        /// <summary>
        /// Create a polygon from a list of at least 3 points with no duplicates.
        /// </summary>
        /// <param name="points">A list of unique points.</param>
        public Polygon(params PolygonPoint[] points)
            : this((IList<PolygonPoint>)points)
        { }


        IEnumerator<TriangulationPoint> IEnumerable<TriangulationPoint>.GetEnumerator()
        {
            return new TriangulationPointEnumerator(mPoints);
        }


        public int IndexOf(TriangulationPoint p)
        {
            return mPoints.IndexOf(p);
        }


        public override void Add(Point2D p)
        {
            Add(p, -1, true);
        }


        public void Add(TriangulationPoint p)
        {
            Add(p, -1, true);
        }


        public void Add(PolygonPoint p)
        {
            Add(p, -1, true);
        }


        protected override void Add(Point2D p, int idx, bool bCalcWindingOrderAndEpsilon)
        {
            TriangulationPoint pt = p as TriangulationPoint;
            if (pt == null)
            {
                // we only store TriangulationPoints and PolygonPoints in this class
                return;
            }

            // do not insert duplicate points
            if (mPointMap.ContainsKey(pt.VertexCode))
            {
                return;
            }
            mPointMap.Add(pt.VertexCode, pt);

            base.Add(p, idx, bCalcWindingOrderAndEpsilon);

            PolygonPoint pp = p as PolygonPoint;
            if (pp != null)
            {
                pp.Previous = _last;
                if (_last != null)
                {
                    pp.Next = _last.Next;
                    _last.Next = pp;
                }
                _last = pp;
            }

            return;
        }


        public void AddRange(IList<PolygonPoint> points, Point2DList.WindingOrderType windingOrder)
        {
            if (points == null || points.Count < 1)
            {
                return;
            }

            if (mWindingOrder == Point2DList.WindingOrderType.Unknown && Count == 0)
            {
                mWindingOrder = windingOrder;
            }
            int numPoints = points.Count;
            bool bReverseReadOrder = (WindingOrder != WindingOrderType.Unknown) && (windingOrder != WindingOrderType.Unknown) && (WindingOrder != windingOrder);
            for (int i = 0; i < numPoints; ++i)
            {
                int idx = i;
                if (bReverseReadOrder)
                {
                    idx = points.Count - i - 1;
                }
                Add(points[idx], -1, false);
            }
            if (mWindingOrder == WindingOrderType.Unknown)
            {
                mWindingOrder = CalculateWindingOrder();
            }
            mEpsilon = CalculateEpsilon();
        }


        public void AddRange(IList<TriangulationPoint> points, Point2DList.WindingOrderType windingOrder)
        {
            if (points == null || points.Count < 1)
            {
                return;
            }

            if (mWindingOrder == Point2DList.WindingOrderType.Unknown && Count == 0)
            {
                mWindingOrder = windingOrder;
            }

            int numPoints = points.Count;
            bool bReverseReadOrder = (WindingOrder != WindingOrderType.Unknown) && (windingOrder != WindingOrderType.Unknown) && (WindingOrder != windingOrder);
            for (int i = 0; i < numPoints; ++i)
            {
                int idx = i;
                if (bReverseReadOrder)
                {
                    idx = points.Count - i - 1;
                }
                Add(points[idx], -1, false);
            }
            if (mWindingOrder == WindingOrderType.Unknown)
            {
                mWindingOrder = CalculateWindingOrder();
            }
            mEpsilon = CalculateEpsilon();
        }


        public void Insert(int idx, TriangulationPoint p)
        {
            Add(p, idx, true);
        }


        public bool Remove(TriangulationPoint p)
        {
            return base.Remove(p);
        }


        /// <summary>
        /// Removes a point from the polygon.  Note this can be a somewhat expensive operation
        /// as it must recalculate the bounding area from scratch.
        /// </summary>
        /// <param name="p"></param>
        public void RemovePoint(PolygonPoint p)
        {
            PolygonPoint next, prev;

            next = p.Next;
            prev = p.Previous;
            prev.Next = next;
            next.Previous = prev;
            mPoints.Remove(p);

            mBoundingBox.Clear();
            foreach (PolygonPoint tmp in mPoints)
            {
                mBoundingBox.AddPoint(tmp);
            }
        }



        public bool Contains(TriangulationPoint p)
        {
            return mPoints.Contains(p);
        }


        public void CopyTo(TriangulationPoint[] array, int arrayIndex)
        {
            int numElementsToCopy = Math.Min(Count, array.Length - arrayIndex);
            for (int i = 0; i < numElementsToCopy; ++i)
            {
                array[arrayIndex + i] = mPoints[i] as TriangulationPoint;
            }
        }


        public void AddSteinerPoint(TriangulationPoint point)
        {
            if (mSteinerPoints == null)
            {
                mSteinerPoints = new List<TriangulationPoint>();
            }
            mSteinerPoints.Add(point);
        }


        public void AddSteinerPoints(List<TriangulationPoint> points)
        {
            if (mSteinerPoints == null)
            {
                mSteinerPoints = new List<TriangulationPoint>();
            }
            mSteinerPoints.AddRange(points);
        }


        public void ClearSteinerPoints()
        {
            if (mSteinerPoints != null)
            {
                mSteinerPoints.Clear();
            }
        }


        /// <summary>
        /// Add a hole to the polygon.
        /// </summary>
        /// <param name="poly">A subtraction polygon fully contained inside this polygon.</param>
        public void AddHole(Polygon poly)
        {
            if (mHoles == null)
            {
                mHoles = new List<Polygon>();
            }
            mHoles.Add(poly);
            // XXX: tests could be made here to be sure it is fully inside
            //        addSubtraction( poly.getPoints() );
        }


        public void AddTriangle(DelaunayTriangle t)
        {
            mTriangles.Add(t);
        }


        public void AddTriangles(IEnumerable<DelaunayTriangle> list)
        {
            mTriangles.AddRange(list);
        }


        public void ClearTriangles()
        {
            if (mTriangles != null)
            {
                mTriangles.Clear();
            }
        }


        public bool IsPointInside(TriangulationPoint p)
        {
            return PolygonUtil.PointInPolygon2D(this, p);
        }


        /// <summary>
        /// Creates constraints and populates the context with points
        /// </summary>
        /// <param name="tcx">The context</param>
        public void Prepare(TriangulationContext tcx)
        {
            if (mTriangles == null)
            {
                mTriangles = new List<DelaunayTriangle>(mPoints.Count);
            }
            else
            {
                mTriangles.Clear();
            }

            // Outer constraints
            for (int i = 0; i < mPoints.Count - 1; i++)
            {
                //tcx.NewConstraint(mPoints[i], mPoints[i + 1]);
                tcx.NewConstraint(this[i], this[i + 1]);
            }
            tcx.NewConstraint(this[0], this[Count - 1]);
            tcx.Points.AddRange(this);

            // Hole constraints
            if (mHoles != null)
            {
                foreach (Polygon p in mHoles)
                {
                    for (int i = 0; i < p.mPoints.Count - 1; i++)
                    {
                        tcx.NewConstraint(p[i], p[i + 1]);
                    }
                    tcx.NewConstraint(p[0], p[p.Count - 1]);
                    tcx.Points.AddRange(p);
                }
            }

            if (mSteinerPoints != null)
            {
                tcx.Points.AddRange(mSteinerPoints);
            }
        }
    }

    public class PolygonPoint : TriangulationPoint
    {
        public PolygonPoint(double x, double y) : base(x, y) { }

        public PolygonPoint Next { get; set; }
        public PolygonPoint Previous { get; set; }

        public static Point2D ToBasePoint(PolygonPoint p)
        {
            return (Point2D)p;
        }

        public static TriangulationPoint ToTriangulationPoint(PolygonPoint p)
        {
            return (TriangulationPoint)p;
        }
    }

    public class PolygonSet
    {
        protected List<Polygon> _polygons = new List<Polygon>();

        public PolygonSet() { }

        public PolygonSet(Polygon poly)
        {
            _polygons.Add(poly);
        }

        public void Add(Polygon p)
        {
            _polygons.Add(p);
        }

        public IEnumerable<Polygon> Polygons { get { return _polygons; } }
    }


    public class PolygonUtil
    {
        public enum PolyUnionError
        {
            None,
            NoIntersections,
            Poly1InsidePoly2,
            InfiniteLoop
        }

        [Flags]
        public enum PolyOperation : uint
        {
            None = 0,
            Union = 1 << 0,
            Intersect = 1 << 1,
            Subtract = 1 << 2,
        }


        public static Point2DList.WindingOrderType CalculateWindingOrder(IList<Point2D> l)
        {
            double area = 0.0;
            for (int i = 0; i < l.Count; i++)
            {
                int j = (i + 1) % l.Count;
                area += l[i].X * l[j].Y;
                area -= l[i].Y * l[j].X;
            }
            area /= 2.0f;

            // the sign of the 'area' of the polygon is all we are interested in.
            if (area < 0.0)
            {
                return Point2DList.WindingOrderType.CW;
            }
            else if (area > 0.0)
            {
                return Point2DList.WindingOrderType.CCW;
            }

            // error condition - not even verts to calculate, non-simple poly, etc.
            return Point2DList.WindingOrderType.Unknown;
        }


        /// <summary>
        /// Check if the polys are similar to within a tolerance (Doesn't include reflections,
        /// but allows for the points to be numbered differently, but not reversed).
        /// </summary>
        /// <param name="poly1"></param>
        /// <param name="poly2"></param>
        /// <returns></returns>
        public static bool PolygonsAreSame2D(IList<Point2D> poly1, IList<Point2D> poly2)
        {
            int numVerts1 = poly1.Count;
            int numVerts2 = poly2.Count;

            if (numVerts1 != numVerts2)
            {
                return false;
            }
            const double kEpsilon = 0.01;
            const double kEpsilonSq = kEpsilon * kEpsilon;

            // Bounds the same to within tolerance, are there polys the same?
            Point2D vdelta = new Point2D(0.0, 0.0);
            for (int k = 0; k < numVerts2; ++k)
            {
                // Look for a match in verts2 to the first vertex in verts1
                vdelta.Set(poly1[0]);
                vdelta.Subtract(poly2[k]);

                if (vdelta.MagnitudeSquared() < kEpsilonSq)
                {
                    // Found match to the first point, now check the other points continuing round
                    // if the points don't match in the first direction we check, then it's possible
                    // that the polygons have a different winding order, so we check going round 
                    // the opposite way as well
                    int matchedVertIndex = k;
                    bool bReverseSearch = false;
                    while (true)
                    {
                        bool bMatchFound = true;
                        for (int i = 1; i < numVerts1; ++i)
                        {
                            if (!bReverseSearch)
                            {
                                ++k;
                            }
                            else
                            {
                                --k;
                                if (k < 0)
                                {
                                    k = numVerts2 - 1;
                                }
                            }

                            vdelta.Set(poly1[i]);
                            vdelta.Subtract(poly2[k % numVerts2]);
                            if (vdelta.MagnitudeSquared() >= kEpsilonSq)
                            {
                                if (bReverseSearch)
                                {
                                    // didn't find a match going in either direction, so the polygons are not the same
                                    return false;
                                }
                                else
                                {
                                    // mismatch in the first direction checked, so check the other direction.
                                    k = matchedVertIndex;
                                    bReverseSearch = true;
                                    bMatchFound = false;
                                    break;
                                }
                            }
                        }

                        if (bMatchFound)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        public static bool PointInPolygon2D(IList<Point2D> polygon, Point2D p)
        {
            if (polygon == null || polygon.Count < 3)
            {
                return false;
            }

            int numVerts = polygon.Count;
            Point2D p0 = polygon[numVerts - 1];
            bool bYFlag0 = (p0.Y >= p.Y) ? true : false;
            Point2D p1 = null;

            bool bInside = false;
            for (int j = 0; j < numVerts; ++j)
            {
                p1 = polygon[j];
                bool bYFlag1 = (p1.Y >= p.Y) ? true : false;
                if (bYFlag0 != bYFlag1)
                {
                    if (((p1.Y - p.Y) * (p0.X - p1.X) >= (p1.X - p.X) * (p0.Y - p1.Y)) == bYFlag1)
                    {
                        bInside = !bInside;
                    }
                }

                // Move to the next pair of vertices, retaining info as possible.
                bYFlag0 = bYFlag1;
                p0 = p1;
            }

            return bInside;
        }


        // Given two polygons and their bounding rects, returns true if the two polygons intersect.
        // This test will NOT determine if one of the two polygons is contained within the other or if the 
        // two polygons are similar - it will return false in all those cases.  The only case it will return
        // true for is if the two polygons actually intersect.
        public static bool PolygonsIntersect2D(IList<Point2D> poly1, Rect2D boundRect1,
                                                IList<Point2D> poly2, Rect2D boundRect2)
        {
            // do some quick tests first before doing any real work
            if (poly1 == null || poly1.Count < 3 || boundRect1 == null || poly2 == null || poly2.Count < 3 || boundRect2 == null)
            {
                return false;
            }

            if (!boundRect1.Intersects(boundRect2))
            {
                return false;
            }

            // We first check whether any edge of one poly intersects any edge of the other poly. If they do,
            // then the two polys intersect.

            // Make the epsilon a function of the size of the polys. We could take the heights of the rects 
            // also into consideration here if needed; but, that should not usually be necessary.
            double epsilon = Math.Max(Math.Min(boundRect1.Width, boundRect2.Width) * 0.001f, MathUtil.EPSILON);

            int numVerts1 = poly1.Count;
            int numVerts2 = poly2.Count;
            for (int i = 0; i < numVerts1; ++i)
            {
                int lineEndVert1 = i + 1;
                if (lineEndVert1 == numVerts1)
                {
                    lineEndVert1 = 0;
                }
                for (int j = 0; j < numVerts2; ++j)
                {
                    int lineEndVert2 = j + 1;
                    if (lineEndVert2 == numVerts2)
                    {
                        lineEndVert2 = 0;
                    }
                    Point2D tmp = null;
                    if (TriangulationUtil.LinesIntersect2D(poly1[i], poly1[lineEndVert1], poly2[j], poly2[lineEndVert2], ref tmp, epsilon))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        public bool PolygonContainsPolygon(IList<Point2D> poly1, Rect2D boundRect1,
                                            IList<Point2D> poly2, Rect2D boundRect2)
        {
            return PolygonContainsPolygon(poly1, boundRect1, poly2, boundRect2, true);
        }


        /// <summary>
        /// Checks to see if poly1 contains poly2.  return true if so, false otherwise.
        ///
        /// If the polygons intersect, then poly1 cannot contain poly2 (or vice-versa for that matter)
        /// Since the poly intersection test can be somewhat expensive, we'll only run it if the user
        /// requests it.   If runIntersectionTest is false, then it is assumed that the user has already
        /// verified that the polygons do not intersect.  If the polygons DO intersect and runIntersectionTest
        /// is false, then the return value is meaningless.  Caveat emptor.
        /// 
        /// As an added bonus, just to cause more user-carnage, if runIntersectionTest is false, then the 
        /// boundRects are not used and can safely be passed in as nulls.   However, if runIntersectionTest
        /// is true and you pass nulls for boundRect1 or boundRect2, you will cause a program crash.
        /// 
        /// Finally, the polygon points are assumed to be passed in Clockwise winding order.   It is possible
        /// that CounterClockwise ordering would work, but I have not verified the behavior in that case. 
        /// 
        /// </summary>
        /// <param name="poly1">points of polygon1</param>
        /// <param name="boundRect1">bounding rect of polygon1.  Only used if runIntersectionTest is true</param>
        /// <param name="poly2">points of polygon2</param>
        /// <param name="boundRect2">bounding rect of polygon2.  Only used if runIntersectionTest is true</param>
        /// <param name="runIntersectionTest">see summary above</param>
        /// <returns>true if poly1 fully contains poly2</returns>
        public static bool PolygonContainsPolygon(IList<Point2D> poly1, Rect2D boundRect1,
                                                    IList<Point2D> poly2, Rect2D boundRect2,
                                                    bool runIntersectionTest)
        {
            // quick early-out tests
            if (poly1 == null || poly1.Count < 3 || poly2 == null || poly2.Count < 3)
            {
                return false;
            }

            if (runIntersectionTest)
            {
                // make sure the polygons are not actually the same...
                if (poly1.Count == poly2.Count)
                {
                    // Check if the polys are similar to within a tolerance (Doesn't include reflections,
                    // but allows for the points to be numbered differently)
                    if (PolygonUtil.PolygonsAreSame2D(poly1, poly2))
                    {
                        return false;
                    }
                }

                bool bIntersect = PolygonUtil.PolygonsIntersect2D(poly1, boundRect1, poly2, boundRect2);
                if (bIntersect)
                {
                    return false;
                }
            }

            // Since we (now) know that the polygons don't intersect and they are not the same, we can just do a
            // single check to see if ANY point in poly2 is inside poly1.  If so, then all points of poly2
            // are inside poly1.  If not, then ALL points of poly2 are outside poly1.
            if (PolygonUtil.PointInPolygon2D(poly1, poly2[0]))
            {
                return true;
            }

            return false;
        }



        ////////////////////////////////////////////////////////////////////////////////
        // ClipPolygonToEdge2D
        //
        // This function clips a polygon against an edge. The portion of the polygon
        // which is to the left of the edge (while going from edgeBegin to edgeEnd) 
        // is returned in "outPoly". Note that the clipped polygon may have more vertices
        // than the input polygon. Make sure that outPolyArraySize is sufficiently large. 
        // Otherwise, you may get incorrect results and may be an assert (hopefully, no crash).
        // Pass in the actual size of the array in "outPolyArraySize".
        //
        // Read Sutherland-Hidgman algorithm description in Foley & van Dam book for 
        // details about this.
        //
        ///////////////////////////////////////////////////////////////////////////
        public static void ClipPolygonToEdge2D(Point2D edgeBegin,
                                                Point2D edgeEnd,
                                                IList<Point2D> poly,
                                                out List<Point2D> outPoly)
        {
            outPoly = null;
            if (edgeBegin == null ||
                edgeEnd == null ||
                poly == null ||
                poly.Count < 3)
            {
                return;
            }

            outPoly = new List<Point2D>();
            int lastVertex = poly.Count - 1;
            Point2D edgeRayVector = new Point2D(edgeEnd.X - edgeBegin.X, edgeEnd.Y - edgeBegin.Y);
            // Note: >= 0 as opposed to <= 0 is intentional. We are 
            // dealing with x and z here. And in our case, z axis goes
            // downward while the x axis goes rightward.
            //bool bLastVertexIsToRight = TriangulationUtil.PointRelativeToLine2D(poly[lastVertex], edgeBegin, edgeEnd) >= 0;
            bool bLastVertexIsToRight = TriangulationUtil.Orient2d(edgeBegin, edgeEnd, poly[lastVertex]) == Orientation.CW ? true : false;
            Point2D tempRay = new Point2D(0.0, 0.0);

            for (int curVertex = 0; curVertex < poly.Count; curVertex++)
            {
                //bool bCurVertexIsToRight = TriangulationUtil.PointRelativeToLine2D(poly[curVertex], edgeBegin, edgeEnd) >= 0;
                bool bCurVertexIsToRight = TriangulationUtil.Orient2d(edgeBegin, edgeEnd, poly[curVertex]) == Orientation.CW ? true : false;
                if (bCurVertexIsToRight)
                {
                    if (bLastVertexIsToRight)
                    {
                        outPoly.Add(poly[curVertex]);
                    }
                    else
                    {
                        tempRay.Set(poly[curVertex].X - poly[lastVertex].X, poly[curVertex].Y - poly[lastVertex].Y);
                        Point2D ptIntersection = new Point2D(0.0, 0.0);
                        bool bIntersect = TriangulationUtil.RaysIntersect2D(poly[lastVertex], tempRay, edgeBegin, edgeRayVector, ref ptIntersection);
                        if (bIntersect)
                        {
                            outPoly.Add(ptIntersection);
                            outPoly.Add(poly[curVertex]);
                        }
                    }
                }
                else if (bLastVertexIsToRight)
                {
                    tempRay.Set(poly[curVertex].X - poly[lastVertex].X, poly[curVertex].Y - poly[lastVertex].Y);
                    Point2D ptIntersection = new Point2D(0.0, 0.0);
                    bool bIntersect = TriangulationUtil.RaysIntersect2D(poly[lastVertex], tempRay, edgeBegin, edgeRayVector, ref ptIntersection);
                    if (bIntersect)
                    {
                        outPoly.Add(ptIntersection);
                    }
                }

                lastVertex = curVertex;
                bLastVertexIsToRight = bCurVertexIsToRight;
            }
        }


        public static void ClipPolygonToPolygon(IList<Point2D> poly, IList<Point2D> clipPoly, out List<Point2D> outPoly)
        {
            outPoly = null;
            if (poly == null || poly.Count < 3 || clipPoly == null || clipPoly.Count < 3)
            {
                return;
            }

            outPoly = new List<Point2D>(poly);
            int numClipVertices = clipPoly.Count;
            int lastVertex = numClipVertices - 1;

            // The algorithm keeps clipping the polygon against each edge of "clipPoly".
            for (int curVertex = 0; curVertex < numClipVertices; curVertex++)
            {
                List<Point2D> clippedPoly = null;
                Point2D edgeBegin = clipPoly[lastVertex];
                Point2D edgeEnd = clipPoly[curVertex];
                PolygonUtil.ClipPolygonToEdge2D(edgeBegin, edgeEnd, outPoly, out clippedPoly);
                outPoly.Clear();
                outPoly.AddRange(clippedPoly);

                lastVertex = curVertex;
            }
        }


        /// Merges two polygons, given that they intersect.
        /// </summary>
        /// <param name="polygon1">The first polygon.</param>
        /// <param name="polygon2">The second polygon.</param>
        /// <param name="union">The union of the two polygons</param>
        /// <returns>The error returned from union</returns>
        public static PolygonUtil.PolyUnionError PolygonUnion(Point2DList polygon1, Point2DList polygon2, out Point2DList union)
        {
            PolygonOperationContext ctx = new PolygonOperationContext();
            ctx.Init(PolygonUtil.PolyOperation.Union, polygon1, polygon2);
            PolygonUnionInternal(ctx);
            union = ctx.Union;
            return ctx.mError;
        }


        protected static void PolygonUnionInternal(PolygonOperationContext ctx)
        {
            Point2DList union = ctx.Union;
            if (ctx.mStartingIndex == -1)
            {
                switch (ctx.mError)
                {
                    case PolygonUtil.PolyUnionError.NoIntersections:
                    case PolygonUtil.PolyUnionError.InfiniteLoop:
                        return;
                    case PolygonUtil.PolyUnionError.Poly1InsidePoly2:
                        union.AddRange(ctx.mOriginalPolygon2);
                        return;
                }
            }

            Point2DList currentPoly = ctx.mPoly1;
            Point2DList otherPoly = ctx.mPoly2;
            List<int> currentPolyVectorAngles = ctx.mPoly1VectorAngles;

            // Store the starting vertex so we can refer to it later.
            Point2D startingVertex = ctx.mPoly1[ctx.mStartingIndex];
            int currentIndex = ctx.mStartingIndex;
            int firstPoly2Index = -1;
            union.Clear();

            do
            {
                // Add the current vertex to the final union
                union.Add(currentPoly[currentIndex]);

                foreach (EdgeIntersectInfo intersect in ctx.mIntersections)
                {
                    // If the current point is an intersection point
                    if (currentPoly[currentIndex].Equals(intersect.IntersectionPoint, currentPoly.Epsilon))
                    {
                        // Make sure we want to swap polygons here.
                        int otherIndex = otherPoly.IndexOf(intersect.IntersectionPoint);

                        // If the next vertex, if we do swap, is not inside the current polygon,
                        // then its safe to swap, otherwise, just carry on with the current poly.
                        int comparePointIndex = otherPoly.NextIndex(otherIndex);
                        Point2D comparePoint = otherPoly[comparePointIndex];
                        bool bPointInPolygonAngle = false;
                        if (currentPolyVectorAngles[comparePointIndex] == -1)
                        {
                            bPointInPolygonAngle = ctx.PointInPolygonAngle(comparePoint, currentPoly);
                            currentPolyVectorAngles[comparePointIndex] = bPointInPolygonAngle ? 1 : 0;
                        }
                        else
                        {
                            bPointInPolygonAngle = (currentPolyVectorAngles[comparePointIndex] == 1) ? true : false;
                        }

                        if (!bPointInPolygonAngle)
                        {
                            // switch polygons
                            if (currentPoly == ctx.mPoly1)
                            {
                                currentPoly = ctx.mPoly2;
                                currentPolyVectorAngles = ctx.mPoly2VectorAngles;
                                otherPoly = ctx.mPoly1;
                                if (firstPoly2Index < 0)
                                {
                                    firstPoly2Index = otherIndex;
                                }
                            }
                            else
                            {
                                currentPoly = ctx.mPoly1;
                                currentPolyVectorAngles = ctx.mPoly1VectorAngles;
                                otherPoly = ctx.mPoly2;
                            }

                            // set currentIndex
                            currentIndex = otherIndex;

                            // Stop checking intersections for this point.
                            break;
                        }
                    }
                }

                // Move to next index
                currentIndex = currentPoly.NextIndex(currentIndex);

                if (currentPoly == ctx.mPoly1)
                {
                    if (currentIndex == 0)
                    {
                        break;
                    }
                }
                else
                {
                    if (firstPoly2Index >= 0 && currentIndex == firstPoly2Index)
                    {
                        break;
                    }
                }
            } while ((currentPoly[currentIndex] != startingVertex) && (union.Count <= (ctx.mPoly1.Count + ctx.mPoly2.Count)));

            // If the number of vertices in the union is more than the combined vertices
            // of the input polygons, then something is wrong and the algorithm will
            // loop forever. Luckily, we check for that.
            if (union.Count > (ctx.mPoly1.Count + ctx.mPoly2.Count))
            {
                ctx.mError = PolygonUtil.PolyUnionError.InfiniteLoop;
            }

            return;
        }


        /// <summary>
        /// Finds the intersection between two polygons.
        /// </summary>
        /// <param name="polygon1">The first polygon.</param>
        /// <param name="polygon2">The second polygon.</param>
        /// <param name="intersectOut">The intersection of the two polygons</param>
        /// <returns>error code</returns>
        public static PolygonUtil.PolyUnionError PolygonIntersect(Point2DList polygon1, Point2DList polygon2, out Point2DList intersectOut)
        {
            PolygonOperationContext ctx = new PolygonOperationContext();
            ctx.Init(PolygonUtil.PolyOperation.Intersect, polygon1, polygon2);
            PolygonIntersectInternal(ctx);
            intersectOut = ctx.Intersect;
            return ctx.mError;
        }


        protected static void PolygonIntersectInternal(PolygonOperationContext ctx)
        {
            Point2DList intersectOut = ctx.Intersect;
            if (ctx.mStartingIndex == -1)
            {
                switch (ctx.mError)
                {
                    case PolygonUtil.PolyUnionError.NoIntersections:
                    case PolygonUtil.PolyUnionError.InfiniteLoop:
                        return;
                    case PolygonUtil.PolyUnionError.Poly1InsidePoly2:
                        intersectOut.AddRange(ctx.mOriginalPolygon2);
                        return;
                }
            }

            Point2DList currentPoly = ctx.mPoly1;
            Point2DList otherPoly = ctx.mPoly2;
            List<int> currentPolyVectorAngles = ctx.mPoly1VectorAngles;

            // Store the starting vertex so we can refer to it later.            
            int currentIndex = ctx.mPoly1.IndexOf(ctx.mIntersections[0].IntersectionPoint);
            Point2D startingVertex = ctx.mPoly1[currentIndex];
            int firstPoly1Index = currentIndex;
            int firstPoly2Index = -1;
            intersectOut.Clear();

            do
            {
                // Add the current vertex to the final intersection
                if (intersectOut.Contains(currentPoly[currentIndex]))
                {
                    // This can happen when the two polygons only share a single edge, and neither is inside the other
                    break;
                }
                intersectOut.Add(currentPoly[currentIndex]);

                foreach (EdgeIntersectInfo intersect in ctx.mIntersections)
                {
                    // If the current point is an intersection point
                    if (currentPoly[currentIndex].Equals(intersect.IntersectionPoint, currentPoly.Epsilon))
                    {
                        // Make sure we want to swap polygons here.
                        int otherIndex = otherPoly.IndexOf(intersect.IntersectionPoint);

                        // If the next vertex, if we do swap, is inside the current polygon,
                        // then its safe to swap, otherwise, just carry on with the current poly.
                        int comparePointIndex = otherPoly.NextIndex(otherIndex);
                        Point2D comparePoint = otherPoly[comparePointIndex];
                        bool bPointInPolygonAngle = false;
                        if (currentPolyVectorAngles[comparePointIndex] == -1)
                        {
                            bPointInPolygonAngle = ctx.PointInPolygonAngle(comparePoint, currentPoly);
                            currentPolyVectorAngles[comparePointIndex] = bPointInPolygonAngle ? 1 : 0;
                        }
                        else
                        {
                            bPointInPolygonAngle = (currentPolyVectorAngles[comparePointIndex] == 1) ? true : false;
                        }

                        if (bPointInPolygonAngle)
                        {
                            // switch polygons
                            if (currentPoly == ctx.mPoly1)
                            {
                                currentPoly = ctx.mPoly2;
                                currentPolyVectorAngles = ctx.mPoly2VectorAngles;
                                otherPoly = ctx.mPoly1;
                                if (firstPoly2Index < 0)
                                {
                                    firstPoly2Index = otherIndex;
                                }
                            }
                            else
                            {
                                currentPoly = ctx.mPoly1;
                                currentPolyVectorAngles = ctx.mPoly1VectorAngles;
                                otherPoly = ctx.mPoly2;
                            }

                            // set currentIndex
                            currentIndex = otherIndex;

                            // Stop checking intersections for this point.
                            break;
                        }
                    }
                }

                // Move to next index
                currentIndex = currentPoly.NextIndex(currentIndex);

                if (currentPoly == ctx.mPoly1)
                {
                    if (currentIndex == firstPoly1Index)
                    {
                        break;
                    }
                }
                else
                {
                    if (firstPoly2Index >= 0 && currentIndex == firstPoly2Index)
                    {
                        break;
                    }
                }
            } while ((currentPoly[currentIndex] != startingVertex) && (intersectOut.Count <= (ctx.mPoly1.Count + ctx.mPoly2.Count)));

            // If the number of vertices in the union is more than the combined vertices
            // of the input polygons, then something is wrong and the algorithm will
            // loop forever. Luckily, we check for that.
            if (intersectOut.Count > (ctx.mPoly1.Count + ctx.mPoly2.Count))
            {
                ctx.mError = PolygonUtil.PolyUnionError.InfiniteLoop;
            }

            return;
        }


        /// <summary>
        /// Subtracts one polygon from another.
        /// </summary>
        /// <param name="polygon1">The base polygon.</param>
        /// <param name="polygon2">The polygon to subtract from the base.</param>
        /// <param name="subtract">The result of the polygon subtraction</param>
        /// <returns>error code</returns>
        public static PolygonUtil.PolyUnionError PolygonSubtract(Point2DList polygon1, Point2DList polygon2, out Point2DList subtract)
        {
            PolygonOperationContext ctx = new PolygonOperationContext();
            ctx.Init(PolygonUtil.PolyOperation.Subtract, polygon1, polygon2);
            PolygonSubtractInternal(ctx);
            subtract = ctx.Subtract;
            return ctx.mError;
        }


        public static void PolygonSubtractInternal(PolygonOperationContext ctx)
        {
            Point2DList subtract = ctx.Subtract;
            if (ctx.mStartingIndex == -1)
            {
                switch (ctx.mError)
                {
                    case PolygonUtil.PolyUnionError.NoIntersections:
                    case PolygonUtil.PolyUnionError.InfiniteLoop:
                    case PolygonUtil.PolyUnionError.Poly1InsidePoly2:
                        return;
                }
            }

            Point2DList currentPoly = ctx.mPoly1;
            Point2DList otherPoly = ctx.mPoly2;
            List<int> currentPolyVectorAngles = ctx.mPoly1VectorAngles;

            // Store the starting vertex so we can refer to it later.
            Point2D startingVertex = ctx.mPoly1[ctx.mStartingIndex];
            int currentIndex = ctx.mStartingIndex;
            subtract.Clear();

            // Trace direction
            bool forward = true;

            do
            {
                // Add the current vertex to the final union
                subtract.Add(currentPoly[currentIndex]);

                foreach (EdgeIntersectInfo intersect in ctx.mIntersections)
                {
                    // If the current point is an intersection point
                    if (currentPoly[currentIndex].Equals(intersect.IntersectionPoint, currentPoly.Epsilon))
                    {
                        // Make sure we want to swap polygons here.
                        int otherIndex = otherPoly.IndexOf(intersect.IntersectionPoint);

                        //Point2D otherVertex;
                        if (forward)
                        {
                            // If the next vertex, if we do swap, is inside the current polygon,
                            // then its safe to swap, otherwise, just carry on with the current poly.
                            int compareIndex = otherPoly.PreviousIndex(otherIndex);
                            Point2D compareVertex = otherPoly[compareIndex];
                            bool bPointInPolygonAngle = false;
                            if (currentPolyVectorAngles[compareIndex] == -1)
                            {
                                bPointInPolygonAngle = ctx.PointInPolygonAngle(compareVertex, currentPoly);
                                currentPolyVectorAngles[compareIndex] = bPointInPolygonAngle ? 1 : 0;
                            }
                            else
                            {
                                bPointInPolygonAngle = (currentPolyVectorAngles[compareIndex] == 1) ? true : false;
                            }

                            if (bPointInPolygonAngle)
                            {
                                // switch polygons
                                if (currentPoly == ctx.mPoly1)
                                {
                                    currentPoly = ctx.mPoly2;
                                    currentPolyVectorAngles = ctx.mPoly2VectorAngles;
                                    otherPoly = ctx.mPoly1;
                                }
                                else
                                {
                                    currentPoly = ctx.mPoly1;
                                    currentPolyVectorAngles = ctx.mPoly1VectorAngles;
                                    otherPoly = ctx.mPoly2;
                                }

                                // set currentIndex
                                currentIndex = otherIndex;

                                // Reverse direction
                                forward = !forward;

                                // Stop checking ctx.mIntersections for this point.
                                break;
                            }
                        }
                        else
                        {
                            // If the next vertex, if we do swap, is outside the current polygon,
                            // then its safe to swap, otherwise, just carry on with the current poly.
                            int compareIndex = otherPoly.NextIndex(otherIndex);
                            Point2D compareVertex = otherPoly[compareIndex];
                            bool bPointInPolygonAngle = false;
                            if (currentPolyVectorAngles[compareIndex] == -1)
                            {
                                bPointInPolygonAngle = ctx.PointInPolygonAngle(compareVertex, currentPoly);
                                currentPolyVectorAngles[compareIndex] = bPointInPolygonAngle ? 1 : 0;
                            }
                            else
                            {
                                bPointInPolygonAngle = (currentPolyVectorAngles[compareIndex] == 1) ? true : false;
                            }

                            if (!bPointInPolygonAngle)
                            {
                                // switch polygons
                                if (currentPoly == ctx.mPoly1)
                                {
                                    currentPoly = ctx.mPoly2;
                                    currentPolyVectorAngles = ctx.mPoly2VectorAngles;
                                    otherPoly = ctx.mPoly1;
                                }
                                else
                                {
                                    currentPoly = ctx.mPoly1;
                                    currentPolyVectorAngles = ctx.mPoly1VectorAngles;
                                    otherPoly = ctx.mPoly2;
                                }

                                // set currentIndex
                                currentIndex = otherIndex;

                                // Reverse direction
                                forward = !forward;

                                // Stop checking intersections for this point.
                                break;
                            }
                        }
                    }
                }

                if (forward)
                {
                    // Move to next index
                    currentIndex = currentPoly.NextIndex(currentIndex);
                }
                else
                {
                    currentIndex = currentPoly.PreviousIndex(currentIndex);
                }
            } while ((currentPoly[currentIndex] != startingVertex) && (subtract.Count <= (ctx.mPoly1.Count + ctx.mPoly2.Count)));


            // If the number of vertices in the union is more than the combined vertices
            // of the input polygons, then something is wrong and the algorithm will
            // loop forever. Luckily, we check for that.
            if (subtract.Count > (ctx.mPoly1.Count + ctx.mPoly2.Count))
            {
                ctx.mError = PolygonUtil.PolyUnionError.InfiniteLoop;
            }

            return;
        }


        /// <summary>
        /// Performs one or more polygon operations on the 2 provided polygons
        /// </summary>
        /// <param name="polygon1">The first polygon.</param>
        /// <param name="polygon2">The second polygon</param>
        /// <param name="subtract">The result of the polygon subtraction</param>
        /// <returns>error code</returns>
        public static PolygonUtil.PolyUnionError PolygonOperation(PolygonUtil.PolyOperation operations, Point2DList polygon1, Point2DList polygon2, out Dictionary<uint, Point2DList> results)
        {
            PolygonOperationContext ctx = new PolygonOperationContext();
            ctx.Init(operations, polygon1, polygon2);
            results = ctx.mOutput;
            return PolygonUtil.PolygonOperation(ctx);
        }


        public static PolygonUtil.PolyUnionError PolygonOperation(PolygonOperationContext ctx)
        {
            if ((ctx.mOperations & PolygonUtil.PolyOperation.Union) == PolygonUtil.PolyOperation.Union)
            {
                PolygonUtil.PolygonUnionInternal(ctx);
            }
            if ((ctx.mOperations & PolygonUtil.PolyOperation.Intersect) == PolygonUtil.PolyOperation.Intersect)
            {
                PolygonUtil.PolygonIntersectInternal(ctx);
            }
            if ((ctx.mOperations & PolygonUtil.PolyOperation.Subtract) == PolygonUtil.PolyOperation.Subtract)
            {
                PolygonUtil.PolygonSubtractInternal(ctx);
            }

            return ctx.mError;
        }


        /// <summary>
        /// Trace the edge of a non-simple polygon and return a simple polygon.
        /// 
        ///Method:
        ///Start at vertex with minimum y (pick maximum x one if there are two).  
        ///We aim our "lastDir" vector at (1.0, 0)
        ///We look at the two rays going off from our start vertex, and follow whichever
        ///has the smallest angle (in -Pi . Pi) wrt lastDir ("rightest" turn)
        ///
        ///Loop until we hit starting vertex:
        ///
        ///We add our current vertex to the list.
        ///We check the seg from current vertex to next vertex for intersections
        ///  - if no intersections, follow to next vertex and continue
        ///  - if intersections, pick one with minimum distance
        ///    - if more than one, pick one with "rightest" next point (two possibilities for each)
        ///    
        /// </summary>
        /// <param name="verts"></param>
        /// <returns></returns>
        public static List<Point2DList> SplitComplexPolygon(Point2DList verts, double epsilon)
        {
            int numVerts = verts.Count;
            int nNodes = 0;
            List<SplitComplexPolygonNode> nodes = new List<SplitComplexPolygonNode>();

            //Add base nodes (raw outline)
            for (int i = 0; i < verts.Count; ++i)
            {
                SplitComplexPolygonNode newNode = new SplitComplexPolygonNode(new Point2D(verts[i].X, verts[i].Y));
                nodes.Add(newNode);
            }
            for (int i = 0; i < verts.Count; ++i)
            {
                int iplus = (i == numVerts - 1) ? 0 : i + 1;
                int iminus = (i == 0) ? numVerts - 1 : i - 1;
                nodes[i].AddConnection(nodes[iplus]);
                nodes[i].AddConnection(nodes[iminus]);
            }
            nNodes = nodes.Count;

            //Process intersection nodes - horribly inefficient
            bool dirty = true;
            int counter = 0;
            while (dirty)
            {
                dirty = false;
                for (int i = 0; !dirty && i < nNodes; ++i)
                {
                    for (int j = 0; !dirty && j < nodes[i].NumConnected; ++j)
                    {
                        for (int k = 0; !dirty && k < nNodes; ++k)
                        {
                            if (k == i || nodes[k] == nodes[i][j])
                            {
                                continue;
                            }
                            for (int l = 0; !dirty && l < nodes[k].NumConnected; ++l)
                            {
                                if (nodes[k][l] == nodes[i][j] || nodes[k][l] == nodes[i])
                                {
                                    continue;
                                }
                                //Check intersection
                                Point2D intersectPt = new Point2D();
                                //if (counter > 100) printf("checking intersection: %d, %d, %d, %d\n",i,j,k,l);
                                bool crosses = TriangulationUtil.LinesIntersect2D(nodes[i].Position,
                                                                                    nodes[i][j].Position,
                                                                                    nodes[k].Position,
                                                                                    nodes[k][l].Position,
                                                                                    true, true, true,
                                                                                    ref intersectPt,
                                                                                    epsilon);
                                if (crosses)
                                {
                                    /*if (counter > 100) {
                                        printf("Found crossing at %f, %f\n",intersectPt.x, intersectPt.y);
                                        printf("Locations: %f,%f - %f,%f | %f,%f - %f,%f\n",
                                                        nodes[i].position.x, nodes[i].position.y,
                                                        nodes[i].connected[j].position.x, nodes[i].connected[j].position.y,
                                                        nodes[k].position.x,nodes[k].position.y,
                                                        nodes[k].connected[l].position.x,nodes[k].connected[l].position.y);
                                        printf("Memory addresses: %d, %d, %d, %d\n",(int)&nodes[i],(int)nodes[i].connected[j],(int)&nodes[k],(int)nodes[k].connected[l]);
                                    }*/
                                    dirty = true;
                                    //Destroy and re-hook connections at crossing point
                                    SplitComplexPolygonNode intersectionNode = new SplitComplexPolygonNode(intersectPt);
                                    int idx = nodes.IndexOf(intersectionNode);
                                    if (idx >= 0 && idx < nodes.Count)
                                    {
                                        intersectionNode = nodes[idx];
                                    }
                                    else
                                    {
                                        nodes.Add(intersectionNode);
                                        nNodes = nodes.Count;
                                    }

                                    SplitComplexPolygonNode nodei = nodes[i];
                                    SplitComplexPolygonNode connij = nodes[i][j];
                                    SplitComplexPolygonNode nodek = nodes[k];
                                    SplitComplexPolygonNode connkl = nodes[k][l];
                                    connij.RemoveConnection(nodei);
                                    nodei.RemoveConnection(connij);
                                    connkl.RemoveConnection(nodek);
                                    nodek.RemoveConnection(connkl);
                                    if (!intersectionNode.Position.Equals(nodei.Position, epsilon))
                                    {
                                        intersectionNode.AddConnection(nodei);
                                        nodei.AddConnection(intersectionNode);
                                    }
                                    if (!intersectionNode.Position.Equals(nodek.Position, epsilon))
                                    {
                                        intersectionNode.AddConnection(nodek);
                                        nodek.AddConnection(intersectionNode);
                                    }
                                    if (!intersectionNode.Position.Equals(connij.Position, epsilon))
                                    {
                                        intersectionNode.AddConnection(connij);
                                        connij.AddConnection(intersectionNode);
                                    }
                                    if (!intersectionNode.Position.Equals(connkl.Position, epsilon))
                                    {
                                        intersectionNode.AddConnection(connkl);
                                        connkl.AddConnection(intersectionNode);
                                    }
                                }
                            }
                        }
                    }
                }
                ++counter;
                //if (counter > 100) printf("Counter: %d\n",counter);
            }

            //    /*
            //    // Debugging: check for connection consistency
            //    for (int i=0; i<nNodes; ++i) {
            //        int nConn = nodes[i].nConnected;
            //        for (int j=0; j<nConn; ++j) {
            //            if (nodes[i].connected[j].nConnected == 0) Assert(false);
            //            SplitComplexPolygonNode* connect = nodes[i].connected[j];
            //            bool found = false;
            //            for (int k=0; k<connect.nConnected; ++k) {
            //                if (connect.connected[k] == &nodes[i]) found = true;
            //            }
            //            Assert(found);
            //        }
            //    }*/

            //Collapse duplicate points
            bool foundDupe = true;
            int nActive = nNodes;
            double epsilonSquared = epsilon * epsilon;
            while (foundDupe)
            {
                foundDupe = false;
                for (int i = 0; i < nNodes; ++i)
                {
                    if (nodes[i].NumConnected == 0)
                    {
                        continue;
                    }
                    for (int j = i + 1; j < nNodes; ++j)
                    {
                        if (nodes[j].NumConnected == 0)
                        {
                            continue;
                        }
                        Point2D diff = nodes[i].Position - nodes[j].Position;
                        if (diff.MagnitudeSquared() <= epsilonSquared)
                        {
                            if (nActive <= 3)
                            {
                                throw new Exception("Eliminated so many duplicate points that resulting polygon has < 3 vertices!");
                            }

                            //printf("Found dupe, %d left\n",nActive);
                            --nActive;
                            foundDupe = true;
                            SplitComplexPolygonNode inode = nodes[i];
                            SplitComplexPolygonNode jnode = nodes[j];
                            //Move all of j's connections to i, and remove j
                            int njConn = jnode.NumConnected;
                            for (int k = 0; k < njConn; ++k)
                            {
                                SplitComplexPolygonNode knode = jnode[k];
                                //Debug.Assert(knode != jnode);
                                if (knode != inode)
                                {
                                    inode.AddConnection(knode);
                                    knode.AddConnection(inode);
                                }
                                knode.RemoveConnection(jnode);
                                //printf("knode %d on node %d now has %d connections\n",k,j,knode.nConnected);
                                //printf("Found duplicate point.\n");
                            }
                            jnode.ClearConnections();   // to help with garbage collection
                            nodes.RemoveAt(j);
                            --nNodes;
                        }
                    }
                }
            }

            //    /*
            //    // Debugging: check for connection consistency
            //    for (int i=0; i<nNodes; ++i) {
            //        int nConn = nodes[i].nConnected;
            //        printf("Node %d has %d connections\n",i,nConn);
            //        for (int j=0; j<nConn; ++j) {
            //            if (nodes[i].connected[j].nConnected == 0) {
            //                printf("Problem with node %d connection at address %d\n",i,(int)(nodes[i].connected[j]));
            //                Assert(false);
            //            }
            //            SplitComplexPolygonNode* connect = nodes[i].connected[j];
            //            bool found = false;
            //            for (int k=0; k<connect.nConnected; ++k) {
            //                if (connect.connected[k] == &nodes[i]) found = true;
            //            }
            //            if (!found) printf("Connection %d (of %d) on node %d (of %d) did not have reciprocal connection.\n",j,nConn,i,nNodes);
            //            Assert(found);
            //        }
            //    }//*/

            //Now walk the edge of the list

            //Find node with minimum y value (max x if equal)
            double minY = double.MaxValue;
            double maxX = -double.MaxValue;
            int minYIndex = -1;
            for (int i = 0; i < nNodes; ++i)
            {
                if (nodes[i].Position.Y < minY && nodes[i].NumConnected > 1)
                {
                    minY = nodes[i].Position.Y;
                    minYIndex = i;
                    maxX = nodes[i].Position.X;
                }
                else if (nodes[i].Position.Y == minY && nodes[i].Position.X > maxX && nodes[i].NumConnected > 1)
                {
                    minYIndex = i;
                    maxX = nodes[i].Position.X;
                }
            }

            Point2D origDir = new Point2D(1.0f, 0.0f);
            List<Point2D> resultVecs = new List<Point2D>();
            SplitComplexPolygonNode currentNode = nodes[minYIndex];
            SplitComplexPolygonNode startNode = currentNode;
            //Debug.Assert(currentNode.nConnected > 0);
            SplitComplexPolygonNode nextNode = currentNode.GetRightestConnection(origDir);
            if (nextNode == null)
            {
                // Borked, clean up our mess and return
                return PolygonUtil.SplitComplexPolygonCleanup(verts);
            }

            resultVecs.Add(startNode.Position);
            while (nextNode != startNode)
            {
                if (resultVecs.Count > (4 * nNodes))
                {
                    //printf("%d, %d, %d\n",(int)startNode,(int)currentNode,(int)nextNode);
                    //printf("%f, %f . %f, %f\n",currentNode.position.x,currentNode.position.y, nextNode.position.x, nextNode.position.y);
                    //verts.printFormatted();
                    //printf("Dumping connection graph: \n");
                    //for (int i=0; i<nNodes; ++i)
                    //{
                    //    printf("nodex[%d] = %f; nodey[%d] = %f;\n",i,nodes[i].position.x,i,nodes[i].position.y);
                    //    printf("//connected to\n");
                    //    for (int j=0; j<nodes[i].nConnected; ++j)
                    //    {
                    //        printf("connx[%d][%d] = %f; conny[%d][%d] = %f;\n",i,j,nodes[i].connected[j].position.x, i,j,nodes[i].connected[j].position.y);
                    //    }
                    //}
                    //printf("Dumping results thus far: \n");
                    //for (int i=0; i<nResultVecs; ++i)
                    //{
                    //    printf("x[%d]=map(%f,-3,3,0,width); y[%d] = map(%f,-3,3,height,0);\n",i,resultVecs[i].x,i,resultVecs[i].y);
                    //}
                    //Debug.Assert(false);
                    //nodes should never be visited four times apiece (proof?), so we've probably hit a loop...crap
                    throw new Exception("nodes should never be visited four times apiece (proof?), so we've probably hit a loop...crap");
                }
                resultVecs.Add(nextNode.Position);
                SplitComplexPolygonNode oldNode = currentNode;
                currentNode = nextNode;
                //printf("Old node connections = %d; address %d\n",oldNode.nConnected, (int)oldNode);
                //printf("Current node connections = %d; address %d\n",currentNode.nConnected, (int)currentNode);
                nextNode = currentNode.GetRightestConnection(oldNode);
                if (nextNode == null)
                {
                    return PolygonUtil.SplitComplexPolygonCleanup(resultVecs);
                }
                // There was a problem, so jump out of the loop and use whatever garbage we've generated so far
                //printf("nextNode address: %d\n",(int)nextNode);
            }

            if (resultVecs.Count < 1)
            {
                // Borked, clean up our mess and return
                return PolygonUtil.SplitComplexPolygonCleanup(verts);
            }
            else
            {
                return PolygonUtil.SplitComplexPolygonCleanup(resultVecs);
            }
        }


        private static List<Point2DList> SplitComplexPolygonCleanup(IList<Point2D> orig)
        {
            List<Point2DList> l = new List<Point2DList>();
            Point2DList origP2DL = new Point2DList(orig);
            l.Add(origP2DL);
            int listIdx = 0;
            int numLists = l.Count;
            while (listIdx < numLists)
            {
                int numPoints = l[listIdx].Count;
                for (int i = 0; i < numPoints; ++i)
                {
                    for (int j = i + 1; j < numPoints; ++j)
                    {
                        if (l[listIdx][i].Equals(l[listIdx][j], origP2DL.Epsilon))
                        {
                            // found a self-intersection loop - split it off into it's own list
                            int numToRemove = j - i;
                            Point2DList newList = new Point2DList();
                            for (int k = i + 1; k <= j; ++k)
                            {
                                newList.Add(l[listIdx][k]);
                            }
                            l[listIdx].RemoveRange(i + 1, numToRemove);
                            l.Add(newList);
                            ++numLists;
                            numPoints -= numToRemove;
                            j = i + 1;
                        }
                    }
                }
                l[listIdx].Simplify();
                ++listIdx;
            }

            return l;
        }

    }


    public class EdgeIntersectInfo
    {
        public EdgeIntersectInfo(Edge edgeOne, Edge edgeTwo, Point2D intersectionPoint)
        {
            EdgeOne = edgeOne;
            EdgeTwo = edgeTwo;
            IntersectionPoint = intersectionPoint;
        }

        public Edge EdgeOne { get; private set; }
        public Edge EdgeTwo { get; private set; }
        public Point2D IntersectionPoint { get; private set; }
    }


    public class SplitComplexPolygonNode
    {
        /*
         * Given sines and cosines, tells if A's angle is less than B's on -Pi, Pi
         * (in other words, is A "righter" than B)
         */
        private List<SplitComplexPolygonNode> mConnected = new List<SplitComplexPolygonNode>();
        private Point2D mPosition = null;

        public int NumConnected { get { return mConnected.Count; } }
        public Point2D Position { get { return mPosition; } set { mPosition = value; } }
        public SplitComplexPolygonNode this[int index]
        {
            get { return mConnected[index]; }
        }


        public SplitComplexPolygonNode()
        {
        }

        public SplitComplexPolygonNode(Point2D pos)
        {
            mPosition = pos;
        }


        public override bool Equals(Object obj)
        {
            SplitComplexPolygonNode pn = obj as SplitComplexPolygonNode;
            if (pn == null)
            {
                return base.Equals(obj);
            }

            return Equals(pn);
        }


        public bool Equals(SplitComplexPolygonNode pn)
        {
            if ((Object)pn == null)
            {
                return false;
            }
            if (mPosition == null || pn.Position == null)
            {
                return false;
            }

            return mPosition.Equals(pn.Position);
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(SplitComplexPolygonNode lhs, SplitComplexPolygonNode rhs) { if ((object)lhs != null) { return lhs.Equals(rhs); } if ((Object)rhs == null) { return true; } else { return false; } }
        public static bool operator !=(SplitComplexPolygonNode lhs, SplitComplexPolygonNode rhs) { if ((object)lhs != null) { return !lhs.Equals(rhs); } if ((Object)rhs == null) { return false; } else { return true; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append(mPosition.ToString());
            sb.Append(" -> ");
            for (int i = 0; i < NumConnected; ++i)
            {
                if (i != 0)
                {
                    sb.Append(", ");
                }
                sb.Append(mConnected[i].Position.ToString());
            }

            return sb.ToString();
        }


        private bool IsRighter(double sinA, double cosA, double sinB, double cosB)
        {
            if (sinA < 0)
            {
                if (sinB > 0 || cosA <= cosB)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (sinB < 0 || cosA <= cosB)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        //Fix for obnoxious behavior for the % operator for negative numbers...
        private int remainder(int x, int modulus)
        {
            int rem = x % modulus;
            while (rem < 0)
            {
                rem += modulus;
            }
            return rem;
        }

        public void AddConnection(SplitComplexPolygonNode toMe)
        {
            // Ignore duplicate additions
            if (!mConnected.Contains(toMe) && toMe != this)
            {
                mConnected.Add(toMe);
            }
        }

        public void RemoveConnection(SplitComplexPolygonNode fromMe)
        {
            mConnected.Remove(fromMe);
        }

        private void RemoveConnectionByIndex(int index)
        {
            if (index < 0 || index >= mConnected.Count)
            {
                return;
            }
            mConnected.RemoveAt(index);
        }

        public void ClearConnections()
        {
            mConnected.Clear();
        }

        private bool IsConnectedTo(SplitComplexPolygonNode me)
        {
            return mConnected.Contains(me);
        }

        public SplitComplexPolygonNode GetRightestConnection(SplitComplexPolygonNode incoming)
        {
            if (NumConnected == 0)
            {
                throw new Exception("the connection graph is inconsistent");
            }
            if (NumConnected == 1)
            {
                //b2Assert(false);
                // Because of the possibility of collapsing nearby points,
                // we may end up with "spider legs" dangling off of a region.
                // The correct behavior here is to turn around.
                return incoming;
            }
            Point2D inDir = mPosition - incoming.mPosition;

            double inLength = inDir.Magnitude();
            inDir.Normalize();

            if (inLength <= MathUtil.EPSILON)
            {
                throw new Exception("Length too small");
            }

            SplitComplexPolygonNode result = null;
            for (int i = 0; i < NumConnected; ++i)
            {
                if (mConnected[i] == incoming)
                {
                    continue;
                }
                Point2D testDir = mConnected[i].mPosition - mPosition;
                double testLengthSqr = testDir.MagnitudeSquared();
                testDir.Normalize();
                /*
                if (testLengthSqr < COLLAPSE_DIST_SQR) {
                    printf("Problem with connection %d\n",i);
                    printf("This node has %d connections\n",nConnected);
                    printf("That one has %d\n",connected[i].nConnected);
                    if (this == connected[i]) printf("This points at itself.\n");
                }*/
                if (testLengthSqr <= (MathUtil.EPSILON * MathUtil.EPSILON))
                {
                    throw new Exception("Length too small");
                }

                double myCos = Point2D.Dot(inDir, testDir);
                double mySin = Point2D.Cross(inDir, testDir);
                if (result != null)
                {
                    Point2D resultDir = result.mPosition - mPosition;
                    resultDir.Normalize();
                    double resCos = Point2D.Dot(inDir, resultDir);
                    double resSin = Point2D.Cross(inDir, resultDir);
                    if (IsRighter(mySin, myCos, resSin, resCos))
                    {
                        result = mConnected[i];
                    }
                }
                else
                {
                    result = mConnected[i];
                }
            }

            //if (B2_POLYGON_REPORT_ERRORS && result != null)
            //{
            //    printf("nConnected = %d\n", nConnected);
            //    for (int i = 0; i < nConnected; ++i)
            //    {
            //        printf("connected[%d] @ %d\n", i, (int)connected[i]);
            //    }
            //}
            //Debug.Assert(result != null);

            return result;
        }

        public SplitComplexPolygonNode GetRightestConnection(Point2D incomingDir)
        {
            Point2D diff = mPosition - incomingDir;
            SplitComplexPolygonNode temp = new SplitComplexPolygonNode(diff);
            SplitComplexPolygonNode res = GetRightestConnection(temp);
            //Debug.Assert(res != null);
            return res;
        }
    }


    public class PolygonOperationContext
    {
        public PolygonUtil.PolyOperation mOperations;
        public Point2DList mOriginalPolygon1;
        public Point2DList mOriginalPolygon2;
        public Point2DList mPoly1;
        public Point2DList mPoly2;
        public List<EdgeIntersectInfo> mIntersections;
        public int mStartingIndex;
        public PolygonUtil.PolyUnionError mError;
        public List<int> mPoly1VectorAngles;
        public List<int> mPoly2VectorAngles;
        public Dictionary<uint, Point2DList> mOutput = new Dictionary<uint, Point2DList>();

        public Point2DList Union
        {
            get
            {
                Point2DList l = null;
                if (!mOutput.TryGetValue((uint)PolygonUtil.PolyOperation.Union, out l))
                {
                    l = new Point2DList();
                    mOutput.Add((uint)PolygonUtil.PolyOperation.Union, l);
                }

                return l;
            }
        }
        public Point2DList Intersect
        {
            get
            {
                Point2DList l = null;
                if (!mOutput.TryGetValue((uint)PolygonUtil.PolyOperation.Intersect, out l))
                {
                    l = new Point2DList();
                    mOutput.Add((uint)PolygonUtil.PolyOperation.Intersect, l);
                }

                return l;
            }
        }
        public Point2DList Subtract
        {
            get
            {
                Point2DList l = null;
                if (!mOutput.TryGetValue((uint)PolygonUtil.PolyOperation.Subtract, out l))
                {
                    l = new Point2DList();
                    mOutput.Add((uint)PolygonUtil.PolyOperation.Subtract, l);
                }

                return l;
            }
        }

        public PolygonOperationContext() { }


        public void Clear()
        {
            mOperations = PolygonUtil.PolyOperation.None;
            mOriginalPolygon1 = null;
            mOriginalPolygon2 = null;
            mPoly1 = null;
            mPoly2 = null;
            mIntersections = null;
            mStartingIndex = -1;
            mError = PolygonUtil.PolyUnionError.None;
            mPoly1VectorAngles = null;
            mPoly2VectorAngles = null;
            mOutput = new Dictionary<uint, Point2DList>();
        }


        public bool Init(PolygonUtil.PolyOperation operations, Point2DList polygon1, Point2DList polygon2)
        {
            Clear();

            mOperations = operations;
            mOriginalPolygon1 = polygon1;
            mOriginalPolygon2 = polygon2;

            // Make a copy of the polygons so that we dont modify the originals, and
            // force vertices to integer (pixel) values.
            mPoly1 = new Point2DList(polygon1);
            mPoly1.WindingOrder = Point2DList.WindingOrderType.Default;
            mPoly2 = new Point2DList(polygon2);
            mPoly2.WindingOrder = Point2DList.WindingOrderType.Default;

            // Find intersection points
            if (!VerticesIntersect(mPoly1, mPoly2, out mIntersections))
            {
                // No intersections found - polygons do not overlap.
                mError = PolygonUtil.PolyUnionError.NoIntersections;
                return false;
            }

            // make sure edges that intersect more than once are updated to have correct start points
            int numIntersections = mIntersections.Count;
            for (int i = 0; i < numIntersections; ++i)
            {
                for (int j = i + 1; j < numIntersections; ++j)
                {
                    if (mIntersections[i].EdgeOne.EdgeStart.Equals(mIntersections[j].EdgeOne.EdgeStart) &&
                        mIntersections[i].EdgeOne.EdgeEnd.Equals(mIntersections[j].EdgeOne.EdgeEnd))
                    {
                        mIntersections[j].EdgeOne.EdgeStart = mIntersections[i].IntersectionPoint;
                    }
                    if (mIntersections[i].EdgeTwo.EdgeStart.Equals(mIntersections[j].EdgeTwo.EdgeStart) &&
                        mIntersections[i].EdgeTwo.EdgeEnd.Equals(mIntersections[j].EdgeTwo.EdgeEnd))
                    {
                        mIntersections[j].EdgeTwo.EdgeStart = mIntersections[i].IntersectionPoint;
                    }
                }
            }

            // Add intersection points to original polygons, ignoring existing points.
            foreach (EdgeIntersectInfo intersect in mIntersections)
            {
                if (!mPoly1.Contains(intersect.IntersectionPoint))
                {
                    mPoly1.Insert(mPoly1.IndexOf(intersect.EdgeOne.EdgeStart) + 1, intersect.IntersectionPoint);
                }

                if (!mPoly2.Contains(intersect.IntersectionPoint))
                {
                    mPoly2.Insert(mPoly2.IndexOf(intersect.EdgeTwo.EdgeStart) + 1, intersect.IntersectionPoint);
                }
            }

            mPoly1VectorAngles = new List<int>();
            for (int i = 0; i < mPoly2.Count; ++i)
            {
                mPoly1VectorAngles.Add(-1);
            }
            mPoly2VectorAngles = new List<int>();
            for (int i = 0; i < mPoly1.Count; ++i)
            {
                mPoly2VectorAngles.Add(-1);
            }

            // Find starting point on the edge of polygon1 that is outside of
            // the intersected area to begin polygon trace.
            int currentIndex = 0;
            do
            {
                bool bPointInPolygonAngle = PointInPolygonAngle(mPoly1[currentIndex], mPoly2);
                mPoly2VectorAngles[currentIndex] = bPointInPolygonAngle ? 1 : 0;
                if (bPointInPolygonAngle)
                {
                    mStartingIndex = currentIndex;
                    break;
                }
                currentIndex = mPoly1.NextIndex(currentIndex);
            } while (currentIndex != 0);

            // If we don't find a point on polygon1 thats outside of the
            // intersect area, the polygon1 must be inside of polygon2,
            // in which case, polygon2 IS the union of the two.
            if (mStartingIndex == -1)
            {
                mError = PolygonUtil.PolyUnionError.Poly1InsidePoly2;
                return false;
            }

            return true;
        }


        /// <summary>
        /// Check and return polygon intersections
        /// </summary>
        /// <param name="polygon1"></param>
        /// <param name="polygon2"></param>
        /// <param name="intersections"></param>
        /// <returns></returns>
        private bool VerticesIntersect(Point2DList polygon1, Point2DList polygon2, out List<EdgeIntersectInfo> intersections)
        {
            intersections = new List<EdgeIntersectInfo>();
            double epsilon = Math.Min(polygon1.Epsilon, polygon2.Epsilon);

            // Iterate through polygon1's edges
            for (int i = 0; i < polygon1.Count; i++)
            {
                // Get edge vertices
                Point2D p1 = polygon1[i];
                Point2D p2 = polygon1[polygon1.NextIndex(i)];

                // Get intersections between this edge and polygon2
                for (int j = 0; j < polygon2.Count; j++)
                {
                    Point2D point = new Point2D();

                    Point2D p3 = polygon2[j];
                    Point2D p4 = polygon2[polygon2.NextIndex(j)];

                    // Check if the edges intersect
                    if (TriangulationUtil.LinesIntersect2D(p1, p2, p3, p4, ref point, epsilon))
                    {
                        // Rounding is not needed since we compare using an epsilon.
                        //// Here, we round the returned intersection point to its nearest whole number.
                        //// This prevents floating point anomolies where 99.9999-> is returned instead of 100.
                        //point = new Point2D((float)Math.Round(point.X, 0), (float)Math.Round(point.Y, 0));
                        // Record the intersection
                        intersections.Add(new EdgeIntersectInfo(new Edge(p1, p2), new Edge(p3, p4), point));
                    }
                }
            }

            // true if any intersections were found.
            return (intersections.Count > 0);
        }


        /// <summary>
        /// * ref: http://ozviz.wasp.uwa.edu.au/~pbourke/geometry/insidepoly/  - Solution 2 
        /// * Compute the sum of the angles made between the test point and each pair of points making up the polygon. 
        /// * If this sum is 2pi then the point is an interior point, if 0 then the point is an exterior point. 
        /// </summary>
        public bool PointInPolygonAngle(Point2D point, Point2DList polygon)
        {
            double angle = 0;

            // Iterate through polygon's edges
            for (int i = 0; i < polygon.Count; i++)
            {
                // Get points
                Point2D p1 = polygon[i] - point;
                Point2D p2 = polygon[polygon.NextIndex(i)] - point;

                angle += VectorAngle(p1, p2);
            }

            if (Math.Abs(angle) < Math.PI)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Return the angle between two vectors on a plane
        /// The angle is from vector 1 to vector 2, positive anticlockwise
        /// The result is between -pi -> pi
        /// </summary>
        public double VectorAngle(Point2D p1, Point2D p2)
        {
            double theta1 = Math.Atan2(p1.Y, p1.X);
            double theta2 = Math.Atan2(p2.Y, p2.X);
            double dtheta = theta2 - theta1;
            while (dtheta > Math.PI)
            {
                dtheta -= (2.0 * Math.PI);
            }
            while (dtheta < -Math.PI)
            {
                dtheta += (2.0 * Math.PI);
            }

            return (dtheta);
        }

    }

    public class ConstrainedPointSet : PointSet
    {
        protected Dictionary<uint, TriangulationConstraint> mConstraintMap = new Dictionary<uint, TriangulationConstraint>();
        protected List<Contour> mHoles = new List<Contour>();

        public override TriangulationMode TriangulationMode { get { return TriangulationMode.Constrained; } }


        public ConstrainedPointSet(List<TriangulationPoint> bounds)
            : base(bounds)
        {
            AddBoundaryConstraints();
        }

        public ConstrainedPointSet(List<TriangulationPoint> bounds, List<TriangulationConstraint> constraints)
            : base(bounds)
        {
            AddBoundaryConstraints();
            AddConstraints(constraints);
        }

        public ConstrainedPointSet(List<TriangulationPoint> bounds, int[] indices)
            : base(bounds)
        {
            AddBoundaryConstraints();
            List<TriangulationConstraint> l = new List<TriangulationConstraint>();
            for (int i = 0; i < indices.Length; i += 2)
            {
                TriangulationConstraint tc = new TriangulationConstraint(bounds[i], bounds[i + 1]);
                l.Add(tc);
            }
            AddConstraints(l);
        }


        protected void AddBoundaryConstraints()
        {
            TriangulationPoint ptLL = null;
            TriangulationPoint ptLR = null;
            TriangulationPoint ptUR = null;
            TriangulationPoint ptUL = null;
            if (!TryGetPoint(MinX, MinY, out ptLL))
            {
                ptLL = new TriangulationPoint(MinX, MinY);
                Add(ptLL);
            }
            if (!TryGetPoint(MaxX, MinY, out ptLR))
            {
                ptLR = new TriangulationPoint(MaxX, MinY);
                Add(ptLR);
            }
            if (!TryGetPoint(MaxX, MaxY, out ptUR))
            {
                ptUR = new TriangulationPoint(MaxX, MaxY);
                Add(ptUR);
            }
            if (!TryGetPoint(MinX, MaxY, out ptUL))
            {
                ptUL = new TriangulationPoint(MinX, MaxY);
                Add(ptUL);
            }
            TriangulationConstraint tcLLtoLR = new TriangulationConstraint(ptLL, ptLR);
            AddConstraint(tcLLtoLR);
            TriangulationConstraint tcLRtoUR = new TriangulationConstraint(ptLR, ptUR);
            AddConstraint(tcLRtoUR);
            TriangulationConstraint tcURtoUL = new TriangulationConstraint(ptUR, ptUL);
            AddConstraint(tcURtoUL);
            TriangulationConstraint tcULtoLL = new TriangulationConstraint(ptUL, ptLL);
            AddConstraint(tcULtoLL);
        }


        public override void Add(Point2D p)
        {
            Add(p as TriangulationPoint, -1, true);
        }


        public override void Add(TriangulationPoint p)
        {
            Add(p, -1, true);
        }


        public override bool AddRange(List<TriangulationPoint> points)
        {
            bool bOK = true;
            foreach (TriangulationPoint p in points)
            {
                bOK = Add(p, -1, true) && bOK;
            }

            return bOK;
        }


        // Assumes that points being passed in the list are connected and form a polygon.
        // Note that some error checking is done for robustness, but for the most part,
        // we have to rely on the user to feed us "correct" data
        public bool AddHole(List<TriangulationPoint> points, string name)
        {
            if (points == null)
            {
                return false;
            }

            //// split our self-intersection sections into their own lists
            List<Contour> pts = new List<Contour>();
            int listIdx = 0;
            {
                Contour c = new Contour(this, points, WindingOrderType.Unknown);
                pts.Add(c);

                // only constrain the points if we actually HAVE a bounding rect
                if (mPoints.Count > 1)
                {
                    // constrain the points to bounding rect
                    int numPoints = pts[listIdx].Count;
                    for (int i = 0; i < numPoints; ++i)
                    {
                        ConstrainPointToBounds(pts[listIdx][i]);
                    }
                }
            }

            while (listIdx < pts.Count)
            {
                // simple sanity checking - remove duplicate coincident points before
                // we check the polygon: fast, simple algorithm that eliminate lots of problems
                // that only more expensive checks will find
                pts[listIdx].RemoveDuplicateNeighborPoints();
                pts[listIdx].WindingOrder = Point2DList.WindingOrderType.Default;

                bool bListOK = true;
                Point2DList.PolygonError err = pts[listIdx].CheckPolygon();
                while (bListOK && err != PolygonError.None)
                {
                    if ((err & PolygonError.NotEnoughVertices) == PolygonError.NotEnoughVertices)
                    {
                        bListOK = false;
                        continue;
                    }
                    if ((err & PolygonError.NotSimple) == PolygonError.NotSimple)
                    {
                        // split the polygons, remove the current list and add the resulting list to the end
                        //List<Point2DList> l = TriangulationUtil.SplitSelfIntersectingPolygon(pts[listIdx], pts[listIdx].Epsilon);
                        List<Point2DList> l = PolygonUtil.SplitComplexPolygon(pts[listIdx], pts[listIdx].Epsilon);
                        pts.RemoveAt(listIdx);
                        foreach (Point2DList newList in l)
                        {
                            Contour c = new Contour(this);
                            c.AddRange(newList);
                            pts.Add(c);
                        }
                        err = pts[listIdx].CheckPolygon();
                        continue;
                    }
                    if ((err & PolygonError.Degenerate) == PolygonError.Degenerate)
                    {
                        pts[listIdx].Simplify(this.Epsilon);
                        err = pts[listIdx].CheckPolygon();
                        continue;
                        //err &= ~(PolygonError.Degenerate);
                        //if (pts[listIdx].Count < 3)
                        //{
                        //    err |= PolygonError.NotEnoughVertices;
                        //    bListOK = false;
                        //    continue;
                        //}
                    }
                    if ((err & PolygonError.AreaTooSmall) == PolygonError.AreaTooSmall ||
                        (err & PolygonError.SidesTooCloseToParallel) == PolygonError.SidesTooCloseToParallel ||
                        (err & PolygonError.TooThin) == PolygonError.TooThin ||
                        (err & PolygonError.Unknown) == PolygonError.Unknown)
                    {
                        bListOK = false;
                        continue;
                    }
                    // non-convex polygons are ok
                    //if ((err & PolygonError.NotConvex) == PolygonError.NotConvex)
                    //{
                    //}
                }
                if (!bListOK && pts[listIdx].Count != 2)
                {
                    pts.RemoveAt(listIdx);
                }
                else
                {
                    ++listIdx;
                }
            }

            bool bOK = true;
            listIdx = 0;
            while (listIdx < pts.Count)
            {
                int numPoints = pts[listIdx].Count;
                if (numPoints < 2)
                {
                    // should not be possible by this point...
                    ++listIdx;
                    bOK = false;
                    continue;
                }
                else if (numPoints == 2)
                {
                    uint constraintCode = TriangulationConstraint.CalculateContraintCode(pts[listIdx][0], pts[listIdx][1]);
                    TriangulationConstraint tc = null;
                    if (!mConstraintMap.TryGetValue(constraintCode, out tc))
                    {
                        tc = new TriangulationConstraint(pts[listIdx][0], pts[listIdx][1]);
                        AddConstraint(tc);
                    }
                }
                else
                {
                    Contour ph = new Contour(this, pts[listIdx], Point2DList.WindingOrderType.Unknown);
                    ph.WindingOrder = Point2DList.WindingOrderType.Default;
                    ph.Name = name + ":" + listIdx.ToString();
                    mHoles.Add(ph);
                }
                ++listIdx;
            }

            return bOK;
        }


        // this method adds constraints singly and does not assume that they form a contour
        // If you are trying to add a "series" or edges (or "contour"), use AddHole instead.
        public bool AddConstraints(List<TriangulationConstraint> constraints)
        {
            if (constraints == null || constraints.Count < 1)
            {
                return false;
            }

            bool bOK = true;
            foreach (TriangulationConstraint tc in constraints)
            {
                if (ConstrainPointToBounds(tc.P) || ConstrainPointToBounds(tc.Q))
                {
                    tc.CalculateContraintCode();
                }

                TriangulationConstraint tcTmp = null;
                if (!mConstraintMap.TryGetValue(tc.ConstraintCode, out tcTmp))
                {
                    tcTmp = tc;
                    bOK = AddConstraint(tcTmp) && bOK;
                }
            }

            return bOK;
        }


        public bool AddConstraint(TriangulationConstraint tc)
        {
            if (tc == null || tc.P == null || tc.Q == null)
            {
                return false;
            }

            // If we already have this constraint, then there's nothing to do.  Since we already have
            // a valid constraint in the map with the same ConstraintCode, then we're guaranteed that
            // the points are also valid (and have the same coordinates as the ones being passed in with
            // this constrain).  Return true to indicate that we successfully "added" the constraint
            if (mConstraintMap.ContainsKey(tc.ConstraintCode))
            {
                return true;
            }

            // Make sure the constraint is not using points that are duplicates of ones already stored
            // If it is, replace the Constraint Points with the points already stored.
            TriangulationPoint p;
            if (TryGetPoint(tc.P.X, tc.P.Y, out p))
            {
                tc.P = p;
            }
            else
            {
                Add(tc.P);
            }

            if (TryGetPoint(tc.Q.X, tc.Q.Y, out p))
            {
                tc.Q = p;
            }
            else
            {
                Add(tc.Q);
            }

            mConstraintMap.Add(tc.ConstraintCode, tc);

            return true;
        }


        public bool TryGetConstraint(uint constraintCode, out TriangulationConstraint tc)
        {
            return mConstraintMap.TryGetValue(constraintCode, out tc);
        }


        public int GetNumConstraints()
        {
            return mConstraintMap.Count;
        }


        public Dictionary<uint, TriangulationConstraint>.Enumerator GetConstraintEnumerator()
        {
            return mConstraintMap.GetEnumerator();
        }


        public int GetNumHoles()
        {
            int numHoles = 0;
            foreach (Contour c in mHoles)
            {
                numHoles += c.GetNumHoles(false);
            }

            return numHoles;
        }


        public Contour GetHole(int idx)
        {
            if (idx < 0 || idx >= mHoles.Count)
            {
                return null;
            }

            return mHoles[idx];
        }


        public int GetActualHoles(out List<Contour> holes)
        {
            holes = new List<Contour>();
            foreach (Contour c in mHoles)
            {
                c.GetActualHoles(false, ref holes);
            }

            return holes.Count;
        }


        protected void InitializeHoles()
        {
            Contour.InitializeHoles(mHoles, this, this);
            foreach (Contour c in mHoles)
            {
                c.InitializeHoles(this);
            }
        }


        public override bool Initialize()
        {
            InitializeHoles();
            return base.Initialize();
        }


        public override void Prepare(TriangulationContext tcx)
        {
            if (!Initialize())
            {
                return;
            }

            base.Prepare(tcx);

            Dictionary<uint, TriangulationConstraint>.Enumerator it = mConstraintMap.GetEnumerator();
            while (it.MoveNext())
            {
                TriangulationConstraint tc = it.Current.Value;
                tcx.NewConstraint(tc.P, tc.Q);
            }
        }


        public override void AddTriangle(DelaunayTriangle t)
        {
            Triangles.Add(t);
        }

    }

    public class PointSet : Point2DList, ITriangulatable, IEnumerable<TriangulationPoint>, IList<TriangulationPoint>
    {
        protected Dictionary<uint, TriangulationPoint> mPointMap = new Dictionary<uint, TriangulationPoint>();
        public IList<TriangulationPoint> Points { get { return this; } private set { } }
        public IList<DelaunayTriangle> Triangles { get; private set; }

        public string FileName { get; set; }
        public bool DisplayFlipX { get; set; }
        public bool DisplayFlipY { get; set; }
        public float DisplayRotate { get; set; }

        protected double mPrecision = TriangulationPoint.kVertexCodeDefaultPrecision;
        public double Precision { get { return mPrecision; } set { mPrecision = value; } }

        public double MinX { get { return mBoundingBox.MinX; } }
        public double MaxX { get { return mBoundingBox.MaxX; } }
        public double MinY { get { return mBoundingBox.MinY; } }
        public double MaxY { get { return mBoundingBox.MaxY; } }
        public Rect2D Bounds { get { return mBoundingBox; } }

        public virtual TriangulationMode TriangulationMode { get { return TriangulationMode.Unconstrained; } }

        public new TriangulationPoint this[int index]
        {
            get { return mPoints[index] as TriangulationPoint; }
            set { mPoints[index] = value; }
        }


        public PointSet(List<TriangulationPoint> bounds)
        {
            //Points = new List<TriangulationPoint>();
            foreach (TriangulationPoint p in bounds)
            {
                Add(p, -1, false);

                // Only the initial points are counted toward min/max x/y as they 
                // are considered to be the boundaries of the point-set
                mBoundingBox.AddPoint(p);
            }
            mEpsilon = CalculateEpsilon();
            mWindingOrder = WindingOrderType.Unknown;   // not valid for a point-set
        }


        IEnumerator<TriangulationPoint> IEnumerable<TriangulationPoint>.GetEnumerator()
        {
            return new TriangulationPointEnumerator(mPoints);
        }


        public int IndexOf(TriangulationPoint p)
        {
            return mPoints.IndexOf(p);
        }


        public override void Add(Point2D p)
        {
            Add(p as TriangulationPoint, -1, false);
        }

        public virtual void Add(TriangulationPoint p)
        {
            Add(p, -1, false);
        }


        protected override void Add(Point2D p, int idx, bool constrainToBounds)
        {
            Add(p as TriangulationPoint, idx, constrainToBounds);
        }


        protected bool Add(TriangulationPoint p, int idx, bool constrainToBounds)
        {
            if (p == null)
            {
                return false;
            }

            if (constrainToBounds)
            {
                ConstrainPointToBounds(p);
            }

            // if we already have an instance of the point, then don't bother inserting it again as duplicate points
            // will actually cause some real problems later on.   Still return true though to indicate that the point
            // is successfully "added"
            if (mPointMap.ContainsKey(p.VertexCode))
            {
                return true;
            }
            mPointMap.Add(p.VertexCode, p);

            if (idx < 0)
            {
                mPoints.Add(p);
            }
            else
            {
                mPoints.Insert(idx, p);
            }

            return true;
        }


        public override void AddRange(IEnumerator<Point2D> iter, WindingOrderType windingOrder)
        {
            if (iter == null)
            {
                return;
            }

            iter.Reset();
            while (iter.MoveNext())
            {
                Add(iter.Current);
            }
        }


        public virtual bool AddRange(List<TriangulationPoint> points)
        {
            bool bOK = true;
            foreach (TriangulationPoint p in points)
            {
                bOK = Add(p, -1, false) && bOK;
            }

            return bOK;
        }


        public bool TryGetPoint(double x, double y, out TriangulationPoint p)
        {
            uint vc = TriangulationPoint.CreateVertexCode(x, y, Precision);
            if (mPointMap.TryGetValue(vc, out p))
            {
                return true;
            }

            return false;
        }


        //public override void Insert(int idx, Point2D item)
        //{
        //    Add(item, idx, true);
        //}


        public void Insert(int idx, TriangulationPoint item)
        {
            mPoints.Insert(idx, item);
        }


        public override bool Remove(Point2D p)
        {
            return mPoints.Remove(p);
        }


        public bool Remove(TriangulationPoint p)
        {
            return mPoints.Remove(p);
        }


        public override void RemoveAt(int idx)
        {
            if (idx < 0 || idx >= Count)
            {
                return;
            }
            mPoints.RemoveAt(idx);
        }


        public bool Contains(TriangulationPoint p)
        {
            return mPoints.Contains(p);
        }


        public void CopyTo(TriangulationPoint[] array, int arrayIndex)
        {
            int numElementsToCopy = Math.Min(Count, array.Length - arrayIndex);
            for (int i = 0; i < numElementsToCopy; ++i)
            {
                array[arrayIndex + i] = mPoints[i] as TriangulationPoint;
            }
        }


        // returns true if the point is changed, false if the point is unchanged
        protected bool ConstrainPointToBounds(Point2D p)
        {
            double oldX = p.X;
            double oldY = p.Y;
            p.X = Math.Max(MinX, p.X);
            p.X = Math.Min(MaxX, p.X);
            p.Y = Math.Max(MinY, p.Y);
            p.Y = Math.Min(MaxY, p.Y);

            return (p.X != oldX) || (p.Y != oldY);
        }


        protected bool ConstrainPointToBounds(TriangulationPoint p)
        {
            double oldX = p.X;
            double oldY = p.Y;
            p.X = Math.Max(MinX, p.X);
            p.X = Math.Min(MaxX, p.X);
            p.Y = Math.Max(MinY, p.Y);
            p.Y = Math.Min(MaxY, p.Y);

            return (p.X != oldX) || (p.Y != oldY);
        }


        public virtual void AddTriangle(DelaunayTriangle t)
        {
            Triangles.Add(t);
        }


        public void AddTriangles(IEnumerable<DelaunayTriangle> list)
        {
            foreach (var tri in list)
            {
                AddTriangle(tri);
            }
        }


        public void ClearTriangles()
        {
            Triangles.Clear();
        }


        public virtual bool Initialize()
        {
            return true;
        }


        public virtual void Prepare(TriangulationContext tcx)
        {
            if (Triangles == null)
            {
                Triangles = new List<DelaunayTriangle>(Points.Count);
            }
            else
            {
                Triangles.Clear();
            }
            tcx.Points.AddRange(Points);
        }
    }

    public class PointGenerator
    {
        static readonly Random RNG = new Random();


        public static List<TriangulationPoint> UniformDistribution(int n, double scale)
        {
            List<TriangulationPoint> points = new List<TriangulationPoint>();
            for (int i = 0; i < n; i++)
            {
                points.Add(new TriangulationPoint(scale * (0.5 - RNG.NextDouble()), scale * (0.5 - RNG.NextDouble())));
            }

            return points;
        }


        public static List<TriangulationPoint> UniformGrid(int n, double scale)
        {
            double x = 0;
            double size = scale / n;
            double halfScale = 0.5 * scale;

            List<TriangulationPoint> points = new List<TriangulationPoint>();
            for (int i = 0; i < n + 1; i++)
            {
                x = halfScale - i * size;
                for (int j = 0; j < n + 1; j++)
                {
                    points.Add(new TriangulationPoint(x, halfScale - j * size));
                }
            }

            return points;
        }
    }

    public class PolygonGenerator
    {
        static readonly Random RNG = new Random();

        private static double PI_2 = 2.0 * Math.PI;

        public static Polygon RandomCircleSweep(double scale, int vertexCount)
        {
            PolygonPoint point;
            PolygonPoint[] points;
            double radius = scale / 4;

            points = new PolygonPoint[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                do
                {
                    if (i % 250 == 0)
                    {
                        radius += scale / 2 * (0.5 - RNG.NextDouble());
                    }
                    else if (i % 50 == 0)
                    {
                        radius += scale / 5 * (0.5 - RNG.NextDouble());
                    }
                    else
                    {
                        radius += 25 * scale / vertexCount * (0.5 - RNG.NextDouble());
                    }
                    radius = radius > scale / 2 ? scale / 2 : radius;
                    radius = radius < scale / 10 ? scale / 10 : radius;
                } while (radius < scale / 10 || radius > scale / 2);
                point = new PolygonPoint(radius * Math.Cos((PI_2 * i) / vertexCount), radius * Math.Sin((PI_2 * i) / vertexCount));
                points[i] = point;
            }
            return new Polygon(points);
        }

        public static Polygon RandomCircleSweep2(double scale, int vertexCount)
        {
            PolygonPoint point;
            PolygonPoint[] points;
            double radius = scale / 4;

            points = new PolygonPoint[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                do
                {
                    radius += scale / 5 * (0.5 - RNG.NextDouble());
                    radius = radius > scale / 2 ? scale / 2 : radius;
                    radius = radius < scale / 10 ? scale / 10 : radius;
                } while (radius < scale / 10 || radius > scale / 2);
                point = new PolygonPoint(radius * Math.Cos((PI_2 * i) / vertexCount), radius * Math.Sin((PI_2 * i) / vertexCount));
                points[i] = point;
            }
            return new Polygon(points);
        }
    }

    public class TriangulationUtil
    {
        /// <summary>
        ///   Requirements:
        /// 1. a,b and c form a triangle.
        /// 2. a and d is know to be on opposite side of bc
        /// <code>
        ///                a
        ///                +
        ///               / \
        ///              /   \
        ///            b/     \c
        ///            +-------+ 
        ///           /    B    \  
        ///          /           \ 
        /// </code>
        ///    Facts:
        ///  d has to be in area B to have a chance to be inside the circle formed by a,b and c
        ///  d is outside B if orient2d(a,b,d) or orient2d(c,a,d) is CW
        ///  This preknowledge gives us a way to optimize the incircle test
        /// </summary>
        /// <param name="pa">triangle point, opposite d</param>
        /// <param name="pb">triangle point</param>
        /// <param name="pc">triangle point</param>
        /// <param name="pd">point opposite a</param>
        /// <returns>true if d is inside circle, false if on circle edge</returns>
        public static bool SmartIncircle(Point2D pa, Point2D pb, Point2D pc, Point2D pd)
        {
            double pdx = pd.X;
            double pdy = pd.Y;
            double adx = pa.X - pdx;
            double ady = pa.Y - pdy;
            double bdx = pb.X - pdx;
            double bdy = pb.Y - pdy;

            double adxbdy = adx * bdy;
            double bdxady = bdx * ady;
            double oabd = adxbdy - bdxady;
            //        oabd = orient2d(pa,pb,pd);
            if (oabd <= 0)
            {
                return false;
            }

            double cdx = pc.X - pdx;
            double cdy = pc.Y - pdy;

            double cdxady = cdx * ady;
            double adxcdy = adx * cdy;
            double ocad = cdxady - adxcdy;
            //      ocad = orient2d(pc,pa,pd);
            if (ocad <= 0)
            {
                return false;
            }

            double bdxcdy = bdx * cdy;
            double cdxbdy = cdx * bdy;

            double alift = adx * adx + ady * ady;
            double blift = bdx * bdx + bdy * bdy;
            double clift = cdx * cdx + cdy * cdy;

            double det = alift * (bdxcdy - cdxbdy) + blift * ocad + clift * oabd;

            return det > 0;
        }


        public static bool InScanArea(Point2D pa, Point2D pb, Point2D pc, Point2D pd)
        {
            double pdx = pd.X;
            double pdy = pd.Y;
            double adx = pa.X - pdx;
            double ady = pa.Y - pdy;
            double bdx = pb.X - pdx;
            double bdy = pb.Y - pdy;

            double adxbdy = adx * bdy;
            double bdxady = bdx * ady;
            double oabd = adxbdy - bdxady;
            //        oabd = orient2d(pa,pb,pd);
            if (oabd <= 0)
            {
                return false;
            }

            double cdx = pc.X - pdx;
            double cdy = pc.Y - pdy;

            double cdxady = cdx * ady;
            double adxcdy = adx * cdy;
            double ocad = cdxady - adxcdy;
            //      ocad = orient2d(pc,pa,pd);
            if (ocad <= 0)
            {
                return false;
            }
            return true;
        }


        /// Forumla to calculate signed area
        /// Positive if CCW
        /// Negative if CW
        /// 0 if collinear
        /// A[P1,P2,P3]  =  (x1*y2 - y1*x2) + (x2*y3 - y2*x3) + (x3*y1 - y3*x1)
        ///              =  (x1-x3)*(y2-y3) - (y1-y3)*(x2-x3)
        public static Orientation Orient2d(Point2D pa, Point2D pb, Point2D pc)
        {
            double detleft = (pa.X - pc.X) * (pb.Y - pc.Y);
            double detright = (pa.Y - pc.Y) * (pb.X - pc.X);
            double val = detleft - detright;
            if (val > -MathUtil.EPSILON && val < MathUtil.EPSILON)
            {
                return Orientation.Collinear;
            }
            else if (val > 0)
            {
                return Orientation.CCW;
            }
            return Orientation.CW;
        }


        ///////////////////////////////////////////////////////////////////////////////
        // PointRelativeToLine2D
        //
        // Returns -1 if point is on left of line, 0 if point is on line, and 1 if 
        // the point is to the right of the line. This assumes a coordinate system
        // whereby the y axis goes upward when the x axis goes rightward. This is how
        // 3D systems (both right and left-handed) and PostScript works, but is not 
        // how the Win32 GUI works. If you are using a 'y goes downward' coordinate 
        // system, simply negate the return value from this function.
        //
        // Given a point (a,b) and a line from (x1,y1) to (x2,y2), we calculate the 
        // following equation:
        //    (y2-y1)*(a-x1)-(x2-x1)*(b-y1)                        (left)
        // If the result is > 0, the point is on             1 --------------> 2
        // the right, else left.                                   (right)
        //
        // For example, with a point at (1,1) and a 
        // line going from (0,0) to (2,0), we get:
        //    (0-0)*(1-0)-(2-0)*(1-0)
        // which equals:
        //    -2
        // Which indicates the point is (correctly)
        // on the left of the directed line.
        //
        // This function has been checked to a good degree.
        // 
        /////////////////////////////////////////////////////////////////////////////
        //public static double PointRelativeToLine2D(Point2D ptPoint, Point2D ptLineBegin, Point2D ptLineEnd)
        //{
        //    return (ptLineEnd.Y - ptLineBegin.Y) * (ptPoint.X - ptLineBegin.X) - (ptLineEnd.X - ptLineBegin.X) * (ptPoint.Y - ptLineBegin.Y);
        //}


        ///////////////////////////////////////////////////////////////////////////
        // PointInBoundingBox - checks if a point is completely inside an 
        // axis-aligned bounding box defined by xmin, xmax, ymin, and ymax.
        // Note that the point must be fully inside for this method to return
        // true - it cannot lie on the border of the bounding box.
        ///////////////////////////////////////////////////////////////////////////
        public static bool PointInBoundingBox(double xmin, double xmax, double ymin, double ymax, Point2D p)
        {
            return (p.X > xmin && p.X < xmax && p.Y > ymin && p.Y < ymax);
        }


        public static bool PointOnLineSegment2D(Point2D lineStart, Point2D lineEnd, Point2D p, double epsilon)
        {
            return TriangulationUtil.PointOnLineSegment2D(lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y, p.X, p.Y, epsilon);
        }


        public static bool PointOnLineSegment2D(double x1, double y1, double x2, double y2, double x, double y, double epsilon)
        {
            // First checking if (x, z) is in the range of the line segment's end points.
            if (MathUtil.IsValueBetween(x, x1, x2, epsilon) && MathUtil.IsValueBetween(y, y1, y2, epsilon))
            {
                if (MathUtil.AreValuesEqual(x2 - x1, 0.0f, epsilon))
                {
                    // Vertical line.
                    return true;
                }

                double slope = (y2 - y1) / (x2 - x1);
                double yIntercept = -(slope * x1) + y1;

                // Checking if (x, y) is on the line passing through the end points.
                double t = y - ((slope * x) + yIntercept);

                return MathUtil.AreValuesEqual(t, 0.0f, epsilon);
            }

            return false;
        }


        public static bool RectsIntersect(Rect2D r1, Rect2D r2)
        {
            return (r1.Right > r2.Left) &&
                    (r1.Left < r2.Right) &&
                    (r1.Bottom > r2.Top) &&
                    (r1.Top < r2.Bottom);
        }


        /// <summary>
        /// This method detects if two line segments (or lines) intersect,
        /// and, if so, the point of intersection. Use the <paramref name="firstIsSegment"/> and
        /// <paramref name="secondIsSegment"/> parameters to set whether the intersection point
        /// must be on the first and second line segments. Setting these
        /// both to true means you are doing a line-segment to line-segment
        /// intersection. Setting one of them to true means you are doing a
        /// line to line-segment intersection test, and so on.
        /// Note: If two line segments are coincident, then 
        /// no intersection is detected (there are actually
        /// infinite intersection points).
        /// </summary>
        /// <param name="ptStart0">The first point of the first line segment.</param>
        /// <param name="ptEnd0">The second point of the first line segment.</param>
        /// <param name="ptStart1">The first point of the second line segment.</param>
        /// <param name="ptEnd1">The second point of the second line segment.</param>
        /// <param name="firstIsSegment">Set this to true to require that the 
        /// intersection point be on the first line segment.</param>
        /// <param name="secondIsSegment">Set this to true to require that the
        /// intersection point be on the second line segment.</param>
        /// <param name="coincidentEndPointCollisions">Set this to true to enable collisions if the line segments share
        /// an endpoint</param>
        /// <param name="pIntersectionPt">This is set to the intersection
        /// point if an intersection is detected.</param>
        /// <returns>True if an intersection is detected, false otherwise.</returns>
        public static bool LinesIntersect2D(Point2D ptStart0, Point2D ptEnd0,
                                                Point2D ptStart1, Point2D ptEnd1,
                                                bool firstIsSegment, bool secondIsSegment, bool coincidentEndPointCollisions,
                                                ref Point2D pIntersectionPt,
                                                double epsilon)
        {
            double d = (ptEnd0.X - ptStart0.X) * (ptStart1.Y - ptEnd1.Y) - (ptStart1.X - ptEnd1.X) * (ptEnd0.Y - ptStart0.Y);
            if (Math.Abs(d) < epsilon)
            {
                //The lines are parallel.
                return false;
            }

            double d0 = (ptStart1.X - ptStart0.X) * (ptStart1.Y - ptEnd1.Y) - (ptStart1.X - ptEnd1.X) * (ptStart1.Y - ptStart0.Y);
            double d1 = (ptEnd0.X - ptStart0.X) * (ptStart1.Y - ptStart0.Y) - (ptStart1.X - ptStart0.X) * (ptEnd0.Y - ptStart0.Y);
            double kOneOverD = 1 / d;
            double t0 = d0 * kOneOverD;
            double t1 = d1 * kOneOverD;

            if ((!firstIsSegment || ((t0 >= 0.0) && (t0 <= 1.0))) &&
                (!secondIsSegment || ((t1 >= 0.0) && (t1 <= 1.0))) &&
                (coincidentEndPointCollisions || (!MathUtil.AreValuesEqual(0.0, t0, epsilon) && !MathUtil.AreValuesEqual(0.0, t1, epsilon))))
            {
                if (pIntersectionPt != null)
                {
                    pIntersectionPt.X = ptStart0.X + t0 * (ptEnd0.X - ptStart0.X);
                    pIntersectionPt.Y = ptStart0.Y + t0 * (ptEnd0.Y - ptStart0.Y);
                }

                return true;
            }

            return false;
        }


        public static bool LinesIntersect2D(Point2D ptStart0, Point2D ptEnd0,
                                                Point2D ptStart1, Point2D ptEnd1,
                                                ref Point2D pIntersectionPt,
                                                double epsilon)
        {
            return TriangulationUtil.LinesIntersect2D(ptStart0, ptEnd0, ptStart1, ptEnd1, true, true, false, ref pIntersectionPt, epsilon);
        }


        ///////////////////////////////////////////////////////////////////////////
        // RaysIntersect2D
        //
        // Given two lines defined by (sorry about the lame notation):
        //    x0 = x00 + vector_x0*s;
        //    y0 = y00 + vector_y0*s;
        //
        //    x1 = x10 + vector_x1*t;
        //    y1 = y10 + vector_y1*t;
        //
        // This function determines the intersection between them, if there is any.
        //
        // This function assumes the lines to have no endpoints and will intersect
        // them anywhere in 2D space.
        //
        // This algorithm taken from "Realtime-Rendering" section 10.12.
        // 
        // This function has been checked to a good degree.
        // 
        ///////////////////////////////////////////////////////////////////////////
        public static double LI2DDotProduct(Point2D v0, Point2D v1)
        {
            return ((v0.X * v1.X) + (v0.Y * v1.Y));
        }


        public static bool RaysIntersect2D(Point2D ptRayOrigin0, Point2D ptRayVector0,
                                            Point2D ptRayOrigin1, Point2D ptRayVector1,
                                            ref Point2D ptIntersection)
        {
            double kEpsilon = 0.01;

            if (ptIntersection != null)
            {
                //If the user wants an actual intersection result...

                //This is a vector from pLineOrigin0 to ptLineOrigin1.
                Point2D ptTemp1 = new Point2D(ptRayOrigin1.X - ptRayOrigin0.X, ptRayOrigin1.Y - ptRayOrigin0.Y);

                //This is a vector perpendicular to ptVector1.
                Point2D ptTemp2 = new Point2D(-ptRayVector1.Y, ptRayVector1.X);

                double fDot1 = TriangulationUtil.LI2DDotProduct(ptRayVector0, ptTemp2);

                if (Math.Abs(fDot1) < kEpsilon)
                {
                    return false; //The lines are essentially parallel.
                }

                double fDot2 = TriangulationUtil.LI2DDotProduct(ptTemp1, ptTemp2);
                double s = fDot2 / fDot1;
                ptIntersection.X = ptRayOrigin0.X + ptRayVector0.X * s;
                ptIntersection.Y = ptRayOrigin0.Y + ptRayVector0.Y * s;
                return true;
            }

            //Else the user just wants to know if there is an intersection...
            //In this case we need only compare the slopes of the lines.
            double delta = ptRayVector1.X - ptRayVector0.X;
            if (Math.Abs(delta) > kEpsilon)
            {
                delta = ptRayVector1.Y - ptRayVector0.Y;
                if (Math.Abs(delta) > kEpsilon)
                {
                    return true; //The lines are not parallel.
                }
            }

            return false;
        }

    }

    public struct FixedArray3<T> : IEnumerable<T> where T : class
    {
        public T _0, _1, _2;
        public T this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return _0;
                    case 1:
                        return _1;
                    case 2:
                        return _2;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        _0 = value;
                        break;
                    case 1:
                        _1 = value;
                        break;
                    case 2:
                        _2 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }


        public bool Contains(T value)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (this[i] != null && this[i].Equals(value))
                {
                    return true;
                }
            }

            return false;
        }


        public int IndexOf(T value)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (this[i] != null && this[i].Equals(value))
                {
                    return i;
                }
            }

            return -1;
        }


        public void Clear()
        {
            _0 = _1 = _2 = null;
        }


        public void Clear(T value)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (this[i] != null && this[i].Equals(value))
                {
                    this[i] = null;
                }
            }
        }


        private IEnumerable<T> Enumerate()
        {
            for (int i = 0; i < 3; ++i)
            {
                yield return this[i];
            }
        }


        public IEnumerator<T> GetEnumerator() { return Enumerate().GetEnumerator(); }


        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }

    public struct FixedBitArray3 : IEnumerable<bool>
    {
        public bool _0, _1, _2;
        public bool this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return _0;
                    case 1:
                        return _1;
                    case 2:
                        return _2;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        _0 = value;
                        break;
                    case 1:
                        _1 = value;
                        break;
                    case 2:
                        _2 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }


        public bool Contains(bool value)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (this[i] == value)
                {
                    return true;
                }
            }

            return false;
        }


        public int IndexOf(bool value)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (this[i] == value)
                {
                    return i;
                }
            }

            return -1;
        }


        public void Clear()
        {
            _0 = _1 = _2 = false;
        }


        public void Clear(bool value)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (this[i] == value)
                {
                    this[i] = false;
                }
            }
        }


        private IEnumerable<bool> Enumerate()
        {
            for (int i = 0; i < 3; ++i)
            {
                yield return this[i];
            }
        }


        public IEnumerator<bool> GetEnumerator() { return Enumerate().GetEnumerator(); }


        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }

    public class MathUtil
    {
        public static double EPSILON = 1e-12;


        public static bool AreValuesEqual(double val1, double val2)
        {
            return AreValuesEqual(val1, val2, EPSILON);
        }


        public static bool AreValuesEqual(double val1, double val2, double tolerance)
        {
            if (val1 >= (val2 - tolerance) && val1 <= (val2 + tolerance))
            {
                return true;
            }

            return false;
        }


        public static bool IsValueBetween(double val, double min, double max, double tolerance)
        {
            if (min > max)
            {
                double tmp = min;
                min = max;
                max = tmp;
            }
            if ((val + tolerance) >= min && (val - tolerance) <= max)
            {
                return true;
            }

            return false;
        }


        public static double RoundWithPrecision(double f, double precision)
        {
            if (precision < 0.0)
            {
                return f;
            }

            double mul = Math.Pow(10.0, precision);
            double fTemp = Math.Floor(f * mul) / mul;

            return fTemp;
        }


        public static double Clamp(double a, double low, double high)
        {
            return Math.Max(low, Math.Min(a, high));
        }


        public static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }


        public static uint Jenkins32Hash(byte[] data, uint nInitialValue)
        {
            foreach (byte b in data)
            {
                nInitialValue += (uint)b;
                nInitialValue += (nInitialValue << 10);
                nInitialValue += (nInitialValue >> 6);
            }

            nInitialValue += (nInitialValue << 3);
            nInitialValue ^= (nInitialValue >> 11);
            nInitialValue += (nInitialValue << 15);

            return nInitialValue;
        }
    }

    public class Point2D : IComparable<Point2D>
    {
        protected double mX = 0.0;
        public virtual double X { get { return mX; } set { mX = value; } }
        protected double mY = 0.0;
        public virtual double Y { get { return mY; } set { mY = value; } }

        public float Xf { get { return (float)X; } }
        public float Yf { get { return (float)Y; } }


        public Point2D()
        {
            mX = 0.0;
            mY = 0.0;
        }


        public Point2D(double x, double y)
        {
            mX = x;
            mY = y;
        }


        public Point2D(Point2D p)
        {
            mX = p.X;
            mY = p.Y;
        }


        public override string ToString()
        {
            return "[" + X.ToString() + "," + Y.ToString() + "]";
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public override bool Equals(Object obj)
        {
            Point2D p = obj as Point2D;
            if (p != null)
            {
                return Equals(p);
            }

            return base.Equals(obj);
        }


        public bool Equals(Point2D p)
        {
            return Equals(p, 0.0);
        }


        public bool Equals(Point2D p, double epsilon)
        {
            if ((object)p == null || !MathUtil.AreValuesEqual(X, p.X, epsilon) || !MathUtil.AreValuesEqual(Y, p.Y, epsilon))
            {
                return false;
            }

            return true;
        }


        public int CompareTo(Point2D other)
        {
            if (Y < other.Y)
            {
                return -1;
            }
            else if (Y > other.Y)
            {
                return 1;
            }
            else
            {
                if (X < other.X)
                {
                    return -1;
                }
                else if (X > other.X)
                {
                    return 1;
                }
            }

            return 0;
        }


        public virtual void Set(double x, double y) { X = x; Y = y; }
        public virtual void Set(Point2D p) { X = p.X; Y = p.Y; }

        public void Add(Point2D p) { X += p.X; Y += p.Y; }
        public void Add(double scalar) { X += scalar; Y += scalar; }
        public void Subtract(Point2D p) { X -= p.X; Y -= p.Y; }
        public void Subtract(double scalar) { X -= scalar; Y -= scalar; }
        public void Multiply(Point2D p) { X *= p.X; Y *= p.Y; }
        public void Multiply(double scalar) { X *= scalar; Y *= scalar; }
        public void Divide(Point2D p) { X /= p.X; Y /= p.Y; }
        public void Divide(double scalar) { X /= scalar; Y /= scalar; }
        public void Negate() { X = -X; Y = -Y; }
        public double Magnitude() { return Math.Sqrt((X * X) + (Y * Y)); }
        public double MagnitudeSquared() { return (X * X) + (Y * Y); }
        public double MagnitudeReciprocal() { return 1.0 / Magnitude(); }
        public void Normalize() { Multiply(MagnitudeReciprocal()); }
        public double Dot(Point2D p) { return (X * p.X) + (Y * p.Y); }
        public double Cross(Point2D p) { return (X * p.Y) - (Y * p.X); }
        public void Clamp(Point2D low, Point2D high) { X = Math.Max(low.X, Math.Min(X, high.X)); Y = Math.Max(low.Y, Math.Min(Y, high.Y)); }
        public void Abs() { X = Math.Abs(X); Y = Math.Abs(Y); }
        public void Reciprocal() { if (X != 0.0 && Y != 0.0) { X = 1.0 / X; Y = 1.0 / Y; } }

        public void Translate(Point2D vector) { Add(vector); }
        public void Translate(double x, double y) { X += x; Y += y; }
        public void Scale(Point2D vector) { Multiply(vector); }
        public void Scale(double scalar) { Multiply(scalar); }
        public void Scale(double x, double y) { X *= x; Y *= y; }
        public void Rotate(double radians)
        {
            double cosr = Math.Cos(radians);
            double sinr = Math.Sin(radians);
            double xold = X;
            double yold = Y;
            X = (xold * cosr) - (yold * sinr);
            Y = (xold * sinr) + (yold * cosr);
        }
        public void RotateDegrees(double degrees)
        {
            double radians = degrees * Math.PI / 180.0;
            Rotate(radians);
        }

        public static double Dot(Point2D lhs, Point2D rhs) { return (lhs.X * rhs.X) + (lhs.Y * rhs.Y); }
        public static double Cross(Point2D lhs, Point2D rhs) { return (lhs.X * rhs.Y) - (lhs.Y * rhs.X); }
        public static Point2D Clamp(Point2D a, Point2D low, Point2D high) { Point2D p = new Point2D(a); p.Clamp(low, high); return p; }
        public static Point2D Min(Point2D a, Point2D b) { Point2D p = new Point2D(); p.X = Math.Min(a.X, b.X); p.Y = Math.Min(a.Y, b.Y); return p; }
        public static Point2D Max(Point2D a, Point2D b) { Point2D p = new Point2D(); p.X = Math.Max(a.X, b.X); p.Y = Math.Max(a.Y, b.Y); return p; }
        public static Point2D Abs(Point2D a) { Point2D p = new Point2D(Math.Abs(a.X), Math.Abs(a.Y)); return p; }
        public static Point2D Reciprocal(Point2D a) { Point2D p = new Point2D(1.0 / a.X, 1.0 / a.Y); return p; }

        // returns a scaled perpendicular vector.  Which direction it goes depends on the order in which the arguments are passed
        public static Point2D Perpendicular(Point2D lhs, double scalar) { Point2D p = new Point2D(lhs.Y * scalar, lhs.X * -scalar); return p; }
        public static Point2D Perpendicular(double scalar, Point2D rhs) { Point2D p = new Point2D(-scalar * rhs.Y, scalar * rhs.X); return p; }


        //
        // operator overloading
        //

        // Binary Operators
        // Note that in C#, when a binary operator is overloaded, its corresponding compound assignment operator is also automatically
        // overloaded.  So, for example, overloading operator + implicitly overloads += as well
        public static Point2D operator +(Point2D lhs, Point2D rhs) { Point2D result = new Point2D(lhs); result.Add(rhs); return result; }
        public static Point2D operator +(Point2D lhs, double scalar) { Point2D result = new Point2D(lhs); result.Add(scalar); return result; }
        public static Point2D operator -(Point2D lhs, Point2D rhs) { Point2D result = new Point2D(lhs); result.Subtract(rhs); return result; }
        public static Point2D operator -(Point2D lhs, double scalar) { Point2D result = new Point2D(lhs); result.Subtract(scalar); return result; }
        public static Point2D operator *(Point2D lhs, Point2D rhs) { Point2D result = new Point2D(lhs); result.Multiply(rhs); return result; }
        public static Point2D operator *(Point2D lhs, double scalar) { Point2D result = new Point2D(lhs); result.Multiply(scalar); return result; }
        public static Point2D operator *(double scalar, Point2D lhs) { Point2D result = new Point2D(lhs); result.Multiply(scalar); return result; }
        public static Point2D operator /(Point2D lhs, Point2D rhs) { Point2D result = new Point2D(lhs); result.Divide(rhs); return result; }
        public static Point2D operator /(Point2D lhs, double scalar) { Point2D result = new Point2D(lhs); result.Divide(scalar); return result; }

        // Unary Operators
        public static Point2D operator -(Point2D p) { Point2D tmp = new Point2D(p); tmp.Negate(); return tmp; }

        // Relational Operators
        //public static bool operator ==(Point2D lhs, Point2D rhs) { if ((object)lhs != null) { return lhs.Equals(rhs, 0.0); } if ((object)rhs == null) { return true; } else { return false; } }
        //public static bool operator !=(Point2D lhs, Point2D rhs) { if ((object)lhs != null) { return !lhs.Equals(rhs, 0.0); } if ((object)rhs == null) { return false; } else { return true; } }
        public static bool operator <(Point2D lhs, Point2D rhs) { return (lhs.CompareTo(rhs) == -1) ? true : false; }
        public static bool operator >(Point2D lhs, Point2D rhs) { return (lhs.CompareTo(rhs) == 1) ? true : false; }
        public static bool operator <=(Point2D lhs, Point2D rhs) { return (lhs.CompareTo(rhs) <= 0) ? true : false; }
        public static bool operator >=(Point2D lhs, Point2D rhs) { return (lhs.CompareTo(rhs) >= 0) ? true : false; }
    }


    public class Point2DEnumerator : IEnumerator<Point2D>
    {
        protected IList<Point2D> mPoints;
        protected int position = -1;  // Enumerators are positioned before the first element until the first MoveNext() call.


        public Point2DEnumerator(IList<Point2D> points)
        {
            mPoints = points;
        }

        public bool MoveNext()
        {
            position++;
            return (position < mPoints.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        void IDisposable.Dispose() { }

        Object IEnumerator.Current { get { return Current; } }

        public Point2D Current
        {
            get
            {
                if (position < 0 || position >= mPoints.Count)
                {
                    return null;
                }
                return mPoints[position];
            }
        }
    }


    public class Point2DList : IEnumerable<Point2D>, IList<Point2D> // : List<Point2D>
    {
        public static readonly int kMaxPolygonVertices = 100000; // adjust to suit...

        /// A small length used as a collision and constraint tolerance. Usually it is
        /// chosen to be numerically significant, but visually insignificant.
        public static readonly double kLinearSlop = 0.005;

        /// A small angle used as a collision and constraint tolerance. Usually it is
        /// chosen to be numerically significant, but visually insignificant.
        public static readonly double kAngularSlop = (2.0 / (180.0 * Math.PI));

        public enum WindingOrderType
        {
            CW,
            CCW,
            Unknown,

            Default = CCW,
        }

        [Flags]
        public enum PolygonError : uint
        {
            None = 0,
            NotEnoughVertices = 1 << 0,
            NotConvex = 1 << 1,
            NotSimple = 1 << 2,
            AreaTooSmall = 1 << 3,
            SidesTooCloseToParallel = 1 << 4,
            TooThin = 1 << 5,
            Degenerate = 1 << 6,
            Unknown = 1 << 30,
        }


        protected List<Point2D> mPoints = new List<Point2D>();
        protected Rect2D mBoundingBox = new Rect2D();
        protected WindingOrderType mWindingOrder = WindingOrderType.Unknown;
        protected double mEpsilon = MathUtil.EPSILON;    // Epsilon is a function of the size of the bounds of the polygon

        public Rect2D BoundingBox { get { return mBoundingBox; } }
        public WindingOrderType WindingOrder
        {
            get { return mWindingOrder; }
            set
            {
                if (mWindingOrder == WindingOrderType.Unknown)
                {
                    mWindingOrder = CalculateWindingOrder();
                }
                if (value != mWindingOrder)
                {
                    mPoints.Reverse();
                    mWindingOrder = value;
                }
            }
        }
        public double Epsilon { get { return mEpsilon; } }
        public Point2D this[int index]
        {
            get { return mPoints[index]; }
            set { mPoints[index] = value; }
        }
        public int Count { get { return mPoints.Count; } }
        public virtual bool IsReadOnly { get { return false; } }


        public Point2DList()
        {
        }


        public Point2DList(int capacity)
        {
            mPoints.Capacity = capacity;
        }


        public Point2DList(IList<Point2D> l)
        {
            AddRange(l.GetEnumerator(), WindingOrderType.Unknown);
        }


        public Point2DList(Point2DList l)
        {
            int numPoints = l.Count;
            for (int i = 0; i < numPoints; ++i)
            {
                mPoints.Add(l[i]);
            }
            mBoundingBox.Set(l.BoundingBox);
            mEpsilon = l.Epsilon;
            mWindingOrder = l.WindingOrder;
        }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < Count; i++)
            {
                builder.Append(this[i].ToString());
                if (i < Count - 1)
                {
                    builder.Append(" ");
                }
            }
            return builder.ToString();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return mPoints.GetEnumerator();
        }


        IEnumerator<Point2D> IEnumerable<Point2D>.GetEnumerator()
        {
            return new Point2DEnumerator(mPoints);
        }


        public void Clear()
        {
            mPoints.Clear();
            mBoundingBox.Clear();
            mEpsilon = MathUtil.EPSILON;
            mWindingOrder = WindingOrderType.Unknown;
        }


        public int IndexOf(Point2D p)
        {
            return mPoints.IndexOf(p);
        }


        public virtual void Add(Point2D p)
        {
            Add(p, -1, true);
        }


        protected virtual void Add(Point2D p, int idx, bool bCalcWindingOrderAndEpsilon)
        {
            if (idx < 0)
            {
                mPoints.Add(p);
            }
            else
            {
                mPoints.Insert(idx, p);
            }
            mBoundingBox.AddPoint(p);
            if (bCalcWindingOrderAndEpsilon)
            {
                if (mWindingOrder == WindingOrderType.Unknown)
                {
                    mWindingOrder = CalculateWindingOrder();
                }
                mEpsilon = CalculateEpsilon();
            }
        }


        public virtual void AddRange(Point2DList l)
        {
            AddRange(l.mPoints.GetEnumerator(), l.WindingOrder);
        }


        public virtual void AddRange(IEnumerator<Point2D> iter, WindingOrderType windingOrder)
        {
            if (iter == null)
            {
                return;
            }

            if (mWindingOrder == WindingOrderType.Unknown && Count == 0)
            {
                mWindingOrder = windingOrder;
            }
            bool bReverseReadOrder = (WindingOrder != WindingOrderType.Unknown) && (windingOrder != WindingOrderType.Unknown) && (WindingOrder != windingOrder);
            bool bAddedFirst = true;
            int startCount = mPoints.Count;
            iter.Reset();
            while (iter.MoveNext())
            {
                if (!bAddedFirst)
                {
                    bAddedFirst = true;
                    mPoints.Add(iter.Current);
                }
                else if (bReverseReadOrder)
                {
                    mPoints.Insert(startCount, iter.Current);
                }
                else
                {
                    mPoints.Add(iter.Current);
                }
                mBoundingBox.AddPoint(iter.Current);
            }
            if (mWindingOrder == WindingOrderType.Unknown && windingOrder == WindingOrderType.Unknown)
            {
                mWindingOrder = CalculateWindingOrder();
            }
            mEpsilon = CalculateEpsilon();
        }


        public virtual void Insert(int idx, Point2D item)
        {
            Add(item, idx, true);
        }


        public virtual bool Remove(Point2D p)
        {
            if (mPoints.Remove(p))
            {
                CalculateBounds();
                mEpsilon = CalculateEpsilon();
                return true;
            }

            return false;
        }


        public virtual void RemoveAt(int idx)
        {
            if (idx < 0 || idx >= Count)
            {
                return;
            }
            mPoints.RemoveAt(idx);
            CalculateBounds();
            mEpsilon = CalculateEpsilon();
        }


        public virtual void RemoveRange(int idxStart, int count)
        {
            if (idxStart < 0 || idxStart >= Count)
            {
                return;
            }
            if (count == 0)
            {
                return;
            }

            mPoints.RemoveRange(idxStart, count);
            CalculateBounds();
            mEpsilon = CalculateEpsilon();
        }


        public bool Contains(Point2D p)
        {
            return mPoints.Contains(p);
        }


        public void CopyTo(Point2D[] array, int arrayIndex)
        {
            int numElementsToCopy = Math.Min(Count, array.Length - arrayIndex);
            for (int i = 0; i < numElementsToCopy; ++i)
            {
                array[arrayIndex + i] = mPoints[i];
            }
        }


        public void CalculateBounds()
        {
            mBoundingBox.Clear();
            foreach (Point2D pt in mPoints)
            {
                mBoundingBox.AddPoint(pt);
            }
        }


        public double CalculateEpsilon()
        {
            return Math.Max(Math.Min(mBoundingBox.Width, mBoundingBox.Height) * 0.001f, MathUtil.EPSILON);
        }


        public WindingOrderType CalculateWindingOrder()
        {
            // the sign of the 'area' of the polygon is all we are interested in.
            double area = GetSignedArea();
            if (area < 0.0)
            {
                return WindingOrderType.CW;
            }
            else if (area > 0.0)
            {
                return WindingOrderType.CCW;
            }

            // error condition - not even verts to calculate, non-simple poly, etc.
            return WindingOrderType.Unknown;
        }


        public int NextIndex(int index)
        {
            if (index == Count - 1)
            {
                return 0;
            }
            return index + 1;
        }


        /// <summary>
        /// Gets the previous index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public int PreviousIndex(int index)
        {
            if (index == 0)
            {
                return Count - 1;
            }
            return index - 1;
        }


        /// <summary>
        /// Gets the signed area.
        /// </summary>
        /// <returns></returns>
        public double GetSignedArea()
        {
            double area = 0.0;
            for (int i = 0; i < Count; i++)
            {
                int j = (i + 1) % Count;
                area += this[i].X * this[j].Y;
                area -= this[i].Y * this[j].X;
            }
            area /= 2.0f;

            return area;
        }


        /// <summary>
        /// Gets the area.
        /// </summary>
        /// <returns></returns>
        public double GetArea()
        {
            int i;
            double area = 0;

            for (i = 0; i < Count; i++)
            {
                int j = (i + 1) % Count;
                area += this[i].X * this[j].Y;
                area -= this[i].Y * this[j].X;
            }
            area /= 2.0f;
            return (area < 0 ? -area : area);
        }


        /// <summary>
        /// Gets the centroid.
        /// </summary>
        /// <returns></returns>
        public Point2D GetCentroid()
        {
            // Same algorithm is used by Box2D

            Point2D c = new Point2D();
            double area = 0.0f;

            const double inv3 = 1.0 / 3.0;
            Point2D pRef = new Point2D();
            for (int i = 0; i < Count; ++i)
            {
                // Triangle vertices.
                Point2D p1 = pRef;
                Point2D p2 = this[i];
                Point2D p3 = i + 1 < Count ? this[i + 1] : this[0];

                Point2D e1 = p2 - p1;
                Point2D e2 = p3 - p1;

                double D = Point2D.Cross(e1, e2);

                double triangleArea = 0.5f * D;
                area += triangleArea;

                // Area weighted centroid
                c += triangleArea * inv3 * (p1 + p2 + p3);
            }

            // Centroid
            c *= 1.0f / area;
            return c;
        }


        //    /// <summary>
        /// Translates the vertices with the specified vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        public void Translate(Point2D vector)
        {
            for (int i = 0; i < Count; i++)
            {
                this[i] += vector;
            }
        }


        /// <summary>
        /// Scales the vertices with the specified vector.
        /// </summary>
        /// <param name="value">The Value.</param>
        public void Scale(Point2D value)
        {
            for (int i = 0; i < Count; i++)
            {
                this[i] *= value;
            }
        }


        /// <summary>
        /// Rotate the vertices with the defined value in radians.
        /// </summary>
        /// <param name="value">The amount to rotate by in radians.</param>
        public void Rotate(double radians)
        {
            // kickin' it old-skool since I don't want to create a Matrix class for now.
            double cosr = Math.Cos(radians);
            double sinr = Math.Sin(radians);
            foreach (Point2D p in mPoints)
            {
                double xold = p.X;
                p.X = xold * cosr - p.Y * sinr;
                p.Y = xold * sinr + p.Y * cosr;
            }
        }

        // A degenerate polygon is one in which some vertex lies on an edge joining two other vertices. 
        // This can happen in one of two ways: either the vertices V(i-1), V(i), and V(i+1) can be collinear or
        // the vertices V(i) and V(i+1) can overlap (fail to be distinct). In either of these cases, our polygon of
        // n vertices will appear to have n - 1 or fewer -- it will have "degenerated" from an n-gon to an (n-1)-gon.
        // (In the case of triangles, this will result in either a line segment or a point.) 
        public bool IsDegenerate()
        {
            if (Count < 3)
            {
                return false;
            }
            if (Count < 3)
            {
                return false;
            }
            for (int k = 0; k < Count; ++k)
            {
                int j = PreviousIndex(k);
                if (mPoints[j].Equals(mPoints[k], Epsilon))
                {
                    return true;
                }
                int i = PreviousIndex(j);
                Orientation orientation = TriangulationUtil.Orient2d(mPoints[i], mPoints[j], mPoints[k]);
                if (orientation == Orientation.Collinear)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Assuming the polygon is simple; determines whether the polygon is convex.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if it is convex; otherwise, <c>false</c>.
        /// </returns>
        public bool IsConvex()
        {
            bool isPositive = false;

            for (int i = 0; i < Count; ++i)
            {
                int lower = (i == 0) ? (Count - 1) : (i - 1);
                int middle = i;
                int upper = (i == Count - 1) ? (0) : (i + 1);

                double dx0 = this[middle].X - this[lower].X;
                double dy0 = this[middle].Y - this[lower].Y;
                double dx1 = this[upper].X - this[middle].X;
                double dy1 = this[upper].Y - this[middle].Y;

                double cross = dx0 * dy1 - dx1 * dy0;

                // Cross product should have same sign
                // for each vertex if poly is convex.
                bool newIsP = (cross >= 0) ? true : false;
                if (i == 0)
                {
                    isPositive = newIsP;
                }
                else if (isPositive != newIsP)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Check for edge crossings
        /// </summary>
        /// <returns></returns>
        public bool IsSimple()
        {
            for (int i = 0; i < Count; ++i)
            {
                int iplus = NextIndex(i);
                for (int j = i + 1; j < Count; ++j)
                {
                    int jplus = NextIndex(j);
                    Point2D temp = null;
                    if (TriangulationUtil.LinesIntersect2D(mPoints[i], mPoints[iplus], mPoints[j], mPoints[jplus], ref temp, mEpsilon))
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// Checks if polygon is valid for use in Box2d engine.
        /// Last ditch effort to ensure no invalid polygons are
        /// added to world geometry.
        ///
        /// Performs a full check, for simplicity, convexity,
        /// orientation, minimum angle, and volume.  This won't
        /// be very efficient, and a lot of it is redundant when
        /// other tools in this section are used.
        ///
        /// From Eric Jordan's convex decomposition library
        /// </summary>
        /// <param name="printErrors"></param>
        /// <returns></returns>
        public PolygonError CheckPolygon()
        {
            PolygonError error = PolygonError.None;
            if (Count < 3 || Count > Point2DList.kMaxPolygonVertices)
            {
                error |= PolygonError.NotEnoughVertices;
                // no other tests will be valid at this point, so just return
                return error;
            }
            if (IsDegenerate())
            {
                error |= PolygonError.Degenerate;
            }
            //bool bIsConvex = IsConvex();
            //if (!IsConvex())
            //{
            //    error |= PolygonError.NotConvex;
            //}
            if (!IsSimple())
            {
                error |= PolygonError.NotSimple;
            }
            if (GetArea() < MathUtil.EPSILON)
            {
                error |= PolygonError.AreaTooSmall;
            }

            // the following tests don't make sense if the polygon is not simple
            if ((error & PolygonError.NotSimple) != PolygonError.NotSimple)
            {
                bool bReversed = false;
                WindingOrderType expectedWindingOrder = WindingOrderType.CCW;
                WindingOrderType reverseWindingOrder = WindingOrderType.CW;
                if (WindingOrder == reverseWindingOrder)
                {
                    WindingOrder = expectedWindingOrder;
                    bReversed = true;
                }

                //Compute normals
                Point2D[] normals = new Point2D[Count];
                Point2DList vertices = new Point2DList(Count);
                for (int i = 0; i < Count; ++i)
                {
                    vertices.Add(new Point2D(this[i].X, this[i].Y));
                    int i1 = i;
                    int i2 = NextIndex(i);
                    Point2D edge = new Point2D(this[i2].X - this[i1].X, this[i2].Y - this[i1].Y);
                    normals[i] = Point2D.Perpendicular(edge, 1.0);
                    normals[i].Normalize();
                }

                //Required side checks
                for (int i = 0; i < Count; ++i)
                {
                    int iminus = PreviousIndex(i);

                    //Parallel sides check
                    double cross = Point2D.Cross(normals[iminus], normals[i]);
                    cross = MathUtil.Clamp(cross, -1.0f, 1.0f);
                    float angle = (float)Math.Asin(cross);
                    if (Math.Abs(angle) <= Point2DList.kAngularSlop)
                    {
                        error |= PolygonError.SidesTooCloseToParallel;
                        break;
                    }

                    // For some reason, the following checks do not seem to work
                    // correctly in all cases - they return false positives.
                    //    //Too skinny check - only valid for convex polygons
                    //    if (bIsConvex)
                    //    {
                    //        for (int j = 0; j < Count; ++j)
                    //        {
                    //            if (j == i || j == NextIndex(i))
                    //            {
                    //                continue;
                    //            }
                    //            Point2D testVector = vertices[j] - vertices[i];
                    //            testVector.Normalize();
                    //            double s = Point2D.Dot(testVector, normals[i]);
                    //            if (s >= -Point2DList.kLinearSlop)
                    //            {
                    //                error |= PolygonError.TooThin;
                    //            }
                    //        }

                    //        Point2D centroid = vertices.GetCentroid();
                    //        Point2D n1 = normals[iminus];
                    //        Point2D n2 = normals[i];
                    //        Point2D v = vertices[i] - centroid;

                    //        Point2D d = new Point2D();
                    //        d.X = Point2D.Dot(n1, v); // - toiSlop;
                    //        d.Y = Point2D.Dot(n2, v); // - toiSlop;

                    //        // Shifting the edge inward by toiSlop should
                    //        // not cause the plane to pass the centroid.
                    //        if ((d.X < 0.0f) || (d.Y < 0.0f))
                    //        {
                    //            error |= PolygonError.TooThin;
                    //        }
                    //    }
                }

                if (bReversed)
                {
                    WindingOrder = reverseWindingOrder;
                }
            }

            //if (error != PolygonError.None)
            //{
            //    Console.WriteLine("Found invalid polygon: {0} {1}\n", Point2DList.GetErrorString(error), this.ToString());
            //}

            return error;
        }


        public static string GetErrorString(PolygonError error)
        {
            StringBuilder sb = new StringBuilder(256);
            if (error == PolygonError.None)
            {
                sb.AppendFormat("No errors.\n");
            }
            else
            {
                if ((error & PolygonError.NotEnoughVertices) == PolygonError.NotEnoughVertices)
                {
                    sb.AppendFormat("NotEnoughVertices: must have between 3 and {0} vertices.\n", kMaxPolygonVertices);
                }
                if ((error & PolygonError.NotConvex) == PolygonError.NotConvex)
                {
                    sb.AppendFormat("NotConvex: Polygon is not convex.\n");
                }
                if ((error & PolygonError.NotSimple) == PolygonError.NotSimple)
                {
                    sb.AppendFormat("NotSimple: Polygon is not simple (i.e. it intersects itself).\n");
                }
                if ((error & PolygonError.AreaTooSmall) == PolygonError.AreaTooSmall)
                {
                    sb.AppendFormat("AreaTooSmall: Polygon's area is too small.\n");
                }
                if ((error & PolygonError.SidesTooCloseToParallel) == PolygonError.SidesTooCloseToParallel)
                {
                    sb.AppendFormat("SidesTooCloseToParallel: Polygon's sides are too close to parallel.\n");
                }
                if ((error & PolygonError.TooThin) == PolygonError.TooThin)
                {
                    sb.AppendFormat("TooThin: Polygon is too thin or core shape generation would move edge past centroid.\n");
                }
                if ((error & PolygonError.Degenerate) == PolygonError.Degenerate)
                {
                    sb.AppendFormat("Degenerate: Polygon is degenerate (contains collinear points or duplicate coincident points).\n");
                }
                if ((error & PolygonError.Unknown) == PolygonError.Unknown)
                {
                    sb.AppendFormat("Unknown: Unknown Polygon error!.\n");
                }
            }

            return sb.ToString();
        }


        /// <summary>
        /// Removes duplicate points that lie next to each other in the list
        /// </summary>
        public void RemoveDuplicateNeighborPoints()
        {
            int numPoints = Count;
            int i = numPoints - 1;
            int j = 0;
            while (numPoints > 1 && j < numPoints)
            {
                if (mPoints[i].Equals(mPoints[j]))
                {
                    int idxToRemove = Math.Max(i, j);
                    mPoints.RemoveAt(idxToRemove);
                    --numPoints;
                    if (i >= numPoints)
                    {
                        // can happen if first element in list is deleted...
                        i = numPoints - 1;
                    }
                    // don't increment i, j in this case because we want to check i against the new value at j
                }
                else
                {
                    i = NextIndex(i);
                    ++j;  // intentionally not wrapping value of j so we have a valid end-point for the loop
                }
            }
        }


        /// <summary>
        /// Removes all collinear points on the polygon.
        /// Has a default bias of 0
        /// </summary>
        /// <param name="polygon">The polygon that needs simplification.</param>
        /// <returns>A simplified polygon.</returns>
        public void Simplify()
        {
            Simplify(0.0);
        }


        /// <summary>
        /// Removes all collinear points on the polygon.   Note that this is NOT safe to run on a complex
        /// polygon as it will remove points that it should not.   For example, consider this polygon:
        /// 
        ///           2
        ///           +
        ///          / \
        ///         /   \
        ///        /     \
        /// 0 +---+-------+
        ///       3       1
        /// 
        /// This algorithm would delete point 3, leaving you with the polygon 0,1,2 - definitely NOT the correct
        /// polygon.  Caveat Emptor!
        /// 
        /// </summary>
        /// <param name="polygon">The polygon that needs simplification.</param>
        /// <param name="bias">The distance bias between points. Points closer than this will be 'joined'.</param>
        /// <returns>A simplified polygon.</returns>
        public void Simplify(double bias)
        {
            //We can't simplify polygons under 3 vertices
            if (Count < 3)
            {
                return;
            }

            //#if DEBUG
            //            if (!IsSimple())
            //            {
            //                throw new Exception("Do not run Simplify on a non-simple polygon!");
            //            }
            //#endif

            int curr = 0;
            int numVerts = Count;
            double biasSquared = bias * bias;
            while (curr < numVerts && numVerts >= 3)
            {
                int prevId = PreviousIndex(curr);
                int nextId = NextIndex(curr);

                Point2D prev = this[prevId];
                Point2D current = this[curr];
                Point2D next = this[nextId];

                //If they are closer than the bias, continue
                if ((prev - current).MagnitudeSquared() <= biasSquared)
                {
                    RemoveAt(curr);
                    --numVerts;
                    continue;
                }

                //If they collinear, continue
                Orientation orientation = TriangulationUtil.Orient2d(prev, current, next);
                if (orientation == Orientation.Collinear)
                {
                    RemoveAt(curr);
                    --numVerts;
                    continue;
                }

                ++curr;
            }
        }


        // From Eric Jordan's convex decomposition library
        /// <summary>
        /// Merges all parallel edges in the list of vertices
        /// </summary>
        /// <param name="tolerance"></param>
        public void MergeParallelEdges(double tolerance)
        {
            if (Count <= 3)
            {
                // Can't do anything useful here to a triangle
                return;
            }

            bool[] mergeMe = new bool[Count];
            int newNVertices = Count;

            //Gather points to process
            for (int i = 0; i < Count; ++i)
            {
                int lower = (i == 0) ? (Count - 1) : (i - 1);
                int middle = i;
                int upper = (i == Count - 1) ? (0) : (i + 1);

                double dx0 = this[middle].X - this[lower].X;
                double dy0 = this[middle].Y - this[lower].Y;
                double dx1 = this[upper].Y - this[middle].X;
                double dy1 = this[upper].Y - this[middle].Y;
                double norm0 = Math.Sqrt(dx0 * dx0 + dy0 * dy0);
                double norm1 = Math.Sqrt(dx1 * dx1 + dy1 * dy1);

                if (!(norm0 > 0.0 && norm1 > 0.0) && newNVertices > 3)
                {
                    //Merge identical points
                    mergeMe[i] = true;
                    --newNVertices;
                }

                dx0 /= norm0;
                dy0 /= norm0;
                dx1 /= norm1;
                dy1 /= norm1;
                double cross = dx0 * dy1 - dx1 * dy0;
                double dot = dx0 * dx1 + dy0 * dy1;

                if (Math.Abs(cross) < tolerance && dot > 0 && newNVertices > 3)
                {
                    mergeMe[i] = true;
                    --newNVertices;
                }
                else
                {
                    mergeMe[i] = false;
                }
            }

            if (newNVertices == Count || newNVertices == 0)
            {
                return;
            }

            int currIndex = 0;

            // Copy the vertices to a new list and clear the old
            Point2DList oldVertices = new Point2DList(this);
            Clear();

            for (int i = 0; i < oldVertices.Count; ++i)
            {
                if (mergeMe[i] || newNVertices == 0 || currIndex == newNVertices)
                {
                    continue;
                }

                if (currIndex >= newNVertices)
                {
                    throw new Exception("Point2DList::MergeParallelEdges - currIndex[ " + currIndex.ToString() + "] >= newNVertices[" + newNVertices + "]");
                }

                mPoints.Add(oldVertices[i]);
                mBoundingBox.AddPoint(oldVertices[i]);
                ++currIndex;
            }
            mWindingOrder = CalculateWindingOrder();
            mEpsilon = CalculateEpsilon();
        }


        /// <summary>
        /// Projects to axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        public void ProjectToAxis(Point2D axis, out double min, out double max)
        {
            // To project a point on an axis use the dot product
            double dotProduct = Point2D.Dot(axis, this[0]);
            min = dotProduct;
            max = dotProduct;

            for (int i = 0; i < Count; i++)
            {
                dotProduct = Point2D.Dot(this[i], axis);
                if (dotProduct < min)
                {
                    min = dotProduct;
                }
                else
                {
                    if (dotProduct > max)
                    {
                        max = dotProduct;
                    }
                }
            }
        }
    }

    public class Rect2D
    {
        private double mMinX;   // left
        private double mMaxX;   // right
        private double mMinY;   // bottom // top
        private double mMaxY;   // top    // bottom

        public double MinX { get { return mMinX; } set { mMinX = value; } }
        public double MaxX { get { return mMaxX; } set { mMaxX = value; } }
        public double MinY { get { return mMinY; } set { mMinY = value; } }
        public double MaxY { get { return mMaxY; } set { mMaxY = value; } }
        public double Left { get { return mMinX; } set { mMinX = value; } }
        public double Right { get { return mMaxX; } set { mMaxX = value; } }
        public double Top { get { return mMaxY; } set { mMaxY = value; } }
        public double Bottom { get { return mMinY; } set { mMinY = value; } }

        public double Width { get { return (Right - Left); } }
        public double Height { get { return (Top - Bottom); } }
        public bool Empty { get { return (Left == Right) || (Top == Bottom); } }


        public Rect2D()
        {
            Clear();
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public override bool Equals(Object obj)
        {
            Rect2D r = obj as Rect2D;
            if (r != null)
            {
                return Equals(r);
            }

            return base.Equals(obj);
        }


        public bool Equals(Rect2D r)
        {
            return Equals(r, MathUtil.EPSILON);
        }


        public bool Equals(Rect2D r, double epsilon)
        {
            if (!MathUtil.AreValuesEqual(MinX, r.MinX, epsilon))
            {
                return false;
            }
            if (!MathUtil.AreValuesEqual(MaxX, r.MaxX))
            {
                return false;
            }
            if (!MathUtil.AreValuesEqual(MinY, r.MinY, epsilon))
            {
                return false;
            }
            if (!MathUtil.AreValuesEqual(MaxY, r.MaxY, epsilon))
            {
                return false;
            }

            return true;
        }


        public void Clear()
        {
            MinX = Double.MaxValue;
            MaxX = Double.MinValue;
            MinY = Double.MaxValue;
            MaxY = Double.MinValue;
        }


        public void Set(double xmin, double xmax, double ymin, double ymax)
        {
            MinX = xmin;
            MaxX = xmax;
            MinY = ymin;
            MaxY = ymax;
            Normalize();
        }


        public void Set(Rect2D b)
        {
            MinX = b.MinX;
            MaxX = b.MaxX;
            MinY = b.MinY;
            MaxY = b.MaxY;
        }


        public void SetSize(double w, double h)
        {
            Right = Left + w;
            Top = Bottom + h;
        }


        /// <summary>
        /// Returns whether the coordinate is inside the bounding box.  Note that this will return
        /// false if the point is ON the edge of the bounding box.  If you want to test for whether
        /// the point is inside OR on the rect, use ContainsInclusive
        /// </summary>
        public bool Contains(double x, double y)
        {
            return (x > Left) && (y > Bottom) && (x < Right) && (y < Top);
        }
        public bool Contains(Point2D p) { return Contains(p.X, p.Y); }
        public bool Contains(Rect2D r)
        {
            return (Left < r.Left) && (Right > r.Right) && (Top < r.Top) && (Bottom > r.Bottom);
        }


        /// <summary>
        /// Returns whether the coordinate is inside the bounding box.  Note that this will return
        /// false if the point is ON the edge of the bounding box.  If you want to test for whether
        /// the point is inside OR on the rect, use ContainsInclusive
        /// </summary>
        public bool ContainsInclusive(double x, double y)
        {
            return (x >= Left) && (y >= Top) && (x <= Right) && (y <= Bottom);
        }
        public bool ContainsInclusive(double x, double y, double epsilon)
        {
            return ((x + epsilon) >= Left) && ((y + epsilon) >= Top) && ((x - epsilon) <= Right) && ((y - epsilon) <= Bottom);
        }
        public bool ContainsInclusive(Point2D p) { return ContainsInclusive(p.X, p.Y); }
        public bool ContainsInclusive(Point2D p, double epsilon) { return ContainsInclusive(p.X, p.Y, epsilon); }
        public bool ContainsInclusive(Rect2D r)
        {
            return (Left <= r.Left) && (Right >= r.Right) && (Top <= r.Top) && (Bottom >= r.Bottom);
        }
        public bool ContainsInclusive(Rect2D r, double epsilon)
        {
            return ((Left - epsilon) <= r.Left) && ((Right + epsilon) >= r.Right) && ((Top - epsilon) <= r.Top) && ((Bottom + epsilon) >= r.Bottom);
        }


        public bool Intersects(Rect2D r)
        {
            return (Right > r.Left) &&
                    (Left < r.Right) &&
                    (Bottom < r.Top) &&
                    (Top > r.Bottom);
        }


        public Point2D GetCenter()
        {
            Point2D p = new Point2D((Left + Right) / 2, (Bottom + Top) / 2);
            return p;
        }


        public bool IsNormalized()
        {
            return (Right >= Left) && (Bottom <= Top);
        }


        public void Normalize()
        {
            if (Left > Right)
            {
                MathUtil.Swap<double>(ref mMinX, ref mMaxX);
            }

            if (Bottom < Top)
            {
                MathUtil.Swap<double>(ref mMinY, ref mMaxY);
            }
        }


        public void AddPoint(Point2D p)
        {
            MinX = Math.Min(MinX, p.X);
            MaxX = Math.Max(MaxX, p.X);
            MinY = Math.Min(MinY, p.Y);
            MaxY = Math.Max(MaxY, p.Y);
        }


        public void Inflate(double w, double h)
        {
            Left -= w;
            Top += h;
            Right += w;
            Bottom -= h;
        }


        public void Inflate(double left, double top, double right, double bottom)
        {
            Left -= left;
            Top += top;
            Right += right;
            Bottom -= bottom;
        }


        public void Offset(double w, double h)
        {
            Left += w;
            Top += h;
            Right += w;
            Bottom += h;
        }


        public void SetPosition(double x, double y)
        {
            double w = Right - Left;
            double h = Bottom - Top;
            Left = x;
            Bottom = y;
            Right = x + w;
            Top = y + h;
        }


        /// Intersection
        ///
        /// Sets the rectangle to the intersection of two rectangles. 
        /// Returns true if there is any intersection between the two rectangles.
        /// If there is no intersection, the rectangle is set to 0, 0, 0, 0.
        /// Either of the input rectangles may be the same as destination rectangle.
        ///
        public bool Intersection(Rect2D r1, Rect2D r2)
        {
            if (!TriangulationUtil.RectsIntersect(r1, r2))
            {
                Left = Right = Top = Bottom = 0.0;
                return false;
            }

            Left = (r1.Left > r2.Left) ? r1.Left : r2.Left;
            Top = (r1.Top < r2.Top) ? r1.Top : r2.Top;
            Right = (r1.Right < r2.Right) ? r1.Right : r2.Right;
            Bottom = (r1.Bottom > r2.Bottom) ? r1.Bottom : r2.Bottom;

            return true;
        }


        /// Union
        ///
        /// Sets the rectangle to the union of two rectangles r1 and r2. 
        /// If either rect is empty, it is ignored. If both are empty, the rectangle
        /// is set to r1.
        /// Either of the input rectangle references may refer to the destination rectangle.
        ///
        public void Union(Rect2D r1, Rect2D r2)
        {
            if ((r2.Right == r2.Left) || (r2.Bottom == r2.Top))
            {
                Set(r1);
            }
            else if ((r1.Right == r1.Left) || (r1.Bottom == r1.Top))
            {
                Set(r2);
            }
            else
            {
                Left = (r1.Left < r2.Left) ? r1.Left : r2.Left;
                Top = (r1.Top > r2.Top) ? r1.Top : r2.Top;
                Right = (r1.Right > r2.Right) ? r1.Right : r2.Right;
                Bottom = (r1.Bottom < r2.Bottom) ? r1.Bottom : r2.Bottom;
            }
        }

    }
}
