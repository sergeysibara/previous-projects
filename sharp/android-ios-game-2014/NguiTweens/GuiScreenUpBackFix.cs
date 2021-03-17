using UnityEngine;
using System.Collections;

public class GuiScreenUpBackFix : MonoBehaviour
{
    [SerializeField]
    private Transform _topMarker;

    [SerializeField]
    private float _yOffset;

    private UIRect _uiItem;

    private void Start()
    {
        SetToTarget();
    }

    private void SetToTarget()
    {
        var uiCamera = transform.root.GetComponentInChildren<Camera>();

        _uiItem = this.GetSafeComponent<UIRect>();
        Vector3 offset = new Vector3(0, _yOffset, 0);
        Vector3 pos = uiCamera.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(_topMarker.position + offset));
        _uiItem.transform.position = pos;
        _uiItem.transform.SetLocalZ(0);
    }
}
