using System;
using System.Collections;
using System.Collections.Generic;
using  System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class BrushVoxel : MonoBehaviour
{
   private World world;
   [SerializeField] private List<GameObject> brushes;
   private Dictionary<float, List<GameObject>> brushLevels;
   [SerializeField] private List<float> orderedKeys;
   private void Awake()
   {
      world = FindObjectOfType<World>();
      for (int i = 0; i < transform.childCount; i++)
      {
         brushes.Add(transform.GetChild(i).gameObject);
      }
      brushLevels = new Dictionary<float, List<GameObject>>();
      for (int i = 0; i < brushes.Count; i++)
      {
         var key = brushes[i].transform.localPosition.y;
         if (!brushLevels.ContainsKey(key))
         {
            brushLevels.Add(key, new List<GameObject>() {brushes[i]});
         }
         else
         {
            brushLevels[key].Add(brushes[i]);
         }
      }

      orderedKeys = brushLevels.Keys.ToList();
      orderedKeys.Sort();

   }

   public void RemoveBlocks()
   {
      var groundVoxels = brushLevels[orderedKeys[0]];
         List<ChunkRenderer> chunkRenderers = new List<ChunkRenderer>();
         List<Vector3> hitPoints = new List<Vector3>();
         List<Vector3> hitNormals = new List<Vector3>();
      for (int i = 0; i < groundVoxels.Count; i++)
      {
         RaycastHit hit;
         if (Physics.Raycast(groundVoxels[i].transform.position,-groundVoxels[i].transform.up,out hit,1,LayerMask.GetMask("Ground")))
         {
            var chunkRender = hit.collider.gameObject.GetComponent<ChunkRenderer>();
            hitPoints.Add(hit.point);
            hitNormals.Add(hit.normal);
        
            chunkRenderers.Add(chunkRender);           

            
         }
         
      }

      world.EditBlocksData(chunkRenderers.ToArray(), hitPoints.ToArray(), hitNormals.ToArray(), BlockType.Air);
   }public void AddBlocks(BlockType blockType)
   {
      var groundVoxels = brushLevels[orderedKeys[0]];
         List<ChunkRenderer> chunkRenderers = new List<ChunkRenderer>();
         List<Vector3> hitPoints = new List<Vector3>();
         List<Vector3> hitNormals = new List<Vector3>();
      for (int i = 0; i < groundVoxels.Count; i++)
      {
         RaycastHit hit;
         if (Physics.Raycast(groundVoxels[i].transform.position,-groundVoxels[i].transform.up,out hit,1,LayerMask.GetMask("Ground")))
         {
            var chunkRender = hit.collider.gameObject.GetComponent<ChunkRenderer>();
            hitPoints.Add(groundVoxels[i].transform.position);
            hitNormals.Add(hit.normal);
            chunkRenderers.Add(chunkRender);
         }
         
      }

      world.EditBlocksData(chunkRenderers.ToArray(), hitPoints.ToArray(), hitNormals.ToArray(), blockType);
   }

   private void Update()
   {
      if (Input.GetMouseButtonDown(0))
      {
         AddBlocks(BlockType.Grass_Dirt);
      }
      else if (Input.GetMouseButtonDown(1))
      {
         RemoveBlocks();
      }
   }
   
}

