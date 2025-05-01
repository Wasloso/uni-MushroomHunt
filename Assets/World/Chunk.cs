using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Chunk : MonoBehaviour
{
    public const int ChunkSize = 16;
    private System.Random _random;
    public float heightScale = 2f;
    public float noiseScale = 0.1f;
    public Material terrainMaterial;
    [SerializeField] private GameObject[] treePrefabs;
    [SerializeField] private GameObject[] mushroomPrefabs;
    [SerializeField] private GameObject[] rockPrefabs;
    [SerializeField] private GameObject[] bushPrefabs;
    [SerializeField] private GameObject[] flowerPrefabs;
    [SerializeField] private GameObject[] stumpPrefabs;
    [SerializeField] private GameObject groundTilePrefab; // if using ground variations
    [SerializeField] private float elevationScale = 2f; // how much elevation varies
    private float[,] _heightMap;

    


    public void Generate(Vector2Int coord, int seed)
    {
        _random = new System.Random(coord.x * 73856093 ^ coord.y * 19349663 ^ seed);
        GenerateGround(coord, seed);
        GenerateFlora(coord);
    }

    private void GenerateFlora(Vector2Int coord)
    {
        int objectCount = _random.Next(30, 50);

        for (int i = 0; i < objectCount; i++)
        {
            float localX = _random.Next(0, ChunkSize);
            float localZ = _random.Next(0, ChunkSize);
            float y = GetHeightAt(localX, localZ);
            Vector3 pos = new Vector3(
                coord.x * ChunkSize + localX,
                y,
                coord.y * ChunkSize + localZ
            );

            double roll = _random.NextDouble();

            if (roll < 0.4 && treePrefabs.Length > 0)
            {
                SpawnFrom(treePrefabs, pos,true);
            }
            else if (roll < 0.55 && bushPrefabs.Length > 0)
            {
                SpawnFrom(bushPrefabs, pos);
            }
            else if (roll < 0.7 && mushroomPrefabs.Length > 0)
            {
                SpawnFrom(mushroomPrefabs, pos);
            }
            else if (roll < 0.85 && rockPrefabs.Length > 0)
            {
                SpawnFrom(rockPrefabs, pos);
            }
            else if (roll < 0.95 && flowerPrefabs.Length > 0)
            {
                SpawnFrom(flowerPrefabs, pos);
            }
            else if (stumpPrefabs.Length > 0)
            {
                SpawnFrom(stumpPrefabs, pos);
            }
        }
    }

    private void GenerateGround(Vector2Int coord, int seed)
    {
        int resolution = ChunkSize + 1;
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[ChunkSize * ChunkSize * 6];
        Vector2[] uvs = new Vector2[vertices.Length];
        _heightMap = new float[resolution, resolution];

        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = z * resolution + x;
                float worldX = coord.x * ChunkSize + x;
                float worldZ = coord.y * ChunkSize + z;

                float height = Mathf.PerlinNoise((worldX + seed) * noiseScale, (worldZ + seed) * noiseScale) * heightScale;
                _heightMap[x, z] = height;
                vertices[i] = new Vector3(x, height, z);
                uvs[i] = new Vector2(x / (float)ChunkSize, z / (float)ChunkSize);
            }
        }

        int t = 0;
        for (int z = 0; z < ChunkSize; z++)
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                int i = z * resolution + x;

                triangles[t++] = i;
                triangles[t++] = i + resolution + 1;
                triangles[t++] = i + resolution;

                triangles[t++] = i;
                triangles[t++] = i + 1;
                triangles[t++] = i + resolution + 1;
            }
        }
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i + 1];
            triangles[i + 1] = triangles[i + 2];
            triangles[i + 2] = temp;
        }

        // Build mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        GameObject terrain = new GameObject("Terrain");
        terrain.transform.parent = transform;
        terrain.transform.localPosition = new Vector3(coord.x * ChunkSize, 0, coord.y * ChunkSize);
        terrain.AddComponent<MeshFilter>().mesh = mesh;
        terrain.AddComponent<MeshRenderer>().material = terrainMaterial;
        terrain.AddComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void SpawnFrom(GameObject[] prefabs, Vector3 position, bool addSizing = false)
    {
        int index = _random.Next(prefabs.Length);
        GameObject obj = Instantiate(prefabs[index], position, Quaternion.Euler(0, _random.Next(0, 360), 0), transform);
        
        if(!addSizing) return;
        float randomScaleFactor = Random.Range(1.1f, 2f);
        obj.transform.localScale = new Vector3(randomScaleFactor, randomScaleFactor, randomScaleFactor);

    }

    public float GetHeightAt(float localX, float localZ)
    {
        int x = Mathf.Clamp(Mathf.RoundToInt(localX), 0, ChunkSize);
        int z = Mathf.Clamp(Mathf.RoundToInt(localZ), 0, ChunkSize);
        return _heightMap[x, z];
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
