using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class Vector
{
    public List<double> elements;
    public int size { get { return elements.Count; }  private set { } }
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
        for(int i=0;i<this.size;i++)
            this.elements[i] *= scalar;
    }
    public void normalize()
    {
        this.multiply(1 / this.norm());
    }
    public static Vector normal(Vector v1,Vector v2,Vector v3)
    {
        Vector t = new Vector(v1.elements[0] - v3.elements[0], v1.elements[1] - v3.elements[1], v1.elements[2] - v3.elements[2]);
        Vector w = new Vector(v2.elements[0] - v3.elements[0], v2.elements[1] - v3.elements[1], v2.elements[2] - v3.elements[2]);
        t = t.vectorial_product(w);
        t.normalize();
        return t;
    }
}

public class Vertex
{
    public double X { get;  set; }
    public double Y { get;  set; }
    public double Z { get;  set; }
    public int he { get;  set; }
    //---OJO al usar un operador se desactualiza he----------
    public Vertex(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
        he = -1; 
    }

    public static Vertex operator + (Vertex v1, Vertex v2)
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

    public static double Euclidian_Distance(Vertex v1,Vertex v2)
    {
        return Math.Sqrt(Math.Pow(Math.Abs(v1.X-v2.X),2)+Math.Pow(Math.Abs(v1.Y - v2.Y), 2)+Math.Pow(Math.Abs(v1.Z - v2.Z), 2));
    }
}

public class Face
{
    public int i { get; private set; }
    public int j { get; private set; }
    public int k { get; private set; }

    public Face(int _i, int _j, int _k)
    {
        i = _i;
        j = _j;
        k = _k;
    }
}

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

public class Mesh
{
    double xMax, xMin, yMax, yMin, zMax, zMin;
    public string Name { get; private set; }
    public int CountVertex { get; private set; }
    public int CountFace { get; private set; }
    public int CountEdge { get; private set; }

    private const int DEFAULT_KMEANS_ITERATION = 3;
    private const int DEFAULT_CLUSTER_AMOUNT = 3;

    private List<Vertex> vertexes;
    private List<Vertex> dual_verts;
    private List<Face> faces;
    private List<Half_Edge> edges;
    private Dictionary<Tuple<int, int>, int> indexOf;
    public double[][] dual_graph_cost { get; private set; }
    public int[,] dual_graph_edges { get; private set; }
    public int[] cluster;
    public int k;


    public Mesh()
	{
        vertexes = new List<Vertex>();
        faces = new List<Face>();
        edges = new List<Half_Edge>();
        indexOf = new Dictionary<Tuple<int, int>, int>();
        dual_verts = new List<Vertex>();

        xMax = -1;
        xMin = -1;
        yMax = -1;
        yMin = -1;
        zMax = -1;
        zMin = -1;
	}

