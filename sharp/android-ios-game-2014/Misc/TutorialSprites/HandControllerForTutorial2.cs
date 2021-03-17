using UnityEngine;
using System.Collections;

public class HandControllerForTutorial2 : MonoBehaviour
{
    [SerializeField]
    private float _clickInterval = 3f;

    [SerializeField]
    private float _movingDuration = 2f;

    [SerializeField]
    private Camera _uiCamera;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private Vector3 _offset;

    private Vector3 _startPos;
    private UIRect _uiItem;

    private bool _isWasClickByTarget;

    private void Start()
    {
        
        _uiItem = this.GetSafeComponent<UIRect>();
        _uiItem.enabled = false;

        EventMessenger.Subscribe(GameEvent.OnTutorialCompleteShowText, this, () => Invoke("ShowSprite", _clickInterval));
        EventMessenger.Subscribe(GameEvent.OnTutorial_ClickByTarget, this, () => Invoke("MoveToButtonStart", 0f));

        Invoke("SetStartPos", 2f);
    }

    private void SetStartPos()
    {
        _startPos = transform.position;
    }

    private void Update()
    {
        if (!_isWasClickByTarget && _uiItem.enabled==true)
            SetToTarget();
    }


    private void ShowSprite()
    {
        SetToTarget();
        _uiItem.enabled = true;
    }

    private void SetToTarget()
    {
        //Vector3 offset = new Vector3(-0.41f, -1f, 0);
        Vector3 pos = _uiCamera.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(_target.position + _offset));
        _uiItem.transform.position = pos;
        _uiItem.transform.SetLocalZ(0);
    }

    private void MoveToButtonStart()
    {
        _isWasClickByTarget = true;
        //var sprite = GetComponent<UISprite>();
        //sprite.spriteName = "08_finger_gray";
        StartCoroutine(MoveToButtonCoroutine());
    }

    private IEnumerator MoveToButtonCoroutine()
    {
        Vector3 prevPos = transform.position;
        float time = 0;
        while (time / _movingDuration <= 1f)
        {
            var t = time / _movingDuration;

            transform.SetX(Mathf.Lerp(prevPos.x, _startPos.x, t));
            transform.SetY(Mathf.Lerp(prevPos.y, _startPos.y, t));

            time += Time.deltaTime;     
            yield return null;
        }    
    }

}
