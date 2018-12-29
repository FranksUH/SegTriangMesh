using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMesh_2._0
{
    public class Vector
    {
        public List<double> elements;
        public int size { get { return elements.Count; } private set { } }

        public Vector()
        {
            elements = new List<double>();
        }
        public Vector(params double[] elem)
        {
            elements = new List<double>();
            foreach (var item in elem)
                elements.Add(item);
        }
        public double scalar_product(Vector v2)
        {
            double res = 0;
            int menor = Math.Min(this.size, v2.size);
            for (int i = 0; i < menor; i++)
            {
                res += this.elements[i] * v2.elements[i];
            }
            return res;
        }
        public double norm()
        {
            double res = 0;
            foreach (var item in this.elements)
                res += Math.Pow(item, 2);
            return Math.Sqrt(res);
        }
        public double cos(Vector v)
        {
            double scalarP = this.scalar_product(v);
            return scalarP / this.norm() * v.norm();
        }
        public Vector vectorial_product(Vector v)
        {
            if (v.size == 3 && this.size == 3)
            {
                Vector result = new Vector();
                double x = elements[0], y = elements[1], z = elements[2];
                result.elements.Add(y * v.elements[2] - z * v.elements[1]);
                result.elements.Add(z * v.elements[0] - x * v.elements[2]);
                result.elements.Add(x * v.elements[1] - y * v.elements[0]);
                return result;
            }
            else return null;
        }
        public void multiply(double scalar)
        {
            for (int i = 0; i < this.size; i++)
                this.elements[i] *= scalar;
        }
        public Vector copy()
        {
            Vector result = new Vector();
            for (int i = 0; i < this.size; i++)
                result.elements.Add(this.elements[i]);
            return result;
        }
        public void normalize()
        {
            this.multiply(1 / this.norm());
        }
        public static Vector normal(Vector v1, Vector v2, Vector v3)
        {
            Vector t = new Vector(v1.elements[0] - v3.elements[0], v1.elements[1] - v3.elements[1], v1.elements[2] - v3.elements[2]);
            Vector w = new Vector(v2.elements[0] - v3.elements[0], v2.elements[1] - v3.elements[1], v2.elements[2] - v3.elements[2]);
            t = t.vectorial_product(w);
            t.normalize();
            return t;
        }
        public static double euclidianDistance(Vector v1, Vector v2)
        {
            return Math.Sqrt(Math.Pow((v1.elements[0]-v2.elements[0]),2)+Math.Pow((v1.elements[1] - v2.elements[1]), 2)+Math.Pow((v1.elements[2] - v2.elements[2]), 2));
        }
    }
}
