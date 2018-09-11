using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class rubik : MonoBehaviour {

    public GameObject CubeBlock;

    private IList<GameObject> blocks = new List<GameObject>();

 	// Use this for initialization
	IEnumerator Start () {
        for (var i = 0; i < 27; i++)
        {
            yield return null;
            var instance = Instantiate<GameObject>(CubeBlock);
            instance.name = "Block" + i;
            var x = ((i % 3) / 3.0f) - .3f;
            var y = (((i / 3) % 3) / 3.0f) - .3f;
            var z = (((i / 9) % 3) / 3.0f) -.3f;
            instance.transform.localPosition = new Vector3(x, y, z);
            blocks.Add(instance);
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("q"))
        {
            StartCoroutine("RotateLeftUp");
        }
        else if (Input.GetKeyDown("a"))
        {
            StartCoroutine("RotateLeftDown");
        }

    }

    private IEnumerator RotateLeftDown()
    {
        var rotatingPlane = GameObject.Find("yz1");
        var rotatingBlocks = blocks.Where(b => b.transform.position.x == -0.3f).ToList();

        rotatingBlocks.ForEach(b => b.transform.parent = rotatingPlane.transform);

        for (var i = 0; i <= 60; i++)
        {
            rotatingPlane.transform.Rotate(-90f * Time.deltaTime, 0, 0);
            yield return null;
        }

        blocks.Where(b => b.transform.position.x == -0.3f).ToList().ForEach(b => b.transform.parent = null);
    }

    private IEnumerator RotateLeftUp()
    {
        var rotatingPlane = GameObject.Find("yz1");
        var rotatingBlocks = blocks.Where(b => b.transform.position.x == -0.3f).ToList();

        rotatingBlocks.ForEach(b => b.transform.parent = rotatingPlane.transform);

        for (var i = 0; i <= 60; i++)
        {
            rotatingPlane.transform.Rotate(90f * Time.deltaTime, 0, 0);
            yield return null;
        }

        rotatingBlocks.ForEach(b => b.transform.parent = null);
    }
}
