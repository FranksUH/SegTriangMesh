using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMesh_2._0
{
    public class Vertex
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public int he { get; set; }
        //---OJO al usar un operador se desactualiza he----------
        public Vertex(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            he = -1;
        }
        public double At(int i)
        {
            switch (i)
            {
                case 1:
                    return this.X;
                case 2:
                    return this.Y;
                case 3:
                    return this.Z;
                default:
                    break;
            }
            throw new Exception("The face has only 3 vertexes");
        }
        public static Vertex operator +(Vertex v1, Vertex v2)
        {
            return new Vertex(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vertex operator -(Vertex v1, Vertex v2)
        {
            return new Vertex(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vertex operator *(Vertex v1, Vertex v2)
        {
            return new Vertex(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }

        public static Vertex operator *(Vertex v1, double alpha)
        {
            return new Vertex(v1.X * alpha, v1.Y * alpha, v1.Z * alpha);
        }

        public static Vertex operator /(Vertex v1, Vertex v2)
        {
            return new Vertex(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
        }

        public static Vertex operator /(Vertex v1, int alpha)
        {
            return new Vertex(v1.X / alpha, v1.Y / alpha, v1.Z / alpha);
        }

        public static double Euclidian_Distance(Vertex v1, Vertex v2)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(v1.X - v2.X), 2) + Math.Pow(Math.Abs(v1.Y - v2.Y), 2) + Math.Pow(Math.Abs(v1.Z - v2.Z), 2));
        }
    }
}
