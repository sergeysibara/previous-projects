using UnityEngine;
using System.Collections;

//Все объекты в игре должны создаваться через этот менеджер, т.к. runtime static батчинг использует имена объектов, 
//а в этом менеджере имена создаваемых объектов устанавливаются как у объектов-оригиналов 

/// <summary>
/// Менеджер для создания и удаления игровых объектов 
/// </summary>
public static class GameObjectManager 
{
    public static GameObject CreateObject(GameObject original)
    {
        GameObject newGO = GameObject.Instantiate(original) as GameObject;
        ConfigureObjectSettings(newGO, original);
        return newGO;
    }

    public static GameObject CreateObject(GameObject original, Vector3 position, Quaternion rotation)
    {
        GameObject newGO = GameObject.Instantiate(original, position, rotation) as GameObject;
        ConfigureObjectSettings(newGO, original);
        return newGO;
    }

    //Пока-что просто создание стрел, вместо использования пула
    public static GameObject CreateArrow(GameObject original, Vector3 position, Quaternion rotation, int bulletLayer, Vector3 force, LayerMask enemyLayerMask, int damage)
    {
        GameObject newArrow = GameObject.Instantiate(original, position, rotation) as GameObject;
        newArrow.layer = bulletLayer;
        newArrow.rigidbody.AddForce(force);

        #if UNITY_EDITOR
            newArrow.transform.parent = GameObjectContainers.Bullets;
        #endif

        ArrowsCollision arrowColl = newArrow.GetComponent<ArrowsCollision>();
        arrowColl.Damage = damage;
        arrowColl.EnemyLayerMask = enemyLayerMask;
        return newArrow;
    }

    static void ConfigureObjectSettings(GameObject obj, GameObject original)
    {
        obj.name = original.name;

        //Установка контейнера-родителя для перемещаемых объектов, либо батчинг для стационарных объектов  
        switch (obj.tag)
        {
            case Tags.Building:
                RuntimeBatching.DoStaticBatching(obj.transform);
                break;

            case Tags.Unit:
                obj.transform.parent=GameObjectContainers.DynamicUnits;
                break;
        }
    }

    /// <summary>
    /// Установка игрока как владельца объекта, а также добавление объекта в ObjectsList игрока
    /// </summary>
    public static void AddToPlayerObjectsList(Player player, Transform transform)
    {
        player.ObjectList.Add(transform);
        InheriteSettingsFromPlayer(player, transform);
    }

    /// <summary>
    /// Установка объекту параметров: layer, EnemyLayerMask, BulletsLayer - передаваемых ему от игрока, которому он уже принадлежит
    /// </summary>
    public static void InheriteSettingsFromPlayer(Player player, Transform obj)
    {
        obj.gameObject.layer = player.ObjectsLayer;

        IAttacker attacker = obj.GetInterfaceComponent<IAttacker>();
        if (attacker != null)
        {
            attacker.EnemyLayerMask = player.EnemyMask;

            IFarRangeAttacker farRangeAttaker = attacker as IFarRangeAttacker;
            if (farRangeAttaker != null)
                farRangeAttaker.BulletLayer = player.BulletsLayer;
        }
    }


    public static void KillObject(Transform obj, float destroyTime)
    {
        //удаление объекта из списка выделенных объектов каждого игрока
        foreach (Player player in GameManager.Players)
        {
            HumanPlayer humanPlayer = player as HumanPlayer;
            if (humanPlayer != null)
                humanPlayer.ObjectSelector.RemoveFromSelectedObjectList(obj);
        }
        //transform.parent = null;
        RemoveFromPlayerObjectList(obj);
        obj.gameObject.layer = GameManager.DefaultLayer;//ставим layer по умолчанию, чтобы объект больше не влиял на игровой мир

        IKillable AI = obj.GetInterfaceComponent<IKillable>();
        if (AI != null)
            AI.Die();

        UnitFactory unitFactory = obj.GetComponent<UnitFactory>();
        if (unitFactory != null)
            Object.Destroy(unitFactory);

        BuildingGrid grid = obj.GetComponent<BuildingGrid>();
        if (grid != null)
            Object.Destroy(grid);

        Rigidbody rb = obj.rigidbody;
        if (rb != null)
        {
            rb.isKinematic = true;
            Object.Destroy(rb);
        }
            
        Collider c = obj.collider;
        if (c != null)
            c.enabled = false; //Object.Destroy(c);
        

        Object.Destroy(obj.gameObject, destroyTime);
    }

    public static GameObject CreateGhost(GameObject original, out BuildingGrid grid)
    {
        GameObject ghost = GameObject.Instantiate(original) as GameObject;
        ghost.name = original.name;
        ghost.layer = GameManager.GhostsLayer;
        grid = null;

        switch (original.tag)
        {
            case Tags.Unit:
                break;

            case Tags.Building:
                {
                    grid = ghost.GetComponent<BuildingGrid>();
                    if (grid == null)
                        Debug.LogError("Object must contain BuildingGrid script");
                    grid.IsGhost = true;
                }
                break;

            default:
                Debug.LogError(string.Format("Tag {0} or {1} is not set", Tags.Unit, Tags.Building));
                break;
        }

        return ghost;
    }

    public static void RemoveGhost(Object obj)
    {
        if (obj != null)
            GameObject.Destroy(obj);
    }


    public static Player GetOwnerPlayer(GameObject obj)
    {
        Player player = null;
        foreach (Player p in GameManager.Players)
        {
            if (p.ObjectIsBelongsThePlayer(obj))
            {
                player = p;
                break;
            }
        }
        return player;
    }

    #region Private Methods

    static void RemoveFromPlayerObjectList(Transform obj)
    {
        Player player = GameObjectManager.GetOwnerPlayer(obj.gameObject);
        if (player != null)
            player.ObjectList.Remove(obj);
    }

    #endregion

}
