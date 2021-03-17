using UnityEngine;
using System.Collections.Generic;

public class AttackTarget_Button : CommandButton
{
    Transform _target;

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public override void OnHumanPlayerClick()
    {
        base.OnHumanPlayerClick();
        
        //не реализовано
        Debug.LogWarning("OnPlayerClick method has not a realization", this.gameObject);
    }

    public override void OnCompPlayerClick(List<Transform> commandReceivers)
    {
        base.OnCompPlayerClick(commandReceivers);

        if (_target!=null)
        {
            var message = new TaskDataMessage
            {
                TaskData = new FollowToTargetForAttack_Task.FollowToTargetForAttack_TaskData(_target, true),
                NewQueue = true
            };
            foreach (Transform receiver in commandReceivers)
                receiver.SendMessage(ObjectCommand.AddTask.ToString(), message, SendMessageOptions.DontRequireReceiver);
        }
    }

    public override string GetText()
    {
        return _text;
    }
}