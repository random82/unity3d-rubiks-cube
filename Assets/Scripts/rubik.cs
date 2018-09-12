using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class rubik : MonoBehaviour {

    public GameObject CubeBlock;

    private IList<GameObject> blocks = new List<GameObject>();

    private bool rotating = false;

 	// Use this for initialization
	IEnumerator Start () {
        for (var i = 0; i < 27; i++)
        {
            yield return null;
            var instance = Instantiate<GameObject>(CubeBlock, transform);
            instance.name = "Block" + i;
            var x = ((i % 3) / 3.0f) - 1/3.0f;
            var y = (((i / 3) % 3) / 3.0f) - 1/3.0f;
            var z = (((i / 9) % 3) / 3.0f) - 1/3.0f;
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
        if (rotating) { yield break; }

        rotating = true;

        var rotatingPlane = GameObject.Find("yz1");
        var rotatingBlocks = blocks.Where(b => b.transform.position.x == -1/3f).ToList();

        rotatingBlocks.ForEach(b => b.transform.parent = rotatingPlane.transform);

        for (var i = 0; i <= 60; i++)
        {
            rotatingPlane.transform.Rotate(-90f * Time.deltaTime, 0, 0);
            yield return null;
        }

        blocks.Where(b => b.transform.position.x == -0.3f).ToList().ForEach(b => b.transform.parent = transform);

        rotating = false;
    }

    private IEnumerator RotateLeftUp()
    {
        if (rotating) { yield break; }

        rotating = true;

        var rotatingPlane = GameObject.Find("yz1");
        var rotatingBlocks = blocks.Where(b => b.transform.position.x == -1/3f).ToList();

        rotatingBlocks.ForEach(b => b.transform.parent = rotatingPlane.transform);

        var currentRot = rotatingPlane.transform.eulerAngles;
        var targetRot = rotatingPlane.transform.eulerAngles;
        targetRot.x = Mathf.MoveTowardsAngle(targetRot.x, targetRot.x + 90f, 90f);

        while (Mathf.Abs(currentRot.x - targetRot.x) > 1)
        {
            rotatingPlane.transform.Rotate(90f * Time.deltaTime,0,0);
            currentRot = rotatingPlane.transform.eulerAngles;
            Debug.LogFormat("Target {0}, current {1}", targetRot.x, rotatingPlane.transform.rotation.eulerAngles);
            yield return null;
        }

        if(rotatingPlane.transform.rotation.x != targetRot.x)
        {
            rotatingPlane.transform.eulerAngles = targetRot;
        }

        rotatingBlocks.ForEach(b => b.transform.parent = transform);

        rotating = false;
    }
}
