using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomBarDaemon : MonoBehaviour
{
    [SerializeField] GameObject _bottomBarPrefab;

    float x = 0;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnBottomBar", 0, 0.1f);
    }

    void SpawnBottomBar() {
        if (!GameState.isGameOver()) {
            Vector3 vector = new Vector3(x, -4.5f, 0);
            Instantiate(_bottomBarPrefab, vector, Quaternion.identity);
            x += 5.67f;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
