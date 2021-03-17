using UnityEngine;
using System.Collections;

public class SpriteChanger : MonoBehaviour
{
    [SerializeField]
    private float _changeInterval;

    [SerializeField]
    private Renderer _targetRenderer;

    [SerializeField]
    private Texture[] _sprites;

    private int _currentSpriteIndex;
    private Texture _defaultSprite;

    public bool IsPlaing { get; private set; }

    private void Start()
    {
        //_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_targetRenderer == null)
            Debug.LogError("targetRenderer not found", this);

        _defaultSprite = _targetRenderer.material.mainTexture;
        enabled = false;
    }

    private IEnumerator ChangeSpritesCoroutine()
    {
        while (true)
        {
            _currentSpriteIndex = RandomUtils.RangeWithExclude(0, _sprites.Length, _currentSpriteIndex);
            _targetRenderer.material.mainTexture = _sprites[_currentSpriteIndex];

            yield return new WaitForSeconds(_changeInterval);
        }
    }

    public void SwitchToOn()
    {
        if (IsPlaing)
            return;
        var sc = GetComponent<SpriteChangingOnClick>();
        if (sc != null && sc.IsPlaing)
            sc.SwitchToOff();

        StartCoroutine(ChangeSpritesCoroutine());
        IsPlaing = true;
    }

    public void SwitchToOff()
    {
        StopAllCoroutines();
        _targetRenderer.material.mainTexture = _defaultSprite;
        IsPlaing = false;
    }

}