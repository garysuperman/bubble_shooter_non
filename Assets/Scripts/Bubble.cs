using System.Collections;
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

        return bubbleNeighborSlots[closest];
    }

    public void TriggerNeighboringBubbles(int prevType)
    {
        if (prevType != bubbleType) return;

        //Find atleast 3 connected bubbles
        if (NumOfBubbles(this.transform, bubbleType, 1) < 3) return;
        
        //Get all bubbles to be triggered
        List<Bubble> bList = new List<Bubble>();
        if (bList == null) return;

        bList = activateNearbyBubbles(bList);

        for(int x = 0; x < bList.Count; x++)
        {
            Bubble b = bList[x].GetComponent<Bubble>();
            if (b == null) continue;

            b.ActiveGravity();
            //Destroy(bubbleList[x].gameObject);
        }
        
        
    }

    public List<Bubble> activateNearbyBubbles(List<Bubble> bubbles)
    {
        if (bubbles.Contains(this)) return bubbles;
        
        bubbles.Add(this);
        Vector2 bubblePosition = this.transform.position;

        RefreshBubbleNeighborSlots();
        for (int x = 0; x < 6; x++)
        {
            Vector2 rayDirection = (bubbleNeighborSlots[x] - bubblePosition).normalized;
            Ray ray = new Ray(bubblePosition, rayDirection);
            RaycastHit hit;
            bool isHit = Physics.Raycast(ray, out hit, 1, -5, QueryTriggerInteraction.Ignore);

            if (isHit == false) continue;

            // cache to local var to improve readability
            Transform t = hit.transform;
            if (t.name.Contains("Bubble") == false) continue;

            // cache to local var and check for null
            Bubble b = t.GetComponent<Bubble>();
            if (b == null) continue;

            //Must be same bubble type and not the prev bubble               
            //4 and 5 means the bubble above is popping, so its ok to pop that too even if its not the same color
            if (bubbleType == b.getType() || x == 4 || x == 5)
                bubbles = b.activateNearbyBubbles(bubbles);
        }
        return bubbles;
    }

    public int NumOfBubbles(Transform prevBubble, int prevType, int currNum)
    {
        // exit function early and avoid deep nesting
        if (currNum >= 3) return currNum;
        if (prevType != bubbleType) return currNum;

        Vector2 bubblePosition = this.transform.position;
        RefreshBubbleNeighborSlots();
        for (int x = 0; x < 6; x++)
        {
            Vector2 rayDirection = (bubbleNeighborSlots[x] - bubblePosition).normalized;
            Ray ray = new Ray(bubblePosition, rayDirection);
            RaycastHit hit;
            bool isHit = Physics.Raycast(ray, out hit, 1, -5, QueryTriggerInteraction.Ignore);

            if (isHit == false) continue;

            // cache to local var to improve readability
            Transform t = hit.transform;
            if (t.name.Contains("Bubble") == false) continue;

            // cache to local var and check for null
            Bubble b = t.GetComponent<Bubble>();
            if (b == null) continue;

            //Must be same bubble type and not the prev bubble
            if (bubbleType == b.getType() && t != prevBubble)
            {
                currNum++;
                if (currNum < 3) //if still not enough, go on
                    return b.NumOfBubbles(this.transform, bubbleType, currNum);
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
