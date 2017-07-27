using System;
using System.IO;
using System.Collections.Generic;

public class Vertex
{
    public double X { get; private set; }
    public double Y { get; private set; }
    public double Z { get; private set; }
    public int he { get;  set; }

    public Vertex(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;

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
    public int dst { get; private set; }
    public int mate { get; private set; }

    public Half_Edge(int _dst, int _mate)
    {
        dst = _dst;
        mate = _mate;

    }
}


public class Mesh
{
    double xMax, xMin, yMax, yMin, zMax, zMin;
    public string Name { get; private set; }
    public int CountVertex { get; private set; }

    public int CountFace { get; private set; }

    public int CountEdge { get; private set; }

    private List<Vertex> vertexes;
    private List<Face> faces;
    private List<Half_Edge> edges;

	public Mesh()
	{
        vertexes = new List<Vertex>();
        faces = new List<Face>();
        edges = new List<Half_Edge>();

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

            for (int i = 0; i < CountVertex; i++)
            {
                sLine = "";
                sLine = reader.ReadLine();
                items = sLine.Split(' ');
                AddVertex(toDouble(items[0]), toDouble(items[1]), toDouble(items[2]));

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
                if (items[0] != "3")
                    throw new ArgumentException("Todas las caras deben ser triángulos");
                AddFace(int.Parse(items[1]), int.Parse(items[2]), int.Parse(items[3]));
                
                AddEdge(int.Parse(items[1]), int.Parse(items[2]));
                AddEdge(int.Parse(items[2]), int.Parse(items[3]));
                AddEdge(int.Parse(items[3]), int.Parse(items[1]));

            }

        }

        reader.Close();
        CenterAndScale();

    }

    private void CenterAndScale()
    {
        Vertex gravCenter = new Vertex((xMin + xMax) / 2.0, (yMin + yMax) / 2.0, (zMin + zMax) / 2.0);
        double scaleFactor = 1 / Math.Max(Math.Max(xMax - xMin, yMax - yMin), zMax - zMin);

        for (int i = 0; i < CountVertex; i++)
        {
            vertexes[i] = (vertexes[i] - gravCenter) * scaleFactor;
        }
    }

    private string[] RemoveSpace(string[] p)
    {
        List<string> result = new List<string>();

        foreach (var item in p)
        {
            if (item != "")
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

    private void AddEdge(int dst, int mate)
    {
        edges.Add(new Half_Edge(dst, mate));
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

    private string Cut(string name)
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


}


