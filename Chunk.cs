using ForgetiveServer.Terrain.Gen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ForgetiveServer.Terrain.Gen
{
    public class Chunk
    {
        public int X, Z;
        public HeightmapData Heights;
        bool _isfinal;

        public bool IsFinal
        {
            get => _isfinal;
            set
            {
                _isfinal = value;
                Heights.ReadOnly = value;
            }
        }

        public Vector2 RealPosition
        {
            get
            {
                return new Vector2(X * Scene.ChunkRealSize, Z * Scene.ChunkRealSize);
            }
        }

        public TerrainScene Scene;

        public Chunk(TerrainScene scene, int x, int z)
        {
            Scene = scene;
            Heights = new HeightmapData(Scene.HeightMapResolution);
            X = x;
            Z = z;
        }
    }
}
