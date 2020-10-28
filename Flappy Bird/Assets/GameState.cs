using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    public static bool isGameInProgress = false;
    public static bool isGameStarted = false;

    public float speedMultiplier = 1.0f;

    public static int score;

    [SerializeField] GameObject _hundredsScoreObject;
    [SerializeField] GameObject _tensScoreObject;
    [SerializeField] GameObject _onesScoreObject;

    [SerializeField] GameObject _hundredsScoreObjectEnd;
    [SerializeField] GameObject _tensScoreObjectEnd;
    [SerializeField] GameObject _onesScoreObjectEnd;

    [SerializeField] GameObject _endUIGameObject;

    public List<Sprite> uiSprites;

    public static GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        gameState = this;
        _endUIGameObject.SetActive(false);

        uiSprites = new List<Sprite> {
            Resources.Load<Sprite>("sprites/0"),
            Resources.Load<Sprite>("sprites/1"),
            Resources.Load<Sprite>("sprites/2"),
            Resources.Load<Sprite>("sprites/3"),
            Resources.Load<Sprite>("sprites/4"),
            Resources.Load<Sprite>("sprites/5"),
            Resources.Load<Sprite>("sprites/6"),
            Resources.Load<Sprite>("sprites/7"),
            Resources.Load<Sprite>("sprites/8"),
            Resources.Load<Sprite>("sprites/9")
        };
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void startGame() {
        isGameInProgress = true;
        isGameStarted = true;

        GameState.gameState.setScore(0);
        PipeDaemon.pipeXStart = Camera.main.transform.position.x + PipeDaemon.pipeXStart - PipeDaemon.pipeXWidthBetween;
    }

    public static bool isGameOver() {
        return !isGameInProgress && isGameStarted;
    }

    public static void setGameOver() {
        isGameInProgress = false;
        gameState.setScore(score);
    }

    public void setScore(int score) {
        if (score > 999) setGameOver();

        GameState.score = score;
        if (GameState.isGameOver()) {
            _hundredsScoreObject.SetActive(false);
            _tensScoreObject.SetActive(false);
            _onesScoreObject.SetActive(false);

            _endUIGameObject.SetActive(true);

            setScore(_hundredsScoreObjectEnd, _tensScoreObjectEnd, _onesScoreObjectEnd);
        } else {
            setScore(_hundredsScoreObject, _tensScoreObject, _onesScoreObject);
        }
    }

    public static void setScore(GameObject hundreds, GameObject tens, GameObject ones) {
        bool hundredsActive = true;
        bool tensActive = score >= 10;
        bool onesActive = score >= 100;

        hundreds.SetActive(hundredsActive);
        tens.SetActive(tensActive);
        ones.SetActive(onesActive);

        int onesNumber = score % 10;
        int tensNumber = (score / 10) % 10;
        int hundredsNumber = (score / 100) % 10;

        try{
            if (onesActive) {
                hundreds.GetComponent<Image>().sprite = gameState.uiSprites[hundredsNumber];
                tens.GetComponent<Image>().sprite = gameState.uiSprites[tensNumber];
                ones.GetComponent<Image>().sprite = gameState.uiSprites[onesNumber];
            } else if (tensActive) {
                hundreds.GetComponent<Image>().sprite = gameState.uiSprites[tensNumber];
                tens.GetComponent<Image>().sprite = gameState.uiSprites[onesNumber];
            } else {
                hundreds.GetComponent<Image>().sprite = gameState.uiSprites[onesNumber];
            }
        }
        catch (MissingComponentException ignored) {
        }
    }

    public static void RestartGame() {
        score = 0;
        isGameInProgress = false;
        isGameStarted = false;
        PipeDaemon.pipeXStart = 10;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
