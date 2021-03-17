using UnityEngine;
using System.Collections;

public class GuiScreenDownBackFix : MonoBehaviour
{
    [SerializeField]
    private Transform _bottomMarker;

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
        Vector3 pos = uiCamera.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(_bottomMarker.position + offset));
        _uiItem.transform.position = pos;
        _uiItem.transform.SetLocalZ(0);
    }
   
}
