using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushVoxel : MonoBehaviour
{
   private World world;
   private void Awake()
   {
      world = FindObjectOfType<World>();
   }

   public void PaintBlock(BlockType blockType)
   {
      
     // world.EditBlock();
   }
}
