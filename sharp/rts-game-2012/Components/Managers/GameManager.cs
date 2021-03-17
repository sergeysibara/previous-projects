using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    #region INSPECTOR VARIABLES
   
    [SerializeField]
    LayerMask _groundLayers;

    #endregion

    #region Outside variables and methods

    public static HumanPlayer CurrentPlayer
    {
        get { return GameManager.Current._currentPlayer; }
    }

    public static Player[] Players
    {
        get { return GameManager.Current._players; }
    }

    public static LayerMask GroundLayers
    {
        get { return GameManager.Current._groundLayers; }
    }

    /// <summary>
    /// Все слои взаимодействующих объектов игры. Исключены следующие слои: Default, TransparentFX, Ignore Raycast, Ground, Ghost, Bullets слои
    /// </summary>
    public static LayerMask AllObjectLayers;

    public static int DefaultLayer { get; private set; }
    public static int GhostsLayer { get; private set; }

    public static float NodeSize {get; private set;}
    public static float HalfNodeSizeSqrt {get; private set;}

    #endregion


    Player[] _players;
    HumanPlayer _currentPlayer;

    protected override void Awake()
    {
        base.Awake();

        //Инициализация контейнеров, чтобы они не инициализировались при вызовах в runtime и не уменьшали FPS
        GameObjectContainers.InitAllContainers();

        NodeSize = AstarPath.active.astarData.gridGraph.nodeSize;
        HalfNodeSizeSqrt = (float)Math.Round(Mathf.Sqrt(NodeSize * 0.5f), 2);
        //print(_sqrtHalfNodeSize);

        InitPlayers();
        InitLayerFields();
    }

    void InitPlayers()
    {
        _players = GameObject.FindGameObjectsWithTag("Player").Select(p => p.GetComponent<Player>()).ToArray<Player>();
        _currentPlayer = _players.First(p => p is HumanPlayer) as HumanPlayer;

        //Добавление юнитов и строений в списки объектов игроков, которым принадлежит эти объекты
        var units = GameObject.FindGameObjectsWithTag(Tags.Unit).Where(o => o.active == true);
        var buildings = GameObject.FindGameObjectsWithTag(Tags.Building).Where(o => o.active == true);
        var objects = units.Union(buildings);

        foreach (Player player in _players)
        {
            var playerObjects = objects.Where(o => o.layer == player.ObjectsLayer).Select<GameObject, Transform>(o => o.transform);
            player.Init(playerObjects);
        }
    }

    //массив _players должен быть заполнен перед вызовом этой функции  
    void InitLayerFields()
    {
        DefaultLayer = 0;

        GhostsLayer = LayerMask.NameToLayer("Ghosts");
        if (GhostsLayer < 0)
            Debug.LogWarning("Ghosts layer not found");


        //Создание маски AllObjectLayers

        AllObjectLayers.value = -1; //(LayerMask)(- 1);
        AllObjectLayers = AllObjectLayers.RemoveFromMask(0);
        AllObjectLayers = AllObjectLayers.RemoveFromMask(1);
        AllObjectLayers = AllObjectLayers.RemoveFromMask(2);
        AllObjectLayers = AllObjectLayers.RemoveFromMask(GhostsLayer);
        AllObjectLayers = AllObjectLayers.RemoveFromMask(_groundLayers);

        foreach (Player player in _players)
            AllObjectLayers = AllObjectLayers.RemoveFromMask(player.BulletsLayer);

        //Debug.Log(AllObjectLayers.value);
        //for (int i = 0; i < 32; i++)
        //    Debug.Log("layer   "+LayerMask.LayerToName(i) +"    InMask: " + AllObjectLayers.IsLayerInLayerMask(i));
    }

}
