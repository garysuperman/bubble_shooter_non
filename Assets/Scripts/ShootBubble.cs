using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBubble : MonoBehaviour
{
    //Forr Bubble Shooting
    private Transform currBubble; //bubble to be fired
    [SerializeField] GameObject bubbleDestinationOutline;
    private bool currBubbleMoving = false;
    private int movePointIndex = -1;
    private Transform targetBubble = null;
    [SerializeField] GameObject bubbles;

    //For Raycast Reflection 
    private int maxReflectionCount = 6;
    private float maxStepDistance = 50f;

    //For Line Renderer
    private List<Vector3> linepath = new List<Vector3>();
    private LineRenderer line;
    [SerializeField] private BubbleScrolling scrolling;

    //

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        
        if (Input.GetMouseButton(0) && !scrolling.IsScolling() && !currBubbleMoving)
        {
            currBubble = this.transform.GetChild(0);
            //translate Mouse Position to World Spacee
            Vector3 target = new Vector3();
            Vector2 mousePos = new Vector2();

            // Get the mouse position
            // Note that the y position from Event is inverted.
            mousePos.x = Input.mousePosition.x;
            mousePos.y = Input.mousePosition.y;
            target = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
            target.z = 0;

            //ensure that the cannon is always pointing at an angle/upwards
            if (target.y < this.transform.position.y+2)
                target.y = this.transform.position.y+2;

            //Rotate Cannon towards mouse
            Vector3 cannonDirection = (target - this.transform.position).normalized;
            Quaternion cannonRotation = Quaternion.LookRotation(cannonDirection);
            this.transform.rotation = cannonRotation;

            Vector3 rayOrigin = this.transform.position;
            Vector3 rayDirection = this.transform.forward;

            //Get Line Renderer Path
            line.enabled = true;
            linepath = new List<Vector3>(); ;
            linepath.Add(rayOrigin);
            linepath = Reflect(rayOrigin + rayDirection * 0.75f, rayDirection, maxReflectionCount-1, linepath);
            
            //Place Tracking bubble
            if(targetBubble != null)
            {
                Bubble targetProperties = targetBubble.GetComponent<Bubble>();
                //Linepath is used as it contains the bubbles ulitmate destination at the end of the list
                Vector3 bubbleDestination = targetProperties.ClosestPosition(linepath[linepath.Count-1]);
                linepath[linepath.Count - 1] = bubbleDestination;
                //places tracking bubble in scene
                bubbleDestinationOutline.SetActive(true);
                bubbleDestinationOutline.transform.position = bubbleDestination;
                targetBubble = null;
            } else bubbleDestinationOutline.SetActive(false);

            //RenderLine
            if (linepath != null && linepath.Count > 1)
            {
                line.positionCount = linepath.Count;
                for (int i = 0; i < linepath.Count; i++)
                {
                    if (linepath[i] != new Vector3())
                    {
                        line.SetPosition(i, linepath[i]);
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && !currBubbleMoving)
        {
            if (bubbleDestinationOutline.activeSelf)
            {
                //confirm that bubble can fire
                currBubbleMoving = true;
                movePointIndex = 0;
            }
            
            line.enabled = false;
            bubbleDestinationOutline.SetActive(false);
        }

        if (currBubbleMoving)
        {
            if (movePointIndex != linepath.Count)
            {
                if (linepath != null)
                {
                    Vector3 pos = currBubble.position;
                    Vector3 dest = linepath[movePointIndex];
                    float moveSpeed = 20;
                    currBubble.transform.position = Vector3.MoveTowards(pos, linepath[movePointIndex], moveSpeed * Time.deltaTime);
                    

                    //remove excess numbers
                    float posX = Mathf.Round(pos.x * 100.0f) * 0.01f;
                    float posY = Mathf.Round(pos.y * 100.0f) * 0.01f;
                    float destX = Mathf.Round(dest.x * 100.0f) * 0.01f;
                    float destY = Mathf.Round(dest.y * 100.0f) * 0.01f;


                    if (posX == destX && posY == destY)
                        movePointIndex++;
                }
                else currBubbleMoving = false;
            }
            else
            {
                //Ball has reached destination
                currBubbleMoving = false;

                //Move Bubble over
                currBubble.transform.SetParent(bubbles.transform);
                currBubble.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

                //Trigger all bubbles that can fall
                Bubble currBubbleProperties = currBubble.GetComponent<Bubble>();
                currBubbleProperties.TriggerNeighboringBubbles(currBubbleProperties.getType());

                //Add new Bubble to cannon

            }
        }

    }

    //Reflect RayCast 
    private List<Vector3> Reflect(Vector3 position, Vector3 direction, int reflectionsRemaining, List<Vector3> linePath)
    {
        string hitName = "";
        Transform hitTransform = null;
        if (reflectionsRemaining == 0)
            return linePath;
        
        Vector3 startingPosition = position;

        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxStepDistance, -5, QueryTriggerInteraction.Ignore))//raycast will ignore triggers
        {
            hitName = hit.transform.name;
            hitTransform = hit.transform;
            direction = Vector3.Reflect(direction, hit.normal);
            position = hit.point;
            //gets position for line renderer
            linePath.Add(position);
        }
        else
        {
            position += direction * maxStepDistance;
            //gets position for line renderer
            linePath.Add(position);
        }
        
        //Debug.DrawLine(startingPosition, position, Color.green);

        if (!hitName.Contains("Bubble")) //This means it hit a bubble if it returns
            Reflect(position, direction, reflectionsRemaining - 1, linePath);
        else targetBubble = hitTransform;

        return linePath; 
    }
}
