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

    private static readonly Dictionary<string, Vector3> rotations = new Dictionary<string, Vector3> { { "yz", Vector3.left }, 
                                                                                                      { "xz", Vector3.up }, 
                                                                                                      { "xy", Vector3.forward }};

    // Use this for initialization
	void Start() {
        blockHolder = GameObject.Find("Blocks");
        blocks = GameObject.FindGameObjectsWithTag("Block");
    }
	
	// Update is called once per frame
	void Update () {

        if (rotating)
            return;

        if (Input.GetKeyDown("q"))
        {
            StartCoroutine(Rotate("yz1", false));
        }
        else if (Input.GetKeyDown("w"))
        {
            StartCoroutine(Rotate("xy1", false));
        }
        else if (Input.GetKeyDown("e"))
        {
            StartCoroutine(Rotate("xz1", false));
        }
        if (Input.GetKeyDown("a"))
        {
            StartCoroutine(Rotate("yz2", false));
        }
        else if (Input.GetKeyDown("s"))
        {
            StartCoroutine(Rotate("xy2", false));
        }
        else if (Input.GetKeyDown("d"))
        {
            StartCoroutine(Rotate("xz2", false));
        }
        if (Input.GetKeyDown("z"))
        {
            StartCoroutine(Rotate("yz3", false));
        }
        else if (Input.GetKeyDown("x"))
        {
            StartCoroutine(Rotate("xy3", false));
        }
        else if (Input.GetKeyDown("c"))
        {
            StartCoroutine(Rotate("xz3", false));
        }
    }

    private IEnumerator Rotate(string plane, bool positive)
    {
        rotating = true;

        var rotatingPlane = GameObject.Find(plane);
        print(rotatingPlane);
        
        var rotatingBlocks = new List<BlockTransform>();
        foreach(var block in blocks)
        {
            print(block.name);
            if (ShouldRotate(rotatingPlane, block))
            {
                rotatingBlocks.Add(new BlockTransform { Block = block, Parent = block.transform.parent, Position = block.transform.position });
                block.transform.parent = rotatingPlane.transform;
            }
        }
        
        var totalRotation = 0f;
        while (totalRotation < 90f)
        {
            var currentRotation = 90f * Time.deltaTime;
            
            if (totalRotation + currentRotation > 90f)
                currentRotation = 90f - totalRotation;

            rotatingPlane.transform.Rotate(rotations[plane.Substring(0,2)], currentRotation * (positive ? 1 : -1));
            totalRotation += currentRotation;
            yield return null;
        }

        foreach(var rotating in rotatingBlocks)
        {
            rotating.Block.transform.parent = rotating.Parent;
            rotating.Block.transform.position = Position(rotatingPlane, rotating.Position);
            // rotating.Block.transform.rotation = rotatingPlane.transform.rotation;
            // rotating.Block.transform.localScale = rotating.Transform.localScale;
        }

        rotatingPlane.transform.rotation = Quaternion.Euler(0, 0, 0);
        yield return null;

        rotating = false;
    }

    private bool ShouldRotate(GameObject rotatingPlane, GameObject block)
    {
        if (!rotatingPlane.name.Contains("x"))
            return Math.Round(rotatingPlane.transform.position.x - block.transform.localPosition.x, 2) == 0;

        if (!rotatingPlane.name.Contains("y"))
            return Math.Round(rotatingPlane.transform.position.y - block.transform.localPosition.y, 2) == 0;

        return Math.Round(rotatingPlane.transform.position.z - block.transform.localPosition.z, 2) == 0;
    }

    private Vector3 Position(GameObject rotatingPlane, Vector3 position)
    {
        if (!rotatingPlane.name.Contains("x"))
            return new Vector3(position.x, -position.z, position.y);

        if (!rotatingPlane.name.Contains("y"))
            return new Vector3(-position.z, position.y, position.x);

        return new Vector3(position.y, -position.x, position.z);
    }
    
    // private IEnumerator RotateLeftUp()
    // {
    //     if (rotating) { yield break; }

    //     rotating = true;

    //     var rotatingPlane = GameObject.Find("yz1");
    //     var rotatingBlocks = blocks.Where(b => b.transform.position.x == -1/3f).ToList();

    //     rotatingBlocks.ForEach(b => b.transform.parent = rotatingPlane.transform);

    //     var currentRot = rotatingPlane.transform.eulerAngles;
    //     var targetRot = rotatingPlane.transform.eulerAngles;
    //     targetRot.x = Mathf.MoveTowardsAngle(targetRot.x, targetRot.x + 90f, 90f);

    //     while (Mathf.Abs(currentRot.x - targetRot.x) > 1)
    //     {
    //         currentRot.x = Mathf.MoveTowardsAngle(currentRot.x, targetRot.x, 90f * Time.deltaTime);
    //         rotatingPlane.transform.eulerAngles = currentRot;
    //         Debug.LogFormat("Target {0}, current {1}", targetRot.x, rotatingPlane.transform.rotation.eulerAngles);
    //         yield return null;
    //     }

    //     if(rotatingPlane.transform.rotation.x != targetRot.x)
    //     {
    //         rotatingPlane.transform.eulerAngles = targetRot;
    //     }

    //     rotatingBlocks.ForEach(b => b.transform.parent = transform);

    //     rotating = false;
    // }
}
