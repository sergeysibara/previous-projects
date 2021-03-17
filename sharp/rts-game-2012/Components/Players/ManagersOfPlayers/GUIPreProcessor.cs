using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GUIPreProcessor : PartlySealedMonoBehaviour
{
    //[HideInInspector]
    public List<CommandButton> CommandButtonList = new List<CommandButton>();

    HumanPlayer _player;

    protected void Awake()
    {
        _player = GetComponent<HumanPlayer>();
    }

    public void SetCommandButtonList()
    { 
        ClearCommandButtonList();

        if ( _player.ObjectSelector.SelectedObjectsIsBelongsThePlayer())
        {
            //здесь надо предварительно убрать все повторяющиеся CommandButtonKitId из _player.ObjectSelector.SelectedObjectList
            

            //получение всех типов кнопок в выделенных объектах
            var allCommandButtonTypes = new List<Type>();
            foreach (SelectedObject obj in _player.ObjectSelector.SelectedObjectList)
            {
                if (obj.Info != null)
                {
                    List<Type> buttonTypeList = CommandButtonTypeGetter.GetCommandButtonTypes(obj.Info.CommandButtonInfoArray);

                    foreach (Type type in buttonTypeList)
                    {
                        if (!allCommandButtonTypes.Contains(type))
                            allCommandButtonTypes.Add(type);
                    }
                }
            }

            foreach (CommandButton button in _player.AvailableCommandButtons)
            {
                if (allCommandButtonTypes.Contains(button.GetType()))
                    CommandButtonList.Add(button);
            }


            //здесь должно быть определение окончательного набора отображаемых кнопок
            //если какие-то кнопки имеют одинаковую позицию, то надо оставить только одну из кнопок, имеющих одинаковую позицию   
        }
    }

    public void UpdateCommandButtonList()
    {
        SetCommandButtonList();
    }

    public void ClearCommandButtonList()
    {
        CommandButtonList.Clear();
    }
}
