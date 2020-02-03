using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ForgetiveServer.Terrain.Gen
{
    public class TerrainScene
    {
        public int HeightMapResolution = 1025;
        public float ChunkRealSize = 1000;
        public int GenSize = 5;
        public int EditPerChunk = 25;

        public int XZMax
        {
            get
            {
                return HeightMapResolution * GenSize;
            }
        }

        public bool XZExists(int x, int z)
        {
            int xzm = XZMax;
            return x >= 0 && x < xzm && z >= 0 && z < xzm;
        }

        public bool XZExists(Vector2Int pos)
        {
            return XZExists(pos.x, pos.y);
        }

        public NoiseMap2D[] NoiseMaps;
        public Chunk[,] Chunks;

        public void Init(NoiseMap2D[] nmaps)
        {
            Chunks = new Chunk[GenSize, GenSize];
            if (nmaps.Length == 0)
                throw new ArgumentException();
            NoiseMaps = nmaps;
        }

        [ExportFunction]
        public Chunk GetChunk(Vector2Int chunkpos)
        {
            return Chunks[chunkpos.x, chunkpos.y];
        }

        public float this[int x, int z]
        {
            get
            {
                int ckx = x / HeightMapResolution,
                    ckz = z / HeightMapResolution,
                    offset_x = x % HeightMapResolution,
                    offset_z = z % HeightMapResolution;
                Chunk selectchk = Chunks[ckx, ckz];
                return selectchk.Heights[offset_z, offset_x];
            }
            set
            {
                int ckx = x / HeightMapResolution,
                    ckz = z / HeightMapResolution,
                    offset_x = x % HeightMapResolution,
                    offset_z = z % HeightMapResolution;
                Chunk selectchk = Chunks[ckx, ckz];
                selectchk.Heights[offset_z, offset_x] = value;
            }
        }

        List<Vector2> GenerateNeighbours(Vector2 pos, int width, int height)
        {
            List<Vector2> neighbours = new List<Vector2>();
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (!(x == 0 && y == 0))
                    {
                        Vector2 nPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width - 1),
                                                    Mathf.Clamp(pos.y + y, 0, height - 1));
                        if (!neighbours.Contains(nPos))
                            neighbours.Add(nPos);
                    }
                }
            }
            return neighbours;
        }

        void htaddp(Vector2Int center, int mapid)
        {
            NoiseMap2D noise = NoiseMaps[mapid];
            int radius = noise.MapSize;
            for (int z = 0; z < radius; z += 2)
            {
                for (int x = 0; x < radius; x += 2)
                {
                    float gw = noise[x, z] * 0.05f;
                    int wx = (z + center.x) / 2, wy = (x + center.y) / 2;
                    if (XZExists(wx, wy))
                        this[wx, wy] += gw;
                }
            }
        }

        public void Gen()
        {
            for (int i = 0; i < GenSize; i++)
            {
                for (int j = 0; j < GenSize; j++)
                {
                    Chunks[i, j] = new Chunk(this, i, j);
                }
            }
            int xzm = XZMax;
            for (int z = 0; z < xzm; z++)
            {
                for (int x = 0; x < xzm; x++)
                {
                    this[z, x] = Mathf.PerlinNoise(x * 0.1f, z * 0.1f) * 0.002f;
                }
            }
            int re_min = -1023;
            int re_max = xzm;
            System.Random sysrd = new System.Random();
            Parallel.For(0, GenSize * GenSize * EditPerChunk, i =>
            {
                htaddp(new Vector2Int(
                    sysrd.Next(re_min, re_max),
                    sysrd.Next(re_min, re_max)),
                    sysrd.Next(0, NoiseMaps.Length));
            });
            for (int y = 0; y < xzm; y++)
            {
                for (int x = 0; x < xzm; x++)
                {
                    float avgHeight = this[x, y];
                    List<Vector2> neighbours = GenerateNeighbours(new Vector2(x, y), xzm, xzm);
                    foreach (Vector2 n in neighbours)
                    {
                        avgHeight += this[(int)n.x, (int)n.y];
                    }
                    this[x, y] = avgHeight / ((float)neighbours.Count + 1);
                }
            }
        }
    }
}
