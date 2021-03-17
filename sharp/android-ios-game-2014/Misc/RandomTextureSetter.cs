using UnityEngine;
using System.Collections;

public class RandomTextureSetter : MonoBehaviour
{
    [SerializeField]
    private Texture[] _textures;

    [SerializeField]
    private bool _alwaysGenerate=false;

    private void Start()
    {
        if (_alwaysGenerate)
            renderer.material.mainTexture = RandomUtils.GetRandomItem(_textures);
        else
        {
            if (Application.loadedLevelName == Consts.SceneNames.Level1.ToString())
                renderer.material.mainTexture = RandomUtils.GetRandomItem(_textures);
        }
    }

}
