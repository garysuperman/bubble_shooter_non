using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2[] bubbleNeighborSlots = new Vector2[6];
    private Vector2 currP;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.transform.GetComponent<Rigidbody>();
    }

    public void TurnOnGravity()
    {
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
    }

    public Vector3 ClosestPosition(Vector2 prevPosition)
    {
        Vector2 bubblePosition = new Vector2(this.transform.position.x, this.transform.position.y);

        //The bubble might have moved, need to constantly refresh slot positions
        refreshBubbleNeighborSlots();

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

    public void refreshBubbleNeighborSlots()
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
