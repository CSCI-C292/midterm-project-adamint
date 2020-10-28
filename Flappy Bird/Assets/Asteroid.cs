using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Asteroid : MonoBehaviour
{
    public KeyCode keyCode;
    public AsteroidDaemon _asteroidDaemon;

    // Start is called before the first frame update
    private float velocityMultiplier = 0.055f;
    void Start()
    {
        if (_asteroidDaemon != null && name != "Asteroid") {
            if (!_asteroidDaemon._asteroidStopText.activeSelf) {
                _asteroidDaemon._asteroidStopText.GetComponent<Text>().text = "HIT " + keyCode.ToString() + " TO STOP ASTEROID";
            }
            _asteroidDaemon._asteroidStopText.SetActive(true);           
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.isGameInProgress && name != "Asteroid") {
            Vector3 distanceVec = _asteroidDaemon._flappyBird.transform.position - transform.position;
            distanceVec.Normalize();
            distanceVec *= velocityMultiplier;

            transform.position += distanceVec;

            if (Input.GetKeyDown(keyCode)) {
                velocityMultiplier = 0f;
                _asteroidDaemon.RemoveAsteroid(this);
            }
        } else if (name != "Asteroid") {
            _asteroidDaemon.RemoveAsteroid(this);
        }
    }
}
