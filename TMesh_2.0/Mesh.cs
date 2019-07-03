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
using System.Diagnostics;

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
    private List<Face>[] cones;
    private Dictionary<Tuple<int, int>, int> indexOf;
    public AABBTree aabb;
    private Stopwatch crono;
    public double[,] dual_graph_cost { get; private set; }//just adyacents(Nx3)
    public double [,] combined_cost { get; private set; }
    public double[][] distancesMatrix { get; private set; }//NxK
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
        crono = new Stopwatch();

        xMax = -1;
        xMin = -1;
        yMax = -1;
        yMin = -1;
        zMax = -1;
        zMin = -1;
	}

    #region File Handle
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
    #endregion

    #region General methods for segment
    public void build_dual_graph(int dim1,bool includeInShadow, bool getAll = false, double angular = 0, double geodesic = 0)
    {
        #region Initialize
        this.dual_graph_cost = new double[this.CountFace, 3];//each face has at most 3 adyacents
        this.dual_graph_edges = new int[this.CountFace, 3];
        this.cones = new List<Face>[this.CountFace];

        if (Distance == DistanceType.Combined1)
        {
            this.combined_cost = new double[this.CountFace, 3];
            this.distancesMatrixCombined = new double[this.CountFace][];
        }
        this.distancesMatrix = new double[this.CountFace][];
        this.afinityMatrix = new double[this.CountFace][];
        for (int i = 0; i < this.CountFace; i++)
        {
            cones[i] = new List<Face>();
            distancesMatrix[i] = new double[dim1];
            if (Distance == DistanceType.Combined1)            
                distancesMatrixCombined[i] = new double[dim1];
            afinityMatrix[i] = new double[dim1];
        }
        #endregion
        #region Precalc
        if (Distance == DistanceType.Volumetric || (Distance == DistanceType.Combined2 && (angular+geodesic) < 1))//Get all cones
        {
            for (int i = 0; i < CountFace; i++)
                cones[i] = getCone(i,includeInShadow);
        }
        for (int i = 0; i < this.CountFace; i++)
        {            
            List<int> adj = adjacent_faces(this.faces[i]);
            for (int j = 0; j < adj.Count; j++)
            {
                dual_graph_edges[i, j] = adj[j];
                if (dual_graph_cost[i, j] == 0)
                {
                    int indexInAdj = adjacent_faces(faces[adj[j]]).FindIndex(x=>x==i);

                    switch (Distance)
                    {
                        case DistanceType.Angular:
                            {
                                double cost = angular_dist(this.faces[i], this.faces[adj[j]]);
                                dual_graph_cost[i, j] = cost;
                                dual_graph_cost[adj[j], indexInAdj] = cost;
                                break;
                            }
                        case DistanceType.Geodesic:
                            {
                                double cost = geodesic_dist(i, adj[j]);
                                dual_graph_cost[i, j] = cost;
                                dual_graph_cost[adj[j], indexInAdj] = cost;
                                break;
                            }
                        case DistanceType.Volumetric:
                            {
                                double cost = volumetricDist(i, adj[j]);
                                dual_graph_cost[i, j] = cost;
                                dual_graph_cost[adj[j], indexInAdj] = cost;
                                break;
                            }
                        case DistanceType.Combined1:
                            {
                                double angCost = angular_dist(this.faces[i], this.faces[adj[j]]);
                                double geodesicCost = geodesic_dist(i, adj[j]);
                                dual_graph_cost[i, j] = angCost;
                                combined_cost[i, j] = geodesicCost;
                                dual_graph_cost[adj[j], indexInAdj] = angCost;
                                combined_cost[adj[j], indexInAdj] = geodesicCost;
                                break;
                            }
                        case DistanceType.Combined2:
                            {
                                double cost = (angular_dist(this.faces[i], this.faces[adj[j]]) * angular) + (geodesic_dist(i, adj[j]) * geodesic);
                                if ((1 - angular - geodesic) > 0)
                                    cost += (volumetricDist(i, adj[j]) * (1 - angular - geodesic));
                                dual_graph_cost[i, j] = cost;
                                dual_graph_cost[adj[j], indexInAdj] = cost;
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            for (int j = adj.Count; j < 3; j++)//rellenar con -1 los que no estan
                dual_graph_edges[i, j] = -1;        
        }
        #endregion
        #region Dijkstra
        if (!getAll)
        {
            #region Get cost to all faces and get the test
            Random r = new Random();
            int firstIndex = r.Next(0, dim1); 
            double[] cost = dijkstra(firstIndex);
            this.nextIndex = new int[dim1];
            this.nextIndex[firstIndex] = 0;
            double farthest = double.MinValue;//Max(Min(each col))
            double[] nearthest = new double[this.CountFace];//Min of each row
            for (int j = 0; j < cost.Length; j++)//column 0
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
                for (int j = 0; j < cost.Length; j++)//update min of each row
                {
                    if (cost[j] < nearthest[j])
                        nearthest[j] = cost[j];
                    this.distancesMatrix[j][i] = cost[j];
                }
                if ((i + 1) < dim1)//choose max
                {
                    farthest = nearthest[0];//get next face
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
                    this.distancesMatrixCombined[j][0] = cost2[j];
                }
                for (int i = 1; i < dim1; i++)
                {
                    cost2 = dijkstra2(nextIndex[i]);
                    for (int j = 0; j < cost2.Length; j++)
                    {
                        if (cost2[j] < nearthest2[j])
                            nearthest2[j] = cost2[j];
                        this.distancesMatrixCombined[j][i] = cost2[j];
                    }
                    if ((i + 1) < dim1)
                    {
                        farthest2 = nearthest2[0];
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
        k_coeficient /= (k * afinityMatrix.Length);
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
    private void kMeans(int partitions = DEFAULT_CLUSTER_AMOUNT, int iterations = DEFAULT_KMEANS_ITERATION)
    {
        cluster = new int[this.CountFace];
        int[] testCluster = new int[cluster.Length];
        double eval = double.MaxValue, bestEval = double.MaxValue;
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
            // Uncoment this to use manual Kmeans

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
            // Uncoment this to use manual Kmeans

            //MyKmeans km = new MyKmeans(afinityMatrix, partitions, euclidianDistance);
            //km.centroids = new double[partitions][];
            //for (int i = 0; i < km.centroids.Length; i++)
            //    km.centroids[i] = afinityMatrix[nextIndex[i]];
            //cluster = km.Compute();  //Compute(this.distancesMatrix);
        }
    }
    public int Segment(bool includeInShadow, int partitions = DEFAULT_CLUSTER_AMOUNT, int iterations = DEFAULT_KMEANS_ITERATION, double angular = 0, double geodesic = 0, int K = 0)
    {
        crono.Reset();
        crono.Start();
        if (buildedGraph == null || buildedGraph.Item1 != Distance || this.afinityMatrix == null || buildedGraph.Item2 != K || buildedGraph.Item3 != angular || buildedGraph.Item4 != geodesic)
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
            build_dual_graph(tam, includeInShadow, false, angular, geodesic);
            this.K = tam;
            buildedGraph = new Tuple<DistanceType, int, double, double>(Distance, K, angular, geodesic);
        }
        kMeans(partitions, iterations);
        this.partitions = partitions;
        crono.Stop();
        return (int)(crono.ElapsedMilliseconds / 1000);
    }
    
    #endregion

    #region To calculate volumetric distance
    private List<Face> getCone(int i,bool includeInShadow,int theta=60)
    {
        if (aabb == null)
            aabb = new AABBTree(faces, vertexes, getleafSize());
        return GetVisibility(i,includeInShadow, theta);
    }
    #endregion

    #region To calculate and test geodesic distance
    public double geodesic_dist(int j, int i)
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
        #region Para rotar y trasladar T2
        Vector vn1 = new Vector(vertexes[common2].X - vertexes[common1].X, vertexes[common2].Y - vertexes[common1].Y, vertexes[common2].Z - vertexes[common1].Z);
        vn1.normalize();
        Vector wn1 = new Vector(vertexes[notCommun].X - vertexes[common1].X, vertexes[notCommun].Y - vertexes[common1].Y, vertexes[notCommun].Z - vertexes[common1].Z);
        Vector aux = vn1.copy();
        aux.multiply(aux.scalar_product(wn1));
        wn1.elements[0] = wn1.elements[0] - aux.elements[0];
        wn1.elements[1] = wn1.elements[1] - aux.elements[1];
        wn1.elements[2] = wn1.elements[2] - aux.elements[2];
        wn1.normalize();
        TMesh_2._0.Matrix t0 = new TMesh_2._0.Matrix(4, 4);
        t0.elements[0, 0] = 1;
        t0.elements[1, 1] = 1;
        t0.elements[2, 2] = 1;
        t0.elements[0, 3] = -1 * vertexes[common1].X;
        t0.elements[1, 3] = -1 * vertexes[common1].Y;
        t0.elements[2, 3] = -1 * vertexes[common1].Z;
        t0.elements[3, 3] = 1;
        TMesh_2._0.Matrix r1 = new TMesh_2._0.Matrix(4, 4);
        r1.elements[0, 0] = vn1.elements[0];
        r1.elements[0, 1] = vn1.elements[1];
        r1.elements[0, 2] = vn1.elements[2];
        r1.elements[1, 0] = wn1.elements[0];
        r1.elements[1, 1] = wn1.elements[1];
        r1.elements[1, 2] = wn1.elements[2];
        Vector toGetNormal = new Vector(vertexes[common2].X - vertexes[common1].X, vertexes[common2].Y - vertexes[common1].Y, vertexes[common2].Z - vertexes[common1].Z);
        toGetNormal = toGetNormal.vectorial_product(new Vector(vertexes[notCommun].X - vertexes[common1].X, vertexes[notCommun].Y - vertexes[common1].Y, vertexes[notCommun].Z - vertexes[common1].Z));
        toGetNormal.normalize();
        r1.elements[2, 0] = toGetNormal.elements[0];
        r1.elements[2, 1] = toGetNormal.elements[1];
        r1.elements[2, 2] = toGetNormal.elements[2];
        r1.elements[3, 3] = 1;
        TMesh_2._0.Matrix toProyect = new TMesh_2._0.Matrix(4, 1);
        toProyect.elements[0, 0] = vertexes[notCommun].X;
        toProyect.elements[1, 0] = vertexes[notCommun].Y;
        toProyect.elements[2, 0] = vertexes[notCommun].Z;
        toProyect.elements[3, 0] = 1;
        TMesh_2._0.Matrix m1 = r1.matricialProd(t0);
        TMesh_2._0.Matrix projection2 = m1.matricialProd(toProyect);

        Vector L = new Vector(vertexes[common2].X - vertexes[common1].X, vertexes[common2].Y - vertexes[common1].Y, vertexes[common2].Z - vertexes[common1].Z);
        Vertex baricenter2 = new Vertex((L.norm() + projection2.elements[0, 0]) / 3, projection2.elements[1, 0] / 3, 0);
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
        TMesh_2._0.Matrix r2 = new TMesh_2._0.Matrix(4, 4);
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
        TMesh_2._0.Matrix toProyect2 = new TMesh_2._0.Matrix(4, 1);
        toProyect2.elements[0, 0] = vertexes[notCommun2].X;
        toProyect2.elements[1, 0] = vertexes[notCommun2].Y;
        toProyect2.elements[2, 0] = vertexes[notCommun2].Z;
        toProyect2.elements[3, 0] = 1;
        TMesh_2._0.Matrix m2 = r2.matricialProd(t0);
        TMesh_2._0.Matrix projection1 = m2.matricialProd(toProyect2);
        if (projection1.elements[1, 0] * projection2.elements[1, 0] > 0)//cambio el <
            projection1.elements[1, 0] *= -1;
        Vertex baricenter1 = new Vertex((L.norm() + projection1.elements[0, 0]) / 3, projection1.elements[1, 0] / 3, 0);
        #endregion
        double interception = L.norm() / 3 + (projection1.elements[1, 0] * projection2.elements[0, 0] - projection2.elements[1, 0] * projection1.elements[0, 0]) / (3 * (projection1.elements[1, 0] - projection2.elements[1, 0]));
        if (interception >= 0 && interception <= L.norm())
        {
            Vector v = new Vector(baricenter1.X - baricenter2.X, baricenter1.Y - baricenter2.Y, baricenter1.Z - baricenter2.Z);
            return v.norm();
        }
        else
        {
            if (interception < 0)
            {
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
    public double testGeodesic(Face t1, Face t2, List<Vertex> v)
    {
        this.vertexes = v;
        this.faces = new List<Face>();
        faces.Add(t1);
        faces.Add(t2);
        return geodesic_dist(0, 1);
    }
    #endregion

    #region To calculate angular distance
    public double angular_dist(Face f1, Face f2)
    {
        return angular_dist(GetNormalToFace(f1), GetNormalToFace(f2));
    }
    public double angular_dist(Vector v1, Vector v2)
    {
        return (1 - v1.cos(v2)) / 2;
    }
    public double angular_dist(double[] a, double[] b)//for K-means
    {
        double scalarProd = 0, normA = 0, normB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            scalarProd += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }
        normA = Math.Sqrt(normA);
        normB = Math.Sqrt(normB);
        return (1 - (scalarProd / (normA * normB))) / 2;
    }
    #endregion

    #region Auxiliar methods for segment
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
        a = euclidianDistance(vertexes[faces[i].i], vertexes[faces[i].j]);
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
    public double euclidianDistance(Vertex v1, Vertex v2)
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
            dist += Math.Pow(Math.Abs(vector1[i] - vector2[i]), 2);
        }
        return Math.Sqrt(dist);
    }
    #endregion

    #region to calculate volumetric distance
    public List<Tuple<double[],double[]>> Getbbx()
    {
        if (aabb == null)
            aabb = new AABBTree(faces, vertexes, getleafSize());
        return aabb.GetBB();
    }
    public double volumetricDist(int i,int j)//return the volumetric dist betwen i-face and j-face
    {
        double communArea=0, combinedArea=0;        
        List<Face> inside = cones[i];
        bool[] catched = new bool[CountFace];//1 si la n-esima cara es visible por la cara i
        for (int k = 0; k < inside.Count; k++)
        {
            catched[inside[k].index] = true;
            combinedArea += AreaFace(inside[k].index);
        }
        List<Face> inside2 = cones[j];
        for (int k = 0; k < inside2.Count; k++)
        {
            double area = AreaFace(inside2[k].index);
            if (!catched[k])
                combinedArea += area;
            else communArea += area;
        }
        if (combinedArea == 0)
            return 1;
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
        //    //for (int j = 0; j < inside.Count; j++)//ya no hace falta
        //    //    good[inside[j].index] = false;
        //}
        #endregion
        #region Raw Testing
        for (int i = 0; i < insideCone.Count - 1; i++)//para eliminar los que resultan en la sombra de alguien
        {
            if (!good[insideCone[i].index])
                continue;
            for (int j = i + 1; j < insideCone.Count; j++)
            {
                if (!good[insideCone[j].index])
                    continue;
                Vector sureInside = Baricenter(insideCone[i]);
                Vector testingInside = Baricenter(insideCone[j]);
                if (AABBTree.pointInsideShadow(c, insideCone[i], new Vertex(sureInside.elements[0], sureInside.elements[1], sureInside.elements[2]), new Vertex(testingInside.elements[0], testingInside.elements[1], testingInside.elements[2]), this.vertexes))
                    good[insideCone[j].index] = false;
            }
        }
        #endregion        
        for (int j = 0; j < insideCone.Count; j++)
        {
            if (!good[insideCone[j].index])
                continue;
            if (normal.scalar_product(GetNormalToFace(insideCone[j])) > 0)
            {
                good[insideCone[j].index] = false;
            }
        }
        List<Face> result = new List<Face>();
        for (int i = 0; i < CountFace; i++)
        {
            if (good[i] && normal.scalar_product(GetNormalToFace(faces[i])) <= 0)
            {
                result.Add(FacesAt(i));
            }
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
        return faces.Count / 20;
        //return Math.Min(Math.Max(faces.Count / 100, 1), 50);
    }
    public List<Face> TestVisibility(int i, bool getInShadow, double theta=60)
    {
        crono.Restart();
        crono.Start();
        aabb = new AABBTree(faces, vertexes, getleafSize());
        //var n = GetNormalToFace(faces[i]);        
        //List<Face>result = aabb.InsideCone(theta, Baricenter(i),n, faces, vertexes);
        //var x = RemoveHiddenFaces(theta, FacesAt(i), n, result);
        //crono.Stop();
        //var time = crono.ElapsedMilliseconds / 1000;
        //return result;
        //return x;
        //return result;
        //return aabb.InsideCone(theta, Baricenter(i), GetNormalToFace(faces[i]), faces, vertexes);
        return GetVisibility(i,getInShadow ,theta);
    }
    public List<Face> GetVisibility(int i, bool includeInShadow,double theta = 120)
    {
        //List<Face> result = new List<Face>();
        Vector n = GetNormalToFace(faces[i]);
        List<Face> result = new List<Face>();
        #region Raw Cone calcule
        //for (int j = 0; j < CountFace; j++)
        //{
        //    var b = Baricenter(faces[j]);
        //    if (AABBTree.pointInsideCone(theta, Baricenter(i), n, new Vertex(b.elements[0], b.elements[1], b.elements[2])))
        //        result.Add(faces[j]);
        //}
        #endregion
        result = aabb.InsideCone(theta, Baricenter(i), n, faces, vertexes);
        #region For false positive
        //for (int j = 0; j < result.Count; j++)
        //{
        //    //if (!good[insideCone[j].index])
        //    //    continue;
        //    if (n.scalar_product(GetNormalToFace(result[j])) > 0)
        //    {
        //        result.RemoveAt(j);
        //        //good[insideCone[j].index] = false;
        //    }
        //}
        #endregion
        if(!includeInShadow)
            result = RemoveHiddenFaces(theta, FacesAt(i), n, result);//tambien elimina las falsas intercepciones
        return result;
    }
    #endregion

    #region Auxiliar methods to the form
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
    private string Cut(string name)//elimina la extension del archivo
    {
        int i = name.Length - 1;
        while (name[i] != '.')
            i--;
        return name.Substring(0, i);
    }
    private static double toDouble(string number)
    {
        int i = number.Length - 1;

        while (i >= 0 && number[i] != '.')
            i--;

        return (i >= 0) ? double.Parse(number) / (Math.Pow(10.0, number.Length - 1 - i)) : double.Parse(number);
    }
    #endregion

    #region class extend methods

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
    #endregion
    
    #region To Comapare Seg
    public double compareWith(int[] s,bool randIndex=true,int numCloster=-1)
    {
        if(randIndex)
            return 1 - RandIndex(this.cluster, s);
        return JackardIndex(this.cluster, s, numCloster);
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
    /// <summary>
    /// to evaluate quality of the segmentation
    /// </summary>
    /// <param name="km"></param>
    /// <param name="labels"></param>
    /// <returns>
    /// The sum of the distance of all element of a group to its centroid
    /// </returns>
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
    private Tuple<int, int> getMatching(double[][] m, bool[] r, bool[] c)
    {
        double max = double.MinValue;
        int row = -1, col = -1;
        for (int i = 0; i < r.Length; i++)
        {
            for (int j = 0; j < c.Length; j++)
            {
                if (!r[i] && !c[j] && m[i][j] > max)
                {
                    max = m[i][j];
                    row = i;
                    col = j;
                }
            }
        }
        return new Tuple<int, int>(row, col);
    }
    private double JackardIndex(int[] s1, int[] s2,int numClusters)
    {
        double[][] communAreas = new double[numClusters][];
        for (int i = 0; i < numClusters; i++)
            communAreas[i] = new double[numClusters];
        double[] totalAreas1 = new double[numClusters];
        double[] totalAreas2 = new double[numClusters];
        for (int i = 0; i < s1.Length; i++)
        {            
            double area = AreaFace(i);
            totalAreas1[s1[i]] += area;
            totalAreas2[s2[i]] += area;
            communAreas[s1[i]][s2[i]] += area; 
        }
        int[] matchings = new int[numClusters];
        bool[] rows = new bool[numClusters], cols = new bool[numClusters];
        for (int i = 0; i < numClusters; i++)
        {
            var m = getMatching(communAreas, rows, cols);
            rows[m.Item1] = true;
            cols[m.Item2] = true;
            matchings[m.Item1] = m.Item2;
        }
        double cuadsSum = 0;
        for (int i = 0; i < numClusters; i++)
        {
            double c = communAreas[i][matchings[i]];
            double t1 = totalAreas1[i];
            double t2 = totalAreas2[matchings[i]];
            cuadsSum += Math.Pow(1 - (c /(t1+t2-c)), 2);
        }
        //To put matchings clusters of the same color
        int[] auxList = new int[matchings.Length];
        for (int i = 0; i < matchings.Length; i++)
        {
            auxList[matchings[i]] = i;
        }
        for (int i = 0; i < s2.Length; i++)
        {
            s2[i] = auxList[s2[i]];
        }
        return (1 / (Math.Sqrt(numClusters))) * Math.Sqrt(cuadsSum);
    }
    #endregion    
}