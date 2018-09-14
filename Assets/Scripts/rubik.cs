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

    public float Speed;

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
        blocks = GameObject.FindGameObjectsWithTag("Block");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (rotating)
            return;

        var shift = Input.GetKey("left shift") || Input.GetKey("right shift");
        LeftHandControls(shift);
        RightHandRotations();
    }

    private void RightHandRotations()
    {
        if (Input.GetKeyDown("y"))
        {
            StartCoroutine(Rotate("z1", true));
        }
        else if (Input.GetKeyDown("n"))
        {
            StartCoroutine(Rotate("z1", false));
        }
        else if (Input.GetKeyDown("u"))
        {
            StartCoroutine(Rotate("z2", true));
        }
        else if (Input.GetKeyDown("m"))
        {
            StartCoroutine(Rotate("z2", false));
        }
        else if (Input.GetKeyDown("i"))
        {
            StartCoroutine(Rotate("z3", true));
        }
        else if (Input.GetKeyDown(","))
        {
            StartCoroutine(Rotate("z3", false));
        }
    }

    private void LeftHandControls(bool shift)
    {
        if (!shift)
        {
            if (Input.GetKeyDown("q"))
            {
                StartCoroutine(Rotate("y1", true));
            }
            else if (Input.GetKeyDown("e") && !shift)
            {
                StartCoroutine(Rotate("y1", false));
            }
            if (Input.GetKeyDown("a"))
            {
                StartCoroutine(Rotate("y2", true));
            }
            else if (Input.GetKeyDown("d") && !shift)
            {
                StartCoroutine(Rotate("y2", false));
            }
            else if (Input.GetKeyDown("z") && !shift)
            {
                StartCoroutine(Rotate("y3", true));
            }
            else if (Input.GetKeyDown("c") && !shift)
            {
                StartCoroutine(Rotate("y3", false));
            }
        }
        else
        {
            if (Input.GetKeyDown("q"))
            {
                StartCoroutine(Rotate("x1", false));
            }
            else if (Input.GetKeyDown("z"))
            {
                StartCoroutine(Rotate("x1", true));
            }
            if (Input.GetKeyDown("w"))
            {
                StartCoroutine(Rotate("x2", false));
            }
            else if (Input.GetKeyDown("x"))
            {
                StartCoroutine(Rotate("x2", true));
            }
            else if (Input.GetKeyDown("e"))
            {
                StartCoroutine(Rotate("x3", false));
            }
            else if (Input.GetKeyDown("c"))
            {
                StartCoroutine(Rotate("x3", true));
            }
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
            var currentRotation = 90f * Time.deltaTime * Speed;
            
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
