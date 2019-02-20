using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMesh_2._0
{
    [Serializable]
    public class Face
    {
        public int i { get; private set; }
        public int j { get; private set; }
        public int k { get; private set; }
        public int index;

        public Face(int _i, int _j, int _k)
        {
            i = _i;
            j = _j;
            k = _k;
        }
        public Face(int _i, int _j, int _k,int index)
        {
            i = _i;
            j = _j;
            k = _k;
            this.index = index;
        }

        public int At(int i)
        {
            switch (i)
            {
                case 1:
                    return this.i;
                case 2:
                    return this.j;
                case 3:
                    return this.k;
                default:
                    break;
            }
            throw new Exception("The face has only 3 vertexes");
        }
    }
    public class FaceComparer : IComparer<Face>
    {
        public int Compare(Face x, Face y)
        {
            throw new NotImplementedException();
        }
    }
}
