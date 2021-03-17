using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

public class BonusManager : RequiredMonoSingleton<BonusManager>
{
    [SerializeField, Tooltip("Префаб иконки бонуса, появляющегося в месте попадания")]
    private GameObject _attachedBonusIconPrefab;

    [SerializeField, Tooltip("Префаб combo текста, появляющегося в месте попадания")]
    private GameObject _attachedComboTextPrefab;

    [SerializeField, TooltipAttribute("минимальная сумма стартового HP убитых юнита, за которую будет всегда даваться бонус")]
    private int _minSumOfStartHpOfKilledUnitsForBonus=5;

    [SerializeField,TooltipAttribute("минимальное стартовое HP юнита, за убиство которого будет всегда даваться бонус")]
    private int _minStartHPForBonus = 3;

    private BaseBonus[] bonuses;

	void Start ()
	{
	    bonuses = GetComponentsInChildren<BaseBonus>().Where(c=>c.gameObject.activeInHierarchy && enabled).ToArray();
	}

    /// <summary>
    /// Вычисление бонусов и score за убийство юнитов. Использовать до причинения урона юнитам.
    /// </summary>
    public void CalculateBonusesForExplosion(Collider[] units,Vector3 position)
    {
        if (BattleManager.CurrentGameMode != GameMode.Normal)
            return;

        UnitStats[] stats = units.Select(c => c.GetComponent<UnitStats>()).ToArray();
        int totalStartHpOfKilled = stats.Where(c => c.WillKilledByCurrenHit).Sum(c=>c.StartHP);
        string bonusDescitption = null;

        bool hasBonus = HasBonus(totalStartHpOfKilled, stats);
        //hasBonus = true;
        if (totalStartHpOfKilled > 1)
        {
            AttachComboText(position, totalStartHpOfKilled, hasBonus);
        }

        if (hasBonus)
        {
            var bonus = GetRandomBonus();
            RunBonus(bonus);
            AttachBonus(bonus, position);
            bonusDescitption = Localization.Get(bonus.LocalizationKey);
        }
        if (totalStartHpOfKilled > 0)
        {
            float factor = 1 + 0.1f*(totalStartHpOfKilled - 1);

            int totalScore=0;
            foreach ( var stat in stats)
            {
                totalScore +=(int)( factor*stat.Cost);
            }
            EventAggregator.PublishT(GameEvent.OnCalculateScore, this, totalScore);
            HitInfoBar.Instance.Show(factor.ToString(), totalScore.ToString(), bonusDescitption);
        }
    }

    private bool HasBonus(int totalStartHpOfKilled, IEnumerable<UnitStats> stats)
    {
        return (totalStartHpOfKilled >= _minSumOfStartHpOfKilledUnitsForBonus || stats.Any(c => c.StartHP >= _minStartHPForBonus && c.WillKilledByCurrenHit));
    }

    /// <summary>
    /// Отображение иконки бонуса в месте попадания
    /// </summary>
    private void AttachBonus(BaseBonus bonus, Vector3 position)
    {
        var bonusGO = NGUITools.AddChild(UIRoot.list[0].gameObject, _attachedBonusIconPrefab);
        var uifollover = bonusGO.GetComponent<UIFollowerToPoint>();
        uifollover.TargetPos = position;
        
        var sprite = bonusGO.GetComponentInChildren<UISprite>();
        sprite.spriteName = bonus.SpriteName;
    }

    private void AttachComboText(Vector3 position, int count, bool hasBonus)
    {
        var bonusGO = NGUITools.AddChild(UIRoot.list[0].gameObject, _attachedComboTextPrefab);
        var uifollover = bonusGO.GetComponent<UIFollowerToPoint>();
        uifollover.TargetPos = position;

        var lbl = bonusGO.GetComponentInChildren<UILabel>();
        lbl.text += count;

        if (hasBonus)
            uifollover.Offset.y += 0.95f;
    }

    private BaseBonus GetRandomBonus()
    {
        return RandomUtils.GetRandomItem(bonuses);
    }

    private void RunBonus(BaseBonus bonus)
    {
        if (bonus.IsConstant)
        {
            bonus.RunEffect();
            return;
        }

        if (!bonus.IsActive)
            bonus.RunEffect();
        else
            bonus.ResetDuration();
    }

}
