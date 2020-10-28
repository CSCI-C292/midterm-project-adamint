using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AsteroidDaemon : MonoBehaviour
{
    [SerializeField] GameObject _asteroidStopText;
    [SerializeField] GameObject _asteroidPrefab;
    [SerializeField] GameObject _flappyBird;

    private GameObject currentAsteroid;
    private int currentKey;

    private System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("AsteroidInit", 1.0f, 1.0f);
    }

    void AsteroidInit() {
        if (currentAsteroid == null) {
            if (random.Next(10) == 0) {
                SpawnAsteroid();
             }
        }
    }

    void SpawnAsteroid() {
        Vector3 birdPosition = _flappyBird.transform.position;

        Vector3 position = new Vector3(birdPosition.x - 10, birdPosition.y - 3, 0.5f);
        currentAsteroid = Instantiate(_asteroidPrefab, position, Quaternion.identity);
    
        currentKey = random.Next(97, 122);

        
    }
}
