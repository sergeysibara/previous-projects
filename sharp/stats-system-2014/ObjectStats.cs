using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

/// <summary>
/// Компонент для хранения статов произвольных типов и работы с ними
/// </summary>
public partial class ObjectStats : MonoBehaviour
{
    private readonly Dictionary<int, StatValue> _statsDictionary = new Dictionary<int, StatValue>();

    private Type _statIdentifierType = typeof(UnitStatIdentifier); //предполагается, что в будущем можно будет задавать тип (UnitStatIdentifier, ItemStatIdentifier) через инспектор

    void Start()
    {
    }

    /// <summary>
    /// Добавляет в объект новый стат 
    /// </summary>
    /// <typeparam name="T">Тип перечисления статов (UnitStatIdentifier)</typeparam>
    /// <param name="statId">Id стата из UnitStatIdentifier</param>
    /// <param name="value">Значение стата</param>
    public void Add<T>(T statId, object value) where T : struct
    {
        Type statType = statId.GetAttributeOfType<DataTypeAttribute>().Value;
        int intStatId = Convert.ToInt32(statId);

        if (_statIdentifierType != typeof(T))
        {
            Debug.LogError("Тип добавляемого стата не соотвествует типу, с которым работает данный ObjectStats", this);
            return;
        }

        if (statType != value.GetType())
        {
            Debug.LogError("Типы стата и переданного значения не совпадают", this);
            return;
        }

        if (_statsDictionary.ContainsKey(intStatId))
        {
            string statName = Enum.GetName(_statIdentifierType, intStatId);
            Debug.LogError(string.Format("Стат {0} уже есть в данном объекте", statName), this);
            return;
        }

        var statValue = new StatValue { Value = value };
        _statsDictionary.Add(intStatId, statValue);
    }

    /// <summary>
    /// Небезопасное обращение к стату. Выдаст exception при отсутствии стата.
    /// </summary>
    public object this[int statId]
    {
        get { return _statsDictionary[statId]; }
        set { _statsDictionary[statId].Value = value; }
    }

    public bool TryGetValue(int statId, out object value)
    {
        StatValue statValue;
        bool isSuccess = _statsDictionary.TryGetValue(statId, out statValue);
        value = statValue != null ? statValue.Value : null;
        return isSuccess;
    }

    /// <summary>
    /// Создает словарь статов из внутреннего словаря и возвращает созданный словарь c именами статов
    /// </summary>
    public Dictionary<string, object> GetAllStatsWithNames()
    {
        return _statsDictionary.ToDictionary(k => Enum.GetName(_statIdentifierType, k.Key), v => v.Value.Value);
    }
}
