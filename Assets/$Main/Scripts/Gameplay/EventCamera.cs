﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCamera : MonoBehaviour
{
    public Canvas canvas;


    private void Start()
    {
        canvas.worldCamera = Camera.main;
    }

}
