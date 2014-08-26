using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModelExporter.GeometryProcessor
{
    class BlockRange
    {
        public BlockData Block;
        public PointF From;
        public PointF To;

        public void Shift(PointF by)
        {
            From = new PointF(From.X + by.X, From.Y + by.Y);
            To = new PointF(To.X + by.X, To.Y + by.Y);
        }
    }
}
