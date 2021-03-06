using UnityEngine;
using System.Collections;

public class UIFollowerToPoint : MonoBehaviour 
{
    public Vector3 TargetPos;

    [SerializeField]
    public Vector3 Offset = Vector3.zero;   // если требуется задать дополнительное смещение гуи-объекта над целью

    [SerializeField]
    public float HidingDuration = 2;

    [SerializeField]
    public float MovementSpeed = 1;

    [SerializeField]
    public float StartHidingTime = 2;

    [SerializeField]
    public float StartMovementTime = 1.5f;
    
    private Camera _uiCamera;

    private Vector3 _moveValue;

    private void Start()
    {
        _uiCamera = UIRoot.list[0].transform.GetComponentInChildren<UICamera>().camera;
        Invoke("StartMovement", StartMovementTime);
        Invoke("StartHiding", StartHidingTime);
    }

    private void StartMovement()
    {
        StartCoroutine(Actions.Position.ConstantMoveCoroutine(Vector3.up, MovementSpeed, (v) => _moveValue += v));
    }

    private void StartHiding()
    {
        var widget = GetComponentInChildren<UIWidget>();
        StartCoroutine(MathfUtils.LerpWithDuration(1, 0, HidingDuration, (v) =>
            {
                widget.alpha = v;
            }, () => Destroy(gameObject)));
    }


    void Update()
    {
        if (_uiCamera != null)
        {
            Vector3 viewpos = Camera.main.WorldToViewportPoint(TargetPos + Offset + _moveValue);
            Vector3 newpos = _uiCamera.ViewportToWorldPoint(viewpos);
            newpos = transform.parent.InverseTransformPoint(newpos);
            transform.localPosition = new Vector3(newpos.x, newpos.y, transform.position.z);
        }
    }
}
