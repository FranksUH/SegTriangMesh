using System;
using System.Collections.Generic;

public class Vertex
{
    double X { get; private set; }
    double Y { get; private set; }
    double Z { get; private set; }
    int he { get; private set; }

    public Vertex(double x, double y, double z, int _he)
    {
        X = x;
        Y = y;
        Z = z;
        he = _he;

    }
}

public class Face
{
    int i { get; private set; }
    int j { get; private set; }
    int k { get; private set; }

    public Face(int _i, int _j, int _k)
    {
        i = _i;
        j = _j;
        k = _k;

    }
}

public class Half_Edge
{
    int dst { get; private set; }
    int mate { get; private set; }

    public Half_Edge(int _dst, int _mate)
    {
        dst = _dst;
        mate = _mate;

    }
}


public class Mesh
{
    string Name { get; private set; }
    int Count { get; private set; }

    private List<Vertex> vertexes;
    private List<Face> faces;

	public Mesh(string fileName)
	{
        vertexes = new List<Vertex>();
        faces = new List<Face>();
	}

    public void AddVertex(double x, double y, double z)
    {
        vertexes.Add(new Vertex())
    }


}


