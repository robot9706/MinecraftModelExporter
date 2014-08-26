using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter
{
    public class Point3
    {
        public int X;
        public int Y;
        public int Z;

        public Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override int GetHashCode()
        {
            return X ^ Y ^ Z;
        }

        public override string ToString()
        {
            return "{" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + "}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Point3)
            { 
                Point3 v = (Point3)obj;
                return (v.X == X && v.Y == Y && v.Z == Z);
            }
            return false;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        public static bool operator ==(Point3 a, Point3 b)
        {
            return (a.X == b.X && a.Y == b.Y && a.Z == b.Z);
        }

        public static bool operator !=(Point3 a, Point3 b)
        {
            return (a.X != b.X || a.Y != b.Y || a.Z != b.Z);
        }

        public static Point3 operator +(Point3 a, Point3 b)
        {
            return new Point3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Point3 operator +(Point3 a, int b)
        {
            return new Point3(a.X + b, a.Y + b, a.Z + b);
        }

        public static Point3 operator -(Point3 a, Point3 b)
        {
            return new Point3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Point3 operator -(Point3 a, int b)
        {
            return new Point3(a.X - b, a.Y - b, a.Z - b);
        }

        public static Point3 operator *(Point3 a, Point3 b)
        {
            return new Point3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Point3 operator *(Point3 a, int b)
        {
            return new Point3(a.X * b, a.Y * b, a.Z * b);
        }

        public static Point3 operator /(Point3 a, Point3 b)
        {
            return new Point3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Point3 operator /(Point3 a, int b)
        {
            return new Point3(a.X / b, a.Y / b, a.Z / b);
        }
    }
}
