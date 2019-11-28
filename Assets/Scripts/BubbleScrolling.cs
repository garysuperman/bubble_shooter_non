using System.Collections;
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

        if (moveBubbles && bubbles.childCount > 1)
        {
            Vector3 pos = bubbles.position;
            Vector3 target = pos;
            target.y -= 1;
            float moveSpeed = 5;
            bubbles.position = Vector3.MoveTowards(pos, target, moveSpeed*Time.deltaTime);
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
