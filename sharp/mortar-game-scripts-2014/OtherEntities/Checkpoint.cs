using System.Collections.Generic;
using UnityEngine;
using System.Collections;

//todo Если будут ситуации, когда юнита выталкивают из чекпоинта и он входит в тот же - то добавить в юнит поле "последний посещенный чекпоинт"
public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    private bool _isFinalCheckpoint;

    [SerializeField]
    [TooltipAttribute("Чекпоинты, к которым есть дороги от текущего чекпоинта")]
    private Checkpoint[] _nextCheckpoints;

    //public bool IsFinalCheckpoint {get { return _isFinalCheckpoint; }}

    public Checkpoint[] NextCheckpoints
    {
        get { return _nextCheckpoints; }
    }

    private void OnTriggerEnter(Collider unit)
    {
        //Debug.LogWarning("CheckPoint ", this);
        if (!_isFinalCheckpoint)
            //EventAggregator.OnCheckpointEnter.PublishToConcrete(this, this,unit.ge);
            unit.SendMessage(Consts.Events.OnCheckpointEnter, this, SendMessageOptions.DontRequireReceiver);
        else
            unit.SendMessage(Consts.Events.OnFinalCheckpointEnter, this,SendMessageOptions.DontRequireReceiver);
    }


    private void OnDrawGizmos()
    {
        GizmosUtils.DrawText(CustomEditorPrefs.GizmoGuiSkin, name, transform.position);
    }

    private  void OnDrawGizmosSelected()
    {
        foreach (var c in _nextCheckpoints)
        {
            Gizmos.color = Color.cyan.WithAlpha(0.5f);
            Gizmos.DrawSphere(c.transform.position, 3f);
            Gizmos.DrawWireSphere(c.transform.position, 3f);
        }
    }

}
