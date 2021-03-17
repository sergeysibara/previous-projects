using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// »спользуетс€ дл€ получени€ информации об объекте, при его выборе игроком
/// </summary>
public class ObjectInfo : MonoBehaviourHeritor 
{
    public CommandButtonInfo[] CommandButtonInfoArray;

    public BaseObjectAI ObjectAI { get; private set; }
    public HP ObjectHP { get; private set; }
    public UnitFactory UnitFactory { get; private set; } 
    
    protected override void Awake()
    {
        base.Awake();

        ObjectHP = GetComponent<HP>();
        ObjectAI = GetComponent<BaseObjectAI>();
        UnitFactory = GetComponent<UnitFactory>();

        var buttonInfoContainer=transform.FindChild("ButtonInfoContainer");
        if (buttonInfoContainer != null)
        {
            CommandButtonInfo[] buttons = buttonInfoContainer.GetComponents<CommandButtonInfo>();

            //”бираем повтор€ющиес€ элементы из buttons
            Dictionary<CommandButtonId, CommandButtonInfo> buttonDictionary = new Dictionary<CommandButtonId, CommandButtonInfo>();
            foreach (CommandButtonInfo button in buttons)
            {
                if (!buttonDictionary.ContainsKey(button.ButtonId))
                    buttonDictionary.Add(button.ButtonId, button);
            }

            CommandButtonInfoArray = new CommandButtonInfo[buttonDictionary.Count];
            buttonDictionary.Values.CopyTo(CommandButtonInfoArray, 0);
        }
        else
            CommandButtonInfoArray = new CommandButtonInfo[0];
    }
}
