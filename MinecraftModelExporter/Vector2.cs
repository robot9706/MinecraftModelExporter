using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter
{
    public class Vector2
    {
        public float X;
        public float Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public string RawToString()
        {
            return X.ToString().Replace(',', '.') + " " + Y.ToString().Replace(',', '.');
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2)
            { 
                Vector2 v = (Vector2)obj;
                return (v.X == X && v.Y == Y);
            }
            return false;
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return (a.X == b.X && a.Y == b.Y);
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return (a.X != b.X || a.Y != b.Y);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator +(Vector2 a, float b)
        {
            return new Vector2(a.X + b, a.Y + b);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator -(Vector2 a, float b)
        {
            return new Vector2(a.X - b, a.Y - b);
        }

        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X * b.X, a.Y * b.Y);
        }

        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2(a.X * b, a.Y * b);
        }

        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X / b.X, a.Y / b.Y);
        }

        public static Vector2 operator /(Vector2 a, float b)
        {
            return new Vector2(a.X / b, a.Y / b);
        }

        public static Vector2 RotateAround(Vector2 point2D, Vector2 origin, float degrees)
        {
            float angleInRadians = degrees * ((float)Math.PI / 180f);
            float cosTheta = (float)Math.Cos(angleInRadians);
            float sinTheta = (float)Math.Sin(angleInRadians);
            Vector2 r = new Vector2((cosTheta * (point2D.X - origin.X) - sinTheta * (point2D.Y - origin.Y) + origin.X),
                (sinTheta * (point2D.X - origin.X) + cosTheta * (point2D.Y - origin.Y) + origin.Y)
            );

            r.X = (float)Math.Round(r.X, 4);
            r.Y = (float)Math.Round(r.Y, 4);

            return r;
        }
    }
}
