using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DestOutlineMat : MonoBehaviour
{
    [SerializeField] private Renderer render;
    [SerializeField] private List<Material> BubbleMatList = new List<Material>();
    /*Bubble Types
        Red     =   1
        Blue    =   2
        Green   =   3
        Orange  =   4
        Purple  =   5
    */

    public void setDestOutlineMat(int bubbleType)
    {
        int index = bubbleType - 1;
        render.material = BubbleMatList[index];
        
    }
}
