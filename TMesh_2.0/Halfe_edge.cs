using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMesh_2._0
{
    [Serializable]
    public class Half_Edge
    {
        public int frm { get; private set; }
        public int dst { get; private set; }
        public int mate { get; set; }

        public Half_Edge(int from, int _dst, int _mate = -1)
        {
            frm = from;
            dst = _dst;
            mate = _mate;
        }
        public int next(int h)
        {
            return (h % 3 == 2) ? h - 2 : h + 1;
        }

        public int prev(int h)
        {
            return (h % 3 == 0) ? h + 2 : h - 1;
        }
    }
}
