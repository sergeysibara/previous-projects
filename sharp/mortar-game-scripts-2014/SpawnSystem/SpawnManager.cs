using UnityEngine;
using System.Collections;
using System.Linq;

public class SpawnManager : RequiredMonoSingleton<SpawnManager>
{
    [SerializeField, Tooltip("минимальный уровень SpawnRate, при достижении которого вызывается принудельный спавн")]
    private int _minSpawnRateSumBeforeImmediateSpawn=5;//

    [SerializeField, Tooltip("максимальный уровень SpawnRate, при достижении которого мобы не создается пока он не уменьшиться")]
    private int _maxSpawnRateSum = 30;

    //пока не юзать, а просто спавнить реже юнитов определенного типа. Например в первом уровне должно быть мало скелетов, т.к. они часто отнимает очки.
    //[SerializeField, Tooltip("ограничие количества мобов данного типа в уровне, тип определеятся по префабу")]
    //private MobtypeLimitPair[] _mobtypeLimits;

    public int CurrentSpawnRateSum { get; private set; }

    [SerializeField]
    private Transform _parentForLoopedSpawnPoints;

    private SpawnPoint[] _loopedSpawnPoints;


    private void Start()
    {
        EventAggregator.Subscribe<int>(GameEvent.OnAddUnitToGame, this, (rate) =>
            {
                CurrentSpawnRateSum += rate;
            });
        EventAggregator.Subscribe<int>(GameEvent.OnRemoveUnitFromGame, this, (rate) =>
            {
                CurrentSpawnRateSum -= rate;
            });

        _loopedSpawnPoints = _parentForLoopedSpawnPoints.GetComponentsInChildren<SpawnPoint>().Where(c => c.enabled && c.gameObject.activeInHierarchy).ToArray();
    }


    void Update ()
    {
        _rateSumText = string.Format("rateSum: {0} ", CurrentSpawnRateSum);
        if (BattleManager.CurrentGameMode != GameMode.Normal)
           return;

        if (CurrentSpawnRateSum < _minSpawnRateSumBeforeImmediateSpawn)
	        ImmediateSpawn();

   
	}

    private string _rateSumText;
    void OnGUI() 
    {
        //GUI.Label(new Rect(1000, 0, 300, 100), _rateSumText);
    }


    public bool CanDoMobSpawn(int unitSpawnRate)
    {
        //Debug.LogWarning((CurrentSpawnRateSum + unitSpawnRate ));
        return (CurrentSpawnRateSum + unitSpawnRate <= _maxSpawnRateSum);
    }

    public Transform[] GetAllLoopedSpawnPoints()
    {
        return _loopedSpawnPoints.Select(c => c.transform).ToArray();
    }

    private int GetFreeSpawnRates()
    {
        return _maxSpawnRateSum - CurrentSpawnRateSum;
    }

    private void ImmediateSpawn()
    {
        var spawnPoints = _loopedSpawnPoints.Where(c => c.CanUseForImmediateSpawn);
        var spawnPoint = RandomUtils.GetRandomItem(spawnPoints);
        if (spawnPoint == null)
            return;
        spawnPoint.ImmediateSpawn();
    }

}

//[System.Serializable]
//public class MobtypeLimitPair
//{
//    public Transform Prefab;
//    public int MaxCountAtCurrentTime;
//    //public int MaxCountAtLevel;
//}