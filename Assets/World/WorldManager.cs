using System.Collections.Generic;
using UnityEngine;

namespace World
{
    
    public class WorldManager : MonoBehaviour
    {
        public Transform player;
        public int viewDistance = 3;
        public int seed = 12345;
        public GameObject chunkPrefab;
        private Dictionary<Vector2Int, GameObject> loadedChunks = new();


        void Update()
        {
            Vector2Int playerCoord = new Vector2Int(
                Mathf.FloorToInt(player.position.x / Chunk.ChunkSize),
                Mathf.FloorToInt(player.position.z / Chunk.ChunkSize)
            );
            HashSet<Vector2Int> neededCoords = new();

            // Load chunks in range
            for (int x = -viewDistance; x <= viewDistance; x++)
            {
                for (int z = -viewDistance; z <= viewDistance; z++)
                {
                    Vector2Int coord = playerCoord + new Vector2Int(x, z);
                    neededCoords.Add(coord);
                    if (!loadedChunks.ContainsKey(coord))
                    {
                        GameObject chunk = Instantiate(chunkPrefab);
                        chunk.GetComponent<Chunk>().Generate(coord, seed);
                        loadedChunks.Add(coord, chunk);
                    }
                }
            }
            List<Vector2Int> toRemove = new();
            foreach (var kvp in loadedChunks)
            {
                if (!neededCoords.Contains(kvp.Key))
                {
                    Destroy(kvp.Value);
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var coord in toRemove)
            {
                loadedChunks.Remove(coord);
            }
        }

    }
}