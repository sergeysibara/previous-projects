using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Класс для отправки комманды объектам игрока. И игрок-человек и игрок-комп используют классы-наследники от данного класса для опправки команд своим юнитам и строениям.
// Вдобавок у игрока-человека некоторые кнопки-комманды могут отображаться в GUI. У таких кнопок-комманд поле ShowInPlayerGUI = true
public abstract class CommandButton:PartlySealedMonoBehaviour
{  
    /// <summary>
    /// Отображать кнопку в меню игрока-человеку. Для игрока-компьютера - это поле не используется
    /// </summary>
    public bool ShowInPlayerGUI = true;

    //public bool Blocked;
    //public Texture Image;
    //public int PositionNumber; //номер позиции в меню игрока

    [SerializeField]
    protected string _text;

    protected Player _player;
    protected bool _ownerPlayerIsHuman;

    public abstract string GetText();

    /// <summary>
    /// Установка игрока-владельца. Должна выполняться до вызова Start
    /// </summary>
    public void SetOwnerPlayer(Player player)
    {
        if (_player != null)
            Debug.LogError("Player already initialized", this);

        _player = player;
        if (_player is HumanPlayer)
            _ownerPlayerIsHuman = true;
    }

    public virtual void OnHumanPlayerClick() 
    {
        if (!_ownerPlayerIsHuman)
            Debug.LogError("OnHumanPlayerClick method can be used a HumanPlayer only", this);
    }

    /// <param name="commandReceivers">Массив объектов игрока-компа, которым отправиться комманда</param>
    public virtual void OnCompPlayerClick(List<Transform> commandReceivers) 
    {
        if (_ownerPlayerIsHuman)
            Debug.LogError("OnCompPlayerClick method can be used a CompPlayer only", this);
    }

    protected virtual void Start()
    {
        if (_player == null)
            Debug.LogError("Player field is empty", this);
    }
}