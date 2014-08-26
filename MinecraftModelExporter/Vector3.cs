using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter
{
    public class Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3 ToPoint3Normal()
        {
            return new Point3((X > 0 ? 1 : (X < 0 ? -1 : 0)), (Y > 0 ? 1 : (Y < 0 ? -1 : 0)), (Z > 0 ? 1 : (Z < 0 ? -1 : 0)));
        }

        public Point3 ToPoint3()
        {
            return new Point3((int)X, (int)Y, (int)Z);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public string RawToString()
        {
            return X.ToString().Replace(',', '.') + " " + Y.ToString().Replace(',', '.') + " " + Z.ToString().Replace(',', '.');
        }

        public override string ToString()
        {
            return "{" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + "}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3)
            { 
                Vector3 v = (Vector3)obj;
                return (v.X == X && v.Y == Y && v.Z == Z);
            }
            return false;
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return (a.X == b.X && a.Y == b.Y && a.Z == b.Z);
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return (a.X != b.X || a.Y != b.Y || a.Z != b.Z);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3 operator +(Vector3 a, float b)
        {
            return new Vector3(a.X + b, a.Y + b, a.Z + b);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3 operator -(Vector3 a, float b)
        {
            return new Vector3(a.X - b, a.Y - b, a.Z - b);
        }

        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            return new Vector3(a.X * b, a.Y * b, a.Z * b);
        }

        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Vector3 operator /(Vector3 a, float b)
        {
            return new Vector3(a.X / b, a.Y / b, a.Z / b);
        }

        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.X, -a.Y, -a.Z);
        }

        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
        }

        public static Vector3 RotateVector(Vector3 p, Vector3 around, float angle)
        {
            float radians = angle * ((float)Math.PI / 180);

            float cosRadians = (float)Math.Cos(radians);
            float sinRadians = (float)Math.Sin(radians);

            cosRadians = (float)Math.Round(cosRadians, 5);
            sinRadians = (float)Math.Round(sinRadians, 5);

            return new Vector3(
                (cosRadians * (p.X - around.X) - sinRadians * (p.Y - around.Y) + around.X),
                (sinRadians * (p.X - around.X) + cosRadians * (p.Y - around.Y) + around.Y),
                (-sinRadians * (p.Y - around.Y) + (cosRadians * (p.Z - around.Z)) + around.Z));
        }

        public static Vector3 RotateX(Vector3 point3D, Vector3 origin, float degrees)
        {
            float cDegrees = ((float)Math.PI * degrees) / 180.0f;
            float cosDegrees = (float)Math.Round(Math.Cos(cDegrees), 2);
            float sinDegrees = (float)Math.Round(Math.Sin(cDegrees), 2);

            float y = ((point3D.Y - origin.Y) * cosDegrees) + ((point3D.Z - origin.Z) * sinDegrees) + origin.Y;
            float z = ((point3D.Y - origin.Y) * -sinDegrees) +((point3D.Z - origin.Z) * cosDegrees) + origin.Z;

            return new Vector3(point3D.X, y, z);
        }

        public static Vector3 RotateY(Vector3 point3D, Vector3 origin, float degrees)
        {
            float cDegrees = ((float)Math.PI * degrees) / 180.0f;
            float cosDegrees = (float)Math.Round(Math.Cos(cDegrees), 2);
            float sinDegrees = (float)Math.Round(Math.Sin(cDegrees), 2);

            float x = ((point3D.X - origin.X) * cosDegrees) + ((point3D.Z - origin.Z) * sinDegrees) + origin.X;
            float z = ((point3D.Y - origin.Y) * -sinDegrees) + ((point3D.Z - origin.Z) * cosDegrees) + origin.Z;

            return new Vector3(x, point3D.Y, z);
        }

        public static Vector3 RotateZ(Vector3 point3D, Vector3 origin, float degrees)
        {
            float cDegrees = ((float)Math.PI * degrees) / 180.0f;
            float cosDegrees = (float)Math.Round(Math.Cos(cDegrees), 2);
            float sinDegrees = (float)Math.Round(Math.Sin(cDegrees), 2);

            float x = ((point3D.X - origin.X) * cosDegrees) + ((point3D.Z - origin.Z) * sinDegrees) + origin.X;
            float y = ((point3D.Y - origin.Y) * cosDegrees) + ((point3D.Z - origin.Z) * sinDegrees) + origin.Y;

            return new Vector3(x, y, point3D.Z);
        }

        public static Vector3 RotateX(Vector3 point3D, float degrees)
        {
            float cDegrees = ((float)Math.PI * degrees) / 180.0f;
            float cosDegrees = (float)Math.Round(Math.Cos(cDegrees), 2);
            float sinDegrees = (float)Math.Round(Math.Sin(cDegrees), 2);

            float y = (point3D.Y * cosDegrees) + (point3D.Z * sinDegrees);
            float z = (point3D.Y * -sinDegrees) + (point3D.Z * cosDegrees);

            return new Vector3(point3D.X, y, z);
        }

        public static Vector3 RotateY(Vector3 point3D, float degrees)
        {
            float cDegrees = ((float)Math.PI * degrees) / 180.0f;
            float cosDegrees = (float)Math.Round(Math.Cos(cDegrees), 2);
            float sinDegrees = (float)Math.Round(Math.Sin(cDegrees), 2);

            float x = (point3D.X * cosDegrees) + (point3D.Z * sinDegrees);
            float z = (point3D.X * -sinDegrees) + (point3D.Z * cosDegrees);

            return new Vector3(x, point3D.Y, z);
        }

        public static Vector3 RotateZ(Vector3 point3D, float degrees)
        {
            float cDegrees = ((float)Math.PI * degrees) / 180.0f;
            float cosDegrees = (float)Math.Round(Math.Cos(cDegrees), 2);
            float sinDegrees = (float)Math.Round(Math.Sin(cDegrees), 2);

            float x = (point3D.X * cosDegrees) + (point3D.Y * sinDegrees);
            float y = (point3D.X * -sinDegrees) + (point3D.Y * cosDegrees);

            return new Vector3(x, y, point3D.Z);
        }
    }
}
