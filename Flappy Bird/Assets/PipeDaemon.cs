using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeDaemon : MonoBehaviour
{
    [SerializeField] GameObject _downPipePrefab;
    [SerializeField] GameObject _upPipePrefab;

    [SerializeField] public static float pipeXStart = 10;
    [SerializeField] public static float pipeXWidthBetween = 7;

    public static List<float> pipePassingLocations = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnPipes", 0, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnPipes() {
        if (GameState.isGameInProgress) {
            pipeXStart += pipeXWidthBetween;


            float downHeight = Mathf.Sin(Random.Range(0f, (float) Mathf.PI / 2)) * (5f - 2.04f);
            float downPipeY = downHeight -5f;

            Vector3 downPosition = new Vector3(pipeXStart, downPipeY, 0.5f);
            Instantiate(_downPipePrefab, downPosition, Quaternion.identity);
       
            float upPipeY = downPipeY + 7.5f;

            Vector3 upPosition = new Vector3(pipeXStart, upPipeY, 0.5f);
            Instantiate(_upPipePrefab, upPosition, Quaternion.identity);    
                        pipePassingLocations.Add(pipeXStart + 1.5f);
        }
    }
}
