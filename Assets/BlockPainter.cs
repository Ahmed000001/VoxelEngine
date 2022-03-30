using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPainter : MonoBehaviour
{
    private Camera Cam;
    private LayerMask Blockmask;

    private World world;
    [SerializeField] private BrushVoxel brushVoxel;
    private Vector3 midPoint;

    private void Start()
    {
        Cam=Camera.main;
        Blockmask = LayerMask.GetMask("Ground");
        world = FindObjectOfType<World>();
    }


    private void Update()
    {
        RaycastHit hit;
        midPoint = Cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f));

        if (Physics.Raycast(midPoint,transform.forward,out hit,100,Blockmask))
        {

            //var chunkRender = hit.collider.gameObject.GetComponent<ChunkRenderer>();
           //  world.EditBlock(chunkRender.ChunkData,chunkRender,hit.point,hit.normal,blockType);
            //Debug.DrawLine(midPoint,(midPoint )+transform.forward*100,Color.black,1);
            var y = hit.point.y;
            var intpos = Vector3Int.RoundToInt(hit.point);
            brushVoxel.transform.position =new Vector3(intpos.x,y,intpos.z);
            var localPos = brushVoxel.transform.localPosition;
            brushVoxel.transform.localPosition = new Vector3(localPos.x, localPos.y + 0.5f, localPos.z);
            brushVoxel.transform.up = hit.normal;


        }
    }
}