    public void LoadOFF(string fileName)
    {
        Name = Cut(fileName);

        StreamReader reader = new StreamReader(fileName);
        string sLine = reader.ReadLine();

        if (sLine != "OFF")
            throw new Exception("El formato del fichero no es correcto");
        else 
        {
            sLine = "";
            sLine = reader.ReadLine();
            var items = sLine.Split(' ');

            CountVertex = int.Parse(items[0]);
            CountFace = int.Parse(items[1]);
            k = (int)Math.Log(CountFace);

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
                AddFace(v1, v2, v3);
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
    public void build_dual_graph(int k)
    {
        this.dual_graph_cost = new double[this.CountFace][];//a lo sumo cada cara tendra 3 adyacentes
        this.dual_graph_edges = new int[this.CountFace, 3];

        for (int i = 0; i < this.CountFace; i++)
            dual_graph_cost[i] = new double[k];

        for (int i = 0; i < this.CountFace; i++)
            for (int j = 0; j < k; j++)
            {
                if (i == j)
                    this.dual_graph_cost[i][j] = 0;
                else
                    this.dual_graph_cost[i][j] = double.MaxValue;
            }

        for (int i = 0; i < this.CountFace; i++)
        {
            List<int> adj = adjacent_faces(this.faces[i]);
            for (int j = 0; j < adj.Count; j++)
            {
                dual_graph_edges[i, j] = adj[j];
                //dual_graph_cost[i][adj[j]] = angular_dist(this.faces[i], this.faces[adj[j]]);
            }
            for (int j = adj.Count; j < 3; j++)//rellenar con -1 los que no estan
                dual_graph_edges[i, j] = -1;
        }
        for (int i = 0; i < this.CountFace; i++)//contrur la matriz de ""Afinidad""(distancias)
        {
            double[] cost = dijkstra(i);
            for (int j = 0; j < cost.Length; j++)
            {
                this.dual_graph_cost[i][ j] = cost[j];
            }
        }
    }
    private double[] dijkstra(int source)
    {
        double[] minCost = new double[dual_graph_cost.GetLength(0)];
        SortedSet<Tuple<double, int>> pending = new SortedSet<Tuple<double, int>>();
        for (int i = 0; i < minCost.Length; i++)
        {
            if (i != source)
            {
                minCost[i] = double.MaxValue;
                pending.Add(new Tuple<double, int>(double.MaxValue, i));
            }
            else
            {
                minCost[i] = 0;
                pending.Add(new Tuple<double, int>(0, i));
            }
        }
        while (pending.Count > 0)
        {
            Tuple<double, int> m = pending.First();
            pending.Remove(m);
            for (int i = 0; i < 3 && dual_graph_edges[m.Item2, i] != -1; i++)//recorrer los adyacentes
            {
                int u = m.Item2, v = dual_graph_edges[u, i];
                if (minCost[v] > minCost[u] + dual_graph_cost[u][v])//relax
                {
                    pending.Remove(new Tuple<double, int>(minCost[v], v));
                    minCost[v] = minCost[u] + dual_graph_cost[u][ v];
                    pending.Add(new Tuple<double, int>(minCost[v], v));
                }
            }
        }
        return minCost;
    }
    public double angular_dist(Face f1, Face f2)
    {
        Vector v1 = new Vector(vertexes[f1.i].X - vertexes[f1.j].X, vertexes[f1.i].Y - vertexes[f1.j].Y, vertexes[f1.i].Z - vertexes[f1.j].Z);
        Vector v2 = new Vector(vertexes[f1.j].X - vertexes[f1.k].X, vertexes[f1.j].Y - vertexes[f1.k].Y, vertexes[f1.j].Z - vertexes[f1.k].Z);
        Vector v3 = new Vector(vertexes[f1.k].X - vertexes[f1.i].X, vertexes[f1.k].Y - vertexes[f1.i].Y, vertexes[f1.k].Z - vertexes[f1.i].Z);
        Vector w1 = new Vector(vertexes[f2.i].X - vertexes[f2.j].X, vertexes[f2.i].Y - vertexes[f2.j].Y, vertexes[f2.i].Z - vertexes[f2.j].Z);
        Vector w2 = new Vector(vertexes[f2.j].X - vertexes[f2.k].X, vertexes[f2.j].Y - vertexes[f2.k].Y, vertexes[f2.j].Z - vertexes[f2.k].Z);
        Vector w3 = new Vector(vertexes[f2.k].X - vertexes[f2.i].X, vertexes[f2.k].Y - vertexes[f2.i].Y, vertexes[f2.k].Z - vertexes[f2.i].Z);
        v1 = Vector.normal(v1, v2, v3);
        v2 = Vector.normal(w1, w2, w3);
        return angular_dist(v1, v2);
    }
    public double angular_dist(Vector v1,Vector v2)
    {
        return (1 -v1.cos(v2)) / 2;
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
    private void CenterAndScale()
    {
        Vertex gravCenter = new Vertex((xMin + xMax) / 2.0, (yMin + yMax) / 2.0, (zMin + zMax) / 2.0);
        double scaleFactor = 1 / Math.Max(Math.Max(xMax - xMin, yMax - yMin), zMax - zMin);

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
    private void AddFace(int i, int j, int k)
    {
        faces.Add(new Face(i, j, k));
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
        List<int> centroidPositions = new List<int>();
        bool[] isCentroide = new bool[this.CountFace];
        this.cluster = new int[dual_verts.Count];//para indicar a que cluster pertenece cada cara

        #region Seed centroids
        //for (int i = 0; i < partitions; i++)
        //{
        //    centroidPositions.Add(i);//se escogen como centroide de forma ordenada(pudiera ser al azar!!!!)
        //    cluster[i] = i;
        //    isCentroide[i] = true;
        //}
        centroidPositions.Add(0);
        isCentroide[0] = true;
        for (int i = 1; i < partitions; i++)
        {
            int sel = select(i, centroidPositions);
            if (sel != -1)
            {
                centroidPositions.Add(sel);
                cluster[sel] = i;
                isCentroide[sel] = true;
            }
            else
            {
                partitions = i;
                break;
            }
        }
        #endregion

        for (int i = 0; i < dual_verts.Count; ++i)
        {
            if(!isCentroide[i])
            cluster[i] = i % partitions; //guarda a que grupo pertenece cada triangulo
        }

        for (int i = 0; i < iterations; ++i)
        {
            // Clustering elements O(n * k)
            for (int j = 0; j < dual_verts.Count; ++j)//por cada triangulo ver la distancia a cada centroide!!!!!!!!!!!!!
            {
                if (!isCentroide[j])//pues no quiero que los centroides cambien de grupo(puede generar perdida de grupos)
                {
                    int bestCluster = 0;
                    for (int k = 1; k < centroidPositions.Count; ++k)
                    {                        
                        if (dual_graph_cost[centroidPositions[k]][ j] < dual_graph_cost[centroidPositions[bestCluster]][ j])//why j+1
                        {   // '<' si se usa D y '>' si se usa W
                            bestCluster = k;
                        }
                    }
                    cluster[j] = bestCluster;
                }
            }

            // Updating centrode O(n * k)
            for (int k = 0; k < centroidPositions.Count; ++k)
            {
                Vertex avg = new Vertex(0, 0, 0);//es la aproximacion del centro del cluster
                int total = 0;
                for (int j = 0; j < dual_verts.Count; ++j)//why 1
                {
                    if (cluster[j] == k)
                    {
                        avg = avg + dual_verts[j];
                        ++total;
                    }
                }
                if (total != 0)
                {
                    avg = avg / total;
                }
                double bestValue = double.MaxValue;//Vertex.Euclidian_Distance(dual_verts[0], avg);  //el vertice 0 no tiene por que pertenecer al grupo K!!!!!!
                for (int j = 0; j < dual_verts.Count; ++j)
                {
                    double temp = Vertex.Euclidian_Distance(dual_verts[j], avg);
                    if (cluster[j] == k && temp < bestValue)
                    {
                        isCentroide[centroidPositions[k]] = false;
                        centroidPositions[k] = j;
                        isCentroide[j] = true;
                        bestValue = temp;
                    }
                }
                //double aff = dual_graph_cost[0, centroidPositions[k]];
                //int indice = 0;
                //for (int j = 1; j < dual_verts.Count; ++j)
                //{
                //    if (dual_graph_cost[j, centroidPositions[k]] < aff)
                //    {  //'<' si se usa D y '>' si se usa W
                //        indice = j;
                //        aff = dual_graph_cost[j, centroidPositions[k]];
                //    }
                //}
                //isCentroide[centroidPositions[k]] = false;
                //centroidPositions[k] = indice;
                //isCentroide[indice] = true;
            }
        }
    }
    public void Segment(int partitions = DEFAULT_CLUSTER_AMOUNT, int iterations = DEFAULT_KMEANS_ITERATION)
    {
        if(dual_graph_cost==null)
            build_dual_graph();
        kMeans(partitions, iterations);
        //este metodo permitira elegir que tipo de segmentacion realizar, etc
    }
    public int select(int k, List<int> chosed)
    {
        bool finded;
        for (int i = k; i < dual_verts.Count; i++)
        {
            finded = true;
            for (int j = 0; j < k; j++)
            {
                if (dual_graph_cost[chosed[j]][i] == 0)
                {
                    finded = false;
                    break;
                }
            }
            if (finded)
                return i;
        }
        return -1;
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
}


