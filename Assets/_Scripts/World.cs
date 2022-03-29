using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class World : MonoBehaviour
{
    public int mapSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 100;
    public int waterThreshold = 50;
    public float noiseScale = 0.03f;
    public GameObject chunkPrefab;

    Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
    Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();
    [SerializeField] private GameObject Player;

    private void Start()
    {
     //   StartCoroutine(CheckPlayerPosition());
    }

    IEnumerator CheckPlayerPosition()
    {
   
    while (true)
    {
        chunkDictionary.Values.ToList().ForEach((renderer =>
        {
            if ((renderer.transform.position-Player.transform.position).sqrMagnitude<=3600)
            {
                renderer.gameObject.SetActive(true);
            }
            else
            {
                renderer.gameObject.SetActive(false);
            }
        } ));
        yield return new WaitForSecondsRealtime(0.5f);
    }
    }
    public void GenerateWorld()
    {
        chunkDataDictionary.Clear();
        foreach (ChunkRenderer chunk in chunkDictionary.Values)
        {
            Destroy(chunk.gameObject);
        }
        chunkDictionary.Clear();

        for (int x = 0; x < mapSizeInChunks; x++)
        {
            for (int z = 0; z < mapSizeInChunks; z++)
            {

                ChunkData data = new ChunkData(chunkSize, chunkHeight, this, new Vector3Int(x * chunkSize, 0, z * chunkSize));
                GenerateVoxels(data);
                chunkDataDictionary.Add(data.worldPosition, data);
            }
        }

        foreach (ChunkData data in chunkDataDictionary.Values)
        {
            MeshData meshData = Chunk.GetChunkMeshData(data);
            GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity);
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            chunkDictionary.Add(data.worldPosition, chunkRenderer);
            chunkRenderer.InitializeChunk(data);
            chunkRenderer.UpdateChunk(meshData);

        }
        Player.gameObject.SetActive(true);
    }

        private void GenerateVoxels(ChunkData data)
        {
            var airGapHeight = chunkHeight/ 1.5;
        for (int x = 0; x < data.chunkSize; x++)
        {
            for (int z = 0; z < data.chunkSize; z++)
            {
                int groundPosition =0;
                
                for (int y = 0; y < chunkHeight; y++)
                {
                    BlockType voxelType = BlockType.Dirt;
                    if (y >= chunkHeight-airGapHeight)
                    {
                        voxelType = BlockType.Air;
                    
                    }
                    
                    else if (y == groundPosition)
                    {
                        voxelType = BlockType.Sand;
                    }else 
                    {
                        voxelType = BlockType.Grass_Dirt;

                    }
                    

                    Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);
                }
            }
        }
    }

        public void EditBlock(ChunkData chunkData,ChunkRenderer chunkRenderer,Vector3 worldPos,Vector3 normal,BlockType blockType)
        {
            var pos = GetBlockPos(worldPos,normal);
            print("Block pos "+pos);
            var blockChunkPos = Chunk.GetBlockInChunkCoordinates(chunkData, pos);
            Chunk.SetBlock(chunkData,blockChunkPos,blockType);
            chunkRenderer.UpdateChunk();
            
        }
        
        private Vector3Int GetBlockPos(Vector3 worldPos,Vector3 normal)
        {
            Vector3 pos = new Vector3(
                GetBlockPositionIn(worldPos.x, normal.x),
                GetBlockPositionIn(worldPos.y, normal.y),
                GetBlockPositionIn(worldPos.z, normal.z)
            );

            return Vector3Int.RoundToInt(pos);
        }

        private float GetBlockPositionIn(float pos, float normal)
        {
            if (Mathf.Abs(pos % 1) == 0.5f)
            {
                pos -= (normal / 2);
            }


            return (float)pos;
        }
    internal BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, x, y, z);
        ChunkData containerChunk = null;

        chunkDataDictionary.TryGetValue(pos, out containerChunk);

        if (containerChunk == null)
            return BlockType.Nothing;
        Vector3Int blockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
        return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockInCHunkCoordinates);
    }

    public void SaveMapToFile()
    {

        var json = JsonConvert.SerializeObject(chunkDataDictionary.Values);
      
      File.WriteAllText(Path.Combine("C:/Users/ahmed/OneDrive/Desktop/Maps","voxel.json"),json);



    }

    public void LoadMapFromFile()
    {
        var file = File.ReadAllText(Path.Combine("C:/Users/ahmed/OneDrive/Desktop/Maps", "voxel.json"));
        
         var chunks= JsonConvert.DeserializeObject<List<ChunkData>>(file);
         chunkDataDictionary.Clear();
         foreach (ChunkRenderer chunk in chunkDictionary.Values)
         {
             Destroy(chunk.gameObject);
         }
         chunkDictionary.Clear();
         foreach (var chunk in chunks)
         {
             chunk.worldReference = this;
             chunkDataDictionary.Add(chunk.worldPosition,chunk);
             
         }
         foreach (ChunkData data in chunkDataDictionary.Values)
         {
             MeshData meshData = Chunk.GetChunkMeshData(data);
             GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity);
             ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
             chunkDictionary.Add(data.worldPosition, chunkRenderer);
             chunkRenderer.InitializeChunk(data);
             chunkRenderer.UpdateChunk(meshData);

         }
         Player.gameObject.SetActive(true);

    }

    private void Awake()
    {
        
    }
}
