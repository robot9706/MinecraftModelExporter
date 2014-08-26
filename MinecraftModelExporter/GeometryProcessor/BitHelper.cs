using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor
{
    class BitHelper
    {
        public static int Help(byte source, int bitStart, int bitLength)
        {
            bool[] bits = new byte[] { source }.SelectMany(Bits).ToArray();

            List<bool> bt = new List<bool>();
            for (int x = bitStart; x < bitStart + bitLength; x++)
                bt.Add(bits[x]);

            return BitsToInt(bt);
        }

        public static bool[] GetBits(byte inp)
        {
            return new byte[] { inp }.SelectMany(Bits).ToArray();
        }

        public static bool IsBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

        static IEnumerable<bool> Bits(byte b)
        {
            for (int i = 0; i < 8; i++)
            {
                yield return (b & 0x80) != 0;
                b *= 2;
            }
        }

        static int BitsToInt(List<bool> bits)
        {
            int i = 0;

            for (int x = 0; x < bits.Count; x++)
            {
                if (bits[x])
                {
                    i |= (1 << ((bits.Count - 1) - x));
                }
            }

            return i;
        }
    }
}
