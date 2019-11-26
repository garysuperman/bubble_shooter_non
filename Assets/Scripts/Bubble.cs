﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private Rigidbody rb;
    private bool activated = false;
    private Vector2[] bubbleNeighborSlots = new Vector2[6];
    /*Bubble Types
        Red     =   1
        Blue    =   2

    */
    [SerializeField] private int bubbleType;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.transform.GetComponent<Rigidbody>();
    }

    public void ActiveGravity()
    {
        activated = true;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
    }

    public Vector3 ClosestPosition(Vector2 prevPosition)
    {
        Vector2 bubblePosition = new Vector2(this.transform.position.x, this.transform.position.y);

        //The bubble might have moved, need to constantly refresh slot positions
        RefreshBubbleNeighborSlots();

        int closest = -1;
        float bestDistance = 0;

        //check which reachable slot is nearest from the cannon 
        for (int x = 0; x < 6; x++)
        {
            if (!Physics.Linecast(bubblePosition, bubbleNeighborSlots[x], -5, QueryTriggerInteraction.Ignore))
                if (bubbleNeighborSlots[x].x <=5 && bubbleNeighborSlots[x].x >= -5)
                {
                    float currDistance = Vector2.Distance(prevPosition, bubbleNeighborSlots[x]);
                    if(closest == -1 || currDistance < bestDistance)
                    {
                        closest = x;
                        bestDistance = currDistance;
                    }

                }
        }

        return bubbleNeighborSlots[closest];
    }

    public void TriggerNeighboringBubbles(int prevType)
    {
        if(prevType == bubbleType)
        {
            //Find atleast 3 connected bubbles
            if(NumOfBubbles(this.transform, bubbleType, 1) >= 3)
            {
                //Get all bubbles to be triggered
                List<Transform> bubbleList = new List<Transform>();
                bubbleList = activateNearbyBubbles(bubbleList);

                for(int x = 0; x < bubbleList.Count; x++)
                {
                    bubbleList[x].GetComponent<Bubble>().ActiveGravity();
                }
            }
        }
    }

    public List<Transform> activateNearbyBubbles(List<Transform> bubbles)
    {
        Debug.Log("in" + this.transform.localPosition);
        if (!isInList(bubbles, this.transform))
        {
            bubbles.Add(this.transform);
            Vector2 bubblePosition = new Vector2(this.transform.position.x, this.transform.position.y);

            RefreshBubbleNeighborSlots();
            for (int x = 0; x < 6; x++)
            {
                Debug.Log("in2 " + x);
                Vector2 rayDirection = (bubbleNeighborSlots[x] - bubblePosition).normalized;
                Ray ray = new Ray(bubblePosition, rayDirection);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1, -5, QueryTriggerInteraction.Ignore))
                {
                    if (hit.transform.name.Contains("Bubble"))
                    {
                        Debug.Log("in3 " + x);
                        //Must be same bubble type and not the prev bubble
                        if (bubbleType == hit.transform.GetComponent<Bubble>().getType())
                            return hit.transform.GetComponent<Bubble>().activateNearbyBubbles(bubbles);
                    }
                }
            }
        }
        return bubbles;
    }

    private bool isInList(List<Transform> bubbleList, Transform bubble)
    {
        for(int x = 0; x < bubbleList.Count; x++)
        {
            if (bubbleList[x].position == bubble.position)
                return true;
        }
        return false;
    }

    public int NumOfBubbles(Transform prevBubble, int prevType, int currNum)
    {
        if (currNum < 3)
        {
            if (prevType == bubbleType)
            {
                Vector2 bubblePosition = new Vector2(this.transform.position.x, this.transform.position.y);
                RefreshBubbleNeighborSlots();
                for (int x = 0; x < 6; x++)
                {
                    Vector2 rayDirection = (bubbleNeighborSlots[x] - bubblePosition).normalized;
                    Ray ray = new Ray(bubblePosition, rayDirection);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 1, -5, QueryTriggerInteraction.Ignore))
                    {
                        if (hit.transform.name.Contains("Bubble"))
                        {
                            //Must be same bubble type and not the prev bubble
                            if (bubbleType == hit.transform.GetComponent<Bubble>().getType() && hit.transform != prevBubble) 
                            {
                                currNum++;
                                if(currNum < 3) //if still not enough, go on
                                    return hit.transform.GetComponent<Bubble>().NumOfBubbles(this.transform, bubbleType, currNum);
                            }
                        }
                    }
                }
            }
        }
        return currNum;
        
    }

    public int getType()
    {
        return bubbleType;
    }

    public bool IsAcitivated()
    {
        return activated;
    }

    public void RefreshBubbleNeighborSlots()
    {
        float currPosX = this.transform.position.x;
        float currPosY = this.transform.position.y;

        //All possible target locations of bubbles
        //Upper Left
        bubbleNeighborSlots[0] = new Vector2(currPosX - 0.5f, currPosY + 1);
        //Upper Right
        bubbleNeighborSlots[1] = new Vector2(currPosX + 0.5f, currPosY + 1);
        //Left
        bubbleNeighborSlots[2] = new Vector2(currPosX + 1, currPosY);
        //Right
        bubbleNeighborSlots[3] = new Vector2(currPosX - 1, currPosY);
        //Lower Left
        bubbleNeighborSlots[4] = new Vector2(currPosX - 0.5f, currPosY - 1);
        //Lower Right
        bubbleNeighborSlots[5] = new Vector2(currPosX + 0.5f, currPosY - 1);
    }
}
