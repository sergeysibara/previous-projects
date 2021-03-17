using UnityEngine;
using System.Collections;

public class AttackRadiusUp_Bonus : BaseBonus 
{
    public override bool IsConstant {
        get { return true; }
    }


	void Start ()
	{

	}


    public override void RunEffect()
    {
        PlayerStats.Instance.ExplosionLevel++;
    }

    protected override void DeleteEffect()
    {

    }
}
