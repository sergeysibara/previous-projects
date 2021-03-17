using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ����� ��� �������� �������� �������� ������. � �����-������� � �����-���� ���������� ������-���������� �� ������� ������ ��� �������� ������ ����� ������ � ���������.
// �������� � ������-�������� ��������� ������-�������� ����� ������������ � GUI. � ����� ������-������� ���� ShowInPlayerGUI = true
public abstract class CommandButton:PartlySealedMonoBehaviour
{  
    /// <summary>
    /// ���������� ������ � ���� ������-��������. ��� ������-���������� - ��� ���� �� ������������
    /// </summary>
    public bool ShowInPlayerGUI = true;

    //public bool Blocked;
    //public Texture Image;
    //public int PositionNumber; //����� ������� � ���� ������

    [SerializeField]
    protected string _text;

    protected Player _player;
    protected bool _ownerPlayerIsHuman;

    public abstract string GetText();

    /// <summary>
    /// ��������� ������-���������. ������ ����������� �� ������ Start
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

    /// <param name="commandReceivers">������ �������� ������-�����, ������� ����������� ��������</param>
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