using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectSelector : MonoBehaviourHeritor
{
    [SerializeField]
    Texture _selectionTexture;

    [SerializeField]
    int _guiDepth = 1;


    public List<SelectedObject> SelectedObjectList = new List<SelectedObject>();
    
    Vector2 _LMB_DownPoint;
    Vector2 _LMB_UpPoint;
    Rect _selectionRect;
    HumanPlayer _player;

    protected override void Awake()
    {
        base.Awake();
        _player = GetComponent<HumanPlayer>();
    }

    void OnGUI()
    {
        if (_player.CursorState == CursorStates.Selecting)
        {
            GUI.depth = _guiDepth;
            GUI.DrawTexture(_selectionRect, _selectionTexture, ScaleMode.StretchToFill, true);
        }
    }

    public void StartSelecting(Vector2 mousePosition)
    {
        _LMB_DownPoint = mousePosition; 
        _player.CursorState = CursorStates.Selecting;
        OnLMB_Pressing(mousePosition);
    }

    public void OnLMB_Pressing(Vector2 mousePosition)
    {
        _LMB_UpPoint = mousePosition;
        float width = _LMB_UpPoint.x - _LMB_DownPoint.x;
        float height = (Screen.height - _LMB_UpPoint.y) - (Screen.height - _LMB_DownPoint.y);
        _selectionRect.Set(_LMB_DownPoint.x, Screen.height - _LMB_DownPoint.y, width, height);
    }

    public void Deselect()
    {
        if (SelectedObjectList.Count > 0)
        {
            foreach (SelectedObject obj in SelectedObjectList)
            {
                if (obj.HasObjectAI)
                    obj.Info.ObjectAI.Deselect();
                    //obj.SendMessage(ObjectCommand.Deselect.ToString(), 0, SendMessageOptions.DontRequireReceiver);
            }
            SelectedObjectList.Clear();
        }

        _player.CursorState = CursorStates.Default;
        _player.GUIPreProcessor.ClearCommandButtonList();
    }

    public void Select()
    {
        Deselect();

        if (_LMB_DownPoint.x == _LMB_UpPoint.x && _LMB_DownPoint.y == _LMB_UpPoint.y)
            CheckObjectInPoint(_LMB_DownPoint.x, _LMB_DownPoint.y); //���� �������� ������ �����
        else
            CheckObjectsInArea(_LMB_DownPoint.x, _LMB_DownPoint.y, _LMB_UpPoint.x, _LMB_UpPoint.y); //���� �������� �������
        
        _player.GUIPreProcessor.SetCommandButtonList();
    }

    public void RemoveFromSelectedObjectList(Transform obj)
    {
        for (int i = 0; i < SelectedObjectList.Count; i++)
        {
            if (SelectedObjectList[i].Transform == obj)
            {
                SelectedObjectList.RemoveAt(i);
                break;
            }
        }
        _player.GUIPreProcessor.UpdateCommandButtonList();
    }

    public bool SelectedObjectsIsBelongsThePlayer()
    {
        if (SelectedObjectList.Count == 1) //���� ���� ������ 1 ���������� ������ 
        {
            if (_player.ObjectIsBelongsThePlayer(SelectedObjectList[0].Transform.gameObject)) //� �� ����������� ������
                return true;
        }
        else 
            if (SelectedObjectList.Count!=0) //���� �������� �����, �� ��� ��� ����������� ������, ������ �� ������� ��������� �������� � ObjectSelector
                return true;

        return false;
    }

    public static bool IsSelectable(Transform obj)
    {
        return GameManager.AllObjectLayers.IsLayerInLayerMask(obj.gameObject.layer);
    }

    void CheckObjectInPoint(float x, float y)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(x, y));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500))
        {
            var newSelectable = new SelectedObject(hit.transform, hit.transform.GetComponent<ObjectInfo>());

            //���� ������ ����� ��������, �� ��������� ��� � ������ ���������� ��������
            if (IsSelectable(hit.transform))
                SelectedObjectList.Add(newSelectable);

            //���� ������ ����������� ������, �� �������� ���
            if (_player.ObjectIsBelongsThePlayer(newSelectable.Transform.gameObject) && newSelectable.HasObjectAI)
                hit.transform.SendMessage(ObjectCommand.Select.ToString(), 0, SendMessageOptions.DontRequireReceiver);
        }
    }

    void CheckObjectsInArea(float x1, float y1, float x2, float y2)
    {
        //GetComponent<MeshFilter>().mesh = Camera.main.GenerateFrustumMesh(x1, y1, x2, y2);
        Plane[] planes = Camera.main.GenerateFrustumPlanes(x1, y1, x2, y2);

        //����� ������ � ���������� ������� � �������� ������ ������
        List<Transform> buildingList = new List<Transform>();
        bool objectsIsFound = false;
        foreach (Transform obj in _player.ObjectList)
        {
            if (!obj.gameObject.active)
                continue;

            if (obj.IsUnit())
            {
                if (GeometryUtility.TestPlanesAABB(planes, obj.collider.bounds))
                {
                    objectsIsFound = true;
                    AddToSelectedObjects(obj);
                }
            }
            else
                if (!objectsIsFound)
                {
                    if (obj.IsBuilding())
                        buildingList.Add(obj);
                }
        }

         //���� ������ ������ �� �������, �� ���� ��� ������, ������ ����� ������ ���������, �.�. � RTS ������ ������ �������� ������ ��������� ������.
        if (!objectsIsFound)
        {
            foreach (Transform obj in buildingList)
            {
                if (GeometryUtility.TestPlanesAABB(planes, obj.collider.bounds))
                {
                    objectsIsFound = true;
                    AddToSelectedObjects(obj);
                    break;
                }
            }
        }

        //�����, ���� �� �������� objectsIsFound=false, �� ����� �������� �� �������� ������ ������� � ������ GO � ����� �������� ���� �� ���.
        //����-��� �� ������ ������ �� ������.
    }

    void AddToSelectedObjects(Transform obj)
    {
        var newSelectable = new SelectedObject(obj, obj.GetComponent<ObjectInfo>());
        SelectedObjectList.Add(newSelectable);

        if (newSelectable.HasObjectAI) //obj.SendMessage(ObjectCommand.Select.ToString(), 0, SendMessageOptions.DontRequireReceiver);
            newSelectable.Info.ObjectAI.Select();  
    }

}
