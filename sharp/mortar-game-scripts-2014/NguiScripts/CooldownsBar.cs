using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using System;

// -  Панель будет искаться в коде BonusManager через nguiAPI
//панель считывает данные с бонусов (оставшееся и общее время бонуса и прочее)
// - активные бонусы панель берет из BonusManager. 
//Подумать, как лучше передавать панели инфу о старте и прекращении такого то бонуса. События моей системой событий вполне норм c передачей бонуса как параметра.

public class CooldownsBar : MonoBehaviour
{
    //[SerializeField]
    private CooldownItem[] _icons;

    private readonly List<BaseBonus> _activeBonuses = new List<BaseBonus>();

    private void Awake()
    {
        var sprites = this.GetComponentsInDirectChildrens<UISprite>().OrderBy(c => c.gameObject.name).ToArray();

        //Debug.LogWarning(sprites.Count());
        _icons = new CooldownItem[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            _icons[i] = new CooldownItem(sprites[i]);
            sprites[i].gameObject.SetActive(false);
        }
    }

    void Start()
    {
        EventAggregator.Subscribe<BaseBonus>(GameEvent.OnStartBonusEffect, this, OnAddBonus);
        EventAggregator.Subscribe<BaseBonus>(GameEvent.OnEndBonusEffect, this, OnRemoveBonus);
	}
	
	void Update () 
	{
        foreach (var icon in _icons)
	    {
            if (icon.FilledSprite.gameObject.activeSelf==false)
                break;

            //Debug.LogWarning(icon.FilledSprite.fillAmount);
            //Debug.LogWarning(icon.FilledSprite.gameObject.name);
            //Debug.LogWarning(icon.CurrentBonus.RemainingTime);
	        icon.FilledSprite.fillAmount = 1-icon.CurrentBonus.RemainingTime/icon.CurrentBonus.Duration;
	    }
	}

    private void OnAddBonus(BaseBonus newBonus)
    {
        if (_activeBonuses.Contains(newBonus))
        {
            //GetAssociatedIcon; DrawHalo//отобразить анимацию ареола
            return;
        }
        if (_activeBonuses.Count > _icons.Length)
            return;

        var firstEmptyIcon = _icons[_activeBonuses.Count];
        //Debug.LogWarning(_activeBonuses.Count);
       // Debug.LogWarning(firstEmptyIcon.FilledSprite.name);

        firstEmptyIcon.SetBonus(newBonus);
        _activeBonuses.Add(newBonus);
        //Debug.LogWarning(firstEmptyIcon.CurrentBonus.SpriteName);
        firstEmptyIcon.FilledSprite.gameObject.SetActive(true);
    }

    private void OnRemoveBonus(BaseBonus bonus)
    {
        //Debug.LogWarning("OnRemoveBonus");
        var index = GetAssociatedIconIndex(bonus);

        
        //Debug.LogWarning(index);

        //сдвиг влево иконок, которые правее текушей
        int lastVisibleIconindex = _activeBonuses.Count-1;
        for (int i = index; i < lastVisibleIconindex; i++)
        {
            _icons[i].SetBonus(_icons[i + 1].CurrentBonus);
        }
        //if (index < lastVisibleIconindex)
            _icons[lastVisibleIconindex].FilledSprite.gameObject.SetActive(false);
        //else
        //    _icons[index].FilledSprite.gameObject.SetActive(false);
        _activeBonuses.Remove(bonus);
    }

    private int GetAssociatedIconIndex(BaseBonus bonus)
    {
        for (int i = 0; i < _activeBonuses.Count; i++)
        {
            if (_icons[i].CurrentBonus == bonus)
                return i;
        }
        Debug.LogError("Associated Icon Index not found");
        return -1;
    }



    [Serializable]
    private class CooldownItem
    {
        public UISprite FilledSprite;
        public BaseBonus CurrentBonus;// { get; private set; }

        [SerializeField]
        private UISprite _icon;

        public CooldownItem(UISprite sprite) //: this()
        {
            FilledSprite = sprite;
            _icon = sprite.GetComponentsInDirectChildrens<UISprite>().First(); //sprite.GetComponentInChildren<UISprite>();
            CurrentBonus = null;
        }

        public void SetBonus(BaseBonus bonus)
        {
            if (bonus == null)
            {
                Debug.LogWarning("bonus=null");
                return;
            }
            //Debug.LogWarning(bonus.SpriteName);
            CurrentBonus = bonus;

            //Debug.LogWarning(CurrentBonus.SpriteName);
            _icon.spriteName = bonus.SpriteName;
            //Debug.LogWarning(CurrentBonus.SpriteName);
        }
    }
}
