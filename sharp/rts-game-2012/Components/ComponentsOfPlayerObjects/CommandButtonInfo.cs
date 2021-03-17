using UnityEngine;

/// <summary>
/// Компонент для связи кнопки-комманды доступной юниту с кнопками-коммандами доступными игроку. Скрипт размещать в объекте CommandButtonInfoContainer в юните/здании.
/// </summary>
public class CommandButtonInfo : PartlySealedMonoBehaviour 
{
    public CommandButtonId ButtonId;
    //public bool Blocked;
 }
