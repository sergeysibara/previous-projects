using Shapes;
using UnityEngine;
using System.Collections;
using System.Linq;

public class MainSceneManager : RequiredMonoSingleton<MainSceneManager>
{
    public enum GameMode
    {
        //Loading,
        Normal,
        InvokeAdjuster, //режим вызова монтажника
        Victory
    }

    [SerializeField]
    private bool _needRoomContentGeneration = false;

    [SerializeField]
    private bool _needShapesGeneration = true;

    private GameObject _UIRoot;

    public static GameMode CurrentGameMode
    {
        get { return Instance._currentGameMode; }
        set { Instance._currentGameMode = value; }
    }

    private GameMode _currentGameMode;

    private void Start ()
    {
        EventMessenger.Subscribe(GameEvent.ConnetorSwitchToOn, this, OnConnetorSwitchToOn);

        _UIRoot = GameObject.Find("UI Root");
        if (_UIRoot == null)
            Debug.LogError("UI Root not found");

        if (_needRoomContentGeneration)
        {
            RoomContentGenerator.Generate();
            EventMessenger.SendMessage(GameEvent.CompleteContentGeneration, this);
        }

        if (_needShapesGeneration)
        {
            ShapesGenerator.Generate();
            EventMessenger.SendMessage(GameEvent.CompleteNodesGeneration, this);
        }

        Invoke("StartGameProcess",0.3f);
        //StartGameProcess();   
	}

    //private void OnDrawGizmos()
    //{
    //    for (float i = 30; i >= 0; i-=0.2f)
    //    {
    //        int x = (int) Mathf.Pow(1.2589254117941672104239541063958f, i);
    //        Debug.LogWarning(1000-x);
    //    }
    //}


    private void StartGameProcess()
    {
        if (!Application.loadedLevelName.Contains("Tutorial"))
        {
            var totalScore = PlayerPrefs.GetInt("totalScore");
            var scoreLabel = _UIRoot.transform.FindChild("MainGamePanel/01_spriteForText/02_lblScore");
            scoreLabel.GetComponent<UILabel>().text = totalScore.ToString();
        }

        //Debug.LogWarning("StartGame");
        CurrentGameMode = GameMode.Normal;
        EventMessenger.SendMessage(GameEvent.StartGameProcess, this);
    }

    private void OnConnetorSwitchToOn()
    {
        if (Instance.CheckVictoryCondition())
            Instance.Victory();
    }

    public static void OnShapeRotateStart(Shape shape)
    {
        SignalsUtils.DestroySignalsInCell(shape.Xindex, shape.Yindex);
        if (CurrentGameMode != GameMode.InvokeAdjuster)
            ConnectorsManager.CheckAllConnections();
    }



    private void Victory()
    {
        bool isUsedHelp = (CurrentGameMode == GameMode.InvokeAdjuster);

        //Debug.LogWarning("<color=green>VICTORY</color>");
        CurrentGameMode = GameMode.Victory;
        EventMessenger.SendMessage(GameEvent.EngGameProcess, this);

        if (Application.loadedLevelName.Contains("Tutorial"))
            return;


        var victoryPanel = _UIRoot.transform.Find("VictoryPanel");
        NGUITools.SetActive(victoryPanel.gameObject, true);
        var timer = _UIRoot.GetComponentsInChildren<LabelTimer>().FirstOrDefault();
        timer.enabled = false;


        int score = isUsedHelp ? 0 : ScoreCounter.TimeToStore(timer.StartTime, timer.RemainTime);

        var prevScore = PlayerPrefs.GetInt(Application.loadedLevelName + "_score");
        //Debug.LogWarning(Application.loadedLevelName + "_score = " + prevScore);

        if (prevScore<score)
            PlayerPrefs.SetInt(Application.loadedLevelName + "_score", score);


        var scoreLabel = _UIRoot.transform.FindChild("MainGamePanel/01_spriteForText/02_lblScore");

        var victorySpriteTransform = _UIRoot.transform.FindChild("VictoryPanel/02_spriteTop");
        var scoreLabelInVictoryPanel = victorySpriteTransform.FindChild("03_lblScore");
        scoreLabelInVictoryPanel.GetComponent<UILabel>().text = score.ToString();

        var victorySprite = victorySpriteTransform.GetComponent<UISprite>();

        switch (ScoreCounter.GetCountStars(score))
        {
            case 1:
                victorySprite.spriteName = "08_result_1_star";
                break;
            case 2:
                victorySprite.spriteName = "09_result_2_star";
                break;
            case 3:
                victorySprite.spriteName = "10_result_3_star";
                break;
        }

        StartCoroutine(
            SumScoreCoroutine(scoreLabel.GetComponent<UILabel>(), scoreLabelInVictoryPanel.GetComponent<UILabel>(), score));
    }

    private IEnumerator SumScoreCoroutine(UILabel totalScoreLabel, UILabel scoreLabelInVictoryPanel, int currentScore)
    {        
        var prevTotalScore = PlayerPrefs.GetInt("totalScore");
        PlayerPrefs.SetInt("totalScore", prevTotalScore + currentScore);//здесь, чтобы сохранить до начала перетекания
        MySocialMain.Instance.SubmitScore(prevTotalScore + currentScore);

        yield return new WaitForSeconds(1f);//пауза перед началом перетекания счета
        
        int score = currentScore;
        var totalScore = prevTotalScore;
        const int k = 9;
        while (score > 0)
        {
            score -= k;
            totalScore+=k;

            totalScore = Mathf.Clamp(totalScore, 0, 999999);
            totalScore = Mathf.Clamp(totalScore, 0, prevTotalScore + currentScore);
            score = Mathf.Clamp(score, 0, currentScore);

            totalScoreLabel.text = totalScore.ToString();
            scoreLabelInVictoryPanel.text = score.ToString();

            yield return new WaitForSeconds(0.01f);
        }
    }


    private bool CheckVictoryCondition()
    {
        return ConnectorsManager.TargetConnectors.Length == ConnectorsManager.TargetConnectors.Count(c => c.IsConnected);
    }

}
