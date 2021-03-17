using UnityEngine;
using System;
using System.Collections;

//����� ���� ����� ����� ������� �� ������������

public class CommandSender :PartlySealedMonoBehaviour
{
    HumanPlayer _player;
    

    protected void Awake()
    {
        _player = GetComponent<HumanPlayer>();
    }

    public void AutoDeterminedPointCommand(Vector3 point)
    {
        Vector3 followPoint;
        Transform target = GetObjectInPoint(point, out followPoint);
        if (target != null)
        {
            if (_player.EnemyMask.IsLayerInLayerMask(target.gameObject.layer))//���� ���� - ����
            {
                var message = new TaskDataMessage{
                                    TaskData = new FollowToTargetForAttack_Task.FollowToTargetForAttack_TaskData(target, false),
                                    NewQueue = true};

                SendCommand(ObjectCommand.AddTask, message);
            }
            //else {} //����� ��������� ������� "��������� �� ��������"
        }
        else
        {
            var message = new TaskDataMessage {
                                TaskData = new FollowToPoint_Task.FollowToPoint_TaskData(followPoint), 
                                NewQueue = true };

            SendCommand(ObjectCommand.AddTask, message);
        }
    }

    /// <summary>
    /// �������� ������� ���� ���������� �������� ������
    /// </summary>
    public void SendCommand(ObjectCommand command, object value)
    {
        if (_player.ObjectSelector.SelectedObjectsIsBelongsThePlayer())  
        {
            foreach (SelectedObject selectedObj in _player.ObjectSelector.SelectedObjectList)
                selectedObj.Transform.SendMessage(command.ToString(), value, SendMessageOptions.DontRequireReceiver);
        }
    }


    Transform GetObjectInPoint(Vector3 screenPoint, out Vector3 hitPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500))
        {
            hitPoint=hit.point;
            if (ObjectSelector.IsSelectable(hit.transform))
                return hit.transform;
        }

        //���� �� ���� ��������� � ���������� ��������, �� ������ Raycast � �����
        if (Physics.Raycast(ray, out hit, 500, GameManager.GroundLayers))
        {
            hitPoint = hit.point;
            return null;
        }

        hitPoint=Vector3.zero;
        return null;
    }
}
