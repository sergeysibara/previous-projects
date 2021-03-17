using UnityEngine;

public class HP : MonoBehaviourHeritor 
{
    [SerializeField]
    int _maxHP = 100;

    [SerializeField]
    int _currentHP = 100;

    [SerializeField]
    float _destroyTime = 15;

    public int CurrentHP
    {
        get { return _currentHP; }
    }

    public void ChangeHP(int adjust) //����� ������������� ��
    {
        _currentHP += adjust;
        if (_currentHP > _maxHP)
            _currentHP = _maxHP;

        CheckHP();
    }

    protected override void Awake()
    {
        base.Awake();
        if (_maxHP < 1)
            _maxHP = 1;
        if (_currentHP < 1)
            _currentHP = 1;
    }

    /// <summary>
    /// ��������� HP � ������� GameObject, ���� HP ������ 1 
    /// </summary>
    void CheckHP() 
    {
        if (CurrentHP < 1) //���� �� ����� � ���� ��� ����
        {
            GameObjectManager.KillObject(transform, _destroyTime);
        }
    }
}
