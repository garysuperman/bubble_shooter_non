using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Catcher : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] GameObject bubbles;
    [SerializeField] GameObject playAgain;

    //audio
    [SerializeField] private AudioSource coinSound;

    private int score = 0;
    
    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();
        if (bubbles.transform.childCount <= 1)
        {
            playAgain.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        coinSound.Play();
        score+= 50;
        Destroy(col.gameObject);
    }

}
