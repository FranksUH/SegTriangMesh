using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMesh_2._0
{
    public class _Kmeans
    {
        public double[][] myData { get; private set; }
        public double[][] centroids { get; set; }
        public int[] clusters { get; private set; }
        public int numClusters { get; private set; }
        private Func<double[], double[], double> distance;
        public double cost { get; private set; }
        public _Kmeans(double[][]data, int numCLusters, Func<double[], double[], double> distance)
        {
            this.myData = (double[][])data.Clone();
            this.centroids = new double[numCLusters][];
            for (int i = 0; i < numCLusters; i++)
                centroids[i] = new double[data[0].Length];
            this.clusters = new int[data.Length];
            this.distance = distance;
            this.numClusters = numCLusters;
        }
        //revisar
        public void randomizeCentroids()
        {
            SortedSet<int> options = new SortedSet<int>();
            for (int i = 0; i < clusters.Length; i++)
                options.Add(i);
            Random r = new Random();
            for (int i = 0; i < numClusters; i++)
            {
                var pos = r.Next(options.Count);
                var selected = options.ElementAt(pos);
                centroids[i] = (double[])myData[selected].Clone();//AQUIIIIIIIIIIIIIIIIIIIIIIIII
                for (int j = 0; j < options.Count; j++)
                {
                    var pibo = options.ElementAt(j);
                    if (distance(myData[pibo], myData[selected]) < 0.5)
                        options.Remove(j);
                }
                options.Remove(selected);
            }
        }

        public double compute()
        {
            double newCost = 99999;
            double previousCost = 999888;
            int iterations = 50;
            while (iterations > 0 && Math.Abs(newCost - previousCost) > 0.001)
            {
                previousCost = newCost;
                newCost = updateGroups();
                if (newCost > previousCost)
                    throw new Exception("Something whre wrong Kmeans not converg");
                updateCentroids();
                iterations--;
            }
            //while (updateGroupsSecondPhase())
            //    updateCentroids();
            return newCost;
        }

        public double updateGroups()
        {
            double cost = 0;
            int cluster = 0;
            double bestCost = 99999;
            for (int i = 0; i < myData.Length; i++)
            {
                bestCost = 99999;
                for (int j = 0; j < numClusters; j++)
                {
                    var actual = distance(myData[i], centroids[j]);
                    if (actual < bestCost)
                    {
                        bestCost = actual;
                        cluster = j;
                    }
                }
                clusters[i] = cluster;
                cost += bestCost;
            }
            return cost;
        }

        public void updateCentroids()
        {
            double[][] componentsTotals = new double[numClusters][];
            int[] numElements = new int[numClusters];
            for (int i = 0; i < numClusters; i++)
                componentsTotals[i] = new double[myData[0].Length];
            for (int i = 0; i < myData.Length; i++)
            {
                int cluster = clusters[i];
                numElements[cluster]++;
                for (int j = 0; j < myData[0].Length; j++)
                    componentsTotals[cluster][j] += myData[i][j];
            }
            for (int i = 0; i < centroids.Length; i++)
            {
                if (numElements[i] == 0)
                    throw new Exception("Fusioned groups");
                for (int j = 0; j < centroids[i].Length; j++)
                    centroids[i][j] = componentsTotals[i][j]/numElements[i];
            }            
        }

        public bool updateGroupsSecondPhase()
        {
            double bestCost = 99999;
            int newCLuster = 0;
            for (int i = 0; i < myData.Length; i++)
            {
                bestCost = 99999;
                for (int j = 0; j < numClusters; j++)
                {
                    var actual = distance(myData[i], centroids[j]);
                    if (actual < bestCost)
                    {
                        bestCost = actual;
                        newCLuster = j;
                    }
                }
                if (newCLuster != clusters[i])
                {
                    clusters[i] = newCLuster;
                    return true;
                }
            }
            return false;
        }
    }
}
