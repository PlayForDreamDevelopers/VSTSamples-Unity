using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public Transform[] quads;
    private void Start()
    {
        //QuadManager.instance.CreateQuad(quads);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            QuadManager.instance.AddQuadPoint();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            QuadManager.instance.CreateQuad();
        }
    }
}
