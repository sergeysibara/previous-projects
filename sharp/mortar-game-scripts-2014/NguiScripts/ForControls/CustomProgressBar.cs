using UnityEngine;
using System.Collections;
using System.Linq;
public class CustomProgressBar : MonoBehaviour
{
    private UISprite[] _sprites;

    [SerializeField]
    private string _filledSpiteName;

    [SerializeField]
    private string _emptySpriteName;

    [SerializeField]
    public int FilledCount;
    private int _prevFilledCount=-1;

	void Start ()
	{
        _sprites = this.GetComponentsInDirectChildrens<UISprite>().OrderBy(c => c.gameObject.name).ToArray();
	}
	
	void Update () 
	{
	    if (FilledCount != _prevFilledCount)
	    {
	        for (int i = 0; i < _sprites.Length; i++)
	        {
	            if (i < FilledCount)
	            {
	                _sprites[i].spriteName = _filledSpiteName;
	                _sprites[i].color = Color.red;
	            }
	            else
	            {
	                _sprites[i].spriteName = _emptySpriteName;
	                _sprites[i].color = Color.white;
	            }
	        }
	        _prevFilledCount = FilledCount;
	    }
	}
}
