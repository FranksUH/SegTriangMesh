using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using Accord.MachineLearning;
using Accord.Statistics;
using TMesh_2._0;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

[Serializable]
public class Half_Edge
{
    public int frm { get; private set; }
    public int dst { get; private set; }
    public int mate { get; set; }

    public Half_Edge(int from,int _dst, int _mate=-1)
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
public class Matrix
{
    public double[,] elements;
    public Matrix(int rows,int cols)
    {
        this.elements = new double[rows, cols];
    }
    public Matrix matricialProd(Matrix m2)
    {
        Matrix result = new Matrix(this.elements.GetLength(0),m2.elements.GetLength(1)); 
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

[Serializable]
public class Mesh
{

    public enum DistanceType { Angular, Geodesic, Volumetric, Combined1, Combined2 }
    double xMax, xMin, yMax, yMin, zMax, zMin;
    public bool GetAffinity;
    public int K;
    public string Name { get; private set; }
    public int CountVertex { get; private set; }
    public int CountFace { get; private set; }
    public int CountEdge { get; private set; }
    public DistanceType Distance;
    private const int DEFAULT_KMEANS_ITERATION = 5;
    private const int DEFAULT_CLUSTER_AMOUNT = 3;
    public int partitions;
    private Tuple<DistanceType,int,double,double> buildedGraph;
    private List<Vertex> vertexes;
    private List<Vertex> dual_verts;
    private List<Face> faces;
    private List<Half_Edge> edges;
    private Dictionary<Tuple<int, int>, int> indexOf;
    public AABBTree aabb;
    public double[,] dual_graph_cost { get; private set; }//solo los adyacentes(Nx3)
    public double [,] combined_cost { get; private set; }
    public double[][] distancesMatrix { get; private set; }//NxK de todos a todos
    public double[][] distancesMatrixCombined { get; private set; }
    public double [][] afinityMatrix { get; set; }
    public int[,] dual_graph_edges { get; private set; }
    public int[] cluster;
    public int countCols;
    public int[] nextIndex { get; set; }//para almacenar [i] que columna fue la escogida para ocupar la i-esima columna en la distancesMatrix
    public int[] nextIndex2 { get; set; }
    public Mesh()
	{
        vertexes = new List<Vertex>();
        faces = new List<Face>();
        edges = new List<Half_Edge>();
        indexOf = new Dictionary<Tuple<int, int>, int>();
        dual_verts = new List<Vertex>();
        this.Distance = DistanceType.Angular;
        this.GetAffinity = true;

        xMax = -1;
        xMin = -1;
        yMax = -1;
        yMin = -1;
        zMax = -1;
        zMin = -1;
	}
    public void Save(string path)
    {
        IFormatter format = new BinaryFormatter();
        Stream s = new FileStream(path, FileMode.OpenOrCreate);
        format.Serialize(s, this);
        s.Close();
    }
    public void LoadOFF(string fileName)
    {
        Name = Cut(fileName);

        StreamReader reader = new StreamReader(fileName);
        string sLine = reader.ReadLine();

        if (sLine != "OFF")
            throw new Exception("El fichero debe ser de tipo OFF");
        else 
        {
            sLine = "";
            sLine = reader.ReadLine();
            var items = sLine.Split(' ');

            CountVertex = int.Parse(items[0]);
            CountFace = int.Parse(items[1]);
            countCols = Math.Max(CountFace,3);

            for (int i = 0; i < CountVertex; i++)
            {
                sLine = "";
                sLine = reader.ReadLine();
                items = sLine.Split(' ');
                items = RemoveSpace(sLine.Split(' '));
                //var num1 = toDouble(items[0]);
                AddVertex(double.Parse(items[0]),double.Parse(items[1]),double.Parse(items[2]));
                
                if (xMax == -1 || vertexes[i].X > xMax)
                    xMax = vertexes[i].X;
                if (xMin == -1 || vertexes[i].X < xMin)
                    xMin = vertexes[i].X;

                if (yMax == -1 || vertexes[i].Y > yMax)
                    yMax = vertexes[i].Y;
                if (yMin == -1 || vertexes[i].Y < yMin)
                    yMin = vertexes[i].Y;

                if (zMax == -1 || vertexes[i].Z > zMax)
                    zMax = vertexes[i].Z;
                if (zMin == -1 || vertexes[i].Z < zMin)
                    zMin = vertexes[i].Z;
            }

            for (int i = 0; i < CountFace; i++)
            {
                sLine = "";
                sLine = reader.ReadLine();
                items = RemoveSpace(sLine.Split(' '));
                //items = sLine.Split(' ');
                if (items[0] != "3")
                    throw new ArgumentException("Todas las caras deben ser triángulos");
                //update faces
                int v1 = int.Parse(items[1]), v2 = int.Parse(items[2]), v3 = int.Parse(items[3]);
                AddFace(v1, v2, v3,i);
                //update halfe edge
                AddEdge(v1, v2);
                AddEdge(v2, v3);
                AddEdge(v3, v1);
                //update dual graph's vertexes
                dual_verts.Add(new Vertex((vertexes[v1].X + vertexes[v2].X + vertexes[v3].X) / 3, (vertexes[v1].Y + vertexes[v2].Y + vertexes[v3].Y) / 3, (vertexes[v1].Z + vertexes[v2].Z + vertexes[v3].Z) / 3));
            }
        }
        reader.Close();
        CenterAndScale();
    }
    public void build_dual_graph(int dim1, bool getAll = false, double angular = 0, double geodesic = 0)
    {
        #region Initialize
        //FilesChat.SendAndOverWrite("build", "0");
        this.dual_graph_cost = new double[this.CountFace, 3];//a lo sumo cada cara tendra 3 adyacentes
        this.dual_graph_edges = new int[this.CountFace, 3];
        if (Distance == DistanceType.Combined1)
        {
            this.combined_cost = new double[this.CountFace, 3];
            this.distancesMatrixCombined = new double[this.CountFace][];
        }
        this.distancesMatrix = new double[this.CountFace][];
        this.afinityMatrix = new double[this.CountFace][];
        //FilesChat.SendAndOverWrite("build", "1");
        for (int i = 0; i < this.CountFace; i++)
        {
            distancesMatrix[i] = new double[dim1];
            if (Distance == DistanceType.Combined1)            
                distancesMatrixCombined[i] = new double[dim1];
            afinityMatrix[i] = new double[dim1];
        }
        //FilesChat.SendAndOverWrite("build", "2");
        #endregion
        #region Precalc
        for (int i = 0; i < this.CountFace; i++)
        {
            List<int> adj = adjacent_faces(this.faces[i]);
            //double porcent = ((double)(i+1) / (double)CountFace) * 100;
            //if (porcent > 2)
            //    FilesChat.SendAndOverWrite("build",porcent.ToString());
            for (int j = 0; j < adj.Count; j++)
            {
                dual_graph_edges[i, j] = adj[j];
                switch (Distance)
                {
                    case DistanceType.Angular:
                        dual_graph_cost[i, j] = angular_dist(this.faces[i], this.faces[adj[j]]);//indica el costo hacia su j-esima cara adyacente
                        break;
                    case DistanceType.Geodesic:
                        dual_graph_cost[i, j] = geodesic_dist(i, adj[j]);
                        break;
                    case DistanceType.Volumetric:
                        dual_graph_cost[i, j] = volumetricDist(i, adj[j]);
                        break;
                    case DistanceType.Combined1:
                        dual_graph_cost[i, j] = angular_dist(this.faces[i], this.faces[adj[j]]);
                        combined_cost[i, j] = geodesic_dist(i, adj[j]);
                        break;
                    case DistanceType.Combined2:
                        dual_graph_cost[i, j] = (angular_dist(this.faces[i], this.faces[adj[j]]) * angular) + (geodesic_dist(i, adj[j]) * geodesic);
                        if ((1 - angular - geodesic) > 0)
                            dual_graph_cost[i, j] += (volumetricDist(i, adj[j]) * (1 - angular - geodesic));
                        break;
                    default:
                        break;
                }
            }
            for (int j = adj.Count; j < 3; j++)//rellenar con -1 los que no estan
                dual_graph_edges[i, j] = -1;        
        }
        #endregion
        #region Dijkstra
        if (!getAll)
        {
            #region Rellenar la matriz de distancias con dijkstra tomando una muestra
            double[] cost = dijkstra(0);
            this.nextIndex = new int[dim1];
            this.nextIndex[0] = 0;
            double farthest = double.MinValue;//maximo de los minimos de las columnas
            double[] nearthest = new double[this.CountFace];//minimo de las filas
            for (int j = 0; j < cost.Length; j++)//columna 0
            {
                if (cost[j] > farthest)
                {
                    farthest = cost[j];
                    nextIndex[1] = j;
                }
                nearthest[j] = cost[j];
                this.distancesMatrix[j][0] = cost[j];
            }
            for (int i = 1; i < dim1; i++)
            {
                cost = dijkstra(nextIndex[i]);
                //cost = dijkstra(i);
                for (int j = 0; j < cost.Length; j++)//actualizar minimo de cada fila
                {
                    if (cost[j] < nearthest[j])
                        nearthest[j] = cost[j];
                    this.distancesMatrix[j][i] = cost[j];//la distancia del nodo j al i-esimo escogido
                }
                if ((i + 1) < dim1)//escoger el maximo de entre todas las filas
                {
                    farthest = nearthest[0];//obtener la proxima cara
                    for (int j = 1; j < this.CountFace; j++)
                        if (nearthest[j] > farthest)
                        {
                            farthest = nearthest[j];
                            nextIndex[i + 1] = j;
                        }
                }
            }
            #region Just for product
            if (Distance == DistanceType.Combined1)
            {
                double[] cost2 = dijkstra2(0);
                //this.nextIndex2 = new int[dim1];//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                this.nextIndex[0] = 0;
                double farthest2 = double.MinValue;//maximo de los minimos de las columnas
                double[] nearthest2 = new double[this.CountFace];//minimo de las filas
                for (int j = 0; j < cost2.Length; j++)//columna 0
                {
                    if (cost2[j] > farthest2)
                    {
                        farthest2 = cost2[j];
                        nextIndex[1] = j;
                    }
                    nearthest2[j] = cost2[j];
                    this.distancesMatrixCombined[j][0] = cost2[j];//************al revez
                }
                for (int i = 1; i < dim1; i++)
                {
                    cost2 = dijkstra2(nextIndex[i]);
                    //cost = dijkstra(i);
                    for (int j = 0; j < cost2.Length; j++)//actualizar minimo de cada fila
                    {
                        if (cost2[j] < nearthest2[j])
                            nearthest2[j] = cost2[j];
                        this.distancesMatrixCombined[j][i] = cost2[j];//la distancia del nodo j al i-esimo escogido
                    }
                    if ((i + 1) < dim1)//escoger el maximo de entre todas las filas
                    {
                        farthest2 = nearthest2[0];//obtener la proxima cara
                        for (int j = 1; j < this.CountFace; j++)
                            if (nearthest2[j] > farthest2)
                            {
                                farthest2 = nearthest2[j];
                                nextIndex[i + 1] = j;
                            }
                    }
                }
            }
            #endregion
            #endregion
        }
        else
        {
            #region Rellenar la matriz usando dijkstra por todas las caras
            for (int i = 0; i < this.CountFace; i++)
            {
                distancesMatrix[i] = dijkstra(i);
            }
            #endregion
        }
        #endregion
        if (Distance == DistanceType.Combined1)//normalize if Product
        {
            double[] geodesicMax = new double[distancesMatrix[0].Length];//k
            double[] angularMax = new double[distancesMatrix[0].Length];
            for (int i = 0; i < geodesicMax.Length; i++)
            {
                geodesicMax[i] = getMaxGeo(i);
                angularMax[i] = getMaxInCol(i);
            }
            for (int i = 0; i < distancesMatrix.Length; i++)
            {
                for (int j = 0; j < distancesMatrix[0].Length; j++)//columna
                {
                    distancesMatrix[i][j] = distancesMatrix[i][j] / angularMax[j] * distancesMatrixCombined[i][j] / geodesicMax[j];
                }
            }
            buildAfinity(dim1);            
        }
        else
        {
            if (GetAffinity)
            {
                double[] ColMax = new double[distancesMatrix[0].Length];
                for (int i = 0; i < ColMax.Length; i++)
                    ColMax[i] = getMaxInCol(i);
                for (int i = 0; i < distancesMatrix.Length; i++)
                {
                    for (int j = 0; j < distancesMatrix[0].Length; j++)//columna
                    {
                        distancesMatrix[i][j] = distancesMatrix[i][j] / ColMax[j];
                    }
                }
                buildAfinity(dim1);
            }
        }
    }
    public void buildAfinity(int k)
    {
        double k_coeficient = 0;
        for (int i = 0; i < afinityMatrix.Length; i++)
        {
            for (int j = 0; j < k; j++)
            {
                k_coeficient += distancesMatrix[i][j];
            }
        }
        k_coeficient /= (k*afinityMatrix.Length);
        for (int i = 0; i < afinityMatrix.Length; i++)
        {
            for (int j = 0; j < k; j++)
            {
                afinityMatrix[i][j] = 1 / (Math.Pow(Math.E, (distancesMatrix[i][j] / (k_coeficient * k_coeficient * 2))));
            }
        }
        double[] norm = new double[distancesMatrix.Length];
        for (int i = 0; i < distancesMatrix.Length; i++)
        {
            double normi = 0;
            for (int j = 0; j < distancesMatrix[0].Length; j++)
            {
                normi += afinityMatrix[i][j] * afinityMatrix[i][j];
            }
            norm[i] = Math.Sqrt(normi);
        }
        for (int i = 0; i < distancesMatrix.Length; i++)
        {
            for (int j = 0; j < distancesMatrix[0].Length; j++)
            {
                afinityMatrix[i][j] /= norm[i];
            }
        }
    }
    public double getMaxGeo(int j)
    {
        double max = double.MinValue;
            for (int i = 0; i < distancesMatrixCombined.Length; i++)
            {
            if (distancesMatrixCombined[i][j] > max)
                max = distancesMatrixCombined[i][j];
            }
        return max;
    }
    public double getMaxInCol(int j)
    {
        double max = double.MinValue;
        for (int i = 0; i < distancesMatrix.Length; i++)
        {
            if (distancesMatrix[i][j] > max)
                max = distancesMatrix[i][j];
        }
        return max;
    }
    public double geodesicFarest()
    {
        double result = double.MinValue;
        for (int i = 0; i < this.CountFace; i++)
        {
            List<int> adj = adjacent_faces(this.faces[i]);
            for (int j = 0; j < adj.Count; j++)
            {
                double aux = geodesic_dist(i, adj[j]);
                if (aux > result)
                    result = aux;
            }
        }
        return result;
    }
    private double[] dijkstra(int source)
    {
        double[] minCost = new double[this.CountFace];
        SortedSet<Tuple<double, int>> pending = new SortedSet<Tuple<double, int>>();
        bool[] visited = new bool[this.CountFace+2];
        for (int i = 0; i < minCost.Length; i++)
            minCost[i] = double.MaxValue;
        minCost[source] = 0;
        pending.Add(new Tuple<double, int>(0, source));
        while (pending.Count > 0)
        {
            Tuple<double, int> m = pending.First();
            pending.Remove(m);
            int u = m.Item2;
            if (visited[u]) continue; //Si el vértice actual ya fue visitado entonces sigo sacando elementos de la cola
            visited[u] = true;         //Marco como visitado el vértice actual

            for (int i = 0; i < 3 && dual_graph_edges[m.Item2, i] != -1; i++)//recorrer los adyacentes
            {
                int v = dual_graph_edges[u, i];
                if (!visited[v] && minCost[v] > minCost[u] + dual_graph_cost[u, i])//relax
                {
                    minCost[v] = minCost[u] + dual_graph_cost[u, i];
                    pending.Add(new Tuple<double, int>(minCost[v], v));
                }
            }
        }
        return minCost;
    }
    private double[] dijkstra2(int source)
    {
        double[] minCost = new double[this.CountFace];
        SortedSet<Tuple<double, int>> pending = new SortedSet<Tuple<double, int>>();
        bool[] visited = new bool[this.CountFace + 2];
        for (int i = 0; i < minCost.Length; i++)
            minCost[i] = double.MaxValue;
        minCost[source] = 0;
        pending.Add(new Tuple<double, int>(0, source));
        while (pending.Count > 0)
        {
            Tuple<double, int> m = pending.First();
            pending.Remove(m);
            int u = m.Item2;
            if (visited[u]) continue; //Si el vértice actual ya fue visitado entonces sigo sacando elementos de la cola
            visited[u] = true;         //Marco como visitado el vértice actual

            for (int i = 0; i < 3 && dual_graph_edges[m.Item2, i] != -1; i++)//recorrer los adyacentes
            {
                int v = dual_graph_edges[u, i];
                if (!visited[v] && minCost[v] > minCost[u] + combined_cost[u, i])//relax
                {
                    minCost[v] = minCost[u] + combined_cost[u, i];
                    pending.Add(new Tuple<double, int>(minCost[v], v));
                }
            }
        }
        return minCost;
    }
    public double testGeodesic(Face t1, Face t2,List<Vertex>v)
    {
        this.vertexes = v;
        this.faces = new List<Face>();
        faces.Add(t1);
        faces.Add(t2);
        return geodesic_dist(0, 1);
    }
    public double geodesic_dist(int j,int i)
    {
        Face f1 = this.faces[i];
        Face f2 = this.faces[j];
        Plane p = new Plane(this.vertexes[f1.i], this.vertexes[f1.j], this.vertexes[f1.k]);
        int notCommun = f2.i;
        int common1 = f2.j;
        int common2 = f2.k;
        if (f2.j != f1.i && f2.j != f1.j && f2.j != f1.k)
        {
            notCommun = f2.j;
            common1 = f2.i;
            common2 = f2.k;
        }
        if (f2.k != f1.i && f2.k != f1.j && f2.k != f1.k)
        {
            notCommun = f2.k;
            common1 = f2.i;
            common2 = f2.j;
        }
        #region Para que pregunte si estan en el mismo plano
        //if (p.Belongs(vertexes[notCommun]))//Si estan en el mismo plano devolver la euclidiana entre los baricentros si la recta esta dentro
        //{
        //    if ()
        //    {

        //    }
        //    double[] c1 = new double[] { vertexes[common1].X, vertexes[common1].Y, vertexes[common1].Z };
        //    double[] c2 = new double[] { vertexes[common2].X, vertexes[common2].Y, vertexes[common2].Z };
        //    double[] v1 = new double[] { dual_verts[i].X, dual_verts[i].Y, dual_verts[i].Z };
        //    double[] v2 = new double[] { dual_verts[j].X, dual_verts[j].Y, dual_verts[j].Z };
        //    double sumA = euclidianDistance(c1, v1) + euclidianDistance(c1, v2);
        //    double sumaB = euclidianDistance(c2, v1) + euclidianDistance(c2, v2);
        //    return Math.Min(sumA, sumaB);
        //}
        #endregion
        #region Para rotar y trasladar T2
        Vector vn1 = new Vector(vertexes[common2].X - vertexes[common1].X, vertexes[common2].Y - vertexes[common1].Y, vertexes[common2].Z - vertexes[common1].Z);
        vn1.normalize();
        Vector wn1 = new Vector(vertexes[notCommun].X-vertexes[common1].X, vertexes[notCommun].Y - vertexes[common1].Y, vertexes[notCommun].Z - vertexes[common1].Z);
        Vector aux = vn1.copy();
        aux.multiply(aux.scalar_product(wn1));
        wn1.elements[0] = wn1.elements[0] - aux.elements[0];
        wn1.elements[1] = wn1.elements[1] - aux.elements[1];
        wn1.elements[2] = wn1.elements[2] - aux.elements[2];
        wn1.normalize();
        Matrix t0 = new Matrix(4, 4);
        t0.elements[0, 0] = 1;
        t0.elements[1, 1] = 1;
        t0.elements[2, 2] = 1;
        t0.elements[0, 3] = -1 * vertexes[common1].X;
        t0.elements[1, 3] = -1 * vertexes[common1].Y;
        t0.elements[2, 3] = -1 * vertexes[common1].Z;
        t0.elements[3, 3] = 1;
        Matrix r1 = new Matrix(4, 4);
        r1.elements[0, 0] = vn1.elements[0];
        r1.elements[0, 1] = vn1.elements[1];
        r1.elements[0, 2] = vn1.elements[2];
        r1.elements[1, 0] = wn1.elements[0];
        r1.elements[1, 1] = wn1.elements[1];
        r1.elements[1, 2] = wn1.elements[2];
        Vector toGetNormal = new Vector(vertexes[common2].X-vertexes[common1].X, vertexes[common2].Y - vertexes[common1].Y, vertexes[common2].Z - vertexes[common1].Z);
        toGetNormal =  toGetNormal.vectorial_product(new Vector(vertexes[notCommun].X - vertexes[common1].X, vertexes[notCommun].Y - vertexes[common1].Y, vertexes[notCommun].Z - vertexes[common1].Z));
        toGetNormal.normalize();
        r1.elements[2, 0] = toGetNormal.elements[0];
        r1.elements[2, 1] = toGetNormal.elements[1];
        r1.elements[2, 2] = toGetNormal.elements[2];
        r1.elements[3, 3] = 1;
        Matrix toProyect = new Matrix(4, 1);
        toProyect.elements[0, 0] = vertexes[notCommun].X;
        toProyect.elements[1, 0] = vertexes[notCommun].Y;
        toProyect.elements[2, 0] = vertexes[notCommun].Z;
        toProyect.elements[3, 0] = 1;
        Matrix m1 = r1.matricialProd(t0);
        Matrix projection2 = m1.matricialProd(toProyect);

        Vector L = new Vector(vertexes[common2].X - vertexes[common1].X, vertexes[common2].Y - vertexes[common1].Y, vertexes[common2].Z - vertexes[common1].Z);
        Vertex baricenter2 = new Vertex((L.norm()+projection2.elements[0,0])/3,projection2.elements[1,0]/3,0);
        #endregion
        #region Para rotar y trasladar T1
        int notCommun2 = f1.i;
        if (f1.j != f2.i && f1.j != f2.j && f1.j != f2.k)
            notCommun2 = f1.j;
        if (f1.k != f2.i && f1.k != f2.j && f1.k != f2.k)
            notCommun2 = f1.k;
        Vector wn2 = new Vector(vertexes[notCommun2].X - vertexes[common1].X, vertexes[notCommun2].Y - vertexes[common1].Y, vertexes[notCommun2].Z - vertexes[common1].Z);
        Vector aux2 = vn1.copy();
        aux2.multiply(aux2.scalar_product(wn2));
        wn2.elements[0] = wn2.elements[0] - aux2.elements[0];
        wn2.elements[1] = wn2.elements[1] - aux2.elements[1];
        wn2.elements[2] = wn2.elements[2] - aux2.elements[2];
        wn2.normalize();      
        Matrix r2 = new Matrix(4, 4);
        r2.elements[0, 0] = vn1.elements[0];
        r2.elements[0, 1] = vn1.elements[1];
        r2.elements[0, 2] = vn1.elements[2];
        r2.elements[1, 0] = wn2.elements[0];
        r2.elements[1, 1] = wn2.elements[1];
        r2.elements[1, 2] = wn2.elements[2];
        Vector toGetNormal2 = new Vector(vertexes[common2].X - vertexes[common1].X, vertexes[common2].Y - vertexes[common1].Y, vertexes[common2].Z - vertexes[common1].Z);
        toGetNormal2 = toGetNormal2.vectorial_product(new Vector(vertexes[notCommun2].X - vertexes[common1].X, vertexes[notCommun2].Y - vertexes[common1].Y, vertexes[notCommun2].Z - vertexes[common1].Z));
        toGetNormal2.normalize();
        r2.elements[2, 0] = toGetNormal2.elements[0];
        r2.elements[2, 1] = toGetNormal2.elements[1];
        r2.elements[2, 2] = toGetNormal2.elements[2];
        r2.elements[3, 3] = 1;
        Matrix toProyect2 = new Matrix(4, 1);
        toProyect2.elements[0, 0] = vertexes[notCommun2].X;
        toProyect2.elements[1, 0] = vertexes[notCommun2].Y;
        toProyect2.elements[2, 0] = vertexes[notCommun2].Z;
        toProyect2.elements[3, 0] = 1;
        Matrix m2 = r2.matricialProd(t0);
        Matrix projection1 = m2.matricialProd(toProyect2);
        if (projection1.elements[1,0] * projection2.elements[1,0] > 0)//cambio el <
            projection1.elements[1,0] *= -1;  
        Vertex baricenter1 = new Vertex((L.norm() + projection1.elements[0, 0]) / 3, projection1.elements[1, 0] / 3, 0);
        #endregion
        //cambio interception
        double interception = L.norm()/3+(projection1.elements[1,0]*projection2.elements[0,0]-projection2.elements[1,0]*projection1.elements[0,0])/(3*(projection1.elements[1,0]-projection2.elements[1,0]));
        //if (Math.Abs(interception) <= L.norm())
        if(interception >= 0 && interception <= L.norm())
        {
            Vector v = new Vector(baricenter1.X - baricenter2.X, baricenter1.Y - baricenter2.Y, baricenter1.Z - baricenter2.Z);
            return v.norm();            
        }
        else
        {   //Cambio forma de calcular
            if (interception < 0)
            {
                //double[] c1 = new double[] { baricenter1.X, baricenter1.Y, baricenter1.Z };
                //double[] c2 = new double[] { baricenter2.X, baricenter2.Y, baricenter2.Z };
                //return euclidianDistance(c1, c2);
                Vector v1 = new Vector(baricenter1.X, baricenter1.Y, baricenter1.Z);
                Vector v2 = new Vector(baricenter2.X, baricenter2.Y, baricenter2.Z);
                return v1.norm() + v2.norm();
            }
            else
            {
                Vector v = new Vector((projection1.elements[0, 0] - 2 * L.norm()) / 3, projection1.elements[1, 0] / 3, 0);
                Vector w = new Vector((projection2.elements[0, 0] - 2 * L.norm()) / 3, projection2.elements[1, 0] / 3, 0);
                return v.norm() + w.norm();
            }
        }
    }
    public double angular_dist(Face f1, Face f2)
    {
        return angular_dist(GetNormalToFace(f1),GetNormalToFace(f2));
    }
    public double angular_dist(Vector v1,Vector v2)
    {
        return (1 -v1.cos(v2)) / 2;
    }
    public double angular_dist(double[] a, double[] b)//for K-means
    {
        double scalarProd = 0,normA=0,normB=0;
        for (int i = 0; i < a.Length; i++)
        {
            scalarProd += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }
        normA = Math.Sqrt(normA);
        normB = Math.Sqrt(normB);
        return (1-(scalarProd/(normA*normB)))/2;
    }
    public Vector GetNormalToFace(Face f1)
    {
        Vector v1 = new Vector(vertexes[f1.i].X - vertexes[f1.j].X, vertexes[f1.i].Y - vertexes[f1.j].Y, vertexes[f1.i].Z - vertexes[f1.j].Z);
        Vector v2 = new Vector(vertexes[f1.j].X - vertexes[f1.k].X, vertexes[f1.j].Y - vertexes[f1.k].Y, vertexes[f1.j].Z - vertexes[f1.k].Z);
        Vector v3 = new Vector(vertexes[f1.k].X - vertexes[f1.i].X, vertexes[f1.k].Y - vertexes[f1.i].Y, vertexes[f1.k].Z - vertexes[f1.i].Z);
        return Vector.normal(v1, v2, v3);
    }
    public double AreaFace(int i)//return i-face's area
    {
        double a, b, c, s;
        a = euclidianDistance(vertexes[faces[i].i] ,vertexes[faces[i].j]);
        b = euclidianDistance(vertexes[faces[i].j], vertexes[faces[i].k]);
        c = euclidianDistance(vertexes[faces[i].k], vertexes[faces[i].i]);
        s = (a + b + c) / 2;
        return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
    }
    public Vector Baricenter(int face)
    {
        Face f = FacesAt(face);
        return new Vector((vertexes[f.i].X + vertexes[f.j].X + vertexes[f.k].X) / 3, (vertexes[f.i].Y + vertexes[f.j].Y + vertexes[f.k].Y) / 3, (vertexes[f.i].Z + vertexes[f.j].Z + vertexes[f.k].Z) / 3);
    }
    public Vector Baricenter(Face f)
    {
        return new Vector((vertexes[f.i].X + vertexes[f.j].X + vertexes[f.k].X) / 3, (vertexes[f.i].Y + vertexes[f.j].Y + vertexes[f.k].Y) / 3, (vertexes[f.i].Z + vertexes[f.j].Z + vertexes[f.k].Z) / 3);
    }
    #region to calculate volumetric distance
    public List<Tuple<double[],double[]>> Getbbx()
    {
        if (aabb == null)
            aabb = new AABBTree(faces, vertexes, getleafSize());
        return aabb.GetBB();
    }
    public double volumetricDist(int i,int j,double theta=60)//return the volumetric dist betwen i-face and j-face
    {
        double communArea=0, combinedArea=0;
        if (aabb == null)
            aabb = new AABBTree(faces, vertexes, getleafSize());
        List<Face> inside = GetVisibility(i, theta);
        bool[] catched = new bool[CountFace];//1 si la n-esima cara es visible por la cara i
        for (int k = 0; k < inside.Count; k++)
        {
            catched[inside[k].index] = true;
            combinedArea += AreaFace(inside[k].index);
        }
        inside.Clear();
        inside = GetVisibility(j, theta);
        for (int k = 0; k < inside.Count; k++)
        {
            double area = AreaFace(inside[k].index);
            if (!catched[k])
                combinedArea += area;
            else communArea += area;
        }

        return 1-(communArea/combinedArea);//area comun = intercepcon de las areas y area conjunta = union de las areas
    }
    public List<Face> RemoveHiddenFaces(double theta, Face center, Vector normal, List<Face> insideCone)
    {
        Vector bc = Baricenter(center);
        Vertex c = new Vertex(bc.elements[0], bc.elements[1], bc.elements[2]);
        QuickSort(insideCone, bc, 0, insideCone.Count - 1);//!!!!!quizas comprobar
        bool[] good = new bool[CountFace];

        for (int i = 0; i < insideCone.Count; i++)//para revizar solo los que estan dentro del cono
            good[insideCone[i].index] = true;

        #region using AABBTree
        //for (int i = 0; i < insideCone.Count - 1; i++)//para eliminar los que resultan en la sombra de alguien
        //{
        //    if (!good[insideCone[i].index])
        //        continue;
        //    List<Face> inside = aabb.InsideShadow(c, insideCone[i], this.vertexes, good);
        //    for (int j = 0; j < inside.Count; j++)//ya no hace falta
        //        good[inside[j].index] = false;
        //}
        #endregion
        #region Raw Testing
        for (int i = 0; i < insideCone.Count - 1; i++)//para eliminar los que resultan en la sombra de alguien
        {
            if (!good[insideCone[i].index])
                continue;
            List<Face> inside = aabb.InsideShadow(c, insideCone[i], this.vertexes, good);
            for (int j = i+1; j < inside.Count; j++)
            {
                Vector sureInside = Baricenter(inside[i]);
                Vector testingInside = Baricenter(inside[j]);
                if (AABBTree.pointInsideShadow(c, insideCone[i],new Vertex(sureInside.elements[0], sureInside.elements[1], sureInside.elements[2]), new Vertex(testingInside.elements[0], testingInside.elements[1], testingInside.elements[2]), this.vertexes))
                    good[inside[j].index] = false;
            }
        }
        #endregion
        Vector nc = GetNormalToFace(center);
        for (int i = 0; i < insideCone.Count; i++)
        {
            if (!good[insideCone[i].index])
                continue;
            if (nc.scalar_product(GetNormalToFace(insideCone[i])) > 0)
                good[insideCone[i].index] = false;
        }
        List<Face> result = new List<Face>();
        for (int i = 0; i < CountFace; i++)
        {
            if (good[i])
                result.Add(FacesAt(i));
        }
        return result;
    }
    public void QuickSort(List<Face> elements, Vector C, int left, int right)
    {
        if(right>left)
        {
            Random r = new Random();
            int pivot = r.Next(left, right);
            int npivot = Partition(elements, C, left, right, pivot);
            QuickSort(elements, C, left, npivot-1);
            QuickSort(elements, C, npivot+1, right);
        }
    }
    int Partition(List<Face> elements, Vector C, int left, int right, int pivot)
    {
        Vector b = Baricenter(elements[pivot]);
        double pval = Vector.euclidianDistance(C, b);
        Face temp = elements[right];
        elements[right] = elements[pivot];
        elements[pivot] = temp;
        int sindex = left;
        for(int i = left; i<right; i++)
        {
            Vector bary = Baricenter(elements[i]);
            if(Vector.euclidianDistance(C, bary) < pval)
            {
                    temp = elements[i];
                    elements[i] = elements[sindex];
                    elements[sindex] = temp;
                    sindex++;
            }
        }
        temp = elements[sindex];
        elements[sindex] = elements[right];
        elements[right] = temp;
        return sindex;
    }
    int getleafSize()
    {
        return faces.Count / 10;
        //return Math.Min(Math.Max(faces.Count / 100, 1), 50);
    }
    public List<Face> TestVisibility(int i,double theta=120)
    {
        aabb = new AABBTree(faces, vertexes, getleafSize());
        var n = GetNormalToFace(faces[i]);        
        List<Face>result = aabb.InsideCone(theta, Baricenter(i), GetNormalToFace(faces[i]), faces, vertexes);
        var x = RemoveHiddenFaces(theta, FacesAt(i), n, result);
        return x;
        //return result;
        //return aabb.InsideCone(theta, Baricenter(i), GetNormalToFace(faces[i]), faces, vertexes);
        //return GetVisibility(i, theta);
    }
    public List<Face> GetVisibility(int i, double theta = 120)
    {
        //List<Face> result = new List<Face>();
        Vector n = GetNormalToFace(faces[i]);
        List<Face> result = aabb.InsideCone(theta,Baricenter(i),n,faces,vertexes);
        //result = RemoveHiddenFaces(theta, FacesAt(i), n, result);//tambien elimina las falsas intercepciones
        return result;
    }
#endregion
    public List<int> adjacent_faces(Face f)
    {
        List<int> adjacents = new List<int>();
        //it's mate is the extern
        Tuple<int, int> he1 = new Tuple<int, int>(f.j, f.i);
        Tuple<int, int> he2 = new Tuple<int, int>(f.k, f.j);
        Tuple<int, int> he3 = new Tuple<int, int>(f.i, f.k);
        if (indexOf.ContainsKey(he1))//there are axactly 3 he(s) by any face
            adjacents.Add(indexOf[he1] / 3);
        if (indexOf.ContainsKey(he2))
            adjacents.Add(indexOf[he2] / 3);
        if (indexOf.ContainsKey(he3))
            adjacents.Add(indexOf[he3] / 3);
        return adjacents;
    }
    private void CenterAndScale()
    {
        Vertex gravCenter = new Vertex((xMin + xMax) / 2.0, (yMin + yMax) / 2.0, (zMin + zMax) / 2.0);
        double scaleFactor = 5 / Math.Max(Math.Max(xMax - xMin, yMax - yMin), zMax - zMin);

        for (int i = 0; i < CountVertex; i++)
        {
            int he = vertexes[i].he;
            vertexes[i] = (vertexes[i] - gravCenter) * scaleFactor;
            vertexes[i].he = he;
        }
    }
    private string[] RemoveSpace(string[] p)//elimina los elementos vacios del array
    {
        List<string> result = new List<string>();

        foreach (var item in p)
        {
            if (item != "" && item!=" ")
                result.Add(item);
        }

        return result.ToArray();
    }
    private void AddVertex(double x, double y, double z)
    {
        vertexes.Add(new Vertex(x, y, z));
    }
    private void AddFace(int i, int j, int k,int index)
    {
        faces.Add(new Face(i, j, k,index));
    }
    private void AddEdge(int frm, int to)
    {        
        edges.Add(new Half_Edge(frm, to));
        if (vertexes[to].he == -1)//ahora el vertice en mate ya tiene algun halfe-edge incidente
            vertexes[to].he = edges.Count-1;
        Tuple<int, int> reverse = new Tuple<int, int>(to, frm);
        Tuple<int, int> newEdge = new Tuple<int, int>(frm, to);
        if (indexOf.ContainsKey(reverse))
        {
            int index = indexOf[reverse];
            edges[index].mate = edges.Count - 1;
            edges[edges.Count - 1].mate = index;
        }
        indexOf[newEdge] = edges.Count - 1;
        CountEdge++;
    }
    public Vertex VertexesAt(int i)
    {
        return vertexes[i];
    }
    public Face FacesAt(int i)
    {
        return faces[i];
    }
    public Half_Edge EdgesAt(int i)
    {
        return edges[i];
    }
    private string Cut(string name)//elimina la extension del archivo
    {
        int i = name.Length-1;
        while (name[i] != '.')
            i--;
        return name.Substring(0, i);
    }
    private static double toDouble(string number)
    {
        int i = number.Length-1;

        while (i >=0 && number[i] != '.')
            i--;

        return (i >= 0) ? double.Parse(number) / (Math.Pow(10.0, number.Length - 1 - i)) : double.Parse(number);
    }
    private void kMeans(int partitions = DEFAULT_CLUSTER_AMOUNT, int iterations = DEFAULT_KMEANS_ITERATION)
    {
        cluster = new int[this.CountFace];
        int[] testCluster = new int[cluster.Length];
        double eval = double.MaxValue, bestEval = double.MaxValue;
        #region Accord Kmeans
        //for (int i = 0; i < iterations; i++)
        //{
        //if (Distance == DistanceType.Combined1 || Distance == DistanceType.Combined2)
        //{
        //    KMeans km = new KMeans(partitions, angular_dist);
        //    km.Randomize(this.afinityMatrix);
        //    testCluster = km.Compute(afinityMatrix);  //Compute(this.distancesMatrix);
        //                                              // eval = Evaluate(km, testCluster);
        //                                              // if (eval < bestEval)
        //    Array.Copy(testCluster, cluster, cluster.Length);
        //}
        //else
        //{
        //    KMeans km = new KMeans(partitions, angular_dist);
        //    km.Randomize(this.afinityMatrix);
        //    cluster = km.Compute(afinityMatrix);  //Compute(this.distancesMatrix);

        //}
   // }
    #endregion
    if (Distance == DistanceType.Combined1 || Distance == DistanceType.Combined2)
    {
            KMeans km = new KMeans(partitions, angular_dist);
            for (int i = 0; i < iterations; i++)
            {
                km.Randomize(this.afinityMatrix);
                testCluster = km.Compute(afinityMatrix);
                eval = Evaluate(km, testCluster);
                if (eval < bestEval)
                {
                    Array.Copy(testCluster, cluster, cluster.Length);
                    bestEval = eval;
                }
            }
            //MyKmeans km = new MyKmeans(afinityMatrix, partitions, euclidianDistance);
            //km.centroids = new double[partitions][];
            //for (int i = 0; i < km.centroids.Length; i++)
            //    km.centroids[i] = afinityMatrix[nextIndex[i]];
            //cluster = km.Compute();  //Compute(this.distancesMatrix);
        }
        else
    {
            KMeans km = new KMeans(partitions, angular_dist);
            for (int i = 0; i < iterations; i++)
            {
                km.Randomize(this.afinityMatrix);
                testCluster = km.Compute(afinityMatrix);
                eval = Evaluate(km, testCluster);
                if (eval < bestEval)
                {
                    Array.Copy(testCluster, cluster, cluster.Length);
                    bestEval = eval;
                }
            }
            //MyKmeans km = new MyKmeans(afinityMatrix, partitions, euclidianDistance);
            //km.centroids = new double[partitions][];
            //for (int i = 0; i < km.centroids.Length; i++)
            //    km.centroids[i] = afinityMatrix[nextIndex[i]];
            //cluster = km.Compute();  //Compute(this.distancesMatrix);
        }
    }
    private double Evaluate(KMeans km, int[] labels)
    {
        double eval = 0;
        for (int i = 0; i < km.Clusters.Count; i++)
        {
            for (int j = 0; j < afinityMatrix.GetLength(0); j++)
            {
                if (labels[j] == i)
                    eval += angular_dist(afinityMatrix[j], km.Clusters[i].Mean);
            }
        }
        return eval;
    }
    public void Segment(int partitions = DEFAULT_CLUSTER_AMOUNT, int iterations = DEFAULT_KMEANS_ITERATION, double angular=0, double geodesic=0,int K=0)
    {
        //build_dual_graph(Math.Max((int)Math.Log(this.CountFace, 2.0), 3));
        //build_dual_graph(Math.Max((int)(0.01 * (double)this.CountFace), 3));
        if (buildedGraph==null || buildedGraph.Item1 != Distance || this.afinityMatrix==null || buildedGraph.Item2!=K || buildedGraph.Item3!=angular || buildedGraph.Item4!=geodesic)
        {
            int tam = K;
            if (K == 0)
            {
                tam = (int)(0.1 * (double)CountFace);
                while (tam > 600)
                    tam /= 10;
                if (tam < 10)
                    tam = 10;
            }
            build_dual_graph(tam, false, angular, geodesic);
            this.K = tam;
            buildedGraph = new Tuple<DistanceType, int,double,double>(Distance,K,angular,geodesic);
        }
        kMeans(partitions, iterations);
        this.partitions = partitions;
    }
    public double compareWith(int[] s)
    {
        return 1 - RandIndex(this.cluster, s);
    }
    private List<int> numDif(int[] s)
    {
        bool[] recorded = new bool[s.Length+1];
        List<int> dif = new List<int>();
        for (int i = 0; i < s.Length; i++)
            if (!recorded[s[i]])
            {
                recorded[s[i]] = true;
                dif.Add(s[i]);
            }
        return dif;
    }
    private int[,] crossTab(int[]s1,int[] s2)
    {
        int counter;
        List<int> dif1 = numDif(s1);
        List<int> dif2 = numDif(s2);
        int[,] result = new int[dif1.Count, dif2.Count];
        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                counter = 0;
                for (int k = 0; k < Math.Min(s1.Length,s2.Length); k++)
                    if (s1[k] == dif1[i] && s2[k] == dif2[j])//asumimos que los elementos empiezan en 0
                        counter++;
                result[i, j] = counter;
            }
        }
        return result;
    }
    private double RandIndex(int[] s1, int[] s2)
    {
        int[,] cross = crossTab(s1, s2);
        double n=0, nis=0, njs=0, t1=0, t2=0, t3=0, sumaI;
        double[] sumaJ = new double[cross.GetLength(1)];
        for (int i = 0; i < cross.GetLength(0); i++)
        {
            sumaI = 0;
            for (int j = 0; j < cross.GetLength(1); j++)
            {
                n += cross[i, j];
                sumaI += cross[i, j];
                sumaJ[j] += cross[i, j];
                t2 += Math.Pow(cross[i, j], 2); 
            }
            nis += Math.Pow(sumaI, 2);
        }
        for (int i = 0; i < sumaJ.Length; i++)
            njs += Math.Pow(sumaJ[i], 2);
        t1 = (n * (n - 1)) / 2;
        t3 = (nis + njs) / 2;
        return (t1 + t2 - t3) / t1;
    }
    public double euclidianDistance(Vertex v1,Vertex v2)
    {
        return Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2) + Math.Pow(v2.Z - v1.Z, 2));
    }
    public double euclidianDistance(double[] vector1, double[] vector2)
    {
        double dist = 0;
        if (vector1.Length != vector2.Length)
            throw new Exception("La distancia euclidiana solo esta definida para vectores con igual dimension");
        for (int i = 0; i < vector1.Length; i++)
        {
            dist += Math.Pow(Math.Abs(vector1[i] - vector2[i]),2);
        }
        return Math.Sqrt(dist);
    }
    public void Vorinoy(int numClust)
    {
        this.partitions = numClust;
        this.cluster = new int[this.CountFace];
        build_dual_graph(10);//porque solo necesito conocer los 10 mas alejados para que sean los clusters
        build_dual_graph(10, true);
        for (int i = 0; i < cluster.Length; i++)
        {
            double dist = double.MaxValue;
            for (int j = 0; j < numClust; j++)
            {
                if (distancesMatrix[i][nextIndex[j]] < dist)
                {
                    dist = distancesMatrix[i][nextIndex[j]];
                    cluster[i] = j;
                }
            }
        }
    }
}