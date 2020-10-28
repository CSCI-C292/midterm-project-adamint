using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AsteroidDaemon : MonoBehaviour
{
    [SerializeField] public GameObject _asteroidStopText;
    [SerializeField] GameObject _asteroidPrefab;
    [SerializeField] public GameObject _flappyBird;

    [SerializeField] GameObject _asteroidNumberText;

    private List<GameObject> asteroids = new List<GameObject>();

    private System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("AsteroidInit", 1.0f, 1.0f);
    }

    void AsteroidInit() {
        if (asteroids.Count < 3 && GameState.isGameInProgress) {
            if (random.Next(5) == 0) {
                SpawnAsteroid();
             }
        }
    }

    void SpawnAsteroid() {
        Vector3 birdPosition = _flappyBird.transform.position;

        Vector3 position = new Vector3(birdPosition.x - 3.2f, birdPosition.y - 3, 0.5f);
        GameObject asteroidGo = Instantiate(_asteroidPrefab, position, Quaternion.identity);
        asteroids.Add(asteroidGo);
        _asteroidNumberText.GetComponent<Text>().text = "Asteroids: " + asteroids.Count; 
        Asteroid asteroid = asteroidGo.GetComponent<Asteroid>();
        asteroid.keyCode = (KeyCode) random.Next(97, 122);
        asteroid._asteroidDaemon = this;
    }

    public void RemoveAsteroid(Asteroid asteroid) {
        if (asteroid.gameObject != null) {
            asteroids.Remove(asteroid.gameObject);
            Destroy(asteroid.gameObject);
            _asteroidNumberText.GetComponent<Text>().text = "Asteroids: " + asteroids.Count;
            if (asteroids.Count == 0) {
                _asteroidStopText.SetActive(false);
            } else {
                Asteroid first = asteroids[0].GetComponent<Asteroid>();
                _asteroidStopText.GetComponent<Text>().text = "HIT " + first.keyCode.ToString() + " TO STOP ASTEROID";
            }
        }
    }
}
