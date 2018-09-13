using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rubik : MonoBehaviour {

    private struct BlockTransform
    {
        public GameObject Block;
        public Transform Parent;
        public Vector3 Position;
    }

    private GameObject blockHolder;

    private IList<GameObject> blocks;

    private bool rotating = false;

    private static readonly Dictionary<char, Vector3> rotations = new Dictionary<char, Vector3> { 
                                                                                                    { 'x', Vector3.left }, 
                                                                                                    { 'y', Vector3.up }, 
                                                                                                    { 'z', Vector3.forward }
                                                                                                };

    // Use this for initialization
	void Start() 
    {
        blockHolder = GameObject.Find("Blocks");
        blocks = GameObject.FindGameObjectsWithTag("Block");
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (rotating)
            return;

        var positive = Input.GetKey("left shift") || Input.GetKey("right shift");

        if (Input.GetKeyDown("q"))
        {
            StartCoroutine(Rotate("x1", positive));
        }
        else if (Input.GetKeyDown("w"))
        {
            StartCoroutine(Rotate("x2", positive));
        }
        else if (Input.GetKeyDown("e"))
        {
            StartCoroutine(Rotate("x3", positive));
        }
        if (Input.GetKeyDown("a"))
        {
            StartCoroutine(Rotate("y1", positive));
        }
        else if (Input.GetKeyDown("s"))
        {
            StartCoroutine(Rotate("y2", positive));
        }
        else if (Input.GetKeyDown("d"))
        {
            StartCoroutine(Rotate("y3", positive));
        }
        if (Input.GetKeyDown("z"))
        {
            StartCoroutine(Rotate("z1", positive));
        }
        else if (Input.GetKeyDown("x"))
        {
            StartCoroutine(Rotate("z2", positive));
        }
        else if (Input.GetKeyDown("c"))
        {
            StartCoroutine(Rotate("z3", positive));
        }
    }

    private IEnumerator Rotate(string plane, bool positive)
    {
        rotating = true;

        var rotatingPlane = GameObject.Find(plane);
        
        var rotatingBlocks = new List<BlockTransform>();
        foreach(var block in blocks)
        {
            if (ShouldRotate(rotatingPlane, block))
            {
                rotatingBlocks.Add(new BlockTransform { 
                                                        Block = block, 
                                                        Parent = block.transform.parent, 
                                                        Position = block.transform.position 
                                                      });
                
                block.transform.parent = rotatingPlane.transform;
            }
        }
        
        var totalRotation = 0f;
        while (totalRotation < 90f)
        {
            var currentRotation = 90f * Time.deltaTime * 5;
            
            if (totalRotation + currentRotation > 90f)
                currentRotation = 90f - totalRotation;

            rotatingPlane.transform.Rotate(rotations[plane[0]], currentRotation * (positive ? 1 : -1));
            totalRotation += currentRotation;
            yield return null;
        }

        foreach(var rotating in rotatingBlocks)
        {
            rotating.Block.transform.parent = rotating.Parent;
            rotating.Block.transform.position = Position(rotatingPlane, positive, rotating.Position);
        }

        // reset rotating plane for next rotation
        // rotatingPlane.transform.rotation = Quaternion.Euler(0, 0, 0);
        
        yield return null;
        rotating = false;
    }

    private bool ShouldRotate(GameObject rotatingPlane, GameObject block)
    {
        if (rotatingPlane.name.Contains("x"))
            return Math.Round(rotatingPlane.transform.position.x - block.transform.localPosition.x, 5) == 0;

        if (rotatingPlane.name.Contains("y"))
            return Math.Round(rotatingPlane.transform.position.y - block.transform.localPosition.y, 5) == 0;

        return Math.Round(rotatingPlane.transform.position.z - block.transform.localPosition.z, 5) == 0;
    }

    private Vector3 Position(GameObject rotatingPlane, bool positive, Vector3 position)
    {
        //x = -y, y = x position modifier -> positive rotation
        //x = y, y = -x position modifier -> negative rotation

        var localXMod = positive ? -1 : 1;
        var localYMod = positive ? 1 : -1;

        if (rotatingPlane.name.Contains("x")) //local x = global z, local y = global y
            return new Vector3(position.x, position.z * localYMod, position.y  * localXMod); 

        if (rotatingPlane.name.Contains("y")) // local x = global z, local y = global x
            return new Vector3(position.z * localYMod, position.y, position.x * localXMod);

        return new Vector3(position.y * localXMod, position.x * localYMod, position.z);
    }
}
