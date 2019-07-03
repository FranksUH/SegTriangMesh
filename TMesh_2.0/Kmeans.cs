using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TMesh_2._0
{
    public class MyKmeans
    {
        /// <summary>
        /// This class contains a manual implementation of Kmeans algorithm
        /// by now is unused
        /// </summary>

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
                //if (isCentroid[j] != -1)
                //    index = isCentroid[j];
                clusters[j] = index;
            }
        }
        public Tuple<int, bool> UpdateCentoids()//reubica cada centroide
        {
            if (centroids == null)//first init centroids
            {
                throw new Exception("Centroids most be initialized before compute Kmeans");
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
