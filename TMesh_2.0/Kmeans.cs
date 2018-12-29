using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TMesh_2._0
{
    public class MyKmeans
    {
        private Func<double[], double[], double> distance;
        private int numClusters;
        public double[][] centroids;//numClusters X k, and is a generic point
        public double[][] rawData;
        public int[] clusters;
        public int[] isCentroid;
        public int[] faceCentroid;
        public MyKmeans(double[][] rowData, int numClusters, Func<double[], double[], double> distance)
        {
            this.rawData = rowData;
            this.numClusters = numClusters;
            this.distance = distance;
            this.clusters = new int[rawData.Length];
            this.isCentroid = new int[rawData.Length];
            this.faceCentroid = new int[numClusters];
            for (int i = 0; i < isCentroid.Length; i++)
                isCentroid[i] = -1;
            this.clusters = new int[rawData.Length];
        }
        public void SetSeeds()
        {
            throw new NotImplementedException();
            //this.centroids = init;
        }
        public void RandomizeCentroids()
        {
            List<int> options = new List<int>();
            for (int i = 0; i < rawData.Length; i++)
                options.Add(i);
            centroids = new double[numClusters][];
            Random r = new Random();
            for (int i = 0; i < numClusters; i++)
            {
                int pos = r.Next(0, options.Count);
                centroids[i] = rawData[options[pos]];
                isCentroid[options[pos]] = i;
                faceCentroid[i] = options[pos];
                options.Remove(options[pos]);
            }
            UpdateGroups();
        }
        public int[] Compute()
        {
            //asumimos que los centroides ya fueron inicializados
            int i = 0, top = 100;
            while (true)
            {
                UpdateGroups();
                Tuple<int, bool> result = UpdateCentoids();
                if ((result.Item1 == 0 || !result.Item2 || i >= top) && i > 0)
                    break;
                i++;
            }
            return this.clusters;
        }
        public void NormalizeData()
        {
            double[] sum = new double[rawData[0].Length];//the sum of each index 
            double[] meanSum = new double[rawData[0].Length];
            double[] distanceSum = new double[rawData[0].Length];

            for (int i = 0; i < rawData[0].Length; i++)
            {
                for (int j = 0; j < rawData.Length; j++)
                {
                    sum[i] += rawData[j][i];
                }
                //ya calculada la suma del i-esimo indice
                for (int j = 0; j < rawData.Length; j++)
                {
                    meanSum[i] = sum[i] / rawData.Length;
                }
                for (int j = 0; j < rawData.Length; j++)
                {
                    distanceSum[i] += Math.Pow(rawData[j][i] - meanSum[i], 2);
                }
            }
            for (int i = 0; i < distanceSum.Length; i++)
            {
                distanceSum[i] /= rawData.Length;
            }
            for (int i = 0; i < rawData.Length; i++)
            {
                for (int j = 0; j < rawData[0].Length; j++)
                {
                    rawData[i][j] = (rawData[i][j] - meanSum[j]) / distanceSum[j];
                }
            }
        }
        public void UpdateGroups()//reubica a cada vector en el cluster del centroide mas cercano
        {
            for (int j = 0; j < rawData.Length; j++)
            {
                double minDist = 9999999999999;
                int index = -1;
                double dist;
                for (int i = 0; i < centroids.Length; i++)
                {
                    dist = this.distance(rawData[j], centroids[i]);
                    if (dist <= minDist)
                    {
                        minDist = dist;
                        index = i;
                    }
                }
                if (isCentroid[j] != -1)
                    index = isCentroid[j];
                clusters[j] = index;
            }
        }
        public Tuple<int, bool> UpdateCentoids()//reubica cada centroide
        {
            if (centroids == null)//first init centroids
            {
                centroids = new double[numClusters][];
                for (int i = 0; i < centroids.Length; i++)
                    centroids[i] = new double[rawData[0].Length];
                Random r = new Random();
                centroids[0] = rawData[r.Next(0, rawData.Length)];
                double farthest = double.MinValue;//maximo de los minimos de las columnas
                double[] nearthest = new double[rawData.Length];//minimo de las filas
                double cost;
                for (int j = 0; j < rawData.Length; j++)//columna 0
                {
                    cost = distance(centroids[0], rawData[j]);
                    if (cost > farthest)
                    {
                        farthest = cost;
                        centroids[1] = rawData[j];
                    }
                    nearthest[j] = cost;
                }
                for (int i = 1; i < numClusters; i++)
                {
                    for (int k = 0; k < rawData.Length; k++)
                    {
                        cost = distance(centroids[i], rawData[k]);
                        if (cost < nearthest[k])
                            nearthest[k] = cost;
                    }
                    if ((i + 1) < numClusters)//escoger el maximo de entre todas las filas
                    {
                        farthest = nearthest[0];//obtener la proxima cara
                        for (int j = 1; j < rawData.Length; j++)
                            if (nearthest[j] > farthest)
                            {
                                farthest = nearthest[j];
                                centroids[i + 1] = rawData[j];
                            }
                    }
                }
                return new Tuple<int, bool>(1, true);
            }
            double[][] sum = new double[numClusters][];//llevar de cada grupo la suma de sus componentes
            int[] numItems = new int[numClusters];
            bool changed = false;
            for (int i = 0; i < numClusters; i++)
            {
                sum[i] = new double[rawData[0].Length];
            }
            for (int i = 0; i < rawData.Length; i++)
            {
                numItems[clusters[i]]++;
                for (int j = 0; j < rawData[0].Length; j++)
                    sum[clusters[i]][j] += rawData[i][j];
            }
            for (int i = 0; i < centroids.Length; i++)//revisar
            {
                for (int j = 0; j < rawData[0].Length; j++)
                {
                    //el i-esimo centroide esta en el i-esimo grupo
                    if (centroids[i][j] != (sum[i][j] / numItems[i]))
                    {
                        isCentroid[faceCentroid[i]] = -1;
                        changed = true; //a algun centroide le cambio al menos una componente
                    }
                    centroids[i][j] = sum[i][j] / numItems[i];
                }
            }
            int less = int.MaxValue;
            for (int i = 0; i < numClusters; i++)
            {
                if (numItems[i] < less)
                    less = numItems[i];
            }
            return new Tuple<int, bool>(less, changed);
        }
    }
}
