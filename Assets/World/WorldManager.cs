using System;
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
        private Vector2Int currentPlayerCoord;
        private const float UpdateThreshold = 0.5f;


        private void Start()
        {
            if (ReferenceEquals(chunkPrefab, null))
            {
                Debug.LogError("Chunk prefab not assigned!");
                enabled = false;
                return;
            }
            if (!chunkPrefab.TryGetComponent(out Chunk _))
            {
                Debug.LogError($"Chunk prefab missing {nameof(Chunk)} component!", chunkPrefab);
                enabled = false;
                return;
            }
            if (ReferenceEquals(player, null))
            {
                player = FindFirstObjectByType<PlayerController>()?.transform; 
        
                if (ReferenceEquals(player, null))
                {
                    Debug.LogError("Player not assigned and couldn't be found in scene! Disabling WorldManager.", this);
                    enabled = false;
                }
            }
            PositionPlayerSafe();
            currentPlayerCoord = GetPlayerChunkCoord();
            UpdateChunks(); 
    
            
        }
        
        private Vector2Int GetPlayerChunkCoord()
        {
            return new Vector2Int(
                Mathf.FloorToInt(player.position.x / Chunk.ChunkSize),
                Mathf.FloorToInt(player.position.z / Chunk.ChunkSize)
            );
        }
        
        void PositionPlayerSafe()
        {
            Vector3 safePosition = new Vector3(
                currentPlayerCoord.x * Chunk.ChunkSize + Chunk.ChunkSize * 0.5f,
                player.position.y, 
                currentPlayerCoord.y * Chunk.ChunkSize + Chunk.ChunkSize * 0.5f
            );
    
            player.position = safePosition;
        }

        void Update()
        {
            Vector2Int newPlayerCoord =GetPlayerChunkCoord();

            if (Vector2Int.Distance(newPlayerCoord, currentPlayerCoord) > UpdateThreshold)
            {
                currentPlayerCoord = newPlayerCoord;
                UpdateChunks();
            }
        }
        
        void UpdateChunks()
        {
            HashSet<Vector2Int> neededCoords = new();
            
            for (int x = -viewDistance; x <= viewDistance; x++)
            {
                for (int z = -viewDistance; z <= viewDistance; z++)
                {
                    Vector2Int coord = currentPlayerCoord + new Vector2Int(x, z);
                    neededCoords.Add(coord);
                    
                    if (!loadedChunks.ContainsKey(coord))
                    {
                        LoadChunk(coord);
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
        
        void LoadChunk(Vector2Int coord)
        {

            GameObject chunk = Instantiate(chunkPrefab);
            Chunk chunkComponent = chunk.GetComponent<Chunk>();
            
            if (!ReferenceEquals(chunkComponent, null))
            {
                
                chunkComponent.Generate(coord, seed);
                loadedChunks.Add(coord, chunk);
                
           
            }
            else
            {
                Debug.LogError("Chunk prefab missing Chunk component!");
                Destroy(chunk);
            }
        }

    }
}