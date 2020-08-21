using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Drawing.Imaging;

namespace TMesh_2._0
{
    public class Stats
    {
        private StatSettings ss;

        public Stats()
        {
            ss = StatSettings._Deserialize();
        }
        public void Save_seg_as_image(Bitmap bmp)
        {            
            bmp.Save(StatSettings.IMAGES_PATH + ss.numImages + ".bmp", ImageFormat.Bmp);
            ss.numImages++;
            ss._Serialize();
        }
        public void save_seg_as_seg(int[] seg)
        {
            FileStream fs = new FileStream(StatSettings.SEGS_PATH + ss.numSeg + ".seg", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            for (int i = 0; i < seg.Length; i++)
                sw.WriteLine(seg[i]);
            sw.Close();
            fs.Close();
            ss.numSeg++;
            ss._Serialize();
        }
        public void save_seg_info(string name, string metric, int numClust,double time, int numFace, int k)
        {
            FileStream fs = new FileStream(StatSettings.INFO_PATH + ss.numSeg + ".txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("name: " + name);
            sw.WriteLine("metric: " + metric);
            sw.WriteLine("numClust: " + numClust);
            sw.WriteLine("time: " + time);
            sw.WriteLine("numFaces: " + numFace);
            sw.WriteLine("k: " + k);
            sw.Close();
            fs.Close();
        }
        public void save_comparison_result(double randResult, double jaccardResult)
        {
            FileStream fs = new FileStream(StatSettings.COMPS_PATH + ss.numComp + ".txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(randResult);
            sw.WriteLine(jaccardResult);
            sw.Close();
            fs.Close();
            ss.numComp++;
            ss._Serialize();
        }
    }
}
