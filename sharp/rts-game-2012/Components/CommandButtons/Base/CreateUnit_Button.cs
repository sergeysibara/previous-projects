using UnityEngine;
using System.Collections.Generic;

//Классы, производные от данного, используются для создания конкретных типов юнитов.
//Ничем, кроме своего типа, эти классы не отличаются. 
//Их тип используется для задания набора создаваемых юнитов в ButtonContainer у юнитов и строений.  
public abstract class CreateUnit_Button : CommandButton
{
    public GameObject Prefab;
    public float CreatingDuration;
    public int Price;

    public override void OnHumanPlayerClick()
    {
        base.OnHumanPlayerClick();

        var message = new CreatingUnit_Data(Prefab, Price, CreatingDuration);
        (_player as HumanPlayer).CommandSender.SendCommand(ObjectCommand.AddUnitToFactory, message);
    }

    public override void OnCompPlayerClick(List<Transform> commandReceivers)
    {
        base.OnCompPlayerClick(commandReceivers);

        var message = new CreatingUnit_Data(Prefab, Price, CreatingDuration);
        foreach (Transform receiver in commandReceivers)
        {
            receiver.SendMessage(ObjectCommand.AddUnitToFactory.ToString(), message,SendMessageOptions.DontRequireReceiver);
        }
    }

    public override string GetText()
    {
        return string.Format("{0}\n{1}$", _text, Price);
    }
}
