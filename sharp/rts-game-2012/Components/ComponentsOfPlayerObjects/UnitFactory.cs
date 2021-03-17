using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class UnitFactory : MonoBehaviourHeritor
{      
    #region Inspector Variables

    [SerializeField]
    Vector3 _spawnPoint; //������� �������� ����� ������������ ������������� �������

    [SerializeField]
    Vector3 _spawnRotation; //������� ������������ ����� ������������ ������������� �������
    
    #endregion

    public Vector3 AssemblyPoint { get; set; }
    public Player OwnerPlayer { get; private set; }

    float _timeBeforeSpawn=0;
    bool _hasCreatingUnit;
    CreatingUnit_Data _currentTaskData;
    List<CreatingUnit_Data> _taskDataList = new List<CreatingUnit_Data>();
    

    public void AddUnitToFactory(CreatingUnit_Data data)
    {
        if (OwnerPlayer == null)
        {
            OwnerPlayer = GameObjectManager.GetOwnerPlayer(gameObject);
            _currentTaskData = null;
        }

        if (data != null)
        {
            if (OwnerPlayer.Money - data.Price >= 0) //�������� ����, ������ ���� � ������ ������� �����
            {
                OwnerPlayer.Money -= data.Price;
                _taskDataList.Add(data);
                enabled = true;
                _hasCreatingUnit = true;
                if (_timeBeforeSpawn <=0)
                    DequeueUnitData();
            }
        }
    }

    public void CancelUnitCreating(int unitNumber)
    { 
    }

    void Update()
    {
        if (_hasCreatingUnit)
        {
            _timeBeforeSpawn -= Time.deltaTime;
            if (_timeBeforeSpawn <= 0)
            {
                Quaternion unitRotation = new Quaternion();
                unitRotation.eulerAngles = _spawnRotation;
                unitRotation = transform.rotation * unitRotation;

                UnitAI newUnit = GameObjectManager.CreateObject(_currentTaskData.Prefab, PositionChecker.FindNearestFreePosition(transform.TransformPoint(_spawnPoint)), unitRotation).GetComponent<UnitAI>();
                GameObjectManager.AddToPlayerObjectsList(OwnerPlayer, newUnit.transform);

                ////�������� ����� � ����� �����
                //var message = new TaskDataMessage {
                //    TaskData = new FollowToPoint_Task.FollowToPoint_TaskData(AssemblyPoint ),
                //    NewQueue = true
                //};
                //newUnit.AddTask(message);

                DequeueUnitData();
            }
        }

    }

    void OnDestroy()
    {
        //���� ����� ������������ �������� ����������� �����, �� ������� ������ ������ 
        if (_taskDataList.Count > 0)
        {
            foreach (CreatingUnit_Data taskData in _taskDataList)
                OwnerPlayer.Money += taskData.Price;
        }
    }

    void DequeueUnitData()
    {
        _currentTaskData=null;
        if (_taskDataList.Count > 0)
        {
            _hasCreatingUnit = true;
            _currentTaskData = _taskDataList[0];
            _timeBeforeSpawn = _currentTaskData.CreatingDuration;
            _taskDataList.RemoveAt(0);
        }
        else
        {
            _hasCreatingUnit = false;
            enabled = false;
        }
    }


    //��������� position � rotation ��� ������ ������ � ���������� ��������. ��� ������, �����, ����� ������ � ���������� ��� ��������� 
    void OnDrawGizmosSelected()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            Gizmos.color = new Color(0.0f, 0.5f, 1.0f, 0.5f);
            var tr = GetComponent<Transform>();//GetComponent ������ transform - ����� �� ���� ������ � ���������
            float radius = 0.3f;

            Vector3 pos = tr.TransformPoint(_spawnPoint);
            pos.y += radius;

            Gizmos.DrawSphere(pos, radius);

            Quaternion rot = new Quaternion();
            rot.eulerAngles = _spawnRotation;
            Vector3 dir;
            rot = tr.rotation * rot;//�������� ������������ ��������

            //forward
            dir = (rot * Vector3.forward).normalized;
            Gizmos.color = new Color(0, 0, 1);
            Gizmos.DrawRay(pos, dir);

            //up
            dir = (rot * Vector3.up).normalized;
            Gizmos.color = new Color(0, 1, 0);
            Gizmos.DrawRay(pos, dir);

            //right
            dir = (rot * Vector3.right).normalized;
            Gizmos.color = new Color(1, 0, 0);
            Gizmos.DrawRay(pos, dir);
        }
    }
}
