using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPainter : MonoBehaviour
{
    private Camera Cam;
    private LayerMask Blockmask;

    private World world;
    private void Start()
    {
        Cam=Camera.main;
        Blockmask = LayerMask.GetMask("Ground");
        world = FindObjectOfType<World>();
    }

    public void CastRay(BlockType blockType)
    {
        var midPoint = Cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f));

        RaycastHit hit;
        Debug.DrawLine(midPoint,(midPoint )+transform.forward*100,Color.black,10);
        if (Physics.Raycast(midPoint,transform.forward,out hit,100,Blockmask))
        {
            print("Ground Hit");

            var chunkRender = hit.collider.gameObject.GetComponent<ChunkRenderer>();
            world.EditBlock(chunkRender.ChunkData,chunkRender,hit,blockType);

        }

        
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CastRay(BlockType.Air);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            
        }
     
    }
}
