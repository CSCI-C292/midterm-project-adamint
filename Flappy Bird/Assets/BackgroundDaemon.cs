using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundDaemon : MonoBehaviour
{
    [SerializeField] GameObject speedText;

    float speedUpNumber = 0.02f;
    
    // Start is called before the first frame update
    void Start()
    {
        float interval = 5f;
        InvokeRepeating("SpeedUp", interval, interval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpeedUp() {
        if (GameState.isGameInProgress) {
            GameState.gameState.speedMultiplier += speedUpNumber;
        } else {
            GameState.gameState.speedMultiplier = 1.0f;
        }
        speedText.GetComponent<Text>().text = "Speed: " + GameState.gameState.speedMultiplier + "x";
    }
}
