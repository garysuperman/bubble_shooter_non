using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBubble : MonoBehaviour
{

    private Transform currentBubble; //bubble to be fired

    //For Raycast Reflection 
    private int maxReflectionCount = 6;
    private float maxStepDistance = 200f;

    //For Line Renderer
    private Vector3[] linepath = new Vector3[5];
    private LineRenderer line;
    [SerializeField] private BubbleScrolling scrolling;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        currentBubble = this.transform.GetChild(0);
        if (Input.GetMouseButton(0) && !scrolling.isScolling())
        {
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

            //Line Renderer Path
            line.enabled = true;
            linepath = new Vector3[6];
            linepath[0] = rayOrigin;
            linepath = reflect(rayOrigin + rayDirection * 0.75f, rayDirection, maxReflectionCount-1, linepath);

            if (linepath != null && linepath.Length > 1)
            {
                line.positionCount = linepath.Length;
                for (int i = 0; i < linepath.Length; i++)
                {
                    if(linepath[i] != null)
                        line.SetPosition(i, linepath[i]);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
            line.enabled = false;
            
    }

    //Reflect RayCast 
    private Vector3[] reflect(Vector3 position, Vector3 direction, int reflectionsRemaining, Vector3[] linePath)
    {
        string hitName = "";

        if (reflectionsRemaining == 0)
            return linePath;
        
        Vector3 startingPosition = position;

        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxStepDistance, -5, QueryTriggerInteraction.Ignore))//raycast will ignore triggers
        {
            hitName = hit.transform.name;
            direction = Vector3.Reflect(direction, hit.normal);
            position = hit.point;
            //gets position for line renderer
            linePath[6 - reflectionsRemaining] = position;
        }
        else
        {
            position += direction * maxStepDistance;
            //gets position for line renderer
            linePath[6 - reflectionsRemaining] = position;
        }
        
        Debug.DrawLine(startingPosition, position, Color.green);

        if (!hitName.Contains("Bubble")) //This means it hit a bubble if it returns
            reflect(position, direction, reflectionsRemaining - 1, linePath);

        return linePath; 
    }
}
