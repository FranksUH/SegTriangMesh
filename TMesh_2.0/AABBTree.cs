using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMesh_2._0
{
    [Serializable]
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
        public List<double[]> tests;


        public AABBTree(List<Face>faces,List<Vertex>vertexes ,int elementsInLeaf)
        {
            insideFaces = new List<Face>();
            buildAABB(faces, vertexes, elementsInLeaf);
            tests = new List<double[]>();
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
                result.Add(new double[] { root, ymin, zmin });
                result.Add(new double[] { root, ymax, zmin });
                result.Add(new double[] { root, ymax, zmax });
                result.Add(new double[] { root, ymin, zmax });
                //added points
                result.Add(new double[] { root, (ymax+ymin)/2, zmax });
                result.Add(new double[] { root, (ymax + ymin) / 2, zmin });
                result.Add(new double[] { root, ymax, (zmin + zmax) / 2 });
                result.Add(new double[] { root, ymin, (zmin + zmax) / 2 });
                //xmax
                result.Add(new double[] { xmin, (ymax + ymin) / 2, zmax });
                result.Add(new double[] { xmin, (ymax + ymin) / 2, zmin });
                result.Add(new double[] { xmin, ymax, (zmin + zmax) / 2 });
                result.Add(new double[] { xmin, ymin, (zmin + zmax) / 2 });
                result.Add(new double[] { xmin, (ymax+ymin)/2, (zmin + zmax) / 2 });
                //xmin
                result.Add(new double[] { xmax, (ymax + ymin) / 2, zmax });
                result.Add(new double[] { xmax, (ymax + ymin) / 2, zmin });
                result.Add(new double[] { xmax, ymax, (zmin + zmax) / 2 });
                result.Add(new double[] { xmax, ymin, (zmin + zmax) / 2 });
                result.Add(new double[] { xmax, (ymax + ymin) / 2, (zmax + zmin) / 2 });
            }

            else if (main_axis == 2)//y
            {
                result.Add(new double[] { xmin, root, zmin });
                result.Add(new double[] { xmax, root, zmin });
                result.Add(new double[] { xmax, root, zmax });
                result.Add(new double[] { xmin, root, zmax });
                //added points
                result.Add(new double[] { (xmin+xmax)/2, root, zmax});
                result.Add(new double[] { (xmin + xmax)/2, root, zmin });
                result.Add(new double[] { xmax, root, (zmax +zmin)/2});
                result.Add(new double[] { xmin, root, (zmax+zmin)/2 });
                //ymax
                result.Add(new double[] { (xmin + xmax) / 2, ymax, zmax });
                result.Add(new double[] { (xmin + xmax) / 2, ymax, zmin });
                result.Add(new double[] { xmax, ymax, (zmax + zmin) / 2 });
                result.Add(new double[] { xmin, ymax, (zmax + zmin) / 2 });
                result.Add(new double[] { (xmax+xmin)/2, ymax, (zmax + zmin) / 2 });
                //ymin
                result.Add(new double[] { (xmin + xmax) / 2, ymin, zmax });
                result.Add(new double[] { (xmin + xmax) / 2, ymin, zmin });
                result.Add(new double[] { xmax, ymin, (zmax + zmin) / 2 });
                result.Add(new double[] { xmin, ymin, (zmax + zmin) / 2 });
                result.Add(new double[] { (xmax+xmin)/2, ymin, (zmax + zmin) / 2 });
            }

            else//z
            {
                result.Add(new double[] { xmin, ymin, root });
                result.Add(new double[] { xmax, ymin, root });
                result.Add(new double[] { xmax, ymax, root });
                result.Add(new double[] { xmin, ymax, root });
                //added points
                result.Add(new double[] { (xmax+xmin)/2, ymax, root});
                result.Add(new double[] { (xmax + xmin)/2, ymin, root });
                result.Add(new double[] { xmax, (ymin + ymax)/2, root });
                result.Add(new double[] { xmin, (ymin + ymax)/2, root });
                //zmax
                result.Add(new double[] { (xmax + xmin) / 2, ymax, zmax });
                result.Add(new double[] { (xmax + xmin) / 2, ymin, zmax });
                result.Add(new double[] { xmax, (ymin + ymax) / 2, zmax });
                result.Add(new double[] { xmin, (ymin + ymax) / 2, zmax });
                result.Add(new double[] { (xmax+xmin)/2, (ymin + ymax) / 2, zmax });
                //zmin
                result.Add(new double[] { (xmax + xmin) / 2, ymax, zmin });
                result.Add(new double[] { (xmax + xmin) / 2, ymin, zmin });
                result.Add(new double[] { xmax, (ymin + ymax) / 2, zmin });
                result.Add(new double[] { xmin, (ymin + ymax) / 2, zmin });
                result.Add(new double[] { (xmax+xmin)/2, (ymin + ymax) / 2, zmin });
            }

            result.Add(new double[] { (xmin + xmax) / 2, (ymax + ymin) / 2, (zmax + zmin) / 2 });
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
            bool inside = IsInterceptedByRay(center, normal);
            List<Face> result = new List<Face>();
            if (!inside)
            {
                List<double[]> extreme = GetExtremes();//un bounding box esta contenido si alguno de sus extremos estan
                this.tests.AddRange(extreme);                

                int i = 0;
                while (i < extreme.Count && !inside)//eliminar los extremos que no estan en el cono, hasta encontrar el 1ro que si
                {
                    inside = pointInsideCone(theta, center, normal, new Vertex(extreme[i][0], extreme[i][1], extreme[i][2]));
                    i++;
                }
                if (!inside)//si ningun extremo esta, probar con su centro u otros puntos centrales
                {
                    List<double[]> insideP = GetDivision();
                    this.tests.AddRange(insideP);
                    //insideP.Insert(0,CenterOfMass());
                    i = 0;
                    while (i < insideP.Count && !inside)
                    {
                        inside = pointInsideCone(theta, center, normal, new Vertex(insideP[i][0], insideP[i][1], insideP[i][2]));
                        i++;
                    }
                }
            }
                if (inside)
                {
                    if (isLeaf)
                    {
                        foreach (var item in elements)//guardar
                        {
                            if (pointInsideCone(theta, center,normal, Baricenter(item, vertexes)))
                                result.Add(item);
                        }
                    }
                    else
                    {
                        result.AddRange(leftChild.InsideCone(theta, center, normal, faces, vertexes));
                        result.AddRange(rigthChild.InsideCone(theta, center, normal, faces, vertexes));
                    }
                }
            return result;
        }
        public static bool pointInsideShadow(Vertex c, Face f, Vertex auxVert,Vertex point, List<Vertex> vertexes)//auxVert puede ser el baricentro de f
        {
            Plane p1 = new Plane(c, vertexes[f.i], vertexes[f.j]);
            Plane p2 = new Plane(c, vertexes[f.j], vertexes[f.k]);
            Plane p3 = new Plane(c, vertexes[f.k], vertexes[f.i]);
            //Plane test = new Plane(new Vertex(-1, 2, 3), new Vertex(6, 0, -2), new Vertex(4, 5, 1));//A=19,B=-11,C=31,D=-52
            double correctSign = p1.Evaluate(auxVert).CompareTo(0);
            if (p2.Evaluate(auxVert).CompareTo(0) != correctSign)
                p2.ChangeSign();
            if (p3.Evaluate(auxVert).CompareTo(0) != correctSign)
                p3.ChangeSign();
            double respectP1 = p1.Evaluate(point).CompareTo(0), respectP2 = p2.Evaluate(point).CompareTo(0), respectP3 = p3.Evaluate(point).CompareTo(0);
            //que los tres tengan el mismo signo y que sea el mismo que un pto interior, como el baricentro del centro + la normal
            if (respectP1 == correctSign && respectP1 == respectP2 && respectP2 == respectP3)//esta dentro del cono
                return true;
            return false;
        }
        public List<Face> InsideShadow(Vertex center, Face f, List<Vertex> vertexes,bool[] analize)
        {
            //quizaz lo que parece mas bobo que es por cada una de las otras caras preguntar si esta dentro de la sombra sea mas facil

            Vertex baricenter = Baricenter(f,vertexes);
            Vector direction = new Vector(baricenter.X-center.X,baricenter.Y-center.Y,baricenter.Z-center.Z);
            bool inside = IsInterceptedByRay(new Vector(center.X,center.Y,center.Z), direction);
            List<Face> result = new List<Face>();
            if (!inside)
            {
                List<double[]> extreme = GetExtremes();//un bounding box esta contenido si alguno de sus extremos estan
                this.tests.AddRange(extreme);

                int i = 0;
                while (i < extreme.Count && !inside)//eliminar los extremos que no estan en el cono, hasta encontrar el 1ro que si
                {
                    inside = pointInsideShadow(center, f,Baricenter(f,vertexes),new Vertex(extreme[i][0], extreme[i][1], extreme[i][2]),vertexes);
                    i++;
                }
                if (!inside)//si ningun extremo esta, probar con su centro u otros puntos centrales
                {
                    List<double[]> insideP = GetDivision();
                    this.tests.AddRange(insideP);
                    //insideP.Insert(0,CenterOfMass());
                    i = 0;
                    while (i < insideP.Count && !inside)
                    {
                        inside = pointInsideShadow(center, f, Baricenter(f, vertexes), new Vertex(insideP[i][0], insideP[i][1], insideP[i][2]), vertexes);
                        i++;
                    }
                }
            }
            if (inside)
            {
                if (isLeaf)
                {
                    foreach (var item in elements)//guardar                         
                        if (item.index != f.index && analize[item.index] && pointInsideShadow(center, f, Baricenter(f, vertexes), Baricenter(item, vertexes), vertexes))//un triangulo esta contenido si su baricentro lo esta
                        {
                            result.Add(item);
                            analize[item.index] = false;
                        }
                }
                else
                {
                    result.AddRange(leftChild.InsideShadow(center,f,vertexes,analize));
                    result.AddRange(rigthChild.InsideShadow(center,f,vertexes,analize));
                }
            }
            return result;
        }
        public bool IsInterceptedByRay(Vector origin, Vector direction)//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        {
            double tmin = double.MaxValue, tmax = double.MinValue, tymin, tymax, tzmin, tzmax;
            if (direction.elements[0] == 0)
            {
                if (origin.elements[0] < xmin || origin.elements[0] > xmax)
                    return false;
            }
            else
            {
                if (direction.elements[0] > 0)
                {
                    tmin = (xmin - origin.elements[0]) / direction.elements[0];
                    tmax = (xmax - origin.elements[0]) / direction.elements[0];
                }
                else
                {
                    tmax = (xmin - origin.elements[0]) / direction.elements[0];
                    tmin = (xmax - origin.elements[0]) / direction.elements[0];
                }
            }
            if (direction.elements[1] == 0)
            {
                if (origin.elements[1] < ymin || origin.elements[1] > ymax)
                    return false;
            }
            else
            {
                if (direction.elements[1] > 0)
                {
                    tymin = (ymin - origin.elements[1]) / direction.elements[1];
                    tymax = (ymax - origin.elements[1]) / direction.elements[1];
                }
                else
                {
                    tymax = (ymin - origin.elements[1]) / direction.elements[1];
                    tymin = (ymax - origin.elements[1]) / direction.elements[1];
                }
                if ((tmin > tymax) || (tymin > tmax))
                    return false;
                if (tymin > tmin)
                    tmin = tymin;
                if (tymax < tmax)
                    tmax = tymax;
            }
            if (direction.elements[2] == 0)
            {
                if (origin.elements[2] < zmin || origin.elements[2] > zmax)
                    return false;
            }
            else
            {
                if (direction.elements[2] > 0)
                {
                    tzmin = (zmin - origin.elements[2]) / direction.elements[2];
                    tzmax = (zmax - origin.elements[2]) / direction.elements[2];
                }
                else
                {
                    tzmin = (zmax - origin.elements[2]) / direction.elements[2];
                    tzmax = (zmin - origin.elements[2]) / direction.elements[2];
                }
                if ((tmin > tzmax) || (tzmin > tmax))
                    return false;
                if (tzmin > tmin)
                    tmin = tzmin;
                if (tzmax < tmax)
                    tmax = tzmax;
            }
            return true;
        }
    }
}
