using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private static float _moveSpeed = 3f;

    public static float getMoveSpeed() {
        return _moveSpeed * GameState.gameState.speedMultiplier;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameState.isGameOver()) {
            transform.position += Vector3.right * Time.deltaTime * getMoveSpeed();
        }
    }
}
