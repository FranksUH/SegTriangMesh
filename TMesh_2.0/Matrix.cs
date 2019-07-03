using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMesh_2._0
{
    public class Matrix
    {
        public double[,] elements;
        public Matrix(int rows, int cols)
        {
            this.elements = new double[rows, cols];
        }
        public Matrix matricialProd(Matrix m2)
        {
            Matrix result = new Matrix(this.elements.GetLength(0), m2.elements.GetLength(1));
            double sum = 0;
            for (int i = 0; i < elements.GetLength(0); i++)
            {
                for (int j = 0; j < m2.elements.GetLength(1); j++)
                {
                    sum = 0;
                    for (int z = 0; z < elements.GetLength(1); z++)
                    {
                        sum += elements[i, z] * m2.elements[z, j];
                    }
                    result.elements[i, j] = sum;
                }
            }
            return result;
        }
    }
}
