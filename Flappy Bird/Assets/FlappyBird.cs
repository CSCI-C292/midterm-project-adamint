using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyBird : MonoBehaviour
{
    [SerializeField] float _pregameMovementRange;
    [SerializeField] float _gameMovementRange;
    [SerializeField] float _speed;

    [SerializeField] float _gravity;
    [SerializeField] float _flapUpwardsSpeed;
    [SerializeField] float _jumpLength;

    float verticalSpeed = 0;
    float currentJumpFrames = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameState.isGameOver()) {
            transform.position += Vector3.right * Time.deltaTime * MainCamera._moveSpeed;        
        }


        if (Input.GetButtonDown("Fire1")) {
            if (!GameState.isGameStarted) {
                GameState.startGame();
            }

            if (currentJumpFrames == -1) {
                verticalSpeed = _flapUpwardsSpeed;
                currentJumpFrames = 0f;
            }
        }

        if (!GameState.isGameStarted && !GameState.isGameOver()) {
            float y = 0 + Mathf.Sin(Time.time * _speed) * _pregameMovementRange;
            transform.position = new Vector3(transform.position.x, y, 0);            
        }
        else {
           if (currentJumpFrames == _jumpLength) {
               currentJumpFrames = -1;
           }

           if (currentJumpFrames != -1) {
               currentJumpFrames++;
           }

            verticalSpeed -= _gravity;
        }

        if (transform.position.y <= -3.7f) {
            transform.position = new Vector3(transform.position.x, -3.7f, transform.position.z);
        } else {
            transform.position += Vector3.up * verticalSpeed;
        }

        if (PipeDaemon.pipePassingLocations.Count > 0 && transform.position.x > PipeDaemon.pipePassingLocations[0]) {
            PipeDaemon.pipePassingLocations.RemoveAt(0);
            GameState.gameState.setScore(GameState.score + 1);
        }
        

    }

   void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name.Contains("BottomBar") || other.gameObject.name.Contains("Pipe")) {
            GameState.setGameOver();
        }
    }

}
