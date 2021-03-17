using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CompPlayer))]
public class CompPlayerAI : MonoBehaviourHeritor 
{
    public float UnitsCreatingInterval = 5.0f;
    public float AttackWaveInterval = 30.0f;
    public int MaxAttackersPerWave = 5;
    public int MaxCreatedUnitsPerCreatingInterval = 5;
    
    CompPlayer _player;

    protected override void Awake()
    {
        base.Awake();
        _player = GetComponent<CompPlayer>();
    }

    void Start()
    {
        StartCoroutine(CreateUnits());
        StartCoroutine(SendToAttack());
    }

    IEnumerator CreateUnits()
    {
        while (true)
        {
            List<Transform> receivers = new List<Transform>();
            List<CommandButton> buttons = new List<CommandButton>();
            int receiverCount = 0;

            //����� ������ ������������� ���� � �������� ������ 
            foreach (Transform obj in _player.ObjectList)
            {
                ObjectInfo objInfo = obj.GetComponent<ObjectInfo>();
                if (objInfo != null)
                {
                    List<Type> buttonTypeList = CommandButtonTypeGetter.GetCommandButtonTypes(objInfo.CommandButtonInfoArray);

                    if (buttonTypeList.Count==0)
                        continue;

                    bool hasButtons = false;
                    foreach (Type buttonType in buttonTypeList)
                    {
                        CommandButton cb = FindButtonOfType(_player.AvailableCommandButtons, buttonType, typeof(CreateSkeleton_Button));
                        if (cb != null)
                        {
                            buttons.Add(cb);
                            hasButtons = true;
                            break; //��� ���������, ����-��� ������� ������ ������ ������
                        }
                    }

                    //���� � ������� ���� ������ ������, �� ��������� ��� � ������ ����������� �������
                    if (hasButtons)
                    {
                        receivers.Add(obj);
                        receiverCount++;
                        if (receiverCount == MaxCreatedUnitsPerCreatingInterval)
                            break;
                    }
                }
            }

            //���������� �������� ��� ������ ������ � ������
            foreach (CommandButton cb in buttons)
                cb.OnCompPlayerClick(receivers);
            
            yield return new WaitForSeconds(UnitsCreatingInterval);
        }
    }

    IEnumerator SendToAttack()
    {
        while (true)
        {
            Transform commandCenter=null;
            foreach (Transform obj in _player.ObjectList)
            {
                if (obj.IsBuilding() && obj.name == "CommandCenter")
                {
                    commandCenter = obj;
                    break;
                }
            }

            if (commandCenter != null)
            {
                Transform target = TargetFindingMethods.FindNearestTargetForCompPlayerAI(commandCenter.transform.position, 1000, _player.EnemyMask);
                if (target != null)
                {
                    List<Transform> receivers = new List<Transform>();
                    List<CommandButton> buttons = new List<CommandButton>();
                    int attackerCount = 0;
                  
                    //����� ������ ������������� ���� � �������� ������ 
                    foreach (Transform obj in _player.ObjectList)
                    {
                        if (obj.IsUnit())
                        {
                            ObjectInfo objInfo = obj.GetComponent<ObjectInfo>();
                            if (objInfo != null && objInfo.ObjectAI != null)
                            {
                                var battleUnitAI = objInfo.ObjectAI as BattleUnitAI;
                                if (battleUnitAI != null && battleUnitAI.State == UnitState.Free)
                                {
                                    List<Type> buttonTypeList = CommandButtonTypeGetter.GetCommandButtonTypes(objInfo.CommandButtonInfoArray);

                                    if (buttonTypeList.Count == 0)
                                        continue;

                                    bool hasButtons = false;
                                    foreach (Type buttonType in buttonTypeList)
                                    {
                                        CommandButton cb = FindButtonOfType(_player.AvailableCommandButtons, buttonType, typeof(AttackTarget_Button));
                                        if (cb != null)
                                        {
                                            buttons.Add(cb);
                                            hasButtons = true;
                                            break;
                                        }
                                    }

                                    //���� � ������� ���� ������ ������, �� ��������� ��� � ������ ����������� �������
                                    if (hasButtons)
                                    {
                                        receivers.Add(obj);
                                        attackerCount++;
                                        if (attackerCount == MaxAttackersPerWave)
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    //���������� �������� ��� ������ ������ � ������
                    foreach (CommandButton cb in buttons)
                    {
                        (cb as AttackTarget_Button).SetTarget(target);
                        cb.OnCompPlayerClick(receivers);
                    }
                }
            }

            yield return new WaitForSeconds(AttackWaveInterval);
        }
    }

    /// <param name="commandButtons">������, � ������� ����� �������� ������ ������</param>
    /// <param name="desiredButtonType">���������� ������� ��� ������</param>
    /// <param name="rootButtonType">��� ������, ���������� �������� ����� ��������</param>
    /// <returns></returns>
    CommandButton FindButtonOfType(CommandButton[] commandButtons, Type desiredButtonType, Type rootButtonType)
    {
        //�������� �� ������, ���� ����������� ������� ������������ ���� 
        if (!(typeof(CommandButton).IsAssignableFrom(rootButtonType)))
        {
            Debug.LogError(rootButtonType.Name + " type is not in the inheritance hierarchy of CommandButton type", transform);
            return null;
        }
        if (desiredButtonType.IsAbstract)
        {
            Debug.LogError(desiredButtonType.Name + " desiredButtonType cannot to be of abstract type", transform);
            return null;
        }

        if (rootButtonType.IsAssignableFrom(desiredButtonType))//���� ������� ��� ������ � �������� ������������ ���� rootButtonType
        {
            foreach (CommandButton cb in commandButtons)
            {
                if (cb.GetType() == desiredButtonType)
                    return cb;
            }
        }
        return null;
    }

}
