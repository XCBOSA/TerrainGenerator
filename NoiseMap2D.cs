using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ForgetiveServer.Terrain.Gen
{
    public class NoiseMap2D
    {
        public float[,] Map;
        public int MapSize;

        internal NoiseMap2D() { }

        public NoiseMap2D(int mpsize)
        {
            MapSize = mpsize;
            Map = new float[mpsize, mpsize];
        }

        public static NoiseMap2D Open(string file)
        {
            BinaryFormatter foramtter = new BinaryFormatter();
            FileStream stream = new FileStream(file,
                FileMode.Open, FileAccess.Read, FileShare.Read);
            NoiseMap2D nm2d = new NoiseMap2D();
            nm2d.Map = (float[,])foramtter.Deserialize(stream);
            stream.Close();
            return nm2d;
        }

        public void Save(string file)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(file,
                FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, Map);
            stream.Close();
        }

        public float this[int x, int y]
        {
            get => Map[x, y];
            set => Map[x, y] = value;
        }
    }
}
