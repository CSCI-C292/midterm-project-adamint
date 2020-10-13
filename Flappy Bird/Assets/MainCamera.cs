using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public static float _moveSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameState.isGameOver()) {
            transform.position += Vector3.right * Time.deltaTime * _moveSpeed;
        }
    }
}
