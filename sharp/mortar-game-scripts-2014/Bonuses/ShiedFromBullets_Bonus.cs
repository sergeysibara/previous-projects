using UnityEngine;
using System.Collections;

public class ShiedFromBullets_Bonus : BaseBonus 
{
	void Start ()
	{
        //Invoke("Test", 3);
	}

    public override void RunEffect()
    {
        base.RunEffect();
        PlayerStats.Instance.HasShieldFromBullets = true;
    }

    protected override void DeleteEffect()
    {
        base.DeleteEffect();
        PlayerStats.Instance.HasShieldFromBullets = false;
    }
}
