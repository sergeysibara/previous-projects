using UnityEngine;
using System.Collections;

public class AttackCooldownForce_Bonus : BaseBonus 
{
    [SerializeField]
    private int _forcingFactor=2;

	void Start ()
	{
        //Invoke("Test",5);
	}

    public override void RunEffect()
    {
        base.RunEffect();
        PlayerStats.Instance.CurrentShootCooldown = PlayerStats.Instance.StartingShootCooldown/_forcingFactor;
    }

    protected override void DeleteEffect()
    {
        base.DeleteEffect();
        PlayerStats.Instance.CurrentShootCooldown = PlayerStats.Instance.StartingShootCooldown;
    }
}
