using UnityEngine;
using System.Collections;

public class Damage
{
    public Damage(DamageType type, int value)
    {
        Type = type;
        Value = value;
    }

    public DamageType Type;
    public int Value;
}
public enum DamageType
{
    Close,
    Far,
}