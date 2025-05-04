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
    [SerializeField] private GameObject groundTilePrefab; 
    private float[,] _heightMap;
    [SerializeField] private LayerMask interactableLayerMask;

    


    public void Generate(Vector2Int coord, int seed)
    {
        transform.position = new Vector3(
            coord.x * ChunkSize, 
            0, 
            coord.y * ChunkSize);
        _random = new System.Random(coord.x * 73856093 ^ coord.y * 19349663 ^ seed);
        GenerateGround(coord, seed);
        Physics.SyncTransforms(); 
        GenerateFlora(coord);
    }

    private void GenerateFlora(Vector2Int coord)
    {
        int objectCount = _random.Next(30, 50);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 pos;
            float localX = (float)_random.NextDouble() * ChunkSize;
            float localZ = (float)_random.NextDouble() * ChunkSize;
            Vector3 rayOrigin = new Vector3(
                transform.position.x + localX,  
                heightScale+1f, 
                transform.position.z + localZ);
            int terrainMask = LayerMask.GetMask("Terrain");

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hitInfo, 200f, terrainMask))
            {
                 pos = hitInfo.point;
            }
            else
            {
                continue;
            }
        

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

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        GameObject terrain = new GameObject("Terrain");
        terrain.transform.parent = transform;
        terrain.transform.localPosition = Vector3.zero;
        terrain.AddComponent<MeshFilter>().mesh = mesh;
        terrain.AddComponent<MeshRenderer>().material = terrainMaterial;
        terrain.AddComponent<MeshCollider>().sharedMesh = mesh;
        terrain.layer = LayerMask.NameToLayer("Terrain");
        
    }

    private GameObject SpawnFrom(GameObject[] prefabs, Vector3 position, bool addSizing = false)
    {
        int index = _random.Next(prefabs.Length);
        GameObject obj = Instantiate(prefabs[index], position, Quaternion.Euler(0, _random.Next(0, 360), 0), transform);
        obj.transform.parent = transform;
        if(!addSizing) return obj;
        float randomScaleFactor = Random.Range(1.1f, 2f);
        obj.transform.localScale = new Vector3(randomScaleFactor, randomScaleFactor, randomScaleFactor);
        return obj;
    }
    
    public float GetHeightAt(float localX, float localZ)
    {
        int x0 = Mathf.FloorToInt(localX);
        int x1 = Mathf.CeilToInt(localX);
        int z0 = Mathf.FloorToInt(localZ);
        int z1 = Mathf.CeilToInt(localZ);
    
        x0 = Mathf.Clamp(x0, 0, ChunkSize);
        x1 = Mathf.Clamp(x1, 0, ChunkSize);
        z0 = Mathf.Clamp(z0, 0, ChunkSize);
        z1 = Mathf.Clamp(z1, 0, ChunkSize);
    
        float h00 = _heightMap[x0, z0];
        float h10 = _heightMap[x1, z0];
        float h01 = _heightMap[x0, z1];
        float h11 = _heightMap[x1, z1];
    
        
        float xLerp = localX - x0;
        float zLerp = localZ - z0;
    
        
        float top = Mathf.Lerp(h00, h10, xLerp);
        float bottom = Mathf.Lerp(h01, h11, xLerp);
        return Mathf.Lerp(top, bottom, zLerp);
    }



    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
