using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMesh_2._0
{
    public class AABBTree
    {
        public List<Face> elements { get; private set; }
        public List<Face> insideFaces { get; private set; }
        public bool isLeaf { get; private set; }
        public AABBTree leftChild;
        public AABBTree rigthChild;
        public int main_axis { get; private set; }
        public double xmin, xmax, ymin, ymax, zmin, zmax, root;
        const double TO_RADIAN = 0.01745329252;

        public AABBTree(List<Face>faces,List<Vertex>vertexes ,int elementsInLeaf)
        {
            insideFaces = new List<Face>();
            buildAABB(faces, vertexes, elementsInLeaf);
        }
        private void buildAABB(List<Face> faces, List<Vertex> vertexes, int elementsInLeaf)
        {
            this.elements = faces;
            Max_Width(vertexes);
            if (faces.Count <= elementsInLeaf)
            {
                this.elements = faces;
                isLeaf = true;
                leftChild = null;
                rigthChild = null;
            }
            else
            {
                isLeaf = false;
                List<double> half_points = new List<double>();
                double minVal = 0, maxVal = 0;
                Face actualFace;

                for (int i = 0; i < faces.Count; i++)
                {
                    actualFace = faces[i];
                    minVal = vertexes[actualFace.i].At(main_axis);//hay que inicializar el main_axis, hacer un metodo buildAABB
                    maxVal = vertexes[actualFace.i].At(main_axis);

                    if (minVal > vertexes[actualFace.j].At(main_axis))
                        minVal = vertexes[actualFace.j].At(main_axis);
                    if (minVal > vertexes[actualFace.k].At(main_axis))
                        minVal = vertexes[actualFace.k].At(main_axis);

                    if (maxVal < vertexes[actualFace.j].At(main_axis))
                        maxVal = vertexes[actualFace.j].At(main_axis);
                    if (maxVal < vertexes[actualFace.k].At(main_axis))
                        maxVal = vertexes[actualFace.k].At(main_axis);

                    half_points.Add((minVal + maxVal) / 2);
                }

                root = Median(half_points);
                List<Face> left = new List<Face>(), right = new List<Face>();

                for (int i = 0; i < faces.Count; i++)
                {
                    actualFace = faces[i];
                    if ((vertexes[actualFace.i].At(main_axis) + vertexes[actualFace.j].At(main_axis) + vertexes[actualFace.k].At(main_axis)) / 3 <= root)
                        left.Add(actualFace);
                    else
                        right.Add(actualFace);
                }

                if (left.Count != faces.Count && right.Count != faces.Count)
                {
                    leftChild = new AABBTree(left, vertexes, elementsInLeaf);
                    rigthChild = new AABBTree(right, vertexes, elementsInLeaf);
                }
                else
                {
                    isLeaf = true;
                    leftChild = null;
                    rigthChild = null;
                    elements = faces;
                }
            }
        }
        private void Max_Width(List<Vertex> vertexes)
        {
            if (vertexes.Count > 0)
            {
                xmin = vertexes[elements[0].i].X;
                xmax = vertexes[elements[0].i].X;
                ymin = vertexes[elements[0].i].Y;
                ymax = vertexes[elements[0].i].Y;
                zmin = vertexes[elements[0].i].Z;
                zmax = vertexes[elements[0].i].Z;

                for (int i = 0; i < elements.Count; i++)
                {
                    Face actualFace = elements[i];
                    for (int j = 1; j < 4; j++)//comprobar sus tres triangulos
                    {
                        if (vertexes[actualFace.At(j)].X < xmin)
                            xmin = vertexes[actualFace.At(j)].X;
                        if (vertexes[actualFace.At(j)].X > xmax)
                            xmax = vertexes[actualFace.At(j)].X;

                        if (vertexes[actualFace.At(j)].Y < ymin)
                            ymin = vertexes[actualFace.At(j)].Y;
                        if (vertexes[actualFace.At(j)].Y > ymax)
                            ymax = vertexes[actualFace.At(j)].Y;

                        if (vertexes[actualFace.At(j)].Z < zmin)
                            zmin = vertexes[actualFace.At(j)].Z;
                        if (vertexes[actualFace.At(j)].Z > zmax)
                            zmax = vertexes[actualFace.At(j)].Z;
                    }
                }

                if ((ymax - ymin) >= (xmax - xmin) && (ymax - ymin) >= (zmax - zmin))
                    main_axis = 2;
                else if ((zmax - zmin) >= (xmax - xmin) && (zmax - zmin) >= (ymax - ymin))
                    main_axis = 3;
                else
                    main_axis = 1;

            }
        }
        private double Median(List<double> elements)
        {
            elements.Sort();
            return elements[elements.Count / 2];
        }
        public List<double[]> GetExtremes()
        {
            List<double[]> extremes = new List<double[]>();
            double[] a1 = { xmin, ymin, zmin };
            double[] a2 = { xmin, ymin, zmax };
            double[] a3 = { xmin, ymax, zmin };
            double[] a4 = { xmin, ymax, zmax };
            double[] a5 = { xmax, ymin, zmin };
            double[] a6 = { xmax, ymin, zmax };
            double[] a7 = { xmax, ymax, zmin };
            double[] a8 = { xmax, ymax, zmax };
            extremes.Add(a1);
            extremes.Add(a2);
            extremes.Add(a3);
            extremes.Add(a4);
            extremes.Add(a5);
            extremes.Add(a6);
            extremes.Add(a7);
            extremes.Add(a8);
            return extremes;
        }
        public List<double[]> GetDivision()
        {
            List<double[]> result = new List<double[]>();

            if (main_axis == 1)//x
            {
                double[] a1 = { root, ymin, zmin };
                double[] a2 = { root, ymax, zmin };
                double[] a3 = { root, ymax, zmax };
                double[] a4 = { root, ymin, zmax };
                result.Add(a1);
                result.Add(a2);
                result.Add(a3);
                result.Add(a4);
            }

            else if (main_axis == 2)//y
            {
                double[] a1 = { xmin, root, zmin };
                double[] a2 = { xmax, root, zmin };
                double[] a3 = { xmax, root, zmax };
                double[] a4 = { xmin, root, zmax };
                result.Add(a1);
                result.Add(a2);
                result.Add(a3);
                result.Add(a4);
            }

            else//z
            {
                double[] a1 = { xmin, ymin, root };
                double[] a2 = { xmax, ymin, root };
                double[] a3 = { xmax, ymax, root };
                double[] a4 = { xmin, ymax, root };
                result.Add(a1);
                result.Add(a2);
                result.Add(a3);
                result.Add(a4);
            }

            return result;
        }
        public List<Tuple<double[],double[]>> GetBBPicture()
        {
            List<Tuple<double[], double[]>> result = new List<Tuple<double[], double[]>>();
            double[] a1 = { xmin, ymin, zmin };
            double[] a2 = { xmin, ymin, zmax };
            double[] a3 = { xmin, ymax, zmin };
            double[] a4 = { xmin, ymax, zmax };
            double[] a5 = { xmax, ymin, zmin };
            double[] a6 = { xmax, ymin, zmax };
            double[] a7 = { xmax, ymax, zmin };
            double[] a8 = { xmax, ymax, zmax };
            result.Add(new Tuple<double[], double[]>(a1,a5));
            result.Add(new Tuple<double[], double[]>(a1, a2));
            result.Add(new Tuple<double[], double[]>(a1, a3));
            result.Add(new Tuple<double[], double[]>(a2, a4));
            result.Add(new Tuple<double[], double[]>(a2, a6));
            result.Add(new Tuple<double[], double[]>(a3, a4));
            result.Add(new Tuple<double[], double[]>(a3, a7));
            result.Add(new Tuple<double[], double[]>(a4, a8));
            result.Add(new Tuple<double[], double[]>(a5, a6));
            result.Add(new Tuple<double[], double[]>(a5, a7));
            result.Add(new Tuple<double[], double[]>(a6, a8));
            result.Add(new Tuple<double[], double[]>(a7, a8));
            return result;
        }
        public List<double[]> GetEdge()
        {
            List<double[]> result = GetDivision();

            if (!isLeaf)
            {
                List<double[]> temp = leftChild.GetEdge();
                for (int i = 0; i < temp.Count; i++)
                    result.Add(temp[i]);

                //temp = new List<double[]>(); //no hace falta
                temp = rigthChild.GetEdge();
                for (int i = 0; i < temp.Count; i++)
                    result.Add(temp[i]);
            }

            return result;
        }
        public double[] CenterOfMass()
        {
            return new double[] {(xmax+xmin)/2,(ymax+ymin)/2,(zmax+zmin)/2 };
        }
        private Vertex Baricenter(Face f, List<Vertex> vertexes)
        {
            return new Vertex((vertexes[f.i].X + vertexes[f.j].X + vertexes[f.k].X) / 3, (vertexes[f.i].Y + vertexes[f.j].Y + vertexes[f.k].Y) / 3, (vertexes[f.i].Z + vertexes[f.j].Z + vertexes[f.k].Z) / 3);
        }
        public List<Tuple<double[],double[]>> GetBB()
        {
            List<Tuple<double[],double[]>> result = new List<Tuple<double[],double[]>>();
            if (isLeaf)
            {
                result = GetBBPicture();
            }
            else
            {
                result.AddRange(GetBBPicture());
                result.AddRange(leftChild.GetBB());
                result.AddRange(rigthChild.GetBB());
            }
            return result;
        }
        public static bool pointInsideCone(double theta,Vector center, Vector normal, Vertex point)//return TRUE if point is inside the cone
        {
            theta /= 2;
            Vector dif = new Vector(center.elements[0], center.elements[1], center.elements[2]) ;
            dif.elements[0] -= point.X;
            dif.elements[1] -= point.Y;
            dif.elements[2] -= point.Z;
            dif.normalize();
            return Math.Cos(theta * TO_RADIAN) <= normal.scalar_product(dif);
        }
        public List<Face> InsideCone(double theta, Vector center, Vector normal, List<Face> faces, List<Vertex> vertexes)
        {
            List<double[]> extreme = GetExtremes();//un bounding box esta contenido si alguno de sus extremos estan
            List<Face> result = new List<Face>();
            bool inside = false;
            int i = 0;

            while (i < extreme.Count && !inside)//eliminar los extremos que no estan en el cono, hasta encontrar el 1ro que si
            {
                inside = pointInsideCone(theta, center, normal, new Vertex(extreme[i][0], extreme[i][1], extreme[i][2]));
                i++;
            }
            if (!inside)//si ningun extremo esta, probar con su centro u otros puntos centrales
            {
                List<double[]> insideP = GetDivision();
                insideP.Insert(0,CenterOfMass());
                i = 0;
                while (i < insideP.Count && !inside)
                {
                    inside = pointInsideCone(theta, center, normal, new Vertex(insideP[i][0], insideP[i][1], insideP[i][2]));
                    i++;
                }
            }
            if (inside)
            {
                if (isLeaf)
                {
                    foreach (var item in elements)//guardar                         
                        if (pointInsideCone(theta, center, normal, Baricenter(item, vertexes)))
                            result.Add(item);
                }
                else
                {
                    result.AddRange(leftChild.InsideCone(theta, center, normal, faces, vertexes));
                    result.AddRange(rigthChild.InsideCone(theta, center, normal, faces, vertexes));
                }
            }
            return result;
        }
        public void RestartInside()
        { this.insideFaces = new List<Face>(); }
        //TO DO: Eliminar las caras tapadas por otras
    }
}
