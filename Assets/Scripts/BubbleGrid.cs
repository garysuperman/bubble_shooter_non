using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    [SerializeField] GameObject[] bubbleTypes = new GameObject[0];
    [SerializeField] int gridLength = 0;
    private List<GameObject[]> bubbleList = new List<GameObject[]>();
    private int lowestPoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        float parY = this.transform.position.y;
        int bIndex;
        for(int y = 0; y < gridLength; y++)
        {
            if(y % 2 == 0)
            {
                bubbleList.Add(new GameObject[11]);
                for (int x = 0; x < 11; x++)
                {
                    bIndex = Random.Range(-1, bubbleTypes.Length);
                    if (bIndex < 0)
                    {
                        bubbleList[y][x] = null;
                        continue;
                    }
                    GameObject newBubble = Instantiate(bubbleTypes[bIndex], new Vector3(x - 5, parY + y, 0), new Quaternion(), this.transform);
                    bubbleList[y][x] = newBubble;
                }
            } else
            {
                bubbleList.Add(new GameObject[10]);
                for (int x = 0; x < 10; x++)
                {
                    bIndex = Random.Range(-1, bubbleTypes.Length);
                    if (bIndex < 0)
                    {
                        bubbleList[y][x] = null;
                        continue;
                    }
                    GameObject newBubble = Instantiate(bubbleTypes[bIndex], new Vector3(x + .5f - 5, parY + y, 0), new Quaternion(), this.transform);
                    bubbleList[y][x] = newBubble;
                }
            }
        }
    }

    public void insertToNewRow(GameObject newBuble, Vector2 dest)
    {
        //cleans input
        dest.x = Mathf.Round(dest.x * 10.0f) * 0.1f;
        dest.y = Mathf.Round(dest.y);
        
        if (dest.y < lowestPoint)// if lower than 0, then it is a new row
        {
            lowestPoint = (int) dest.y; 
            int rowLength;
            int targetIndex;

            if (bubbleList[0].Length == 11) //row is 11 in length
            {
                targetIndex = (int)(dest.x - 0.5f + 5);
                rowLength = 10;
            }
            else//row is 10 in length
            {
                targetIndex = (int)dest.x + 5;
                rowLength = 11;
            }

             bubbleList.Insert(0, new GameObject[rowLength]);

            for (int x = 0; x < rowLength; x++)
            {
                if (x != targetIndex)
                    bubbleList[0][x] = null;
                else
                    bubbleList[0][x] = newBuble;
            }
            //for(int x = 0; x < bubbleList[0].Length; x++)
                //Debug.Log(bubbleList[0][x]);
        } else //add to existing row
        {
            int colIndex = (int) (dest.y + (Mathf.Abs(lowestPoint)));
            int targetIndex;

            if (bubbleList[colIndex].Length == 10) //row is 10 in length
                targetIndex = (int) (dest.x - 0.5f + 5);
            else //row is 11 in length
                targetIndex = (int)dest.x + 5;

            bubbleList[colIndex][targetIndex] = newBuble;
            //for (int x = 0; x < bubbleList[colIndex].Length; x++)
                //Debug.Log(bubbleList[colIndex][x]);
        }
    }

    public void triggerBubbles(Vector2 dest)
    {
        //cleans input
        dest.x = Mathf.Round(dest.x * 10.0f) * 0.1f;
        dest.y = Mathf.Round(dest.y);

        int startY = (int) (dest.y + (Mathf.Abs(lowestPoint)));
        int startX;

        if(bubbleList[startY].Length == 10)
            startX = (int)(dest.x - 0.5f + 5);
        else startX = (int)dest.x + 5;
        //must be connected to atleast 3 of the same type
        Bubble b = bubbleList[startY][startX].GetComponent<Bubble>();
        if (b == null) return;

        List<GameObject> bubbles = new List<GameObject>();
        bubbles = getSameBubbles(startX, startY, b.getType(), bubbles);
        Debug.Log(bubbles.Count);
        if (bubbles.Count >= 3)
        {
            bubbles = new List<GameObject>();
            bubbles = getEligibleBubbles(startX, startY, b.getType(), bubbles, false);
            Debug.Log(bubbles.Count);
            //trigger all connections
            for (int x = 0; x < bubbles.Count; x++)
            {
                b = bubbles[x].GetComponent<Bubble>();
                if (b == null) continue;
                b.ActiveGravity();
            }
        }
    }

    //This will stop at 3
    public List<GameObject> getSameBubbles(int currX, int currY, int type, List<GameObject> sameBubbles)
    {
        if (sameBubbles.Count >= 3) return sameBubbles;
        if (!isValidCoord(currX, currY)) return sameBubbles;
        if (sameBubbles.Contains(bubbleList[currY][currX])) return sameBubbles;
        if (bubbleList[currY][currX] == null) return sameBubbles;
        Bubble b = bubbleList[currY][currX].GetComponent<Bubble>();
        if (b == null) return sameBubbles;
        if (b.getType() != type) return sameBubbles;

        sameBubbles.Add(bubbleList[currY][currX]);

        sameBubbles = getSameBubbles(currX, currY + 1, type, sameBubbles);
        sameBubbles = getSameBubbles(currX - 1, currY, type, sameBubbles);
        sameBubbles = getSameBubbles(currX + 1, currY, type, sameBubbles);
        sameBubbles = getSameBubbles(currX, currY - 1, type, sameBubbles);

        int rowSize = bubbleList[currY].Length;
        if (rowSize == 11)
        {
            sameBubbles = getSameBubbles(currX - 1, currY - 1, type, sameBubbles);
            sameBubbles = getSameBubbles(currX - 1, currY + 1, type, sameBubbles);
        }
        else if (rowSize == 10)
        {
            sameBubbles = getSameBubbles(currX + 1, currY - 1, type, sameBubbles);
            sameBubbles = getSameBubbles(currX + 1, currY + 1, type, sameBubbles);
        }

        return sameBubbles;
    }

    public List<GameObject> getEligibleBubbles(int currX, int currY, int type, List<GameObject> bubbles, bool fromAbove)
    {
        if (!isValidCoord(currX, currY)) return bubbles;
        if (bubbles.Contains(bubbleList[currY][currX])) return bubbles;
        if (bubbleList[currY][currX] == null) return bubbles;
        Bubble b = bubbleList[currY][currX].GetComponent<Bubble>();
        if (b == null) return bubbles;
        if (b.getType() != type && !fromAbove) return bubbles;

        bubbles.Add(bubbleList[currY][currX]);

        bubbles = getEligibleBubbles(currX, currY + 1, type, bubbles, false);
        bubbles = getEligibleBubbles(currX - 1, currY, type, bubbles, false);
        bubbles = getEligibleBubbles(currX + 1, currY, type, bubbles, false);
        bubbles = getEligibleBubbles(currX, currY - 1, type, bubbles, true);

        int rowSize = bubbleList[currY].Length;
        if (rowSize == 11)
        {
            bubbles = getEligibleBubbles(currX - 1, currY - 1, type, bubbles, true);
            bubbles = getEligibleBubbles(currX - 1, currY + 1, type, bubbles, false);
        }
        else if (rowSize == 10)
        {
            bubbles = getEligibleBubbles(currX + 1, currY - 1, type, bubbles, true);
            bubbles = getEligibleBubbles(currX + 1, currY + 1, type, bubbles, false);
        }

        return bubbles;
    }

    public bool isValidCoord(int x, int y)
    {
        if (y < 0 || y >= bubbleList.Count ) return false;
        if (x < 0 || x >= bubbleList[y].Length ) return false;
        
        return true;
    }

    public List<GameObject[]> getGrid()
    {
        return bubbleList;
    }
}
