using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(PlayerObjectsLayerDDL))]
[RequireComponent(typeof(PlayerBulletsLayerDDL))]
public abstract class Player : MonoBehaviourHeritor
{
    #region Inspector Variables
    
    public int Money = 1000;
    public LayerMask EnemyMask;

    #endregion

    public CommandButton[] AvailableCommandButtons;

    //[HideInInspector]
    public int ObjectsLayer;
    //[HideInInspector]
    public int BulletsLayer;

    //[HideInInspector]
    public List<Transform> ObjectList; //список построек и юнитов игрока

    /// <summary>
    /// Инициализации объектов, принадлежащих игроку. (Метод используется в GameManager)
    /// </summary>
    public void Init(IEnumerable<Transform> playerObjects)
    {
        ObjectList = playerObjects.ToList();
        InitPlayerObjects();
    }

    public bool ObjectIsBelongsThePlayer(GameObject obj)
    {
        return (ObjectsLayer == obj.layer);
    }

    protected override void Awake()
    {
        base.Awake();
        ObjectsLayer = LayerMask.NameToLayer(GetComponent<PlayerObjectsLayerDDL>().Layer);
        BulletsLayer = LayerMask.NameToLayer(GetComponent<PlayerBulletsLayerDDL>().Layer);

        Transform buttonContainer = transform.FindChild("ButtonContainer");
        if (buttonContainer == null)
            Debug.LogWarning("ButtonContainer not found");
        else
            AvailableCommandButtons = buttonContainer.GetComponents<CommandButton>();

        foreach (CommandButton button in AvailableCommandButtons)
        {
            button.SetOwnerPlayer(this);
        }
    }

    /// <summary>
    /// Установка параметров: layer, EnemyLayerMask, BulletsLayer - всем объектам игрока
    /// </summary>
    void InitPlayerObjects()
    {
        foreach (Transform tr in ObjectList)
        {
            GameObjectManager.InheriteSettingsFromPlayer(this, tr);
        }
    }
}