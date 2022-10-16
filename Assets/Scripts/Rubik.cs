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

    private int planeNumber;
    private GameObject selectedPlane;

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
        selectedPlane = GameObject.Find("y1");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (rotating)
            return;

        if (Input.GetKeyDown("s"))
        {
            ShiftPlane("y", 1);
        }

        if (Input.GetKeyDown("w"))
        {
            ShiftPlane("y", -1);
        }
        
        else if (Input.GetKeyDown("a"))
        {
            ShiftPlane("z", -1);
        }

        else if (Input.GetKeyDown("d"))
        {
            ShiftPlane("z", 1);
        }
        
        else if (Input.GetKeyDown("q"))
        {
            ShiftPlane("x", -1);
        }
        
        else if (Input.GetKeyDown("e"))
        {
            ShiftPlane("x", 1);
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            StartCoroutine(Rotate(true));
        }

        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            StartCoroutine(Rotate(false));
        }
    }

    private void ShiftPlane(string plane, int increment)
    {
        selectedPlane.GetComponent<Renderer>().enabled = false;

        if (selectedPlane.name[0] != plane[0])
            planeNumber = 0;

        planeNumber += increment;
        
        if (planeNumber > 3) planeNumber = 1;
        if (planeNumber < 1) planeNumber = 3;

        selectedPlane = GameObject.Find(plane + planeNumber);

        selectedPlane.GetComponent<Renderer>().enabled = true;
    }

    private IEnumerator Rotate(bool positive)
    {
        rotating = true;

        var rotatingBlocks = new List<BlockTransform>();
        foreach(var block in blocks)
        {
            if (ShouldRotate(block))
            {
                rotatingBlocks.Add(new BlockTransform { 
                                                        Block = block, 
                                                        Parent = block.transform.parent, 
                                                        Position = block.transform.position 
                                                      });
                
                block.transform.parent = selectedPlane.transform;
            }
        }
        
        print(rotatingBlocks.Count);
        var totalRotation = 0f;
        while (totalRotation < 90f)
        {
            var currentRotation = 90f * Time.deltaTime * Speed;
            
            if (totalRotation + currentRotation > 90f)
                currentRotation = 90f - totalRotation;

            selectedPlane.transform.Rotate(rotations[selectedPlane.name[0]], currentRotation * (positive ? 1 : -1));
            totalRotation += currentRotation;
            yield return null;
        }

        foreach(var rotating in rotatingBlocks)
        {
            rotating.Block.transform.parent = rotating.Parent;
            rotating.Block.transform.position = Position(positive, rotating.Position);
        }

        yield return null;
        rotating = false;
    }

    private bool ShouldRotate(GameObject block)
    {
        if (selectedPlane.name.Contains("x"))
            return Math.Round(selectedPlane.transform.position.x - block.transform.localPosition.x, 5) == 0;

        if (selectedPlane.name.Contains("y"))
            return Math.Round(selectedPlane.transform.position.y - block.transform.localPosition.y, 5) == 0;

        return Math.Round(selectedPlane.transform.position.z - block.transform.localPosition.z, 5) == 0;
    }

    private Vector3 Position(bool positive, Vector3 position)
    {
        //x = -y, y = x position modifier -> positive rotation
        //x = y, y = -x position modifier -> negative rotation

        var localXMod = positive ? -1 : 1;
        var localYMod = positive ? 1 : -1;

        if (selectedPlane.name.Contains("x")) //local x = global z, local y = global y
            return new Vector3(position.x, position.z * localYMod, position.y  * localXMod); 

        if (selectedPlane.name.Contains("y")) // local x = global z, local y = global x
            return new Vector3(position.z * localYMod, position.y, position.x * localXMod);

        return new Vector3(position.y * localXMod, position.x * localYMod, position.z);
    }
}
