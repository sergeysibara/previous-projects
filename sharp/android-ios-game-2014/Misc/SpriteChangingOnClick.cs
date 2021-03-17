using UnityEngine;
using System.Collections;

public class SpriteChangingOnClick : MonoBehaviour
{
    [SerializeField]
    private Texture[] _sprites;

    [SerializeField]
    private Renderer _targetRenderer;

    [SerializeField]
    private float _duration = 1f;

    private Texture _defaultSprite;
    public bool IsPlaing { get; private set; }

    private void Start()
    {
        if (_targetRenderer == null)
            Debug.LogError("targetRenderer not found", this);

        _defaultSprite = _targetRenderer.material.mainTexture;
    }


    private void OnClick()
    {
        var sc = GetComponent<SpriteChanger>();
        if (IsPlaing || (sc != null && sc.IsPlaing))
            return;

            IsPlaing = true;
        if (_sprites.Length == 1)
        {
            _targetRenderer.material.mainTexture = _sprites[0];
            Invoke("SetDefaultSprite", _duration);
        }
        else
            StartCoroutine(ChangeSpritesCoroutine());
    }

    private IEnumerator ChangeSpritesCoroutine()
    {
        var startTime = Time.time;
        int currentSpriteIndex = 0;
        while (_duration>(Time.time-startTime))
        {
            _targetRenderer.material.mainTexture = _sprites[currentSpriteIndex];
            
            currentSpriteIndex++;
            if (currentSpriteIndex >= _sprites.Length)
                currentSpriteIndex = 0;

            yield return new WaitForSeconds(0.1f);
        }

        SetDefaultSprite();
    }

    private void SetDefaultSprite()
    {
        _targetRenderer.material.mainTexture = _defaultSprite;
        IsPlaing = false;
    }

    public void SwitchToOff()
    {
        StopAllCoroutines();
        SetDefaultSprite();
    }

}
