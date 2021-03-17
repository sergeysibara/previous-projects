using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class ScoreResults : MonoBehaviour
{
    [SerializeField]
    private GameObject _linePrefab;

    private Transform _grid;

    #if UNITY_IPHONE || UNITY_ANDROID
	// Use this for initialization
	void Start ()
    {
	    _grid = GetComponent<UIGrid>().transform;

        GPGManager.loadScoresSucceededEvent += FillTable;
        ScoresNativeManager.loadScoresSucceededEvent += FillTable;

        MySocialMain.Instance.LoadScoresForLeaderboard(true);

        Invoke("FillTest",0.5f);

        //todo после 28-го протестить на >100 игроков. Хотя не надо, т.к. всего 25 за раз присылается и больше 100 не отобразиться.
    }

    void FillTest()
    {
        FillTable(CreateTestItems(30));
    }


    public void FillTable(List<GPGScore> scores)
    {
        UIScrollView sv = UIRoot.list[0].transform.Find("Panel1/Scroll View").GetComponent<UIScrollView>();
        sv.ResetPosition();

        Debug.Log("loadScoresSucceededEvent");
        Prime31.Utils.logObject(scores);

        List<GameObject> destroyedItems= (from Transform tr in _grid select tr.gameObject).ToList();

        foreach (GameObject go in destroyedItems)
            DestroyImmediate(go);

        int index = 0;
        //int yPos = 0;
        int visibleScores = (MySocialMain.Instance.MaxVisibleScores > 0) ? MySocialMain.Instance.MaxVisibleScores : int.MaxValue;

        foreach (var score in scores.Take(visibleScores))
        {
            var lineItemGO = NGUITools.AddChild(_grid.gameObject, _linePrefab);
            //var lineItemUI = lineItemGO.GetComponent<UIWidget>();

            index++;

            if (index < 10)
            lineItemGO.name = "00"+index.ToString();
            else if (index > 9 && index < 100)
                lineItemGO.name = "0" + index.ToString();
            else if (index > 99)// && yPos < 1000)
                lineItemGO.name = index.ToString();

            AddGridLine(score, lineItemGO.transform.GetChild(0));  
        }
        _grid.GetComponent<UIGrid>().Reposition();

        EventMessenger.SendMessage(GameEvent.OnFillRecordsTable, this);
        Debug.Log("loadScoresSucceededEvent_end");
    }

	int ccc=0;
	
    private void AddGridLine(GPGScore score, Transform line)
    {
        var lbl1 = line.GetChild(0).GetComponent<UILabel>();
        lbl1.text = score.rank.ToString();

        var lbl2 = line.GetChild(1).GetComponent<UILabel>();
        lbl2.text = score.displayName;//countScores.ToString();
        if (lbl2.text.Length>20)
            lbl2.text = lbl2.text.Remove(20); //удаление избыточных символов

        var lbl3 = line.GetChild(2).GetComponent<UILabel>();
        lbl3.text = score.value.ToString(); //destroedLines.ToString()+"            "; //


        var sprite = line.GetSafeComponent<UISprite>();
        if (MySocialMain.Instance.GetLocalPlayerInfo() != null && score.playerId == MySocialMain.Instance.GetLocalPlayerInfo().playerId)
		{
		if (ccc==0)
				sprite.spriteName = "06_line_red";
				else
				{
				sprite.spriteName = "05_line";
				}
				ccc++;
			}
        else
            sprite.spriteName = "05_line";
    }

    public List<GPGScore> CreateTestItems(int n)
    {
        List<GPGScore> scores = new List<GPGScore>();

        for (int i = 0; i < n; i++)
        {
            var sc = new GPGScore();
            sc.displayName = "Test Name" + i;
            sc.rank = i+99999;
            sc.value = i+i+888;
            scores.Add(sc);
        }
        return scores;
    }

    private void OnDestroy()
    {
        GPGManager.loadScoresSucceededEvent -= FillTable;
        ScoresNativeManager.loadScoresSucceededEvent -= FillTable;
    }
#endif
}
