using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TMesh_2._0
{
    [Serializable]
    public class StatSettings
    {
        public int numComp { get; set; }
        public int numImages { get; set; }
        public int numSeg { get; set; }
        public const string SETTINGS_PATH = "../../../Stadistics/settings.bin";
        public const string INFO_PATH = "../../../Stadistics/Info/";
        public const string IMAGES_PATH = "../../../Stadistics/Images/";
        public const string SEGS_PATH = "../../../Stadistics/Segmentations/";
        public const string COMPS_PATH = "../../../Stadistics/Comparisons/";


        public StatSettings()
        {
            this.numComp = 0;
            this.numImages = 0;
            this.numSeg = 0;
        }

        public void _Serialize()
        {
            BinaryFormatter bf = new BinaryFormatter();
            if (File.Exists(SETTINGS_PATH))
                File.Delete(SETTINGS_PATH);
            Stream setting = new FileStream(SETTINGS_PATH, FileMode.Create);
            bf.Serialize(setting, this);
            setting.Close();
        }

        public static StatSettings _Deserialize()
        {
            BinaryFormatter bf = new BinaryFormatter();
            if (!File.Exists(SETTINGS_PATH))
                return new StatSettings();
            Stream setting = new FileStream(SETTINGS_PATH, FileMode.OpenOrCreate);
            if (setting.Length == 0)
                return new StatSettings();
            var result = (StatSettings)bf.Deserialize(setting);
            setting.Close();
            return result;
        }
    }
}
