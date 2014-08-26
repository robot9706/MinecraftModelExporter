using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter
{
    class Vector4
    {
        public float X;
        public float Y;
        public float Z;
        public float Level;

        public Vector4(float x, float y, float z, float level)
        {
            X = x;
            Y = y;
            Z = z;
            Level = level;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ Level.GetHashCode();
        }

        public override string ToString()
        {
            return "{" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ", " + Level.ToString() + "}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector4)
            { 
                Vector4 v = (Vector4)obj;
                return (v.X == X && v.Y == Y && v.Z == Z && v.Level == Level);
            }
            return false;
        }

        public static bool operator ==(Vector4 a, Vector4 b)
        {
            return (a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.Level == b.Level);
        }
    
        public static bool operator !=(Vector4 a, Vector4 b)
        {
            return (a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.Level != b.Level);
        }
    }
}
