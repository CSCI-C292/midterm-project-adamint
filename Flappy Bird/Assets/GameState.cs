using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameState : MonoBehaviour
{
    public LeaderboardData leaderboardData;
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

    [SerializeField] GameObject _scoreGameObject;

    [SerializeField] GameObject _leaderboardWrapperGameObject;
    [SerializeField] GameObject _leaderboardGameObject;

    [SerializeField] GameObject _leaderboardEntryGameObject;

    [SerializeField] GameObject _addToLeaderboardWrapper;

    public List<Sprite> uiSprites;

    public static GameState gameState;

    private GameObject _addToLeaderboardImp;

    // Start is called before the first frame update
    void Start()
    {
        leaderboardData = new LeaderboardData();
        leaderboardData.GetFromJson();
        leaderboardData.SaveToJson();

        gameState = this;
        _endUIGameObject.SetActive(false);
        GameState.gameState._scoreGameObject.SetActive(true);

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
        gameState._hundredsScoreObjectEnd.SetActive(true);
        GameState.gameState._leaderboardWrapperGameObject.SetActive(true);
        PopulateLeaderboard();
    }

    public static void PopulateLeaderboard() {
        GameState gameState = GameState.gameState;

        List<LeaderboardEntry> topLeaderboard = gameState.leaderboardData.entries
            .OrderByDescending(entry => entry.Score)
            .Take(5)
            .ToList();
        
        float y = gameState._leaderboardEntryGameObject.transform.position.y;
        float height = 29f;
        float yTransform = 0f;
        var position = 1;
        foreach (var entry in topLeaderboard) {
            Vector3 vector = new Vector3(0, 98.595f, 0);
            GameObject leaderboardEntry = Instantiate(gameState._leaderboardEntryGameObject, vector, Quaternion.identity, gameState._leaderboardGameObject.transform);
            leaderboardEntry.SetActive(true);
            leaderboardEntry.transform.position = new Vector3(163, 160 + 98.595f - yTransform, 0);
            leaderboardEntry.transform.Find("NumberInLeaderboard").gameObject.GetComponent<Text>().text = "#" + position;
            leaderboardEntry.transform.Find("EntryName").gameObject.GetComponent<Text>().text = entry.Username;
            leaderboardEntry.transform.Find("EntryScore").gameObject.GetComponent<Text>().text = entry.Score.ToString();

            //leaderboardEntry.transform.position += Vector3.down * yTransform;
            yTransform += height;
            position++;
        }

        Vector3 addToLeaderboardVector = new Vector3(0, 98.595f, 0);
        GameObject addToLeaderboard = Instantiate(gameState._addToLeaderboardWrapper, addToLeaderboardVector, Quaternion.identity, gameState._leaderboardGameObject.transform);
        addToLeaderboard.SetActive(true);
        addToLeaderboard.transform.position = new Vector3(163, 160 + 98.595f - yTransform, 0);
        gameState._addToLeaderboardImp = addToLeaderboard;
    }

    public void setScore(int score) {
        if (score > 999) setGameOver();

        GameState.score = score;
        if (GameState.isGameOver()) {
            _endUIGameObject.SetActive(true);
            _hundredsScoreObject.SetActive(false);
            _tensScoreObject.SetActive(false);
            _onesScoreObject.SetActive(false);


            Invoke("SetEndScore", 0.1f);
        } else {
            setScore(_hundredsScoreObject, _tensScoreObject, _onesScoreObject);
        }
    }

    void SetEndScore() {
        setScore(_hundredsScoreObjectEnd, _tensScoreObjectEnd, _onesScoreObjectEnd); 
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
            Debug.Log(ignored);
        }
    }

    public static void RestartGame() {
        score = 0;
        isGameInProgress = false;
        isGameStarted = false;
        PipeDaemon.pipeXStart = 10;
        PipeDaemon.pipePassingLocations.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void AddCurrentScoreToLeaderboard() {
        GameObject inputObject = gameState._addToLeaderboardImp
            .gameObject.transform.Find("UsernameLeaderboardInput").gameObject;
        string name = inputObject.GetComponent<InputField>().text;
        if (name.Length == 0) return;

        gameState.leaderboardData.entries.Add(new LeaderboardEntry { Username = name, Score = GameState.score});
        gameState.leaderboardData.SaveToJson();

        inputObject.GetComponent<InputField>().text = "";
        RestartGame();
    }
}
