using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForgetiveServer.Terrain.Gen
{
    public class HeightmapData
    {
        internal float[,] HeightMap;
        public bool ReadOnly;
        
        public HeightmapData(int resolution)
        {
            HeightMap = new float[resolution, resolution];
        }

        public float this[int x, int z]
        {
            get => HeightMap[x, z];
            set
            {
                if (ReadOnly)
                    throw new System.Exception();
                HeightMap[x, z] = value;
            }
        }
    }
}
