using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class ObjectPlacer : PartlySealedMonoBehaviour
{
    public BuildingGrid PlaceableObjectGrid { get; set; }
    public int PlaceableObjectPrice { get; set; }

    HumanPlayer _player;
    bool _placingMode;
    MaterialChanger _materialChanger;

    bool _hasDefaultRigidbody;

    void Awake()
    {
        _player = GetComponent<HumanPlayer>();
    }

    public void StartPlacing(BuildingGrid grid, int price)
    {
        _player.ObjectSelector.Deselect();
        _player.CursorState = CursorStates.Placing;
        grid.transform.position = new Vector3(0.0f, -10000f, 0.0f);//��������� ���������� ������� � �������, ��������� ��� �������.

        PlaceableObjectGrid = grid;
        PlaceableObjectPrice = price;

        PlaceableObjectGrid.collider.isTrigger = true;//��������� ������� ������� �� ������ � ����
       
        //���������� AI, ���� ����
        var objectAI = PlaceableObjectGrid.GetComponent<BaseObjectAI>();
        if (objectAI != null)
            objectAI.enabled = false;

        _materialChanger = PlaceableObjectGrid.GetComponent<MaterialChanger>();
        if (_materialChanger != null)
            _materialChanger.SetTransparent(0.6f);

        //��������� Rigidbody ��� ������ � �������� OnTrigger, OnCollision � BuildingGrid
        var objectRigidbody = PlaceableObjectGrid.GetComponent<Rigidbody>();
        if (objectRigidbody == null)
        {
            _hasDefaultRigidbody = false;
            objectRigidbody=grid.gameObject.AddComponent<Rigidbody>();
            objectRigidbody.isKinematic = true;
        }
        else
            _hasDefaultRigidbody = true;

        _placingMode = true;
    }

    public void Place()
    {
        if (!_placingMode)
            Debug.LogError("placing mode not enabled");

        if (_materialChanger != null)
        {
            _materialChanger.RevertMaterials();
            //Destroy(_materialChanger);
        }

        //������� Rigidbody, ���� �� �� �������� ����� � StartPlacing()
        if (!_hasDefaultRigidbody)
        {
            var objectRigidbody = PlaceableObjectGrid.GetComponent<Rigidbody>();
            Destroy(objectRigidbody);
        }

        PlaceableObjectGrid.collider.isTrigger = false;//�������� �������� ������� �� ������ � ����

        var objectAI = PlaceableObjectGrid.GetComponent<BaseObjectAI>();
        if (objectAI != null)
            objectAI.enabled = true;


        GameObjectManager.AddToPlayerObjectsList(_player, PlaceableObjectGrid.transform);
        _player.Money -= PlaceableObjectPrice; //������� ��������� ������� �� ����� ������
      
        PlaceableObjectGrid.DoUpdateGraphs(GridTags.Buildings); //���������� �����
        PlaceableObjectGrid.IsGhost = false;

        RuntimeBatching.DoStaticBatching(PlaceableObjectGrid.transform);


        PlaceableObjectGrid = null;
        PlaceableObjectPrice = 0;
        _player.CursorState = CursorStates.Default;
        _placingMode = false;
    }

    //�������� �������� �������, ����� �� ���� �����. (����� ������ ������ ��������� �� ��� �����, ��� ��������� ������ ��� ��������� ������ ����. ����� ��������� ������������ � ������)
    public void CanselPlacing()
    {
        if (_placingMode)
        {
            GameObjectManager.RemoveGhost(PlaceableObjectGrid.gameObject);
            PlaceableObjectGrid = null;
            PlaceableObjectPrice = 0;

            _player.CursorState = CursorStates.Default;
            _placingMode = false;
        }
    }

    /// <summary>
    /// ��������� ����������� ���������� ������� � ����� ������ ���� ������� � ����������� - ����� ��� ��� ��������� ������ � ������� ������� 
    /// </summary>
    public bool PlacementIsAvailable()
    {
        if (!_placingMode)
            Debug.LogError("Placing mode not enabled");

        bool isAvailable = true;
        if (PlaceableObjectGrid.CurrentCollisionAmount > 0)
        {
            isAvailable = false;
        }
        else
        {
            Node[] nodes = PlaceableObjectGrid.FindNodes();
            foreach (Node node in nodes)
            {
                if (node.tags == (int)GridTags.Buildings || !node.walkable)
                {
                    isAvailable = false;
                    break;
                }
            }
        }

        if (_materialChanger != null)
        {
            if (isAvailable)
                _materialChanger.SetAsAvailable();
            else
                _materialChanger.SetAsBanned(2.0f);
        }

        return isAvailable;
    }

    public void MoveObjectToNodeCenter(Vector3 nodePosition)
    {
        if (!_placingMode)
            Debug.LogError("Placing mode not enabled");

        //�������� ���� �����(�����) � �� �������(�����). ����� �������� ������ � ����� ����
        Node node = AstarPath.active.GetNearest(nodePosition);
        Vector3 newPos = new Vector3(node.position.x * 0.001f, nodePosition.y, node.position.z * 0.001f);
        PlaceableObjectGrid.transform.position = newPos;
    }

}
