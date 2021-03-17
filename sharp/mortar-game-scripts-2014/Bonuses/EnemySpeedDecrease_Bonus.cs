using UnityEngine;
using System.Collections;

public class EnemySpeedDecrease_Bonus : BaseBonus 
{
	void Start ()
	{
        //Invoke("Test", 8);
	}

    public override void RunEffect()
    {
        base.RunEffect();
        BattleManager.Instance.UnitsSpeedFactor = 2;
    }

    protected override void DeleteEffect()
    {
        base.DeleteEffect();
        BattleManager.Instance.UnitsSpeedFactor = 1;
    }
}
