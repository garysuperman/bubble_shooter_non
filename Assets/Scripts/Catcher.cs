using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Catcher : MonoBehaviour
{
    [SerializeField] Text scoreText;
    private int score = 0;
    
    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
    }

    private void OnTriggerEnter(Collider col)
    {
        score+= 50;
        Destroy(col.gameObject);
    }

}
