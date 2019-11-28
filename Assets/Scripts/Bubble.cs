using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Bubble : MonoBehaviour
{
    private Rigidbody rb;
    private SphereCollider collider;
    private bool activated = false;
    private Vector2[] bubbleNeighborSlots = new Vector2[6];
    /*Bubble Types
        Red     =   1
        Blue    =   2
        Green   =   3
        Orange  =   4
        Purple  =   5
    */
    [SerializeField] private int bubbleType;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.transform.GetComponent<Rigidbody>();
        collider = this.transform.GetComponent<SphereCollider>();
    }

    public void ActiveGravity()
    {
        Vector3 pos = this.transform.position;
        pos.z -= 1;
        this.transform.position = pos;
        activated = true;
        rb.useGravity = true;
        collider.radius = 0.5f;
        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
    }

    public int ClosestPositionIndex(Vector2 prevPosition)
    {
        Vector2 bubblePosition = this.transform.position;

        //The bubble might have moved, need to constantly refresh slot positions
        RefreshBubbleNeighborSlots();

        int closest = -1;
        float bestDistance = 0;

        //check which reachable slot is nearest from the cannon 
        for (int x = 0; x < 6; x++)
        {
            if (Physics.Linecast(bubblePosition, bubbleNeighborSlots[x], -5, QueryTriggerInteraction.Ignore)) continue;

            if (bubbleNeighborSlots[x].x > 5 || bubbleNeighborSlots[x].x < -5) continue;
            
            float currDistance = Vector2.Distance(prevPosition, bubbleNeighborSlots[x]);

            if (closest != -1 && currDistance > bestDistance) continue;

            closest = x;
            bestDistance = currDistance;
        }

        return closest;
    }
    
    public Vector3 getClosestPosition(int index)
    {
        return bubbleNeighborSlots[index];
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
