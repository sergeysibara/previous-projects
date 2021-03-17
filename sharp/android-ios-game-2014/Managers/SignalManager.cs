using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Shapes;

public class SignalManager : RequiredMonoSingleton<SignalManager>
{
    [System.Serializable]
    private class SpawnItem
    {
        public Shape Shape = null;
        public Direction Side = Direction.None;
    }


    [SerializeField]
    private GameObject _prefab;

    [SerializeField]
    private float _firstSpawnTime = 1f;

    [SerializeField]
    private float _repeatSpawnInerval = 7f;

    [SerializeField]
    private List<SpawnItem> _spawnItems;

    [SerializeField]
    private bool _randomCloning = false;

    [SerializeField, Range(1,100)]
    private int _maxSignalsCount = 100;

    [SerializeField, ReadOnlyInInspector]
    private int _signalsCount = 0;

    public static bool IsAllowedCreateSignal
    {
        get
        {
            if (Instance == null)
                return false;
            return Instance._signalsCount < Instance._maxSignalsCount;
        }
    }

    /// <summary>
    /// При отключенной опции, клонирование сигнала будет выполняться всегда при ветвлении провода. При влюченной - выбор клонировать или нет выполняется случайно.
    /// </summary>
    public static bool IsRandomCloning
    {
        get { return Instance._randomCloning; }
    }


    private void Start()
    {
        if (Application.loadedLevelName == Consts.SceneNames.Title.ToString())
            InvokeRepeating("CreateSignal", _firstSpawnTime, _repeatSpawnInerval);
        else if (Application.loadedLevelName==Consts.SceneNames.Tutorial2.ToString())
        {
            EventMessenger.Subscribe(GameEvent.OnTutorial_ClickByTarget, this, OnStartGameProcess);
        }
        else
        {
            EventMessenger.Subscribe(GameEvent.StartGameProcess, this, OnStartGameProcess);
            EventMessenger.Subscribe(GameEvent.CompleteNodesGeneration, this, OnCompleteNodesGeneration);
            EventMessenger.Subscribe(GameEvent.InvokeAdjuster, this, () => InvokeSignal(1f));
        }
        EventMessenger.Subscribe(GameEvent.OnCreateSignal, this, OnCreateSignal);
        EventMessenger.Subscribe(GameEvent.OnDestroySignal, this, OnDestroySignal);
    }

    private void OnCreateSignal()
    {
        _signalsCount++;
    }

    private void OnDestroySignal()
    {
        _signalsCount--;
    }

    /// <summary>
    /// Создание сигнала через заданное время
    /// </summary>
    private void InvokeSignal(float time)
    {
        Invoke("CreateSignal", time);
    }

    private void OnCompleteNodesGeneration()
    {
        _spawnItems.Clear();//на тот случай, если в редакторе случайно добавлены элементы
        var shape = ConnectorsManager.StartConnector.NearestNode.Shape;
        _spawnItems.Add(new SpawnItem { Shape = shape, Side = ConnectorsManager.StartConnector.CurrentDirection });
    }

    private void OnStartGameProcess()
    {
        InvokeRepeating("CreateSignal", _firstSpawnTime, _repeatSpawnInerval);
    }


    private void CreateSignal()
    {
        if (!IsAllowedCreateSignal || _spawnItems.Count==0)
            return;

        int index = Random.Range(0, _spawnItems.Count);

        var shape = _spawnItems[index].Shape;
        var direction = _spawnItems[index].Side;

        if (shape==null || shape.GetPath(direction).Count == 0)
        {
            return;
        }
        var startPoint = shape.GetPath(direction).First();

        var signalGO = (GameObject)Instantiate(_prefab, startPoint, new Quaternion(0, 0, 0, 0));
        var signal = signalGO.GetComponent<Signal>();
        signal.Init(direction, _prefab);
    }

    private void OnDrawGizmosSelected()
    {
        if (_spawnItems == null || _spawnItems.Count == 0)
            return;
        Gizmos.color = ColorUtils.CreateWithAlpha(Color.green, 0.5f);
        foreach (var item in _spawnItems)
        {
            if (item.Shape!=null)
                Gizmos.DrawSphere(item.Shape.transform.position, 0.3f);
        }
    }

}


