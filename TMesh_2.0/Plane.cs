using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMesh_2._0
{
    public class Plane
    {
        public double A, B, C, D;
        public Plane(Vertex v1, Vertex v2, Vertex v3)
        {
            Vector aux1 = new Vector(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);
            Vector aux2 = new Vector(v3.X - v1.X, v3.Y - v1.Y, v3.Z - v1.Z);
            Vector prod = aux1.vectorial_product(aux2);
            this.A = prod.elements[0];
            this.B = prod.elements[1];
            this.C = prod.elements[2];
            this.D = -1 * (prod.elements[0] * v1.X + prod.elements[1] * v1.Y + prod.elements[2] * v1.Z);
        }
        public bool Belongs(Vertex v)//returns TRUE when v belongs to this plane
        {
            return (v.X * this.A + v.Y * this.B + v.Z * this.C + this.D) == 0;
        }
        public double Evaluate(Vertex v)
        {
            return (v.X * this.A + v.Y * this.B + v.Z * this.C + this.D);
        }
        public void ChangeSign()
        {
            this.A *= -1;
            this.B *= -1;
            this.C *= -1;
            this.D *= -1;
        }
        //public Vertex PutInto(Vertex v)//metodo para trasladar a v a este plano
        //{

        //}
    }
}
