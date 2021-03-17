using UnityEngine;
using System.Collections;

public class AddTime_Bonus : BaseBonus 
{
    public override bool IsConstant {
        get { return true; }
    }


	void Start ()
	{

	}


    public override void RunEffect()
    {
        EventAggregator.PublishT(GameEvent.AddTime, this, (int)Duration);
    }

    protected override void DeleteEffect()
    {

    }
}
