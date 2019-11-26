﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleScrolling : MonoBehaviour
{
    [SerializeField] private Transform bubbles; //The bubbles in the field to be hit
    private bool moveBubbles = true; 
    private int numOfBubblesInStop = 0;

    // Update is called once per frame
    private void Update()
    {
        //only move down bubbles in field if there is no more bubbles at stop
        if (numOfBubblesInStop == 0)
            moveBubbles = true;
        else moveBubbles = false;

        if (moveBubbles)
        {
            Vector3 pos = bubbles.position;
            Vector3 target = pos;
            target.y -= 1;
            bubbles.position = Vector3.Lerp(pos, target, Time.deltaTime);
        }
    }

    private void OnTriggerEnter()
    {
        numOfBubblesInStop++;
    }

    private void OnTriggerExit()
    {
        numOfBubblesInStop--;
    }

    public bool IsScolling()
    {
        return numOfBubblesInStop == 0;
    }
}
